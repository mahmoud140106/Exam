using ExamApp.DTOs.Choice;

namespace ExamApp.DTOs.Question
{
    public class EditQuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public List<ChoiceDto>? Choices { get; set; }
    }
}
