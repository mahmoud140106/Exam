using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Models;

public partial class Exam
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Title { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EndTime { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("Exams")]
    public virtual User? CreatedByNavigation { get; set; }

    [InverseProperty("Exam")]
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    [InverseProperty("Exam")]
    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
}
