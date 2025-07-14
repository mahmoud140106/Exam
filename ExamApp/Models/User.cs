using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Models;

[Index("Username", Name = "UQ__Users__536C85E4FCF4D59F", IsUnique = true)]
[Index("Email", Name = "UQ__Users__A9D105346A8BC8A1", IsUnique = true)]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Username { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(20)]
    public string Role { get; set; } = null!;

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [InverseProperty("Student")]
    public virtual ICollection<Result> Results { get; set; } = new List<Result>();

    public bool IsDeleted { get; set; } = false;
}
