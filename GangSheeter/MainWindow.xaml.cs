using GangSheeter.Services;
using GangSheeter.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace GangSheeter
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }
    }
}
