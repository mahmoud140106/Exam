using AutoMapper;
using ExamApp.Data;
using ExamApp.DTOs;
using ExamApp.Models;
using ExamApp.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ExamApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public DashboardController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IConfiguration config)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _config = config;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStatistics()
    {
        var allStudents = await _unitOfWork.UserRepo.GetAllAsync();
        var totalStudents = allStudents.Count(u => !u.IsDeleted);

        var allExams = await _unitOfWork.ExamRepo.GetAllAsync();
        var totalExams = allExams.Count();
        var now = DateTime.Now;
        var ongoingExams = allExams.Count(e => e.StartTime <= now && e.EndTime >= now);
        var upcomingExams = allExams.Count(e => e.StartTime > now);

        var allResults = await _unitOfWork.ResultRepo.GetAllAsync();
        var completedExams = allResults.Count();
        var recentResults = allResults.Where(r => r.TakenAt >= DateTime.Now.AddDays(-7));

        return Success(new
        {
            totalStudents,
            totalExams,
            completedExams,
            ongoingExams,
            upcomingExams,
            recentResultsCount = recentResults.Count()
        });
    }

    [HttpGet("chart/students-per-month")]
    public async Task<IActionResult> GetStudentsPerMonth()
    {
        var students = await _unitOfWork.UserRepo.GetAllAsync();

        var grouped = students
            .GroupBy(s => s.Id % 12 + 1)//هبداية علشان مفيش كريتت ات
            .Select(g => new {
                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                Count = g.Count()
            })
            .OrderBy(x => DateTime.ParseExact(x.Month, "MMMM", CultureInfo.CurrentCulture).Month);

        return Success(grouped);
    }

    [HttpGet("ai/insights")]
    public async Task<IActionResult> GetAiInsights()
    {
        var insights = new List<object>();

        try
        {
            var exams = await _unitOfWork.ExamRepo.GetAllAsync();
            var allResults = await _unitOfWork.ResultRepo.GetAllAsync();

            foreach (var exam in exams)
            {
                var examResults = allResults.Where(r => r.ExamId == exam.Id && r.Score.HasValue).ToList();

                if (examResults.Any())
                {
                    var averageScore = examResults.Average(r => r.Score.Value);
                    var passRate = examResults.Count(r => r.Score >= 60) / (double)examResults.Count * 100;
                    var completionRate = examResults.Count() / (double)examResults.Count * 100;

                    var difficultyLevel = AnalyzeDifficulty(averageScore, passRate);
                    var timeAnalysis = AnalyzeTimePerformance(examResults);
                    var examInsights = GenerateExamInsights(exam, averageScore, passRate, completionRate, difficultyLevel, timeAnalysis).Take(1).ToList();

                    if (examInsights.Any())
                    {
                        insights.Add(new
                        {
                            examId = exam.Id,
                            examTitle = exam.Title,
                            averageScore = Math.Round(averageScore, 2),
                            passRate = Math.Round(passRate, 2),
                            completionRate = Math.Round(completionRate, 2),
                            difficultyLevel,
                            timeAnalysis,
                            insights = examInsights
                        });
                    }
                }
            }

            var systemInsights = GenerateSystemInsights(allResults).Take(1).ToList();
            if (systemInsights.Any())
            {
                insights.Add(new
                {
                    type = "system",
                    insights = systemInsights
                });
            }

            var studentInsights = await GenerateStudentInsights();
            studentInsights = studentInsights.Take(1).ToList();

            if (studentInsights.Any())
            {
                insights.Add(new
                {
                    type = "students",
                    insights = studentInsights
                });
            }

            return Success(insights);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error analyzing data: {ex.Message}");
        }
    }

    [HttpGet("ai/predictions")]
    public async Task<IActionResult> GetPredictions()
    {
        var predictions = new List<object>();

        try
        {
            var exams = await _unitOfWork.ExamRepo.GetAllAsync();
            var allResults = await _unitOfWork.ResultRepo.GetAllAsync();

            foreach (var exam in exams.Where(e => e.StartTime > DateTime.Now))
            {
                var prediction = PredictExamPerformance(exam, allResults);

                if (prediction != null)
                {
                    predictions.Add(prediction);
                }
            }

            return Success(predictions);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error in prediction: {ex.Message}");
        }
    }

    #region AI Analysis Methods

    private string AnalyzeDifficulty(double averageScore, double passRate)
    {
        if (averageScore >= 85 && passRate >= 90)
            return "Easy";
        else if (averageScore >= 70 && passRate >= 70)
            return "Medium";
        else if (averageScore >= 60 && passRate >= 50)
            return "Hard";
        else
            return "Very Hard";
    }

    private object AnalyzeTimePerformance(List<Result> results)
    {
        var completedResults = results.Where(r => r.TakenAt.HasValue).ToList();

        if (!completedResults.Any())
            return new { message = "No completed results for analysis" };

        var avgCompletionTime = completedResults.Average(r =>
            r.TakenAt.HasValue ? (DateTime.Now - r.TakenAt.Value).TotalMinutes : 0);

        return new
        {
            averageCompletionTime = Math.Round(avgCompletionTime, 2),
            fastestCompletion = completedResults.Min(r =>
                r.TakenAt.HasValue ? (DateTime.Now - r.TakenAt.Value).TotalMinutes : 0),
            slowestCompletion = completedResults.Max(r =>
                r.TakenAt.HasValue ? (DateTime.Now - r.TakenAt.Value).TotalMinutes : 0)
        };
    }

    private List<string> GenerateExamInsights(Exam exam, double averageScore, double passRate,
        double completionRate, string difficultyLevel, object timeAnalysis)
    {
        var insights = new List<string>();

        if (averageScore < 60)
            insights.Add("Low average score, recommend reviewing exam content");

        if (passRate < 50)
            insights.Add("Low pass rate, exam may need adjustment");

        if (completionRate < 80)
            insights.Add("Low completion rate, time limit may be insufficient");

        if (difficultyLevel == "Very Hard")
            insights.Add("Exam is very difficult, recommend reducing difficulty");
        else if (difficultyLevel == "Easy")
            insights.Add("Exam is easy, can increase challenge level");

        if (exam.StartTime > DateTime.Now)
            insights.Add("Upcoming exam - ensure student readiness");

        return insights;
    }

    private List<string> GenerateSystemInsights(List<Result> allResults)
    {
        var insights = new List<string>();

        if (!allResults.Any())
            return insights;

        var completedResults = allResults.Where(r => r.Score.HasValue).ToList();

        if (!completedResults.Any())
            return insights;

        var overallAverage = completedResults.Average(r => r.Score.Value);

        if (overallAverage < 65)
            insights.Add("Overall system performance is low, recommend curriculum review");

        var recentResults = completedResults.Where(r => r.TakenAt >= DateTime.Now.AddDays(-30));
        if (recentResults.Any())
        {
            var recentAverage = recentResults.Where(r => r.Score.HasValue).Average(r => r.Score.Value);
            if (recentAverage > overallAverage + 5)
                insights.Add("Performance improvement in the last month");
            else if (recentAverage < overallAverage - 5)
                insights.Add("Performance decline in the last month");
        }

        return insights;
    }

    private async Task<List<string>> GenerateStudentInsights()
    {
        var insights = new List<string>();

        var students = await _unitOfWork.UserRepo.GetAllAsync();
        var results = await _unitOfWork.ResultRepo.GetAllAsync();

        var activeStudents = students.Where(s =>
            results.Any(r => r.TakenAt >= DateTime.Now.AddDays(-30)));

        var inactiveStudents = students.Count() - activeStudents.Count();

        if (inactiveStudents > students.Count() * 0.3)
            insights.Add($"Large number of inactive students: {inactiveStudents}");

        var topPerformers = results
            .Where(r => r.Score.HasValue && r.Score >= 90)
            .GroupBy(r => r.ExamId)
            .Count();

        if (topPerformers > 0)
            insights.Add($"Found {topPerformers} high-performing exam results");

        return insights;
    }

    private object PredictExamPerformance(Exam exam, List<Result> allResults)
    {
        var similarExams = allResults
            .Where(r => r.Score.HasValue)
            .GroupBy(r => r.ExamId)
            .Where(g => g.Count() >= 5)
            .ToList();

        if (!similarExams.Any())
            return null;

        var avgPerformance = similarExams.Average(g => g.Average(r => r.Score.Value));
        var avgPassRate = similarExams.Average(g => g.Count(r => r.Score >= 60) / (double)g.Count() * 100);

        return new
        {
            examId = exam.Id,
            examTitle = exam.Title,
            predictedAverageScore = Math.Round(avgPerformance, 2),
            predictedPassRate = Math.Round(avgPassRate, 2),
            confidence = similarExams.Count() >= 10 ? "High" : "Medium",
            recommendation = avgPerformance < 60 ?
                "Recommend reviewing exam before starting" :
                "Exam ready for execution"
        };
    }

    #endregion
}