using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Models;

public partial class Choice
{
    [Key]
    public int Id { get; set; }

    public int? QuestionId { get; set; }

    [StringLength(255)]
    public string Text { get; set; } = null!;

    public bool IsCorrect { get; set; }

    [InverseProperty("Choice")]
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    [ForeignKey("QuestionId")]
    [InverseProperty("Choices")]
    public virtual Question? Question { get; set; }
}
