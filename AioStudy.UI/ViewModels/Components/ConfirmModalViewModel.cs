using AioStudy.UI.Commands;
using AioStudy.UI.Views.Components;
using AioStudy.UI.Views.Forms;
using AioStudy.UI.WpfServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AioStudy.UI.ViewModels.Components
{
    public class ConfirmModalViewModel : ViewModelBase
    {
        private string _title;
        private string _message;

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public bool? DialogResult { get; set; }

        public RelayCommand ConfirmCommand { get; }
        public RelayCommand CancelCommand { get; }

        public ConfirmModalViewModel(string title, string message = "")
        {
            Title = title;
            Message = message;
            CancelCommand = new RelayCommand(CloseModal);
            ConfirmCommand = new RelayCommand(ConfirmAction);
        }

        private void ConfirmAction(object? obj)
        {
            var modal = Application.Current.Windows.OfType<ConfirmModal>().FirstOrDefault();
            if (modal != null)
            {
                DialogResult = true;
                modal.DialogResult = true;
                modal.Close();
            }
        }

        private async void CloseModal(object? obj)
        {
            var modal = Application.Current.Windows.OfType<ConfirmModal>().FirstOrDefault();
            if (modal != null)
            {
                DialogResult = false;
                modal.DialogResult = false;
                modal.Close();
                await ToastService.ShowInfoAsync("Action Cancelled", "The action has been cancelled.");
            }
        }
    }
}
