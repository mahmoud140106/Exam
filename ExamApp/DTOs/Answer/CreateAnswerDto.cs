namespace ExamApp.DTOs.Answer
{
    public class CreateAnswerDto
    {
        public int? ResultId { get; set; }
        public int? QuestionId { get; set; }
        public int? ChoiceId { get; set; }
    }
}
