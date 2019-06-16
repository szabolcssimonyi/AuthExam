using AuthExam.Infrastructure.Interfaces;
using AuthExam.Infrastructure.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Mvc5;

namespace AuthExam
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            container.RegisterType<IImageRepository, ImageFileRepository>();
            container.RegisterType<ApplicationUserManager>();
            container.RegisterType<RoleManager<IdentityRole>>();
            container.RegisterType<ApplicationDbInitializer>();
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();
            container.RegisterType<IRoleStore<IdentityRole, string>, RoleStore<IdentityRole, string, IdentityUserRole>>();
            container.RegisterType<DbContext, ApplicationDbContext>(new HierarchicalLifetimeManager());
            container.RegisterType<UserManager<ApplicationUser>>(new HierarchicalLifetimeManager());
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(new HierarchicalLifetimeManager());
            container.RegisterType<IAuthenticationManager>(
                    new InjectionFactory(c => HttpContext.Current.GetOwinContext().Authentication));
        }
    }
}