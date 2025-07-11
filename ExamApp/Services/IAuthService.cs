namespace ExamApp.Services.Interface
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(string email, string password);
    }
}
