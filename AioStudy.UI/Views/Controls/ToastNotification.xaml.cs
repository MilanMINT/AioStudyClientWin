using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AioStudy.UI.Views.Controls
{
    /// <summary>
    /// Interaction logic for ToastNotification.xaml
    /// </summary>
    public partial class ToastNotification : UserControl, INotifyPropertyChanged
    {
        public enum ToastType { Success, Error, Warning, Info }

        private string _title = string.Empty;
        private string _message = string.Empty;
        private string _icon = string.Empty;
        private Color _borderColor;
        private Brush _iconBackground = Brushes.Transparent;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public string Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        public Color BorderColor
        {
            get => _borderColor;
            set => SetProperty(ref _borderColor, value);
        }

        public Brush IconBackground
        {
            get => _iconBackground;
            set => SetProperty(ref _iconBackground, value);
        }

        public ToastNotification()
        {
            InitializeComponent();
            DataContext = this;
        }

        public async Task ShowAsync(string title, string message, ToastType type, int durationMs = 3000)
        {
            Title = title;
            Message = message;
            SetupAppearance(type);

            Visibility = Visibility.Visible;

            // Show Animation
            var showStoryboard = (Storyboard)Resources["ShowAnimation"];
            showStoryboard.Begin(ToastContainer);

            // Auto-Hide nach Duration
            await Task.Delay(durationMs);
            await HideAsync();
        }

        public async Task HideAsync()
        {
            var hideStoryboard = (Storyboard)Resources["HideAnimation"];
            hideStoryboard.Completed += (s, e) => Visibility = Visibility.Collapsed;
            hideStoryboard.Begin(ToastContainer);

            await Task.Delay(200); // Animation duration
        }

        private void SetupAppearance(ToastType type)
        {
            switch (type)
            {
                case ToastType.Success:
                    Icon = "✓";
                    BorderColor = Color.FromRgb(72, 187, 120); // Green
                    IconBackground = new SolidColorBrush(Color.FromRgb(72, 187, 120));
                    break;
                case ToastType.Error:
                    Icon = "✕";
                    BorderColor = Color.FromRgb(239, 68, 68); // Red
                    IconBackground = new SolidColorBrush(Color.FromRgb(239, 68, 68));
                    break;
                case ToastType.Warning:
                    Icon = "⚠";
                    BorderColor = Color.FromRgb(245, 158, 11); // Orange
                    IconBackground = new SolidColorBrush(Color.FromRgb(245, 158, 11));
                    break;
                case ToastType.Info:
                    Icon = "ⓘ";
                    BorderColor = Color.FromRgb(59, 130, 246); // Blue
                    IconBackground = new SolidColorBrush(Color.FromRgb(59, 130, 246));
                    break;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal async Task CloseAsync()
        {
            await HideAsync();
        }

        private void ToastContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ = CloseAsync();
        }
    }
}
