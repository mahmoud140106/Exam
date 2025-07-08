using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Models;

public partial class Result
{
    [Key]
    public int Id { get; set; }

    public int? StudentId { get; set; }

    public int? ExamId { get; set; }

    public int AttemptNumber { get; set; }

    public double? Score { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? StartTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TakenAt { get; set; }

    [InverseProperty("Result")]
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    [ForeignKey("ExamId")]
    [InverseProperty("Results")]
    public virtual Exam? Exam { get; set; }

    [ForeignKey("StudentId")]
    [InverseProperty("Results")]
    public virtual User? Student { get; set; }
}
