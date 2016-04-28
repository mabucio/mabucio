using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FirstWebApplication.Startup))]
namespace FirstWebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
