using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CanvasMVC.Startup))]
namespace CanvasMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
