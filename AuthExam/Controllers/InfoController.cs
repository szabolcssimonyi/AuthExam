using AuthExam.Infrastructure.Interfaces;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AuthExam.Controllers
{
    public class InfoController : Controller
    {
        private readonly ApplicationUserManager userManager;
        private readonly IImageRepository imageRepository;

        public InfoController(IImageRepository imageRepository, ApplicationUserManager userManager)
        {

            this.userManager = userManager;
            this.imageRepository = imageRepository;
        }

        public async Task<JsonResult> Get(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var basePath = Server.MapPath(imageRepository.BasePath);
            var result = imageRepository.getAll(basePath, user.Email);
            var paths = result.FileNames.Select(f => Path.Combine(result.Directory, f));
            return Json(paths, JsonRequestBehavior.AllowGet);
        }
    }
}