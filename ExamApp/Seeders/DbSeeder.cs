using ExamApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Data
{
    public static class DbSeeder
    {
        public static void Seed(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            //context.Database.EnsureDeleted();
            //context.Database.Migrate();

            if (!context.Users.Any())
            {
                var admin = new User
                {
                    Username = "admin",
                    Email = "admin@exam.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = "Admin"
                };

                var student = new User
                {
                    Username = "mahmoud",
                    Email = "mahmoud@student.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456789@M"),
                    Role = "Student"
                };

                context.Users.AddRange(admin, student);
                context.SaveChanges();

                // Create Exam
                var exam = new Exam
                {
                    Title = "C# Fundamentals Exam",
                    Description = "A test on basic concepts of C# language.",
                    CreatedAt = DateTime.Now.AddDays(-3),
                    StartTime = DateTime.Today.AddHours(10),
                    EndTime = DateTime.Today.AddHours(12),
                    CreatedBy = admin.Id,
                    IsActive = true
                };

                context.Exams.Add(exam);
                context.SaveChanges();

                // Create Questions and Choices
                var question1 = new Question
                {
                    Text = "What is the output of Console.WriteLine(2 + \"2\")?",
                    ExamId = exam.Id
                };
                var choices1 = new List<Choice>
                {
                    new Choice { Text = "4", IsCorrect = false },
                    new Choice { Text = "22", IsCorrect = true },
                    new Choice { Text = "Error", IsCorrect = false },
                    new Choice { Text = "None of the above", IsCorrect = false }
                };
                question1.Choices = choices1;

                var question2 = new Question
                {
                    Text = "Which of the following is a value type in C#?",
                    ExamId = exam.Id
                };
                var choices2 = new List<Choice>
                {
                    new Choice { Text = "string", IsCorrect = false },
                    new Choice { Text = "int", IsCorrect = true },
                    new Choice { Text = "object", IsCorrect = false },
                    new Choice { Text = "class", IsCorrect = false }
                };
                question2.Choices = choices2;

                context.Questions.AddRange(question1, question2);
                context.SaveChanges();

                // Create Result
                var result = new Result
                {
                    StudentId = student.Id,
                    ExamId = exam.Id,
                    AttemptNumber = 1,
                    Score = 1.0,
                    Status = "Completed",
                    StartTime = DateTime.Now.AddMinutes(-20),
                    EndTime = DateTime.Now,
                    TakenAt = DateTime.Now
                };

                context.Results.Add(result);
                context.SaveChanges();

                // Create Answers
                var answer1 = new Answer
                {
                    QuestionId = question1.Id,
                    ChoiceId = choices1.First(c => c.IsCorrect).Id,
                    ResultId = result.Id
                };
                var answer2 = new Answer
                {
                    QuestionId = question2.Id,
                    ChoiceId = choices2.First(c => !c.IsCorrect).Id,
                    ResultId = result.Id
                };

                context.Answers.AddRange(answer1, answer2);
                context.SaveChanges();
            }
        }
    }
}
