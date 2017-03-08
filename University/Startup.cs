using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(University.Startup))]
namespace University
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
