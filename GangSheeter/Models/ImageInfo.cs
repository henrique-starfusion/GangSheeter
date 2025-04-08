using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GangSheeter.Models
{
    public class ImageInfo : INotifyPropertyChanged
    {
        private string _filePath = string.Empty;
        private string _fileName = string.Empty;
        private int _copies = 1;
        private double _widthCm;
        private double _heightCm;
        private int _dpi = 300;

        public Guid Id { get; } = Guid.NewGuid();

        public string FilePath
        {
            get => _filePath;
            set => SetField(ref _filePath, value);
        }

        public string FileName
        {
            get => _fileName;
            set => SetField(ref _fileName, value);
        }

        public int Copies
        {
            get => _copies;
            set => SetField(ref _copies, Math.Max(1, value));
        }

        public double WidthCm
        {
            get => _widthCm;
            set => SetField(ref _widthCm, value);
        }

        public double HeightCm
        {
            get => _heightCm;
            set => SetField(ref _heightCm, value);
        }

        public int DPI
        {
            get => _dpi;
            set => SetField(ref _dpi, Math.Max(72, value));
        }

        // Helper methods for pixel dimensions
        public double WidthPx => WidthCm * DPI / 2.54;
        public double HeightPx => HeightCm * DPI / 2.54;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
