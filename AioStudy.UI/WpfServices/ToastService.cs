using AioStudy.UI.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AioStudy.UI.WpfServices
{
    public static class ToastService
    {
        public static async Task ShowSuccessAsync(string title, string message, int durationMs = 3000)
        {
            await ShowToastAsync(title, message, ToastNotification.ToastType.Success, durationMs);
        }

        public static async Task ShowErrorAsync(string title, string message, int durationMs = 4000)
        {
            await ShowToastAsync(title, message, ToastNotification.ToastType.Error, durationMs);
        }

        public static async Task ShowWarningAsync(string title, string message, int durationMs = 3500)
        {
            await ShowToastAsync(title, message, ToastNotification.ToastType.Warning, durationMs);
        }

        public static async Task ShowInfoAsync(string title, string message, int durationMs = 3000)
        {
            await ShowToastAsync(title, message, ToastNotification.ToastType.Info, durationMs);
        }


        private static async Task ShowToastAsync(string title, string message, ToastNotification.ToastType type, int duration)
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    var toastControl = mainWindow.GetToastOverlay();

                    if (toastControl != null)
                    {
                        await toastControl.ShowAsync(title, message, type, duration);
                    }
                }
            });
        }
    }
}
