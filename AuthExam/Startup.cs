using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AuthExam.Startup))]
namespace AuthExam
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
