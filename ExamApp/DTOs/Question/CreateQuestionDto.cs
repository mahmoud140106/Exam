using System.ComponentModel.DataAnnotations;

namespace ExamApp.DTOs.Question
{
    public class CreateQuestionDto
    {
        [Required]
        public string Text { get; set; } = null!;

        [Required]
        public int ExamId { get; set; }
    }
}
