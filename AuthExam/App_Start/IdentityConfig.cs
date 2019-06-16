using AuthExam.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthExam
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public ICollection<AddressViewModel> Addresses { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<CityViewModel> Cities { get; set; }
        public DbSet<AddressViewModel> Addresses { get; set; }
        public ApplicationDbContext()
           : base("DefaultConnection", throwIfV1Schema: false)
        {
           // Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationDbContext>());
        }
    }

    public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        private readonly ApplicationUserManager userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private List<string> cities = new List<string> { "Budapest", "Győr", "Szeged", "Kecskemét", "Nyíregyháza", "Debrecen" };

        public ApplicationDbInitializer(ApplicationUserManager userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            if (!context.Cities.Any())
            {
                context.Cities.AddRange(cities.Select(c => new CityViewModel { Name = c }));
                context.SaveChanges();
            }
            if (context.Users.Any(u => u.Email == "admin@auth.com"))
            {
                return;
            }
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole
                {
                    Name = "Admin"
                });
                roleManager.Create(new IdentityRole
                {
                    Name = "User"
                });
            }
            var result = userManager.Create(new ApplicationUser
            {
                Email = "admin@auth.com",
                UserName = "admin@auth.com",
                Name = "AdminUser",
                PhoneNumber = "123456",
            });
            if (result.Succeeded)
            {
                var user = userManager.Users.ToList().Single(u => u.Email == "admin@auth.com");
                userManager.AddPassword(user.Id, "secretUser01!");
                userManager.AddToRole(user.Id, "Admin");
                context.Addresses.AddRange(new List<AddressViewModel>
                {
                    new AddressViewModel
                    {
                        City=context.Cities.First(),
                        HouseNumber="11",
                        Street="Kossuth Lajos utca",
                        ZipCode="222000",
                        UserId=user.Id

                    },
                    new AddressViewModel
                    {
                        City=context.Cities.First(),
                        HouseNumber="11",
                        Street="Kossuth Lajos utca",
                        ZipCode="222000",
                         UserId=user.Id
                   }
                });
                context.SaveChanges();
            }
            base.Seed(context);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
