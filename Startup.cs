using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Chess.Startup))]
namespace Chess
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
