using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Models.DailyPlannerModels
{
    public class DailyPlan
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public DailyPlan(){ }

        public DailyPlan(DateOnly date)
        {
            Date = date;
        }
    }
}
