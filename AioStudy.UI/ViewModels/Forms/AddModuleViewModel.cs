using AioStudy.UI.Commands;
using AioStudy.UI.Views.Forms;
using AioStudy.UI.WpfServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AioStudy.UI.ViewModels.Forms
{
    public class AddModuleViewModel : ViewModelBase
    {
        private readonly ModulesViewModel _modulesViewModel;

        public RelayCommand CancelAddModuleCommand { get; }

        public AddModuleViewModel(ModulesViewModel modulesViewModel)
        {
            _modulesViewModel = modulesViewModel;
            CancelAddModuleCommand = new RelayCommand(async _ => await CancelAddModule());
        }

        private async Task CancelAddModule()
        {
            Application.Current.Windows.OfType<AddModuleView>().FirstOrDefault()?.Close();
            await ToastService.ShowSuccessAsync("Abgebrochen", "Das Hinzufügen des Moduls wurde abgebrochen.");
        }
    }
}
