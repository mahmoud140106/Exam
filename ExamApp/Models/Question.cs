using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Models;

public partial class Question
{
    [Key]
    public int Id { get; set; }

    public int? ExamId { get; set; }

    [StringLength(255)]
    public string Text { get; set; } = null!;

    [InverseProperty("Question")]
    public virtual ICollection<Answer>? Answers { get; set; } = new List<Answer>();

    [InverseProperty("Question")]
    public virtual ICollection<Choice>? Choices { get; set; } = new List<Choice>();

    [ForeignKey("ExamId")]
    [InverseProperty("Questions")]
    public virtual Exam? Exam { get; set; }
}
