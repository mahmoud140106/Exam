using AutoMapper;
using ExamApp.DTOs.Exam;
using ExamApp.DTOs.Question;
using ExamApp.DTOs.User;
using ExamApp.Models;

namespace ExamApp.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Exam, ExamDto>();
            CreateMap<CreateExamDto, Exam>();
            CreateMap<ExamDto, Exam>();


            CreateMap<UserRegisterDto, User>();
            CreateMap<User, UserResponseDto>();


            CreateMap<CreateQuestionDto, Question>();
            CreateMap<Question, QuestionDto>();

        }
    }
}
