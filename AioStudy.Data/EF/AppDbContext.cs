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
        public DbSet<User> Users { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<DailyModuleStats> DailyModuleStats { get; set; }
        public DbSet<LearnSession> LearnSessions { get; set; }  

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Module>()
                .HasOne(m => m.Semester)
                .WithMany()
                .HasForeignKey(m => m.SemesterId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DailyModuleStats>()
                .HasOne(d => d.Module)
                .WithMany()
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);
        }
    }
}
