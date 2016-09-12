using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OfficeSpace.Startup))]
namespace OfficeSpace
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
