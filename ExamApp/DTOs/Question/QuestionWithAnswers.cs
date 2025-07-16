using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExamApp.DTOs.Answer;
using ExamApp.DTOs.Choice;

namespace ExamApp.DTOs.Question
{
    public class QuestionWithAnswers
    {
        public int Id { get; set; }

        public int? ExamId { get; set; }

        public string Text { get; set; } = null!;

       
        public virtual ICollection<AnswerDto>? Answers { get; set; } = new List<AnswerDto>();

        
        public virtual ICollection<ChoiceDto>? Choices { get; set; } = new List<ChoiceDto>();

    }
}
