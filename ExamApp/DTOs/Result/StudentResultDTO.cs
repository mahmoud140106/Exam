using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamApp.DTOs.Result
{
    public class StudentResultDTO
    {
        public int Id { get; set; }

        public int? StudentId { get; set; }

        public int? ExamId { get; set; }

        public int AttemptNumber { get; set; }

        public double? Score { get; set; }

        public string Status { get; set; } = null!;

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime? TakenAt { get; set; }

        public virtual Models.Exam? Exam { get; set; }
    }
}
