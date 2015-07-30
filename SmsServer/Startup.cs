using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SmsServer.Startup))]
namespace SmsServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
