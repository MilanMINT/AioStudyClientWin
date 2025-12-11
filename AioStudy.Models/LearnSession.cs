using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Models
{
    public class LearnSession
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime StartTime { get; set; } = DateTime.Now;
        [AllowNull]
        public int? LearnedModuleId { get; set; } = null;
        [AllowNull]
        public Module? LearnedModule { get; set; } = null;
        [AllowNull]
        public DateTime? EndTime { get; set; } = null;
        public bool SessionCompleted { get; set; } = false;
        public int CurrentLearnedMinutes { get; set; } = 0;

        public LearnSession() { }

        public TimeSpan GetTotalDuration()
        {
            if (EndTime.HasValue)
            {
                var duration = EndTime.Value - StartTime;
                return duration;
            }
            else
            {
                return TimeSpan.Zero;
            }
        }
    }
}
