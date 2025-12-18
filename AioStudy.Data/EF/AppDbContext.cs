using AioStudy.Models;
using AioStudy.Models.DailyPlannerModels;
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
        public DbSet<QuickTimer> QuickTimers { get; set; }
        public DbSet<DailyPlan> DailyPlans { get; set; }
        public DbSet<DailyTask> DailyTasks { get; set; }
        public DbSet<DailySubTask> DailySubTasks { get; set; }

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

            modelBuilder.Entity<LearnSession>()
                .HasOne(l => l.LearnedModule)
                .WithMany()
                .HasForeignKey(l => l.LearnedModuleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<QuickTimer>()
                .HasOne(q => q.Module)
                .WithMany()
                .HasForeignKey(q => q.ModuleId)
                .OnDelete(DeleteBehavior.SetNull);

            var dateOnlyConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateOnly, string>(
                d => d.ToString("dd-MM-yyyy"),
                s => DateOnly.Parse(s));

            var timeOnlyConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<TimeOnly, string>(
                t => t.ToString("HH:mm:ss"),
                s => TimeOnly.Parse(s));

            modelBuilder.Entity<DailyPlan>(b =>
            {
                b.Property(p => p.Date)
                    .HasConversion(dateOnlyConverter)
                    .HasColumnType("TEXT")
                    .IsRequired();

                b.HasMany<DailyTask>()
                 .WithOne(t => t.DailyPlan)
                 .HasForeignKey(t => t.DailyPlanId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DailyTask>(b =>
            {
                b.Property(t => t.StartTime)
                    .HasConversion(timeOnlyConverter)
                    .HasColumnType("TEXT")
                    .IsRequired();

                b.Property(t => t.EndTime)
                    .HasConversion(timeOnlyConverter)
                    .HasColumnType("TEXT")
                    .IsRequired();

                b.HasMany<DailySubTask>()
                 .WithOne(s => s.DailyTask)
                 .HasForeignKey(s => s.DailyTaskId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(t => t.Module)
                 .WithMany()
                 .HasForeignKey(t => t.ModuleId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<DailySubTask>(b =>
            {
                b.Property(s => s.Name).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
