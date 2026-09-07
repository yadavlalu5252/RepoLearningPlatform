using RepoLearningPlatform.Models;

namespace RepoLearningPlatform.Interfaces
{
    public interface IAllCoursesService
    {
        List<MasterCourse> GetMasterCourses();

        List<SubCourse> GetSubCourses();

        List<Subscription> GetSubscriptions();
        decimal GetMasterCourseAmount(int masterCourseId);
    }
}
