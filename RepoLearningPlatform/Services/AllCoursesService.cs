using RepoLearningPlatform.Data;
using RepoLearningPlatform.Interfaces;
using RepoLearningPlatform.Models;

namespace RepoLearningPlatform.Services
{
    public class AllCoursesService : IAllCoursesService
    {
        private readonly AppDbContext db;
        public AllCoursesService(AppDbContext db)
        {
            this.db = db;
        }

        public List<MasterCourse> GetMasterCourses()
        {
            var data = db.MasterCourse.ToList();
            return data;
        }

        public List<SubCourse> GetSubCourses()
        {
            var data = db.SubCourse.ToList();
            return data;
        }

        public List<Subscription> GetSubscriptions()
        {
            var data = db.Subscriptions.ToList();
            return data;
        }
        public decimal GetMasterCourseAmount(int masterCourseId)
        {
            var amount = db.SubCourse.Where(x => x.MasterCourseId == masterCourseId).Sum(x => x.Amount);
            return amount;

        }
    }
}
