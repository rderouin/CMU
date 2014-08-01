using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CurriculumManagement.Startup))]
namespace CurriculumManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
