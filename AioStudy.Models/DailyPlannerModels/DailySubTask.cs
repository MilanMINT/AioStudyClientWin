using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Models.DailyPlannerModels
{
    public class DailySubTask
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [AllowNull]
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        [Required]
        public DailyTask DailyTask { get; set; }
        [Required]
        public int DailyTaskId { get; set; }

        public DailySubTask(){}

        public DailySubTask(string name, DailyTask dailyTask, string? description = null)
        {
            Name = name;
            DailyTask = dailyTask;
            Description = description;
        }
    }
}
