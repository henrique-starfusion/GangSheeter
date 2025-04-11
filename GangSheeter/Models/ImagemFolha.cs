using System;
using System.ComponentModel;

namespace GangSheeter.Models
{
    public class ImagemFolha : INotifyPropertyChanged
    {
        private double _posicaoX;
        private double _posicaoY;
        private double _rotacao;
        private int _quantidade;
        private bool _selecionada = true;
        private Imagem _imagemOriginal;
        private double _escala = 1.0;

        public ImagemFolha(Imagem imagemOriginal)
        {
            _imagemOriginal = imagemOriginal;
            _quantidade = imagemOriginal.Quantidade;
            _rotacao = imagemOriginal.Rotacao;
        }

        public Imagem ImagemOriginal
        {
            get => _imagemOriginal;
            set
            {
                _imagemOriginal = value;
                OnPropertyChanged(nameof(ImagemOriginal));
            }
        }

        public string NomeArquivo => ImagemOriginal.NomeArquivo;
        public string CaminhoCompleto => ImagemOriginal.CaminhoCompleto;
        public int DPI => ImagemOriginal.DPI;
        public double LarguraCm => ImagemOriginal.LarguraCm;
        public double AlturaCm => ImagemOriginal.AlturaCm;

        public double PosicaoX
        {
            get => _posicaoX;
            set
            {
                _posicaoX = value;
                OnPropertyChanged(nameof(PosicaoX));
            }
        }

        public double PosicaoY
        {
            get => _posicaoY;
            set
            {
                _posicaoY = value;
                OnPropertyChanged(nameof(PosicaoY));
            }
        }

        public double Rotacao
        {
            get => _rotacao;
            set
            {
                _rotacao = value % 360;
                OnPropertyChanged(nameof(Rotacao));
            }
        }

        public double Escala
        {
            get => _escala;
            set
            {
                if (value <= 0) value = 1.0;
                _escala = value;
                OnPropertyChanged(nameof(Escala));
            }
        }

        public int Quantidade
        {
            get => _quantidade;
            set
            {
                if (value < 1) value = 1;
                if (value > 100) value = 100;
                _quantidade = value;
                OnPropertyChanged(nameof(Quantidade));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 