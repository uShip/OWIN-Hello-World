using System.Web.Http;
using Owin;

namespace OwinHelloWorld
{
    public class StartOwin
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
            config.MessageHandlers.Add(new StreamReadingDelegatingHandler());

            appBuilder.Use<OwinContextMiddleware>();
            appBuilder.Use<RequestBufferingMiddleware>();
            appBuilder.UseWebApi(config);
        }
    }
}