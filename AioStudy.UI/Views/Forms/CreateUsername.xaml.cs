using AioStudy.UI.ViewModels.Forms;
using System;
using System.Windows;

namespace AioStudy.UI.Views.Forms
{
    public partial class CreateUsername : Window
    {
        public CreateUsername()
        {
            InitializeComponent();
            DataContextChanged += CreateUsername_DataContextChanged;
        }

        private void CreateUsername_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is CreateUsernameViewModel oldViewModel)
                oldViewModel.RequestClose -= ViewModel_RequestClose;
            if (e.NewValue is CreateUsernameViewModel newViewModel)
                newViewModel.RequestClose += ViewModel_RequestClose;
        }

        private void ViewModel_RequestClose(object? sender, bool created)
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    DialogResult = created;
                }
                catch (InvalidOperationException)
                {
                    // Ignore
                }
                Close();
            });
        }
    }
}