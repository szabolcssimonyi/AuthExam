using AuthExam.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AuthExam.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationUserManager userManager;
        private readonly ApplicationDbContext context;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserController(ApplicationUserManager userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public JsonResult Cities()
        {
            return Json(context.Cities.ToList(), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<ActionResult> VerifyEmail(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return Json($"Email {email} is already in use.", JsonRequestBehavior.AllowGet);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // GET: User
        public ActionResult Index()
        {
            var adminId = roleManager.Roles.Single(r => r.Name == "Admin").Id;
            var userModels = userManager.Users.Select(u => new UserViewModel
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name,
                PhoneNumber = u.PhoneNumber,
                Addresses = u.Addresses.ToList(),
                IsAdmin = u.Roles.Select(ur => ur.RoleId).Contains(adminId)
            });
            return View(userModels);
        }

        [HttpPost, HttpGet]
        public ActionResult Details()
        {
            return RedirectToAction("");
        }

        // GET: User/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            var user = await userManager.FindByIdAsync(id);
            var adminId = roleManager.Roles.Single(r => r.Name == "Admin").Id;
            return View(new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Addresses = context.Addresses.Where(a => a.UserId == id).ToList(),
                IsAdmin = user.Roles.Select(ur => ur.RoleId).Contains(adminId)
            });
        }

        // GET: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserViewModel user)
        {
            var result = await userManager.CreateAsync(new ApplicationUser
            {
                Email = user.Email,
                UserName = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Addresses = user.Addresses.ToList()
            });
            if (result.Succeeded)
            {
                var newUser = await userManager.FindByEmailAsync(user.Email);
                await userManager.AddToRoleAsync(newUser.Id, "User");
            }
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View(new UserViewModel
            {
                Addresses = new List<AddressViewModel>
                {
                    new AddressViewModel
                    {
                        Cities = context.Cities.ToList()
                    }
                },
            });
        }

        // GET: User/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var adminId = roleManager.Roles.Single(r => r.Name == "Admin").Id;
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            var user = await userManager.FindByIdAsync(id);
            if (user.Roles.Select(ur => ur.RoleId).Contains(adminId))
            {
                return RedirectToAction("Index");
            }

            var addresses = context.Addresses.Where(a => a.User.Id == user.Id).ToList();
            var cities = context.Cities.ToList();
            addresses.ForEach(a => a.Cities = cities);
            var model = new UserViewModel
            {
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Addresses = addresses,
                Cities = context.Cities.ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserViewModel user)
        {
            var adminId = roleManager.Roles.Single(r => r.Name == "Admin").Id;
            var appUser = await userManager.FindByIdAsync(user.Id);
            if (appUser.Roles.Select(ur => ur.RoleId).Contains(adminId))
            {
                return RedirectToAction("Index");
            }
            var roles = await userManager.GetRolesAsync(appUser.Id);
            if (roles.Any(r => r == "Admin"))
            {
                return RedirectToAction("Index");
            }

            context.Addresses.RemoveRange(context.Addresses.Where(a => a.UserId == user.Id));
            await context.SaveChangesAsync();
            var addresses = user.Addresses;
            foreach (var address in addresses)
            {
                address.Id = 0;
            }
            appUser.PhoneNumber = user.PhoneNumber;
            appUser.Name = user.Name;
            appUser.UserName = user.Email;
            appUser.Email = user.Email;
            appUser.Addresses = addresses;
            await userManager.UpdateAsync(appUser);
            if (!string.IsNullOrEmpty(user.Password))
            {
                var result = await userManager.PasswordValidator.ValidateAsync(user.Password);
                if (!result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                await userManager.RemovePasswordAsync(appUser.Id);
                await userManager.AddPasswordAsync(appUser.Id, user.Password);
            }
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            context.Addresses.RemoveRange(context.Addresses.Where(a => a.UserId == id));
            await context.SaveChangesAsync();
            await userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }
    }
}
