using Microsoft.AspNetCore.Mvc;
using RepoLearningPlatform.Interfaces;
using RepoLearningPlatform.Models;

namespace RepoLearningPlatform.Controllers
{
    public class MyCoursesController : Controller
    {
        private readonly IMyCoursesService mcs;

        public MyCoursesController(IMyCoursesService mcs)
        {
            this.mcs = mcs;
        }

        public IActionResult Index()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var courses = mcs.GetMyCourses(userId);

            return View(courses);
        }

        public IActionResult DownloadAssignment(int id)
        {
            var material = mcs.GetMaterial(id);

            if (material == null ||
                string.IsNullOrEmpty(material.AssignmentAttachment))
            {
                return NotFound();
            }

            var fileName = material.AssignmentAttachment;

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            return PhysicalFile(
                filePath,
                "application/pdf",
                fileName);
        }

        [HttpPost]
        public IActionResult UploadSolution(
            int id,
            int topicId,
            int materialId,
            IFormFile solutionFile)
        {
            if (solutionFile == null || solutionFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a file.";

                return RedirectToAction("Learn", new
                {
                    id = id,
                    topicId = topicId
                });
            }

            int userId =
                HttpContext.Session.GetInt32("UserId") ?? 0;

            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "solutions");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var extension =
                Path.GetExtension(solutionFile.FileName);

            var fileName = userId + "_" +
                           materialId + "_" +
                           topicId +
                           extension;

            var filePath = Path.Combine(
                folderPath,
                fileName);

            using (var stream =
                   new FileStream(filePath, FileMode.Create))
            {
                solutionFile.CopyTo(stream);
            }

            TempData["SuccessMessage"] =
                "Solution uploaded successfully.";

            return RedirectToAction("Learn", new
            {
                id = id,
                topicId = topicId
            });
        }

        public IActionResult StartCourse(int id)
        {
            return RedirectToAction(
                "Learn",
                new
                {
                    id = id
                });
        }

        public IActionResult Learn(int id, int? topicId)
        {
            var subCourse = mcs.GetSubCourse(id);

            if (subCourse == null)
            {
                return NotFound();
            }

            int userId =
                HttpContext.Session.GetInt32("UserId") ?? 0;

            var topics = mcs.GetTopics(id);

            if (topics.Count == 0)
            {
                return View(subCourse);
            }

            AddTopic currentTopic;

            if (topicId == null)
            {
                currentTopic = topics.First();
            }
            else
            {
                currentTopic = topics
                    .FirstOrDefault(x => x.Id == topicId);

                if (currentTopic == null)
                {
                    return NotFound();
                }
            }

            bool topicUnlocked =
                mcs.IsPreviousTopicCompleted(
                    userId,
                    id,
                    currentTopic.Id);

            if (!topicUnlocked)
            {
                TempData["ErrorMessage"] =
                    "Please complete the previous topic first.";

                return RedirectToAction("Learn", new
                {
                    id = id,
                    topicId = topics.First().Id
                });
            }

            var material =
                mcs.GetMaterialByTopic(currentTopic.Id);

            bool currentCompleted =
                mcs.IsTopicCompleted(
                    userId,
                    currentTopic.Id);

            bool courseCompleted =
                mcs.IsCourseCompleted(
                    userId,
                    id);

            var topicAccess =
                new Dictionary<int, bool>();

            foreach (var topic in topics)
            {
                topicAccess[topic.Id] =
                    mcs.IsPreviousTopicCompleted(
                        userId,
                        id,
                        topic.Id);
            }

            var topicCompleted =
                new Dictionary<int, bool>();

            foreach (var topic in topics)
            {
                topicCompleted[topic.Id] =
                    mcs.IsTopicCompleted(
                        userId,
                        topic.Id);
            }

            ViewBag.Topics = topics;
            ViewBag.CurrentTopic = currentTopic;
            ViewBag.Material = material;
            ViewBag.CurrentCompleted = currentCompleted;
            ViewBag.CourseCompleted = courseCompleted;
            ViewBag.TopicAccess = topicAccess;
            ViewBag.TopicCompleted = topicCompleted;

            return View(subCourse);
        }

        [HttpPost]
        public IActionResult SubmitMCQ(
            int id,
            int topicId,
            int materialId,
            string mcq1,
            string mcq2,
            string mcq3)
        {
            var material = mcs.GetMaterial(materialId);

            if (material == null)
            {
                return NotFound();
            }

            bool answer1 =
                (mcq1 ?? "").Trim().ToUpper() ==
                material.MCQ1Answer.Trim().ToUpper();

            bool answer2 =
                (mcq2 ?? "").Trim().ToUpper() ==
                material.MCQ2Answer.Trim().ToUpper();

            bool answer3 =
                (mcq3 ?? "").Trim().ToUpper() ==
                material.MCQ3Answer.Trim().ToUpper();

            if (answer1 && answer2 && answer3)
            {
                int userId =
                    HttpContext.Session.GetInt32("UserId") ?? 0;

                var topic = mcs.GetTopics(id)
                    .FirstOrDefault(x => x.Id == topicId);

                if (topic == null)
                {
                    return NotFound();
                }

                mcs.CompleteTopic(
                    userId,
                    topic.MasterCourseId,
                    topicId);

                TempData["SuccessMessage"] =
                    "All 3 answers are correct. Topic completed!";
            }
            else
            {
                TempData["ErrorMessage"] =
                    "All 3 answers must be correct to unlock the next topic.";
            }

            return RedirectToAction("Learn", new
            {
                id = id,
                topicId = topicId
            });
        }

        [HttpGet]
        public IActionResult DownloadCertificate(int id)
        {
            int userId =
                HttpContext.Session.GetInt32("UserId") ?? 0;

            var course = mcs.GetSubCourse(id);

            if (course == null)
            {
                return Content("Course is null");
            }

            bool courseCompleted =
                mcs.IsCourseCompleted(
                    userId,
                    id);

            if (!courseCompleted)
            {
                return Content("Course is not completed");
            }

            mcs.GenerateCertificate(
                userId,
                course.MasterCourseId);

            var fileName =
                "Certificate_" +
                userId +
                "_" +
                course.MasterCourseId +
                ".pdf";

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "certificates",
                fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return Content(
                    "PDF file not found: " + filePath);
            }

            return PhysicalFile(
                filePath,
                "application/pdf",
                fileName);
        }
    }
}