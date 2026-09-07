using RepoLearningPlatform.Data;
using RepoLearningPlatform.Interfaces;
using RepoLearningPlatform.Models;

namespace RepoLearningPlatform.Services
{
    public class UserDashboardService : IUserDashboardService
    {
        private readonly AppDbContext db;
        public UserDashboardService(AppDbContext db)
        {
            this.db = db;
        }

        public User GetUserById(int id)
        {
            var user = db.Users.Find(id);
            return user;
        }
    }
}
