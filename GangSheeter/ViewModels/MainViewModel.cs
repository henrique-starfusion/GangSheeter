using GangSheeter.Models;
using GangSheeter.Services;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Forms = System.Windows.Forms;
using System.Collections.Generic;

namespace GangSheeter.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private Canvas _previewCanvas;
        private Grid _previewGrid;

        private const int MAX_TENTATIVAS = 10;
        private const double ESPACO_MINIMO = 10; // 1cm em pixels
        private const double MARGEM_FOLHA = 5; // 0,5cm em pixels
        private const double TOLERANCIA_SIMILARIDADE = 0.1; // 10% de tolerância
        private const int MAX_HISTORICO_DISTRIBUICOES = 100; // Limite de distribuições anteriores
        private readonly List<Dictionary<ImagemFolha, (double X, double Y, double Rotacao, double Escala)>> _distribuicoesAnteriores = new();
        private Random _random = new Random();

        public double PreviewWidth { get; set; } = 580;
        public double PreviewHeight { get; set; } = 827; // Assuming a default value

        private double _zoomLevel = 100;
        public double ZoomLevel
        {
            get => _zoomLevel;
            set
            {
                if (value < 10) value = 10;
                if (value > 400) value = 400;
                if (SetField(ref _zoomLevel, value))
                {
                    UpdatePreviewSize();
                }
            }
        }

        public ObservableCollection<Imagem> Imagens { get; } = new ObservableCollection<Imagem>();
        public ObservableCollection<ImagemFolha> ImagensFolha { get; } = new ObservableCollection<ImagemFolha>();

        public ICommand SelecionarImagensCommand { get; }
        public ICommand RemoverImagemCommand { get; }
        public ICommand GerarFolhaCommand { get; }
        public ICommand RotacionarImagemCommand { get; }
        public ICommand LimparListaCommand { get; }
        public ICommand AdicionarImagemFolhaCommand { get; }
        public ICommand RemoverImagemFolhaCommand { get; }
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand FitToScreenCommand { get; }
        public ICommand DistribuirImagensFolhaCommand { get; private set; }
        public ICommand SalvarFolhaTIFFCommand { get; }
        public ICommand SalvarFolhaPNGCommand { get; }

        private ImagemFolha? _draggedImage;
        private System.Windows.Point _dragStartPoint;
        private System.Windows.Point _dragStartPosition;

        public MainViewModel(Canvas canvas, Grid grid)
        {
            _previewCanvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            _previewGrid = grid ?? throw new ArgumentNullException(nameof(grid));

            SelecionarImagensCommand = new RelayCommand(SelecionarImagens);
            RemoverImagemCommand = new RelayCommand<Imagem>(RemoverImagem, (img) => Imagens.Any() && img != null);
            GerarFolhaCommand = new RelayCommand(GerarFolha, () => Imagens.Any(i => i.Selecionada));
            RotacionarImagemCommand = new RelayCommand<ImagemFolha>(RotacionarImagemFolha, (img) => ImagensFolha.Any() && img != null);
            LimparListaCommand = new RelayCommand(LimparLista, () => Imagens.Any());
            AdicionarImagemFolhaCommand = new RelayCommand<Imagem>(AdicionarImagemFolha, (img) => Imagens.Any() && img != null);
            RemoverImagemFolhaCommand = new RelayCommand<ImagemFolha>(RemoverImagemFolha, (img) => ImagensFolha.Any() && img != null);
            ZoomInCommand = new RelayCommand(() => ZoomLevel += 25);
            ZoomOutCommand = new RelayCommand(() => ZoomLevel -= 25);
            FitToScreenCommand = new RelayCommand(FitToScreen);

            DistribuirImagensFolhaCommand = new RelayCommand(DistribuirImagensFolha, () => ImagensFolha.Any());

            SalvarFolhaTIFFCommand = new RelayCommand(SalvarFolhaTIFF, () => ImagensFolha.Any());
            SalvarFolhaPNGCommand = new RelayCommand(SalvarFolhaPNG, () => ImagensFolha.Any());

            UpdatePreviewSize();
        }

        private void SelecionarImagens()
        {
            var dialog = new Forms.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Imagens|*.png;*.tiff",
                Title = "Selecionar Imagens para Impressão"
            };

            if (dialog.ShowDialog() == Forms.DialogResult.OK)
            {
                foreach (var fileName in dialog.FileNames)
                {
                    try
                    {
                        using var img = System.Drawing.Image.FromFile(fileName);
                        var dpiX = img.HorizontalResolution;
                        var dpiY = img.VerticalResolution;

                        // Converter pixels para centímetros
                        var widthCm = img.Width / dpiX * 2.54;
                        var heightCm = img.Height / dpiY * 2.54;

                        var fileInfo = new FileInfo(fileName);

                        Imagens.Add(new Imagem
                        {
                            NomeArquivo = Path.GetFileName(fileName),
                            CaminhoCompleto = fileName,
                            DPI = (int)Math.Round(dpiX),
                            LarguraCm = Math.Round(widthCm, 2),
                            AlturaCm = Math.Round(heightCm, 2),
                            TamanhoBytes = fileInfo.Length
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao carregar imagem {fileName}: {ex.Message}", "Erro",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void RemoverImagem(Imagem imagem)
        {
            if (imagem != null && Imagens.Contains(imagem))
                Imagens.Remove(imagem);
        }

        private void AdicionarImagemFolha(Imagem imagem)
        {
            if (imagem != null)
            {
                var imagemFolha = new ImagemFolha(imagem);
                this.ImagensFolha.Add(imagemFolha);
            }
        }

        private void RemoverImagemFolha(ImagemFolha? imagemFolha)
        {
            if (imagemFolha != null && ImagensFolha.Contains(imagemFolha))
            {
                ImagensFolha.Remove(imagemFolha);
            }

            this.AtualizarPreview();
        }

        private void RotacionarImagemFolha(ImagemFolha? imagemFolha)
        {
            if (imagemFolha != null)
            {
                imagemFolha.Rotacao += 90;
            }

            this.AtualizarPreview();
        }

        /// <summary>
        /// Gera a folha de impressão com os arquivos selecionados da lista de imagens
        /// </summary>
        private void GerarFolha()
        {
            if (_previewCanvas == null) return;

            // Primeiro, adicionar todas as imagens selecionadas da lista principal que ainda não estão na folha
            foreach (var imagem in Imagens.Where(i => i.Selecionada))
            {
                for (int i = 0; i < imagem.Quantidade; i++)
                {
                    this.AdicionarImagemFolha(imagem);
                }
            }

            this.DistribuirImagensFolha();
        }

        /// <summary>
        /// Atualizar a pré-visualização da folha de impressão
        /// </summary>
        private void AtualizarPreview()
        {
            if (_previewCanvas == null) return;

            _previewCanvas.Children.Clear();

            foreach (var imgFolha in ImagensFolha)
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imgFolha.CaminhoCompleto);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    bitmap.EndInit();
                    bitmap.Freeze(); // Freeze for better performance

                    // Aplicar a escala à imagem
                    double larguraReal = imgFolha.LarguraCm * 10 * (imgFolha.Escala > 0 ? imgFolha.Escala : 1);
                    double alturaReal = imgFolha.AlturaCm * 10 * (imgFolha.Escala > 0 ? imgFolha.Escala : 1);

                    var imageControl = new System.Windows.Controls.Image
                    {
                        Source = bitmap,
                        Width = larguraReal,
                        Height = alturaReal,
                        Stretch = Stretch.Uniform,
                        Margin = new Thickness(imgFolha.PosicaoX, imgFolha.PosicaoY, 0, 0),
                        RenderTransform = new RotateTransform(imgFolha.Rotacao),
                    };

                    imageControl.MouseMove += this.ImageSheetMove;
                    imageControl.MouseLeftButtonUp += this.ImageSheetUnselect;
                    imageControl.MouseLeftButtonDown += (s, e) =>
                    {
                        if (e.ClickCount == 1)
                        {
                            var image = s as System.Windows.Controls.Image;
                            if (image != null)
                            {
                                image.CaptureMouse();
                                _draggedImage = imgFolha;
                                _dragStartPoint = e.GetPosition(_previewCanvas);
                                _dragStartPosition = new System.Windows.Point(_draggedImage.PosicaoX, _draggedImage.PosicaoY);
                            }
                        }
                    };

                    _previewCanvas.Children.Add(imageControl);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao carregar imagem para pré-visualização: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Move a imagem na folha de impressão
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageSheetMove(object sender, MouseEventArgs e)
        {
            var image = sender as System.Windows.Controls.Image;
            if (image != null && _draggedImage != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPosition = e.GetPosition(_previewCanvas);
                var deltaX = currentPosition.X - _dragStartPoint.X;
                var deltaY = currentPosition.Y - _dragStartPoint.Y;

                _draggedImage.PosicaoX = _dragStartPosition.X + deltaX;
                _draggedImage.PosicaoY = _dragStartPosition.Y + deltaY;

                // Atualizar a posição da imagem no canvas
                image.Margin = new Thickness(_draggedImage.PosicaoX, _draggedImage.PosicaoY, 0, 0);
            }
        }

        private void ImageSheetUnselect(object sender, MouseButtonEventArgs e)
        {
            var image = sender as System.Windows.Controls.Image;
            if (image != null)
            {
                image.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Remove apenas as imagens da lista de imagens
        /// </summary>
        private void LimparLista()
        {
            Imagens.Clear();
        }

        /// <summary>
        /// Salvar a folha de impressão em um arquivo TIFF
        /// </summary>
        private void SalvarFolhaTIFF()
        {
            if (_previewCanvas == null) return;

            var dialog = new Forms.SaveFileDialog
            {
                Filter = "Imagem TIFF|*.tiff",
                Title = "Salvar Folha de Impressão",
                DefaultExt = "tiff"
            };

            if (dialog.ShowDialog() == Forms.DialogResult.OK)
            {
                try
                {
                    // Criar um bitmap com fundo transparente
                    var width = (int)_previewCanvas.Width;
                    var height = (int)_previewCanvas.Height;

                    // Criar um bitmap com fundo transparente
                    var bitmap = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        // Configurar para alta qualidade
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                        // Desenhar o fundo transparente
                        graphics.Clear(System.Drawing.Color.Transparent);

                        // Converter o canvas WPF para um bitmap
                        var visual = new DrawingVisual();
                        using (var drawingContext = visual.RenderOpen())
                        {
                            var brush = new VisualBrush(_previewCanvas);
                            drawingContext.DrawRectangle(brush, null, new Rect(0, 0, width, height));
                        }

                        // Renderizar o visual para o bitmap
                        var renderBitmap = new RenderTargetBitmap(
                            width, height, 300, 300, PixelFormats.Pbgra32);
                        renderBitmap.Render(visual);

                        // Converter para System.Drawing.Bitmap
                        var bitmapSource = renderBitmap;
                        var stride = width * 4;
                        var pixels = new byte[height * stride];
                        bitmapSource.CopyPixels(pixels, stride, 0);

                        var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, width, height),
                            System.Drawing.Imaging.ImageLockMode.WriteOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                        System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
                        bitmap.UnlockBits(bitmapData);
                    }

                    // Salvar como TIFF com compressão LZW
                    var encoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
                    encoderParameters.Param[0] = new System.Drawing.Imaging.EncoderParameter(
                        System.Drawing.Imaging.Encoder.Quality, 100L);

                    var tiffCodec = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders()
                        .First(codec => codec.FormatID == System.Drawing.Imaging.ImageFormat.Tiff.Guid);

                    bitmap.Save(dialog.FileName, tiffCodec, encoderParameters);

                    MessageBox.Show("Folha de impressão salva com sucesso!", "Sucesso",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao salvar a folha de impressão: {ex.Message}", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Faz um rearrando das imagens dentro da folha de impressão
        /// </summary>
        /// <param name="sourceImagem"></param>
        /// <param name="targetImagem"></param>
        public void ReordenarImagens(Imagem sourceImagem, Imagem targetImagem)
        {
            if (sourceImagem == null || targetImagem == null || sourceImagem == targetImagem)
                return;

            int sourceIndex = Imagens.IndexOf(sourceImagem);
            int targetIndex = Imagens.IndexOf(targetImagem);

            if (sourceIndex >= 0 && targetIndex >= 0)
            {
                Imagens.RemoveAt(sourceIndex);
                Imagens.Insert(targetIndex, sourceImagem);
            }
        }

        /// <summary>
        /// Faz a distribuição das imagens na folha de impressão usando um algoritmo de IA
        /// </summary>
        private void DistribuirImagensFolha()
        {
            if (ImagensFolha.Count == 0) return;

            // Salvar a distribuição atual antes de gerar uma nova
            SalvarDistribuicaoAtual();

            // Gerar uma nova distribuição diferente das anteriores
            GerarNovaDistribuicao();
        }

        /// <summary>
        /// Salva a distribuição atual das imagens na folha
        /// </summary>
        private void SalvarDistribuicaoAtual()
        {
            var distribuicaoAtual = new Dictionary<ImagemFolha, (double X, double Y, double Rotacao, double Escala)>();
            
            foreach (var imagem in ImagensFolha)
            {
                distribuicaoAtual[imagem] = (imagem.PosicaoX, imagem.PosicaoY, imagem.Rotacao, imagem.Escala);
            }
            
            _distribuicoesAnteriores.Add(distribuicaoAtual);
            
            // Limitar o número de distribuições armazenadas para evitar consumo excessivo de memória
            if (_distribuicoesAnteriores.Count > 10)
            {
                _distribuicoesAnteriores.RemoveAt(0);
            }
        }

        /// <summary>
        /// Gera uma nova distribuição diferente das anteriores
        /// </summary>
        private void GerarNovaDistribuicao()
        {
            if (ImagensFolha.Count == 0) return;

            // Limpar o canvas
            _previewCanvas.Children.Clear();

            // Ordenar imagens por altura (maior para menor)
            var imagensOrdenadas = ImagensFolha.OrderByDescending(img => img.AlturaCm * 10 * (img.Escala > 0 ? img.Escala : 1)).ToList();

            // Calcular dimensões disponíveis
            double larguraDisponivel = PreviewWidth - (2 * MARGEM_FOLHA);
            double alturaDisponivel = PreviewHeight - (2 * MARGEM_FOLHA);

            // Tentar gerar uma distribuição diferente das anteriores
            bool distribuicaoValida = false;
            int tentativas = 0;
            Dictionary<ImagemFolha, (double X, double Y, double Rotacao, double Escala)> distribuicaoAtual = null;
            List<List<ImagemFolha>> colunas = null;

            while (!distribuicaoValida && tentativas < MAX_TENTATIVAS)
            {
                // Calcular o número ideal de colunas usando análise de densidade
                double larguraMediaImagens = imagensOrdenadas.Average(img => img.LarguraCm * 10 * (img.Escala > 0 ? img.Escala : 1));
                double alturaMediaImagens = imagensOrdenadas.Average(img => img.AlturaCm * 10 * (img.Escala > 0 ? img.Escala : 1));
                
                // Calcular a densidade ideal (proporção largura/altura) para minimizar o espaço desperdiçado
                double densidadeIdeal = larguraMediaImagens / alturaMediaImagens;
                
                // Ajustar o número de colunas com base na densidade
                int numColunasIdeal = (int)Math.Floor(larguraDisponivel / (larguraMediaImagens + ESPACO_MINIMO));
                numColunasIdeal = Math.Max(1, numColunasIdeal); // Pelo menos 1 coluna
                
                // Ajustar o número de colunas com base na densidade ideal
                if (densidadeIdeal > 1.5) // Imagens mais largas que altas
                {
                    numColunasIdeal = Math.Max(1, numColunasIdeal - 1); // Reduzir colunas para imagens mais largas
                }
                else if (densidadeIdeal < 0.7) // Imagens mais altas que largas
                {
                    numColunasIdeal = Math.Min(5, numColunasIdeal + 1); // Aumentar colunas para imagens mais altas
                }

                // Adicionar variação aleatória ao número de colunas para evitar repetições
                if (_distribuicoesAnteriores.Count > 0)
                {
                    // Variação de -1 a +1 no número de colunas
                    int variacao = _random.Next(-1, 2);
                    numColunasIdeal = Math.Max(1, Math.Min(5, numColunasIdeal + variacao));
                }

                // Criar lista de colunas
                colunas = new List<List<ImagemFolha>>();
                for (int i = 0; i < numColunasIdeal; i++)
                {
                    colunas.Add(new List<ImagemFolha>());
                }

                // Distribuir imagens entre as colunas usando um algoritmo de "bin packing" adaptado
                foreach (var imagem in imagensOrdenadas)
                {
                    // Calcular a pontuação para cada coluna
                    var colunasPontuadas = colunas.Select((coluna, index) => new
                    {
                        Coluna = coluna,
                        Index = index,
                        AlturaTotal = coluna.Sum(img => img.AlturaCm * 10 * (img.Escala > 0 ? img.Escala : 1)),
                        Pontuacao = CalcularPontuacaoColuna(coluna, imagem, index, numColunasIdeal)
                    }).ToList();

                    // Escolher a coluna com a melhor pontuação
                    var melhorColuna = colunasPontuadas.OrderByDescending(c => c.Pontuacao).First();
                    melhorColuna.Coluna.Add(imagem);
                }

                // Calcular a largura ideal para cada coluna
                double larguraPorColuna = (larguraDisponivel - ((numColunasIdeal - 1) * ESPACO_MINIMO)) / numColunasIdeal;

                // Posicionar imagens em cada coluna
                double posicaoX = MARGEM_FOLHA;
                distribuicaoAtual = new Dictionary<ImagemFolha, (double X, double Y, double Rotacao, double Escala)>();
                List<(double X, double Y, double W, double H)> areasOcupadas = new List<(double X, double Y, double W, double H)>();

                foreach (var coluna in colunas)
                {
                    double posicaoY = MARGEM_FOLHA;
                    foreach (var imagem in coluna)
                    {
                        // Calcular dimensões reais considerando a escala
                        double larguraReal = imagem.LarguraCm * 10 * (imagem.Escala > 0 ? imagem.Escala : 1);
                        double alturaReal = imagem.AlturaCm * 10 * (imagem.Escala > 0 ? imagem.Escala : 1);
                        
                        // Centralizar a imagem na coluna
                        double offsetX = (larguraPorColuna - larguraReal) / 2;
                        double posX = posicaoX + offsetX;
                        
                        // Verificar se a imagem ultrapassa a largura disponível
                        if (posX + larguraReal > PreviewWidth - MARGEM_FOLHA)
                        {
                            // Ajustar a escala para caber na largura disponível
                            double novaEscala = (PreviewWidth - MARGEM_FOLHA - posX) / (imagem.LarguraCm * 10);
                            if (novaEscala > 0)
                            {
                                imagem.Escala = Math.Min(imagem.Escala, novaEscala);
                                larguraReal = imagem.LarguraCm * 10 * imagem.Escala;
                            }
                        }

                        // Verificar sobreposição com outras imagens
                        bool sobreposicao = false;
                        foreach (var area in areasOcupadas)
                        {
                            if (VerificarSobreposicao(posX, posicaoY, larguraReal, alturaReal, area.X, area.Y, area.W, area.H))
                            {
                                sobreposicao = true;
                                break;
                            }
                        }

                        // Se houver sobreposição, ajustar a posição Y
                        while (sobreposicao && posicaoY + alturaReal < PreviewHeight - MARGEM_FOLHA)
                        {
                            posicaoY += ESPACO_MINIMO;
                            sobreposicao = false;
                            foreach (var area in areasOcupadas)
                            {
                                if (VerificarSobreposicao(posX, posicaoY, larguraReal, alturaReal, area.X, area.Y, area.W, area.H))
                                {
                                    sobreposicao = true;
                                    break;
                                }
                            }
                        }
                        
                        // Adicionar à distribuição atual
                        distribuicaoAtual[imagem] = (posX, posicaoY, imagem.Rotacao, imagem.Escala);
                        
                        // Adicionar à lista de áreas ocupadas
                        areasOcupadas.Add((posX, posicaoY, larguraReal, alturaReal));
                        
                        posicaoY += alturaReal + ESPACO_MINIMO;
                    }
                    posicaoX += larguraPorColuna + ESPACO_MINIMO;
                }

                // Verificar se a distribuição é válida e diferente das anteriores
                distribuicaoValida = true;
                foreach (var coluna in colunas)
                {
                    if (coluna.Count == 0) continue;

                    double alturaTotal = coluna.Sum(img => img.AlturaCm * 10 * (img.Escala > 0 ? img.Escala : 1));
                    if (alturaTotal > alturaDisponivel)
                    {
                        distribuicaoValida = false;
                        break;
                    }
                }

                if (distribuicaoValida && distribuicaoAtual != null)
                {
                    distribuicaoValida = !EhDistribuicaoSimilar(distribuicaoAtual);
                }

                tentativas++;
            }

            // Se encontrou uma distribuição válida, aplica ao canvas
            if (distribuicaoValida && distribuicaoAtual != null)
            {
                foreach (var kvp in distribuicaoAtual)
                {
                    var imagem = kvp.Key;
                    var pos = kvp.Value;

                    var image = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri(imagem.CaminhoCompleto)),
                        Width = imagem.LarguraCm * 10 * pos.Escala,
                        Height = imagem.AlturaCm * 10 * pos.Escala,
                        RenderTransform = new RotateTransform(pos.Rotacao)
                    };

                    Canvas.SetLeft(image, pos.X);
                    Canvas.SetTop(image, pos.Y);
                    _previewCanvas.Children.Add(image);
                }

                // Adiciona a distribuição ao histórico
                AdicionarDistribuicaoAoHistorico(distribuicaoAtual);
            }
            else
            {
                // Se não encontrou uma distribuição válida após todas as tentativas,
                // usa a distribuição mais recente ou gera uma distribuição simples
                ReorganizarAutomaticamente();
            }
        }

        /// <summary>
        /// Verifica se uma distribuição é muito similar a uma distribuição anterior
        /// </summary>
        private bool EhDistribuicaoSimilar(Dictionary<ImagemFolha, (double X, double Y, double Rotacao, double Escala)> distribuicaoAtual)
        {
            if (_distribuicoesAnteriores.Count == 0) return false;

            foreach (var distribuicaoAnterior in _distribuicoesAnteriores)
            {
                if (distribuicaoAnterior.Count != distribuicaoAtual.Count) continue;

                bool todasImagensSimilares = true;
                foreach (var kvp in distribuicaoAtual)
                {
                    if (!distribuicaoAnterior.TryGetValue(kvp.Key, out var posAnterior))
                    {
                        todasImagensSimilares = false;
                        break;
                    }

                    var posAtual = kvp.Value;
                    var difX = Math.Abs(posAtual.X - posAnterior.X);
                    var difY = Math.Abs(posAtual.Y - posAnterior.Y);
                    var difRotacao = Math.Abs(posAtual.Rotacao - posAnterior.Rotacao);
                    var difEscala = Math.Abs(posAtual.Escala - posAnterior.Escala);

                    // Verifica se a posição, rotação e escala são similares
                    if (difX > ESPACO_MINIMO || difY > ESPACO_MINIMO || 
                        difRotacao > TOLERANCIA_SIMILARIDADE || difEscala > TOLERANCIA_SIMILARIDADE)
                    {
                        todasImagensSimilares = false;
                        break;
                    }
                }

                if (todasImagensSimilares) return true;
            }

            return false;
        }

        private void AdicionarDistribuicaoAoHistorico(Dictionary<ImagemFolha, (double X, double Y, double Rotacao, double Escala)> distribuicao)
        {
            // Adiciona a nova distribuição ao histórico
            _distribuicoesAnteriores.Add(new Dictionary<ImagemFolha, (double X, double Y, double Rotacao, double Escala)>(distribuicao));

            // Mantém apenas as últimas MAX_HISTORICO_DISTRIBUICOES distribuições
            while (_distribuicoesAnteriores.Count > MAX_HISTORICO_DISTRIBUICOES)
            {
                _distribuicoesAnteriores.RemoveAt(0);
            }
        }

        /// <summary>
        /// Calcula a pontuação para uma coluna específica, considerando vários fatores
        /// </summary>
        private double CalcularPontuacaoColuna(List<ImagemFolha> coluna, ImagemFolha novaImagem, int indexColuna, int totalColunas)
        {
            // Fator 1: Altura total da coluna (menor é melhor)
            double alturaTotal = coluna.Sum(img => img.AlturaCm * 10 * (img.Escala > 0 ? img.Escala : 1));
            double fatorAltura = 1.0 / (alturaTotal + 1); // Evitar divisão por zero
            
            // Fator 2: Compatibilidade de largura (imagens de larguras similares ficam juntas)
            double larguraMedia = coluna.Count > 0 ? 
                coluna.Average(img => img.LarguraCm * (img.Escala > 0 ? img.Escala : 1)) : 
                novaImagem.LarguraCm * (novaImagem.Escala > 0 ? novaImagem.Escala : 1);
            double diferencaLargura = Math.Abs(larguraMedia - (novaImagem.LarguraCm * (novaImagem.Escala > 0 ? novaImagem.Escala : 1)));
            double fatorCompatibilidade = 1.0 / (diferencaLargura + 1);
            
            // Fator 3: Distribuição entre colunas (preferir colunas mais vazias)
            double fatorDistribuicao = 1.0 - ((double)coluna.Count / (ImagensFolha.Count + 1));
            
            // Fator 4: Posição da coluna (preferir colunas centrais para melhor equilíbrio visual)
            double posicaoRelativa = Math.Abs(indexColuna - (totalColunas - 1) / 2.0) / totalColunas;
            double fatorPosicao = 1.0 - posicaoRelativa;
            
            // Fator 5: Agrupamento por tamanho (imagens de tamanhos similares ficam juntas)
            double alturaMedia = coluna.Count > 0 ? 
                coluna.Average(img => img.AlturaCm * (img.Escala > 0 ? img.Escala : 1)) : 
                novaImagem.AlturaCm * (novaImagem.Escala > 0 ? novaImagem.Escala : 1);
            double diferencaAltura = Math.Abs(alturaMedia - (novaImagem.AlturaCm * (novaImagem.Escala > 0 ? novaImagem.Escala : 1)));
            double fatorAgrupamento = 1.0 / (diferencaAltura + 1);
            
            // Pesos para cada fator
            double pesoAltura = 0.3;
            double pesoCompatibilidade = 0.2;
            double pesoDistribuicao = 0.2;
            double pesoPosicao = 0.15;
            double pesoAgrupamento = 0.15;
            
            // Calcular pontuação final
            return (fatorAltura * pesoAltura) +
                   (fatorCompatibilidade * pesoCompatibilidade) +
                   (fatorDistribuicao * pesoDistribuicao) +
                   (fatorPosicao * pesoPosicao) +
                   (fatorAgrupamento * pesoAgrupamento);
        }

        private void UpdatePreviewSize()
        {
            if (_previewCanvas == null || this._previewGrid == null) return;

            double scale = ZoomLevel / 100.0;
            _previewGrid.RenderTransform = new ScaleTransform(scale, scale);
        }

        private void FitToScreen()
        {
            if (_previewCanvas == null || this._previewGrid == null) return;

            // Obter o tamanho atual da área de visualização
            var viewportWidth = _previewCanvas.ActualWidth;
            var viewportHeight = _previewCanvas.ActualHeight;

            if (viewportWidth <= 0 || viewportHeight <= 0) return;

            // Calcular a escala necessária para ajustar à tela
            double scaleX = viewportWidth / this._previewCanvas.Width;
            double scaleY = viewportHeight / this._previewCanvas.Height;
            double scale = Math.Min(scaleX, scaleY) * 100;

            // Aplicar o zoom
            ZoomLevel = Math.Round(scale);
            this.UpdatePreviewSize();
        }

        /// <summary>
        /// Salvar a folha de impressão em um arquivo PNG
        /// </summary>
        private void SalvarFolhaPNG()
        {
            var saveFileDialog = new Forms.SaveFileDialog
            {
                Filter = "Imagem PNG|*.png",
                Title = "Salvar Folha de Impressão"
            };

            if (saveFileDialog.ShowDialog() == Forms.DialogResult.OK)
            {
                try
                {
                    // Criar um bitmap com o tamanho do grid
                    var bitmap = new RenderTargetBitmap(
                        (int)_previewCanvas.Width,
                        (int)_previewCanvas.Height,
                        300, 300, PixelFormats.Default);

                    // Renderizar o grid para o bitmap
                    bitmap.Render(_previewCanvas);

                    // Salvar o bitmap como PNG
                    using var stream = new FileStream(saveFileDialog.FileName, FileMode.Create);
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(stream);

                    MessageBox.Show("Folha salva com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao salvar a folha: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ReorganizarAutomaticamente()
        {
            if (ImagensFolha.Count == 0) return;

            // Limpar o canvas
            _previewCanvas.Children.Clear();

            // Ordenar imagens por altura (maior para menor)
            var imagensOrdenadas = ImagensFolha.OrderByDescending(img => img.AlturaCm * 10 * (img.Escala > 0 ? img.Escala : 1)).ToList();

            // Calcular dimensões disponíveis
            double larguraDisponivel = PreviewWidth - (2 * MARGEM_FOLHA);
            double alturaDisponivel = PreviewHeight - (2 * MARGEM_FOLHA);

            // Calcular o número ideal de colunas
            double larguraMediaImagens = imagensOrdenadas.Average(img => img.LarguraCm * 10 * (img.Escala > 0 ? img.Escala : 1));
            int numColunas = (int)Math.Floor(larguraDisponivel / (larguraMediaImagens + ESPACO_MINIMO));
            numColunas = Math.Max(1, numColunas);

            // Criar lista de colunas
            var colunas = new List<List<ImagemFolha>>();
            for (int i = 0; i < numColunas; i++)
            {
                colunas.Add(new List<ImagemFolha>());
            }

            // Distribuir imagens nas colunas
            int colunaAtual = 0;
            foreach (var imagem in imagensOrdenadas)
            {
                colunas[colunaAtual].Add(imagem);
                colunaAtual = (colunaAtual + 1) % numColunas;
            }

            // Posicionar imagens em cada coluna
            double posicaoX = MARGEM_FOLHA;
            List<(double X, double Y, double W, double H)> areasOcupadas = new List<(double X, double Y, double W, double H)>();

            foreach (var coluna in colunas)
            {
                double posicaoY = MARGEM_FOLHA;
                foreach (var imagem in coluna)
                {
                    double larguraReal = imagem.LarguraCm * 10 * (imagem.Escala > 0 ? imagem.Escala : 1);
                    double alturaReal = imagem.AlturaCm * 10 * (imagem.Escala > 0 ? imagem.Escala : 1);

                    // Verificar sobreposição com outras imagens
                    bool sobreposicao = false;
                    foreach (var area in areasOcupadas)
                    {
                        if (VerificarSobreposicao(posicaoX, posicaoY, larguraReal, alturaReal, area.X, area.Y, area.W, area.H))
                        {
                            sobreposicao = true;
                            break;
                        }
                    }

                    // Se houver sobreposição, ajustar a posição Y
                    while (sobreposicao && posicaoY + alturaReal < PreviewHeight - MARGEM_FOLHA)
                    {
                        posicaoY += ESPACO_MINIMO;
                        sobreposicao = false;
                        foreach (var area in areasOcupadas)
                        {
                            if (VerificarSobreposicao(posicaoX, posicaoY, larguraReal, alturaReal, area.X, area.Y, area.W, area.H))
                            {
                                sobreposicao = true;
                                break;
                            }
                        }
                    }

                    imagem.PosicaoX = posicaoX;
                    imagem.PosicaoY = posicaoY;
                    areasOcupadas.Add((posicaoX, posicaoY, larguraReal, alturaReal));
                    posicaoY += alturaReal + ESPACO_MINIMO;
                }
                posicaoX += larguraMediaImagens + ESPACO_MINIMO;
            }

            // Atualizar o preview
            AtualizarPreview();
        }

        private bool VerificarSobreposicao(double x1, double y1, double w1, double h1, double x2, double y2, double w2, double h2)
        {
            return !(x1 + w1 < x2 || x2 + w2 < x1 || y1 + h1 < y2 || y2 + h2 < y1);
        }
    }
}
