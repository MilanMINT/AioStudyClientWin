using AioStudy.Core.Data.Services;
using AioStudy.Models;
using AioStudy.UI.Commands;
using AioStudy.UI.WpfServices;
using System;
using System.Threading.Tasks;

namespace AioStudy.UI.ViewModels.Forms
{
    public class CreateUsernameViewModel : ViewModelBase
    {
        private string _username = string.Empty;
        private readonly UserDbService _userDbService;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        public RelayCommand CancelCreateUserCommand { get; }
        public RelayCommand CreateUserCommand { get; }

        public event EventHandler<bool>? RequestClose;

        public CreateUsernameViewModel(UserDbService userDbService)
        {
            _userDbService = userDbService;
            CancelCreateUserCommand = new RelayCommand(ExecuteCancelCreateUser);
            CreateUserCommand = new RelayCommand(async _ => await ExecuteCreateUserAsync());
        }

        private async Task ExecuteCreateUserAsync()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                await ToastService.ShowErrorAsync("Error", "Username cant be empty!");
                return;
            }

            var user = new User { Username = this.Username };

            try
            {
                var created = await _userDbService.CreateUserAsync(user);
                if (created != null)
                {
                    RequestClose?.Invoke(this, true);
                    await ToastService.ShowSuccessAsync("Success", $"User with Name: '{Username}' successfully created!");
                }
                else
                {
                    RequestClose?.Invoke(this, false);
                    await ToastService.ShowErrorAsync("Error", $"User with Name: '{Username}' could not be created.");
                }
            }
            catch (Exception)
            {
                RequestClose?.Invoke(this, false);
                await ToastService.ShowErrorAsync("Error", $"User with Name: '{Username}' could not be created.");
            }
        }

        private void ExecuteCancelCreateUser(object? obj)
        {
            Environment.Exit(0);
        }
    }
}

//await ToastService.ShowSuccessAsync("Success", $"Module with Name: '{ModuleName}' successfully created!");
