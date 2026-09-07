using Microsoft.EntityFrameworkCore;
using RepoLearningPlatform.Data;
using RepoLearningPlatform.Interfaces;
using RepoLearningPlatform.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace RepoLearningPlatform.Services
{
    public class MyCoursesService : IMyCoursesService
    {
        private readonly AppDbContext db;
        public MyCoursesService(AppDbContext db)
        {
            this.db = db;
        }

        public List<Purchase> GetMyCourses(int userId)
        {
            var data = db.Purchases
                .Include(x => x.MasterCourseData)
                .Include(x => x.SubCourseData)
                .Include(x => x.SubscriptionData)
                .Where(x => x.UserId == userId)
                .ToList();

            return data;
        }
        public SubCourse GetSubCourse(int id)
        {
            var data = db.SubCourse
                .FirstOrDefault(x => x.Id == id);

            return data!;
        }

        public List<AddTopic> GetTopics(int subCourseId)
        {
            var data = db.AddTopics
                .Where(x => x.SubCourseId == subCourseId &&
                            x.Status == "Active")
                .OrderBy(x => x.Id)
                .ToList();

            return data;
        }
        public AddMaterial GetMaterialByTopic(int topicId)
        {
            var data = db.AddMaterials
                .FirstOrDefault(x => x.TopicId == topicId &&
                                     x.Status == "Active");

            return data!;
        }
        public void CompleteTopic(
            int userId,
            int masterCourseId,
            int topicId)
        {
            var data = db.CourseProgress
                .FirstOrDefault(x =>
                    x.UserId == userId &&
                    x.MasterCourseId == masterCourseId &&
                    x.TopicId == topicId);

            if (data == null)
            {
                var progress = new CourseProgress()
                {
                    UserId = userId,
                    MasterCourseId = masterCourseId,
                    TopicId = topicId,
                    IsCompleted = true,
                    McqCompleted = true,
                    CompletedAt = DateTime.Now
                };

                db.CourseProgress.Add(progress);
            }
            else
            {
                data.IsCompleted = true;
                data.McqCompleted = true;
                data.CompletedAt = DateTime.Now;
            }

            db.SaveChanges();
        }

        public bool IsTopicCompleted(
            int userId,
            int topicId)
        {
            var data = db.CourseProgress
                .FirstOrDefault(x =>
                    x.UserId == userId &&
                    x.TopicId == topicId &&
                    x.McqCompleted == true &&
                    x.IsCompleted == true);

            return data != null;
        }

        public bool IsPreviousTopicCompleted(
            int userId,
            int subCourseId,
            int topicId)
        {
            var topics = db.AddTopics
                .Where(x => x.SubCourseId == subCourseId &&
                            x.Status == "Active")
                .OrderBy(x => x.Id)
                .ToList();

            var currentIndex = topics.FindIndex(
                x => x.Id == topicId);

            // First topic is always unlocked
            if (currentIndex <= 0)
            {
                return true;
            }

            var previousTopic = topics[currentIndex - 1];

            return IsTopicCompleted(
                userId,
                previousTopic.Id);
        }

        public bool IsCourseCompleted(
            int userId,
            int subCourseId)
        {
            var topics = db.AddTopics
                .Where(x => x.SubCourseId == subCourseId &&
                            x.Status == "Active")
                .OrderBy(x => x.Id)
                .ToList();

            if (topics.Count == 0)
            {
                return false;
            }

            foreach (var topic in topics)
            {
                var completed = IsTopicCompleted(
                    userId,
                    topic.Id);

                if (!completed)
                {
                    return false;
                }
            }

            return true;
        }


        public AddMaterial GetMaterial(int materialId)
        {
            var data = db.AddMaterials
                .FirstOrDefault(x => x.MaterialId == materialId &&
                                     x.Status == "Active");

            return data!;
        }


        public void GenerateCertificate(
    int userId,
    int masterCourseId)
        {
            var user = db.Users
                .FirstOrDefault(x => x.UserId == userId);

            var course = db.MasterCourse
                .FirstOrDefault(x => x.Id == masterCourseId);

            if (user == null || course == null)
            {
                return;
            }

            var certificateNumber =
                "CERT-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            var fileName =
    "Certificate_" +
    userId +
    "_" +
    masterCourseId +
    ".pdf";

            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "certificates");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(
                folderPath,
                fileName);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);

                    page.Content()
                        .Border(5)
                        .Padding(40)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            column.Item()
                                .AlignCenter()
                                .Text("CERTIFICATE OF COMPLETION")
                                .FontSize(28)
                                .Bold();

                            column.Item()
                                .AlignCenter()
                                .Text("This certificate is proudly presented to")
                                .FontSize(16);

                            column.Item()
                                .AlignCenter()
                                .Text(user.Name)
                                .FontSize(24)
                                .Bold();

                            column.Item()
                                .AlignCenter()
                                .Text("for successfully completing")
                                .FontSize(16);

                            column.Item()
                                .AlignCenter()
                                .Text(course.MasterCourseName)
                                .FontSize(22)
                                .Bold();

                            column.Item()
                                .AlignCenter()
                                .Text("Date: " +
                                      DateTime.Now.ToString("dd-MM-yyyy"))
                                .FontSize(14);

                            column.Item()
                                .AlignCenter()
                                .Text("Certificate No: " +
                                      certificateNumber)
                                .FontSize(12);
                        });
                });
            })
            .GeneratePdf(filePath);

            var certificate = new Certificate()
            {
                UserId = userId,
                MasterCourseId = masterCourseId,
                CertificateNumber = certificateNumber,
                IssuedAt = DateTime.Now,
                CertificateFile = fileName
            };

            db.Certificates.Add(certificate);

            db.SaveChanges();
        }
    }
}
