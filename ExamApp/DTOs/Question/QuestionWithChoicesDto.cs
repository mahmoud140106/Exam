using ExamApp.DTOs.Choice;

namespace ExamApp.DTOs.Question
{
    public class QuestionWithChoicesDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public int? ExamId { get; set; }

        public List<ChoiceDto>? Choices { get; set; }
    }
}
