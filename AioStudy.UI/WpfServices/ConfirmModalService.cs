using AioStudy.UI.ViewModels.Components;
using AioStudy.UI.Views.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AioStudy.UI.WpfServices
{
    public static class ConfirmModalService
    {
        public static bool Show(string title, string message = "")
        {
            bool result = false;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var confirmModal = new ConfirmModal();
                var viewModel = new ConfirmModalViewModel(title, message);
                confirmModal.DataContext = viewModel;
                confirmModal.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
                                   ?? Application.Current.MainWindow;
                confirmModal.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                var dialogResult = confirmModal.ShowDialog();
                result = dialogResult == true;
            });

            return result;
        }

        public static bool ShowDeleteConfirmation(string itemName)
        {
            return Show(
                "Confirm Delete",
                $"Are you sure you want to delete '{itemName}'?\nThis action cannot be undone."
            );
        }

        public static bool ShowConfirmation(string message)
        {
            return Show("Confirm Action", message);
        }

        public static bool ShowConfirmation(string title, string message, Window owner = null)
        {
            bool result = false;

            Action showDialog = () =>
            {
                var confirmModal = new ConfirmModal();
                var viewModel = new ConfirmModalViewModel(title, message);
                confirmModal.DataContext = viewModel;
                confirmModal.Owner = owner ?? Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
                                         ?? Application.Current.MainWindow;
                confirmModal.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                var dialogResult = confirmModal.ShowDialog();
                result = dialogResult == true;
            };

            if (Application.Current.Dispatcher.CheckAccess())
            {
                showDialog();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(showDialog);
            }
            return result;
        }
    }
}
