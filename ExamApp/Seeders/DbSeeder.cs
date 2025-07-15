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

            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User { Username = "admin", Email = "admin@exam.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), Role = "Admin" },
                    new User { Username = "kareem", Email = "kareem@student.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345@M"), Role = "Student" },
                    new User { Username = "mahmoud", Email = "mahmoud@student.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345@M"), Role = "Student" },
                    new User { Username = "ahmed", Email = "ahmed@student.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345678"), Role = "Student" },
                    new User { Username = "sara", Email = "sara@student.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("sara@123"), Role = "Student" },
                    new User { Username = "noor", Email = "noor@student.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("noor@123"), Role = "Student" }
                };

                context.Users.AddRange(users);
                context.SaveChanges();

                var adminId = users.First(u => u.Role == "Admin").Id;
                var students = users.Where(u => u.Role == "Student").ToList();

                // Create 5 Exams
                var exams = new List<Exam>();
                for (int i = 1; i <= 5; i++)
                {
                    exams.Add(new Exam
                    {
                        Title = $"Exam {i}",
                        Description = $"This is exam {i} on general programming.",
                        CreatedAt = DateTime.Now.AddDays(-i),
                        StartTime = DateTime.Today.AddHours(9 + i),
                        EndTime = DateTime.Today.AddHours(10 + i),
                        CreatedBy = adminId,
                        IsActive = true
                    });
                }
                context.Exams.AddRange(exams);
                context.SaveChanges();

                // Create 2 questions per exam
                var allQuestions = new List<Question>();
                var allChoices = new List<Choice>();
                foreach (var exam in exams)
                {
                    for (int q = 1; q <= 2; q++)
                    {
                        var question = new Question
                        {
                            ExamId = exam.Id,
                            Text = $"Question {q} for Exam {exam.Id}"
                        };

                        var choices = new List<Choice>
                        {
                            new Choice { Text = "Answer A", IsCorrect = false },
                            new Choice { Text = "Answer B", IsCorrect = true },
                            new Choice { Text = "Answer C", IsCorrect = false },
                            new Choice { Text = "Answer D", IsCorrect = false },
                        };
                        question.Choices = choices;
                        allQuestions.Add(question);
                        allChoices.AddRange(choices);
                    }
                }
                context.Questions.AddRange(allQuestions);
                context.SaveChanges();

                // Create Results per student for first 3 exams
                var results = new List<Result>();
                foreach (var student in students)
                {
                    foreach (var exam in exams.Take(3))
                    {
                        results.Add(new Result
                        {
                            StudentId = student.Id,
                            ExamId = exam.Id,
                            AttemptNumber = 1,
                            Score = 2.0,
                            Status = "Completed",
                            StartTime = DateTime.Now.AddMinutes(-30),
                            EndTime = DateTime.Now,
                            TakenAt = DateTime.Now
                        });
                    }
                }
                context.Results.AddRange(results);
                context.SaveChanges();

                // Create Answers per result
                var answers = new List<Answer>();
                foreach (var result in results)
                {
                    var examQuestions = allQuestions.Where(q => q.ExamId == result.ExamId).ToList();

                    foreach (var question in examQuestions)
                    {
                        var correctChoice = question.Choices.FirstOrDefault(c => c.IsCorrect) ?? question.Choices.First();
                        answers.Add(new Answer
                        {
                            ResultId = result.Id,
                            QuestionId = question.Id,
                            ChoiceId = correctChoice.Id
                        });
                    }
                }

                context.Answers.AddRange(answers);
                context.SaveChanges();
            }
        }
    }
}
