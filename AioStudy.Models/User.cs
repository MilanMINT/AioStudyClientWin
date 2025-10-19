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
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Username { get; set; }
        public int LearnedMinutes { get; set; } = 0;
        [AllowNull]
        public float? CurrentGradeAverage { get; set; }
        [AllowNull]
        public string? UkNumber { get; set; } = "";
        [AllowNull]
        public string? Email { get; set; } = "";
        [AllowNull]
        public int? MatrikelNumber { get; set; } = null;

        public DateTime Created { get; set; } = DateTime.Now;

        [NotMapped]
        public string CreatedString { get { return Created.ToString("dd.MM.yyyy HH:mm"); } }

        public User(){}

        public User(string username, string? ukNumber = "", string? email = "", int? matrikelNumber = null)
        {
            Username = username;
            UkNumber = ukNumber;
            Email = email;
            MatrikelNumber = matrikelNumber;
        }
    }
}
