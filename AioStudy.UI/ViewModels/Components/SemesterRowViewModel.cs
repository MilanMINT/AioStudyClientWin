using AioStudy.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.UI.ViewModels.Components
{
    public class SemesterRowViewModel
    {
        public Semester Semester { get; set; } = default!;
        public ObservableCollection<Module> Modules { get; set; } = new();
        public int TotalCredits { get; set; }
        public int ModulesCount { get; set; }
        public int SemesterNumber { get; set; }
        public int ParentWidth { get; set; } = 1000;
        public int ModulesContainerWidth { get; set; }
        public int SemesterInfoContainerWidth { get; set; }
        public int MarginCompensation { get; set; } = 8;
    }
}
