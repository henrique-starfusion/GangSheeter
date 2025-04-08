using System.ComponentModel;
using System.Runtime.CompilerServices;

using GangSheeter.Models;

public class ImageSheet : INotifyPropertyChanged
{
    public ImageInfo Image { get; set; } = null!;

    private double _x;
    private double _y;
    private double _angle;
    private bool _isRotated;

    public double X
    {
        get => _x;
        set => SetField(ref _x, value);
    }

    public double Y
    {
        get => _y;
        set => SetField(ref _y, value);
    }

    public double Angle
    {
        get => _angle;
        set => SetField(ref _angle, value);
    }

    public bool IsRotated
    {
        get => _isRotated;
        set => SetField(ref _isRotated, value);
    }

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