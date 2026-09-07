using RepoLearningPlatform.Models;

namespace RepoLearningPlatform.Interfaces
{
    public interface IMyCoursesService
    {
        List<Purchase> GetMyCourses(int userId);

        SubCourse GetSubCourse(int id);

        List<AddTopic> GetTopics(int subCourseId);

        AddMaterial GetMaterial(int materialId);

        AddMaterial GetMaterialByTopic(int topicId);


        void CompleteTopic(
            int userId,
            int masterCourseId,
            int topicId);

        bool IsTopicCompleted(
            int userId,
            int topicId);

        bool IsPreviousTopicCompleted(
            int userId,
            int subCourseId,
            int topicId);

        bool IsCourseCompleted(
            int userId,
            int subCourseId);
        void GenerateCertificate(
    int userId,
    int masterCourseId);
    }
}