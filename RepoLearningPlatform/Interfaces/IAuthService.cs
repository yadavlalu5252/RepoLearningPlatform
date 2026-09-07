using RepoLearningPlatform.Models;

namespace RepoLearningPlatform.Interfaces
{
    public interface IAuthService
    {
        bool RegisterUser(User user);
        User LoginUser(User user);
    }
}
