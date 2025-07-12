using System.Text;
using AutoMapper;
using ExamApp.Data;
using ExamApp.Helpers;
using ExamApp.Middleware;
using ExamApp.Models;
using ExamApp.Repositories.Implementations;
using ExamApp.Repositories.Interface;
using ExamApp.Services;
using ExamApp.Services.Interface;
using ExamApp.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ExamApp
{  
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var corsPolicyName = "MyCorsPolicy";

            // Controllers
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            }); ;

            // Swagger (OpenAPI)
            //builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options
                        //.UseLazyLoadingProxies()
                       .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Authentication (JWT)
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });

            // Authorization
            builder.Services.AddAuthorization();

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile)); // Update this line

            // Repositories & Services
            //builder.Services.AddScoped<AuthService>();
            //builder.Services.AddScoped<IExamRepository, ExamRepository>();
            //builder.Services.AddScoped<IUserRepository, UserRepository>();
            //builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUnitOfWork,UnitOfWork.UnitOfWork>();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(corsPolicyName, policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();
            DbSeeder.Seed(app);
            app.UseMiddleware<ExceptionMiddleware>();

            // Middleware
            app.UseCors(corsPolicyName);

            if (app.Environment.IsDevelopment())
            {
                //app.MapOpenApi();
                //app.UseSwaggerUI(op => op.SwaggerEndpoint("/openapi/v1.json", "v1"));
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    c.RoutePrefix = "swagger";
                });


            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
