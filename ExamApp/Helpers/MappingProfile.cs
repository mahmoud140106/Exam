﻿using AutoMapper;
using ExamApp.DTOs.Answer;
using ExamApp.DTOs.Choice;
using ExamApp.DTOs.Exam;
using ExamApp.DTOs.Question;
using ExamApp.DTOs.Result;
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
            CreateMap<User, UserDto>();
            CreateMap<UserCreateDto, User>();


            CreateMap<CreateQuestionDto, Question>();
            CreateMap<Question, QuestionDto>();
            
            CreateMap<Choice, ChoiceDto>().ReverseMap();
            CreateMap<CreateChoiceDto, Choice>();

            CreateMap<CreateAnswerDto, Answer>();
            CreateMap<Answer, AnswerDto>().ReverseMap();

            CreateMap<Result, ResultDto>();
            CreateMap<CreateResultDto, Result>();
            CreateMap<UpdateResultDto, Result>();



            CreateMap<Question, EditQuestionDto>().ReverseMap();

            CreateMap<Question, QuestionWithChoicesDto>().ReverseMap();

            CreateMap<Result, StudentResultDTO>()
                //.ForMember(dest => dest., opt => opt.MapFrom(src => src.Customer.Name))
                ;

            CreateMap<Answer,AnswerWithQuestions>().ReverseMap();

        }
    }
}
