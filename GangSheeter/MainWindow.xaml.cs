using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Fluent;
using GangSheeter.Models;
using GangSheeter.ViewModels;

namespace GangSheeter
{
    public partial class MainWindow : RibbonWindow
    {
        private readonly MainViewModel _viewModel;
        private ListViewItem? _draggedItem;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel(PreviewCanvas, PreviewGrid);
            DataContext = _viewModel;

            // Configurar eventos de drag and drop para a lista de imagens
            ListViewImagens.PreviewMouseLeftButtonDown += ListViewImagens_PreviewMouseLeftButtonDown;
            ListViewImagens.PreviewMouseMove += ListViewImagens_PreviewMouseMove;
            ListViewImagens.PreviewMouseLeftButtonUp += ListViewImagens_PreviewMouseLeftButtonUp;
        }

        private void ListViewImagens_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = GetListViewItemFromPoint(ListViewImagens, e.GetPosition(ListViewImagens));
            if (item != null)
            {
                _draggedItem = item;
            }
        }

        private void ListViewImagens_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_draggedItem != null && e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(ListViewImagens, _draggedItem.DataContext, DragDropEffects.Move);
            }
        }

        private void ListViewImagens_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _draggedItem = null;
        }

        private void ListViewImagens_Drop(object sender, DragEventArgs e)
        {
            var sourceImagem = e.Data.GetData(typeof(Imagem)) as Imagem;
            var targetItem = GetListViewItemFromPoint(ListViewImagens, e.GetPosition(ListViewImagens));
            
            if (sourceImagem != null && targetItem?.DataContext is Imagem targetImagem)
            {
                _viewModel.ReordenarImagens(sourceImagem, targetImagem);
            }
        }

        private static ListViewItem? GetListViewItemFromPoint(ListView listView, Point point)
        {
            var element = listView.InputHitTest(point) as DependencyObject;
            while (element != null)
            {
                if (element is ListViewItem item)
                    return item;
                element = VisualTreeHelper.GetParent(element);
            }
            return null;
        }
    }
}
