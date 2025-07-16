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
                    new User { Username = "noor", Email = "noor@student.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("noor@123"), Role = "Student" },
                    new User { Username = "omar", Email = "omar@student.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("omar@123"), Role = "Student" },
                    new User { Username = "Rawan", Email = "Rawan@student.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Rawan@123"), Role = "Student" }
                };

                context.Users.AddRange(users);
                context.SaveChanges();

                var adminId = users.First(u => u.Role == "Admin").Id;
                var students = users.Where(u => u.Role == "Student").ToList();

                var examTitles = new[]
                {
                    "JavaScript Basics",
                    "Advanced TypeScript",
                    "Angular Core Concepts",
                    "React State Management",
                    "HTML & CSS Mastery",
                    "SQL and Database Design",
                    "Node.js APIs",
                    "ASP.NET Core APIs",
                    "Software Testing",
                    "Design Patterns"
                };

                var exams = examTitles.Select((title, i) => new Exam
                {
                    Title = title,
                    Description = $"Comprehensive exam on {title}",
                    CreatedAt = DateTime.Now.AddDays(-i),
                    StartTime = DateTime.Today.AddHours(9 + i),
                    EndTime = DateTime.Today.AddHours(10 + i),
                    CreatedBy = adminId,
                    IsActive = true
                }).ToList();

                context.Exams.AddRange(exams);
                context.SaveChanges();

                var allQuestions = new List<Question>();
                var allChoices = new List<Choice>();

                foreach (var exam in exams)
                {
                    for (int q = 1; q <= 5; q++)
                    {
                        var question = new Question
                        {
                            ExamId = exam.Id,
                            Text = $"In {exam.Title}, what does concept {q} refer to?"
                        };

                        var choices = new List<Choice>
                        {
                            new Choice { Text = "Option A", IsCorrect = q % 4 == 0 },
                            new Choice { Text = "Option B", IsCorrect = q % 4 == 1 },
                            new Choice { Text = "Option C", IsCorrect = q % 4 == 2 },
                            new Choice { Text = "Option D", IsCorrect = q % 4 == 3 },
                        };

                        question.Choices = choices;
                        allQuestions.Add(question);
                        allChoices.AddRange(choices);
                    }
                }

                context.Questions.AddRange(allQuestions);
                context.SaveChanges();

                var results = new List<Result>();
                var random = new Random();

                foreach (var student in students)
                {
                    foreach (var exam in exams.Take(7))
                    {
                        var score = random.NextDouble() * 5;
                        var status = score > 3.5 ? "Completed" : score > 2 ? "Started" : "Failed";

                        results.Add(new Result
                        {
                            StudentId = student.Id,
                            ExamId = exam.Id,
                            AttemptNumber = 1,
                            Score = Math.Round(score, 2),
                            Status = status,
                            StartTime = DateTime.Now.AddMinutes(-random.Next(20, 60)),
                            EndTime = DateTime.Now,
                            TakenAt = DateTime.Now
                        });
                    }
                }

                context.Results.AddRange(results);
                context.SaveChanges();

                var answers = new List<Answer>();

                foreach (var result in results)
                {
                    var examQuestions = allQuestions.Where(q => q.ExamId == result.ExamId).ToList();

                    foreach (var question in examQuestions)
                    {
                        var randomChoice = question.Choices.OrderBy(_ => random.Next()).First();
                        answers.Add(new Answer
                        {
                            ResultId = result.Id,
                            QuestionId = question.Id,
                            ChoiceId = randomChoice.Id
                        });
                    }
                }

                context.Answers.AddRange(answers);
                context.SaveChanges();
            }
        }
    }
}
