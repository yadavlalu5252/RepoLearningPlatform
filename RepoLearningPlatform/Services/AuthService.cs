using Microsoft.EntityFrameworkCore;
using RepoLearningPlatform.Data;
using RepoLearningPlatform.Interfaces;
using RepoLearningPlatform.Models;

namespace RepoLearningPlatform.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext db;

        public AuthService(AppDbContext db)
        {
            this.db = db;
        }
        public bool RegisterUser(User user)
        {
            var existingUser = db.Users.FirstOrDefault(x => x.Email == user.Email);

            if (existingUser == null)
            {
                user.RoleId = 2;
                db.Users.Add(user);
                db.SaveChanges();

                return true;
            }

            return false;
        }

        public User LoginUser(User user)
        {
            var data = db.Users.FirstOrDefault(x =>
                x.Email == user.Email &&
                x.Password == user.Password);

            return data;
        }
    }
}
