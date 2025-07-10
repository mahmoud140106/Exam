namespace ExamApp.DTOs.Question
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public int? ExamId { get; set; }
    }
}
