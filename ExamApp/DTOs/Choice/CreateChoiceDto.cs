using System.ComponentModel.DataAnnotations;

namespace ExamApp.DTOs.Choice
{
    public class CreateChoiceDto
    {
        public int? QuestionId { get; set; }

        [StringLength(255)]
        public string Text { get; set; } = null!;

        public bool IsCorrect { get; set; }

    }
}
