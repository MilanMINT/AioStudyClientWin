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
        public DateTime? EndTime { get; set; } = null;
        public int CurrentLearnedMinutes { get; set; } = 0;


        public LearnSession() { }
    }
}
