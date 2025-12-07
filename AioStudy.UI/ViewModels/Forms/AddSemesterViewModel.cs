using AioStudy.Core.Data.Services;
using AioStudy.Models;
using AioStudy.UI.Commands;
using AioStudy.UI.Views.Forms;
using AioStudy.UI.WpfServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AioStudy.UI.ViewModels.Forms
{
    public class AddSemesterViewModel : ViewModelBase
    {
        private readonly SemesterViewModel _semesterViewModel;
        private readonly SemesterDbService _semesterDbService;

        private string _semesterName = string.Empty;
        private string _description = string.Empty; 
        private Color? _semesterColor;
        private DateTime _startDate = DateTime.Now;
        private DateTime _endDate = DateTime.Now.AddMonths(6);

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string SemesterName
        {
            get => _semesterName;
            set
            {
                _semesterName = value;
                OnPropertyChanged(nameof(SemesterName));
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        public Color? SemesterColor
        {
            get => _semesterColor;
            set
            {
                _semesterColor = value;
                OnPropertyChanged(nameof(SemesterColor));
            }
        }

        public RelayCommand CancelAddSemesterCommand { get; }
        public RelayCommand AddSemesterCommand { get; }

        public AddSemesterViewModel(SemesterViewModel semesterViewModel, SemesterDbService semesterDbService)
        {
            _semesterViewModel = semesterViewModel;
            _semesterDbService = semesterDbService;

            CancelAddSemesterCommand = new RelayCommand(CancelAddSemester);
            AddSemesterCommand = new RelayCommand(AddSemester);
        }

        private async void AddSemester(object? obj)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SemesterName))
                {
                    await ToastService.ShowErrorAsync("Error", "Semester name cannot be empty!");
                    return;
                }

                if (StartDate == default || EndDate == default)
                {
                    await ToastService.ShowErrorAsync("Error", "Start date and end date must be set!");
                    return;
                }

                if (StartDate >= EndDate)
                {
                    await ToastService.ShowErrorAsync("Error", "Start date must be before end date!");
                    return;
                }

                var newSemester = new Semester
                {
                    Name = SemesterName,
                    Description = Description,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    Color = SemesterColor.HasValue ? SemesterColor.Value.ToString() : null
                };

                var res = await _semesterDbService.CreateSemesterAsync(newSemester);

                if (res != null)
                {
                    _semesterViewModel.RefreshSemesters();
                    Application.Current.Windows.OfType<AddSemesterView>().FirstOrDefault()?.Close();
                    await ToastService.ShowSuccessAsync("Success", $"Semester '{SemesterName}' added successfully!");
                }
                else
                {
                    await ToastService.ShowErrorAsync("Error", "Failed to add semester.");
                }
            }
            catch (Exception)
            {
                await ToastService.ShowErrorAsync("Error", "Failed to add semester.");
            }
        }

        private async void CancelAddSemester(object? obj)
        {
            Application.Current.Windows.OfType<AddSemesterView>().FirstOrDefault()?.Close();
            await ToastService.ShowInfoAsync("Info", "Cancelled adding semester");
        }
    }
}
