using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Models.DailyPlannerModels
{
    public class DailyTask
    {
        [Key]
        public int Id { get; set; }
        [AllowNull]
        public string? Name { get; set; } = "Task";
        [AllowNull]
        public TimeOnly? StartTime { get; set; }
        [AllowNull]
        public TimeOnly? EndTime { get; set; }
        public bool IsCompleted { get; set; } = false;
        [AllowNull]
        public string? Priority { get; set; }
        [Required]
        public DailyPlan DailyPlan { get; set; }
        [Required]
        public int DailyPlanId { get; set; }
        [AllowNull]
        public Module? Module { get; set; }
        [AllowNull]
        public int? ModuleId { get; set; }

        public DailyTask(){}

        public DailyTask(DailyPlan dailyPlan, TimeOnly? startTime = null, TimeOnly? endTime = null, string? name = null, Module? module = null, string? priority = null)
        {
            StartTime = startTime;
            EndTime = endTime;
            Priority = priority;
            DailyPlan = dailyPlan;
            Module = module;
            Name = name;
        }
    }
}
