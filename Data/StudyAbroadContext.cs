using Microsoft.EntityFrameworkCore;
using COMP584StudyAbroadServer.Models;

namespace COMP584StudyAbroadServer.Data;

public class StudyAbroadContext : DbContext
{
    public StudyAbroadContext(DbContextOptions<StudyAbroadContext> options)
        : base(options)
    {
    }

    public DbSet<University> Universities { get; set; } = null!;
    public DbSet<StudyProgram> StudyPrograms { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<University>(entity =>
        {
            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(u => u.Country)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.City)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Type)
                .HasMaxLength(50);
        });

        modelBuilder.Entity<StudyProgram>(entity =>
        {
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.DegreeLevel)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(p => p.Language)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(p => p.TuitionPerYear)
                .HasColumnType("decimal(18,2)");

            entity.HasOne(p => p.University)
                .WithMany(u => u.Programs)
                .HasForeignKey(p => p.UniversityId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
