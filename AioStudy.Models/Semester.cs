using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Models
{
    public class Semester
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int LearnedSemesterMinutes { get; set; } = 0;
        [AllowNull]
        public DateTime StartDate { get; set; }
        [AllowNull]
        public DateTime EndDate { get; set; }
        [AllowNull]
        public string? Color { get; set; } = null;
        [AllowNull]
        public string? Description { get; set; } = null;
        [AllowNull]
        public float? AverageSemesterGrade { get; set; } = null;
        [NotMapped]
        public string StartDateString { get { return StartDate.ToString("dd.MM.yyyy"); } }
        [NotMapped]
        public string EndDateString { get { return EndDate.ToString("dd.MM.yyyy"); } }
        [NotMapped]
        public int ModulesCount { get; set; }
        [NotMapped]
        public string ModulesCountString => $"{ModulesCount} Module(s)";


        public Semester() { }

        public Semester(string name, DateTime startDate, DateTime endDate)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
