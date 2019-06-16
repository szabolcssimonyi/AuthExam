using AuthExam.Models;
using System.Web;

namespace AuthExam.Infrastructure.Interfaces
{
    public interface IImageRepository
    {
        string BasePath
        {
            get;
        }
        UserResourceViewModel getAll(string basePath, string userId);
        void Add(string basePath, HttpPostedFileBase files);
        UserResourceViewModel Get(string basePath, string path, string id);
    }
}
