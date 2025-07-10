using ExamApp.Models;
using ExamApp.Repositories.Interface;

namespace ExamApp.Repositories.Implementations
{
    public class ChoiceRepository:IChoiceRepository
    {
        public ChoiceRepository(ApplicationDbContext _db)
        {
            Db = _db;
        }

        public ApplicationDbContext Db { get; }
    }
}
