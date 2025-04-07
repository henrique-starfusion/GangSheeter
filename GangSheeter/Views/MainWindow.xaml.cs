using System.Windows;

namespace GangSheeter.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            // Logic to open images
            StatusText.Text = "Loading images...";
            // Simulate loading images
            // After loading images, update the status
            StatusText.Text = "Images loaded successfully.";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Logic to save images
            StatusText.Text = "Saving images...";
            // Simulate saving images
            // After saving images, update the status
            StatusText.Text = "Images saved successfully.";
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            // Logic for undo
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            // Logic for redo
        }

        private void ToggleStatusBar_Click(object sender, RoutedEventArgs e)
        {
            // Logic to toggle status bar visibility
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            // Logic to show about information
        }
    }
}
