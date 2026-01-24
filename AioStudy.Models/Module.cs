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
    public class Module
    {
        [Key]
        public int Id { get; set; }
        [AllowNull]
        public int? SemesterId { get; set; } = null;
        [AllowNull]
        public Semester? Semester { get; set; } = null;
        [Required]
        public string Name { get; set; } 
        [Required]
        public int LearnedMinutes { get; set; } = 0;
        [Required]
        public int ModuleAttempt { get; set; } = 1;
        [AllowNull]
        public DateTime? ExamDate { get; set; }
        [Required]
        public int Weighting { get; set; } = 1;
        [AllowNull]
        public string? ExamStatus { get; set; } = null;
        [AllowNull]
        public float? Grade { get; set; } = null;
        [AllowNull]
        public int? ModuleCredits { get; set; } = null;
        [AllowNull]
        public bool IsKeyCompetence { get; set; } = false;
        [AllowNull]
        public string? Color { get; set; } = null;
        [Required]
        public DateTime Created { get; set; } = DateTime.Now;
        [NotMapped]
        public string CreatedString { get { return Created.ToString("dd.MM.yyyy"); } }
        [NotMapped]
        public string? ExamDateString { get { return ExamDate?.ToString("dd.MM.yyyy HH:mm"); } }

        public Module() { }
        public Module(string name)
        {
            Name = name;
        }

        public Module(string name, Semester? semester, int? moduleCredits, string? color)
        {
            Name = name;
            Semester = semester;
            ModuleCredits = moduleCredits;
            Color = color;
        }
    }
}
