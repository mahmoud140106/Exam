using System.ComponentModel.DataAnnotations.Schema;
using ExamApp.DTOs.Choice;
using ExamApp.DTOs.Question;

namespace ExamApp.DTOs.Answer
{
    public class AnswerWithQuestions
    {
        public int Id { get; set; }

        public int? ResultId { get; set; }

        public int? QuestionId { get; set; }

        public int? ChoiceId { get; set; }
        public List<QuestionWithChoicesDto>? Question {  get; set; }

    }
}
