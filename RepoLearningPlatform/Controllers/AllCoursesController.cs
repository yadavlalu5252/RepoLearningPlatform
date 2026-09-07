using Microsoft.AspNetCore.Mvc;
using RepoLearningPlatform.Interfaces;

namespace RepoLearningPlatform.Controllers
{
    public class AllCoursesController : Controller
    {
        private readonly IAllCoursesService acs;
        public AllCoursesController(IAllCoursesService acs)
        {
            this.acs = acs;
        }
        public IActionResult Index()
        {
            var masterCourses = acs.GetMasterCourses();

            var subCourses = acs.GetSubCourses();

            var subscriptions = acs.GetSubscriptions();

            ViewBag.MasterCourses = masterCourses;

            ViewBag.SubCourses = subCourses;

            ViewBag.Subscriptions = subscriptions;

            var masterCourseAmounts = new Dictionary<int, decimal>();

            foreach (var item in masterCourses)
            {
                masterCourseAmounts[item.Id] =
                    acs.GetMasterCourseAmount(item.Id);
            }

            ViewBag.MasterCourseAmounts = masterCourseAmounts;
            return View();
        }
    }
}
