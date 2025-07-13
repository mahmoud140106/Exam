using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Choice> Choices { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<User> Users { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=.;Database=ExamSystemDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Answers__3214EC073AC27FF5");

            entity.HasOne(d => d.Choice).WithMany(p => p.Answers).HasConstraintName("FK__Answers__ChoiceI__4F7CD00D");

            entity.HasOne(d => d.Question).WithMany(p => p.Answers).HasConstraintName("FK__Answers__Questio__4E88ABD4");

            entity.HasOne(d => d.Result).WithMany(p => p.Answers)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Answers__ResultI__4D94879B");
        });

        modelBuilder.Entity<Choice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Choices__3214EC075E2F5F6A");

            entity.HasOne(d => d.Question).WithMany(p => p.Choices)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Choices__Questio__440B1D61");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Exams__3214EC07DFF4F908");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Exams).HasConstraintName("FK__Exams__CreatedBy__3D5E1FD2");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC0721A3EAB8");

            entity.HasOne(d => d.Exam).WithMany(p => p.Questions)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Questions__ExamI__412EB0B6");
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Results__3214EC077CB216A1");

            entity.Property(e => e.Status).HasDefaultValue("Pending");
            entity.Property(e => e.TakenAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Exam).WithMany(p => p.Results).HasConstraintName("FK__Results__ExamId__47DBAE45");

            entity.HasOne(d => d.Student).WithMany(p => p.Results).HasConstraintName("FK__Results__Student__46E78A0C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07BAAD2308");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
