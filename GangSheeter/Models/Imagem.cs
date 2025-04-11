using System;
using System.ComponentModel;

namespace GangSheeter.Models
{
    public class Imagem : INotifyPropertyChanged
    {
        private int _quantidade = 1;
        private double _rotacao;
        private double _posicaoX;
        private double _posicaoY;
        
        public string NomeArquivo { get; set; } = string.Empty;
        public string CaminhoCompleto { get; set; } = string.Empty;
        public int DPI { get; set; }
        public double LarguraCm { get; set; }
        public double AlturaCm { get; set; }
        public long TamanhoBytes { get; set; }
        public bool Selecionada { get; set; } = true;
        
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
        
        public double Rotacao
        {
            get => _rotacao;
            set
            {
                _rotacao = value % 360;
                OnPropertyChanged(nameof(Rotacao));
            }
        }
        
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
        
        public event PropertyChangedEventHandler? PropertyChanged;
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
