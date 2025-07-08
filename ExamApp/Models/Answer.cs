using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Models;

public partial class Answer
{
    [Key]
    public int Id { get; set; }

    public int? ResultId { get; set; }

    public int? QuestionId { get; set; }

    public int? ChoiceId { get; set; }

    [ForeignKey("ChoiceId")]
    [InverseProperty("Answers")]
    public virtual Choice? Choice { get; set; }

    [ForeignKey("QuestionId")]
    [InverseProperty("Answers")]
    public virtual Question? Question { get; set; }

    [ForeignKey("ResultId")]
    [InverseProperty("Answers")]
    public virtual Result? Result { get; set; }
}
