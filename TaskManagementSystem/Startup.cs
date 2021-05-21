using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TaskManagementSystem.Startup))]
namespace TaskManagementSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
