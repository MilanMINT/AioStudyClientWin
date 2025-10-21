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


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    string appDataPath = Path.Combine(
        //        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        //        "AioStudy"
        //    );
        //    Directory.CreateDirectory(appDataPath);
        //    string dbPath = System.IO.Path.Combine(appDataPath, "aiostudy.db");

        //    optionsBuilder.UseNpgsql("Host=ep-young-snow-ag8hi4ql-pooler.c-2.eu-central-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_5U1dTCFncLMt;SSL Mode=Require;Trust Server Certificate=true;");
        //}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // Explizite Konfiguration für PostgreSQL
        //    modelBuilder.Entity<Semester>(entity =>
        //    {
        //        entity.HasKey(e => e.Id);
        //        entity.Property(e => e.Id).UseIdentityByDefaultColumn();
        //        entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        //        entity.Property(e => e.StartDate).IsRequired();
        //        entity.Property(e => e.EndDate).IsRequired();
        //        entity.Property(e => e.Color).HasMaxLength(50);
        //        entity.Property(e => e.Description).HasMaxLength(1000);
        //        entity.Property(e => e.AverageSemesterGrade);
        //    });

        //    modelBuilder.Entity<User>(entity =>
        //    {
        //        entity.HasKey(e => e.Id);
        //        entity.Property(e => e.Id).UseIdentityByDefaultColumn();
        //        entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
        //        entity.Property(e => e.LearnedMinutes).HasDefaultValue(0);
        //        entity.Property(e => e.CurrentGradeAverage);
        //        entity.Property(e => e.UkNumber).HasMaxLength(50);
        //        entity.Property(e => e.Email).HasMaxLength(200);
        //        entity.Property(e => e.MatrikelNumber);
        //        entity.Property(e => e.Created).HasDefaultValueSql("NOW()");
        //    });

        //    modelBuilder.Entity<Module>(entity =>
        //    {
        //        entity.HasKey(e => e.Id);
        //        entity.Property(e => e.Id).UseIdentityByDefaultColumn();
        //        entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        //        entity.Property(e => e.LearnedMinutes).HasDefaultValue(0);
        //        entity.Property(e => e.ExamDate);
        //        entity.Property(e => e.Grade);
        //        entity.Property(e => e.Color).HasMaxLength(50);
        //        entity.Property(e => e.Created).HasDefaultValueSql("NOW()");

        //        entity.HasOne(m => m.Semester)
        //            .WithMany()
        //            .HasForeignKey(m => m.SemesterId)
        //            .OnDelete(DeleteBehavior.SetNull);
        //    });

        //    modelBuilder.Entity<DailyModuleStats>(entity =>
        //    {
        //        entity.HasKey(e => e.Id);
        //        entity.Property(e => e.Id).UseIdentityByDefaultColumn();
        //        entity.Property(e => e.Date).HasDefaultValueSql("CURRENT_DATE");
        //        entity.Property(e => e.LearnedMinutes).HasDefaultValue(0);
        //        entity.Property(e => e.SessionsCount).HasDefaultValue(0);

        //        entity.HasOne(d => d.Module)
        //            .WithMany()
        //            .HasForeignKey(d => d.ModuleId)
        //            .OnDelete(DeleteBehavior.SetNull);
        //    });

        //    modelBuilder.Entity<LearnSession>(entity =>
        //    {
        //        entity.HasKey(e => e.Id);
        //        entity.Property(e => e.Id).UseIdentityByDefaultColumn();
        //        entity.Property(e => e.StartTime).HasDefaultValueSql("NOW()");
        //        entity.Property(e => e.EndTime);
        //        entity.Property(e => e.CurrentLearnedMinutes).HasDefaultValue(0);
        //    });

        //    base.OnModelCreating(modelBuilder);
        //}

        //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        System.Diagnostics.Debug.WriteLine("AppDbContext: SaveChangesAsync wird ausgeführt...");
        //        var result = await base.SaveChangesAsync(cancellationToken);
        //        System.Diagnostics.Debug.WriteLine($"AppDbContext: {result} Änderungen gespeichert");
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"AppDbContext: Fehler beim Speichern: {ex}");
        //        throw;
        //    }
        //}
    }
}
