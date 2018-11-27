using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EasyFreight.Startup))]
namespace EasyFreight
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
