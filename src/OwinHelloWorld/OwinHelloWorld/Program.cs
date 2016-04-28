using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
            config.MessageHandlers.Add(new StreamReadingDelegatingHandler());

            appBuilder.Use<OwinContextMiddleware>();
            appBuilder.Use<RequestBufferingMiddleware>();
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

        public string Post([FromBody]HelloWorldModel model)
        {
            return model.Message;
        }

        public class HelloWorldModel
        {
            public string Message { get; set; }
        }
    }

    public class StreamReadingDelegatingHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var stream = request.Content.ReadAsStreamAsync().Result;
            var content = new StreamReader(stream).ReadToEnd();
            stream.Seek(0, SeekOrigin.Begin);
            Console.WriteLine("Content is " + content);
            
            return base.SendAsync(request, cancellationToken);
        }
    }
}