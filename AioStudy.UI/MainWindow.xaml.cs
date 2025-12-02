using AioStudy.Core.Services;
using AioStudy.UI.ViewModels;
using AioStudy.UI.ViewModels.Components;
using AioStudy.UI.Views;
using AioStudy.UI.Views.Components;
using AioStudy.UI.Views.Controls;
using AioStudy.UI.WpfServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace AioStudy.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GlobalHotKeyService _hotKeyService;
        private readonly PomodoroViewModel _pomodoroViewModel;
        private readonly MainViewModel _mainViewModel;
        private TimerOverlayWindow _timerOverlayWindow;
        private int _currentCornerPosition = 0;

        public ToastNotification GetToastOverlay()
        {
            return ToastOverlay;
        }

        public MainWindow()
        {
            InitializeComponent();
            _mainViewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();
            DataContext = _mainViewModel;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            _hotKeyService = App.ServiceProvider.GetRequiredService<GlobalHotKeyService>();
            _pomodoroViewModel = App.ServiceProvider.GetRequiredService<PomodoroViewModel>();

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
            StateChanged += MainWindow_StateChanged;
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

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            _mainViewModel.IsMainWindowMinimized = (WindowState == WindowState.Minimized);

            UpdateTimerOverlayWindow();
        }

        private void UpdateTimerOverlayWindow()
        {
            if (_mainViewModel.TimerOverlayViewModel.IsVisible)
            {
                if (_timerOverlayWindow == null)
                {
                    _timerOverlayWindow = new TimerOverlayWindow
                    {
                        DataContext = _mainViewModel.TimerOverlayViewModel
                    };

                    PositionTimerOverlayWindow(0);

                    _mainViewModel.TimerOverlayViewModel.PropertyChanged += TimerOverlayViewModel_PropertyChanged;
                }

                _timerOverlayWindow.Show();
            }
            else
            {
                _timerOverlayWindow?.Hide();
            }
        }

        private void PositionTimerOverlayWindow(int cornerPosition)
        {
            if (_timerOverlayWindow == null) return;

            const double margin = 20;
            var workArea = SystemParameters.WorkArea;

            switch (cornerPosition)
            {
                case 0: // UntenRechts
                    _timerOverlayWindow.Left = workArea.Right - _timerOverlayWindow.Width - margin;
                    _timerOverlayWindow.Top = workArea.Bottom - _timerOverlayWindow.Height - margin;
                    break;

                case 1: // UntenLinks
                    _timerOverlayWindow.Left = workArea.Left + margin;
                    _timerOverlayWindow.Top = workArea.Bottom - _timerOverlayWindow.Height - margin;
                    break;

                case 2: // ObenLinks
                    _timerOverlayWindow.Left = workArea.Left + margin;
                    _timerOverlayWindow.Top = workArea.Top + margin;
                    break;

                case 3: // ObenRechts
                    _timerOverlayWindow.Left = workArea.Right - _timerOverlayWindow.Width - margin;
                    _timerOverlayWindow.Top = workArea.Top + margin;
                    break;
            }
        }

        private void MoveTimerOverlayToNextCorner()
        {
            _currentCornerPosition = (_currentCornerPosition + 1) % 4;
            PositionTimerOverlayWindow(_currentCornerPosition);
        }

        private void TimerOverlayViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TimerOverlayViewModel.IsVisible))
            {
                UpdateTimerOverlayWindow();
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            _hotKeyService.Initialize(helper.Handle);

            if (_pomodoroViewModel != null)
            {
                _hotKeyService.ToggleTimerRequested += (s, args) =>
                {
                    _pomodoroViewModel.ControlTimerCommand?.RaiseCanExecuteChanged();

                    if (_pomodoroViewModel.ControlTimerCommand != null && !_pomodoroViewModel.IsBreakActive)
                    {
                        Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            _pomodoroViewModel.ControlTimerCommand.Execute(null);
                        }, System.Windows.Threading.DispatcherPriority.Input);
                    }
                };

                _hotKeyService.ResetTimerRequested += (s, args) =>
                {
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        _pomodoroViewModel.ResetTimerCommand?.Execute(null);
                    }, System.Windows.Threading.DispatcherPriority.Input);
                };

                _hotKeyService.MoveTimerWindowRequested += (s, args) =>
                {
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        if (_timerOverlayWindow != null && _timerOverlayWindow.IsVisible)
                        {
                            MoveTimerOverlayToNextCorner();
                        }
                    }, System.Windows.Threading.DispatcherPriority.Input);
                };
            }
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            if (_mainViewModel.TimerOverlayViewModel != null)
            {
                _mainViewModel.TimerOverlayViewModel.PropertyChanged -= TimerOverlayViewModel_PropertyChanged;
            }

            _hotKeyService?.Dispose();
            _timerOverlayWindow?.Close();
        }
    }
}