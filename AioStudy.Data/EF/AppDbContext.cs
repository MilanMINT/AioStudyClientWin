using AioStudy.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Data.EF
{
    public class AppDbContext : DbContext
    {
        //public DbSet<Person> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "AioStudy"
            );
            Directory.CreateDirectory(appDataPath);
            string dbPath = System.IO.Path.Combine(appDataPath, "aiostudy.db");

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}
