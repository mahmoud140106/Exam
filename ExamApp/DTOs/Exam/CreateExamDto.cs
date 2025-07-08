namespace ExamApp.DTOs.Exam
{
    public class CreateExamDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int CreatedBy { get; set; }
    }
}
