using System.ComponentModel.DataAnnotations;
using ExamApp.DTOs.Choice;

namespace ExamApp.DTOs.Question
{
    public class CreateQuestionDto
    {
        [Required]
        public string Text { get; set; } = null!;

        [Required]
        public int ExamId { get; set; }

        public List<CreateChoiceDto>? ChoiceDtos { get; set; }
    }
}
