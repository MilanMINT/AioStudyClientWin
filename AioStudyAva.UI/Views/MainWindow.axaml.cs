using AioStudyAva.UI.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AioStudyAva.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            // Enable window dragging
            this.PointerPressed += OnPointerPressed;
        }

        private void OnPointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                this.BeginMoveDrag(e);
            }
        }

        private void BtnCloseApp_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnMaxApp_Click(object? sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void BtnMinApp_Click(object? sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}