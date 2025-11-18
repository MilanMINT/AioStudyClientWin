using AioStudy.UI.ViewModels;
using AioStudy.UI.Views.Controls;
using AioStudy.UI.WpfServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AioStudy.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GlobalHotKeyService _hotKeyService;
        private PomodoroViewModel _pomodoroViewModel;

        public ToastNotification GetToastOverlay()
        {
            return ToastOverlay;
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<MainViewModel>();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            _hotKeyService = App.ServiceProvider.GetRequiredService<GlobalHotKeyService>();
            _pomodoroViewModel = App.ServiceProvider.GetRequiredService<PomodoroViewModel>();

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);


        private void MainControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }

        private void MainControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void BtnCloseApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void BtnMaxApp_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void BtnMinApp_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Hole Window Handle
            var helper = new WindowInteropHelper(this);
            _hotKeyService.Initialize(helper.Handle);

            // Verbinde Events mit PomodoroViewModel
            if (_pomodoroViewModel != null)
            {
                _hotKeyService.ToggleTimerRequested += (s, args) =>
                {
                    _pomodoroViewModel.ControlTimerCommand?.Execute(null);
                };

                _hotKeyService.ResetTimerRequested += (s, args) =>
                {
                    _pomodoroViewModel.ResetTimerCommand?.Execute(null);
                };
            }
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            _hotKeyService?.Dispose();
        }
    }
}   