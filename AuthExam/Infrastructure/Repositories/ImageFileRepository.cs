using AuthExam.Infrastructure.Interfaces;
using AuthExam.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AuthExam.Infrastructure.Repositories
{
    public class ImageFileRepository : IImageRepository
    {
        public string BasePath => "~/Content/Uploads";
        private const string notFoundPic = "Content/Uploads/notfound.jpg";

        public ImageFileRepository()
        {
        }

        public void Add(string basePath, HttpPostedFileBase files)
        {
            var fileName = Path.GetFileName(files.FileName);
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            var fullName = Path.Combine(basePath, fileName);
            files.SaveAs(fullName);
        }

        public UserResourceViewModel Get(string basePath, string path, string id)
        {
            var resources = new UserResourceViewModel
            {
                FileNames = new List<string> { id },
                Directory = $@"/Content/Uploads/{path}/"
            };
            return resources;
        }

        public UserResourceViewModel getAll(string basePath, string userId)
        {
            var path = Path.Combine(basePath, userId);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fileNames = Directory.GetFiles(Path.Combine(basePath, userId)).ToList().Select(p => Path.GetFileName(p));
            var resources = new UserResourceViewModel
            {
                FileNames = fileNames,
                Directory = $@"/Content/Uploads/{userId}/"
            };
            return resources;
        }
    }
}