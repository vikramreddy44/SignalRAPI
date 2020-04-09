using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
[assembly: OwinStartup(typeof(SignalRAPI.Startup))]

namespace SignalRAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           
            app.UseCors(CorsOptions.AllowAll);
            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                HubConfiguration hubConfiguration = new HubConfiguration
                {
                    EnableDetailedErrors = true,
                    EnableJavaScriptProxies = true
                };
               
                map.RunSignalR(hubConfiguration);
            });
        }
    }
}