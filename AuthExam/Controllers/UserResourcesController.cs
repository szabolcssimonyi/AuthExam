using AuthExam.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AuthExam.Controllers
{
    [Authorize]
    public class UserResourcesController : Controller
    {
        private readonly IImageRepository repository;
        private readonly ApplicationUserManager userManager;

        public UserResourcesController(IImageRepository repository, ApplicationUserManager userManager)
        {
            this.repository = repository;
            this.userManager = userManager;
        }

        // GET: UserResources
        public async Task<ActionResult> Index()
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);
            ViewBag.UserId = user.Id;
            var basePath = Server.MapPath(repository.BasePath);
            return View(repository.getAll(basePath, User.Identity.Name));
        }

        public ActionResult Get(string id)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(id);
            var imageName = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            var basePath = Server.MapPath(repository.BasePath);
            var userDir = User.Identity.Name;
            var imagePath = repository.Get(basePath, userDir, imageName);
            return View(imagePath);
        }

        [HttpPost]
        public ActionResult Add(IEnumerable<HttpPostedFileBase> files)
        {
            var basePath = Server.MapPath(repository.BasePath);
            var userDir = User.Identity.Name;
            var path = Path.Combine(basePath, userDir);
            files.ToList().ForEach(f => repository.Add(path, f));
            return RedirectToAction("Index");
        }

    }
}
