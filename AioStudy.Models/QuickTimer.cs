using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Models
{
    public class QuickTimer
    {
        [Key]
        public int Id { get; set; }
        [AllowNull]
        public int? ModuleId { get; set; } = null;
        [AllowNull]
        public Module? Module { get; set; } = null;
        [Required]
        public TimeSpan Duration { get; set; }

        public QuickTimer(){}

        public QuickTimer(int moduleId, TimeSpan duration)
        {
            Duration = duration;
        }
    }
}
