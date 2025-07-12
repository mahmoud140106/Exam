using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ExamApp.Models;

namespace ExamApp.DTOs.Choice
{
    public class ChoiceDto
    {
            public int Id { get; set; }

            public int? QuestionId { get; set; }

            [StringLength(255)]
            public string Text { get; set; } = null!;
            
            public bool IsCorrect { get; set; }

    }
}
