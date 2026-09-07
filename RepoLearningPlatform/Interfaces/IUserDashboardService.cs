using RepoLearningPlatform.Models;

namespace RepoLearningPlatform.Interfaces
{
    public interface IUserDashboardService
    {
        User GetUserById(int id);
    }
}
