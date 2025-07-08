using AutoMapper;
using ExamApp.DTOs.Exam;
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

        }
    }
}
