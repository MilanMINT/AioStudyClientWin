using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Models
{
    public class DailyModuleStats
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Date { get; set; } = DateTime.Today;
        [Required]
        public int LearnedMinutes { get; set; } = 0;
        public int SessionsCount { get; set; } = 0;
        [AllowNull]
        public int? ModuleId { get; set; } = null;
        [AllowNull]
        public Module? Module { get; set; } = null;

        public DailyModuleStats(){}

        public DailyModuleStats(int? moduleId = null)
        {
            ModuleId = moduleId;
        }
    }
}
