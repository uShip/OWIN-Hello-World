using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Web.Http;
using Topshelf;

namespace OwinHelloWorld
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return (int)HostFactory.Run(x =>
            {
                x.Service<OwinService>(s =>
                {
                    s.ConstructUsing(() => new OwinService());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });
            });
        }
    }

    public class OwinService
    {
        private IDisposable _webApp;

        public void Start()
        {
            _webApp = WebApp.Start<StartOwin>("http://localhost:9000");
        }

        public void Stop()
        {
            _webApp.Dispose();
        }
    }

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

            appBuilder.Use<OwinContextMiddleware>();
            appBuilder.UseWebApi(config);
        }
    }

    public class HelloWorldController : ApiController
    {
        public string Get()
        {
            const string messageKey = "message";
            const string messageValue = "Hello, World!";
            OwinCallContext.Current.Set(messageKey, messageValue);
            return OwinCallContext.Current.Get<string>(messageKey);
        }
    }
}