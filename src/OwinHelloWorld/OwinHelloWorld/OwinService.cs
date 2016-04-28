using System;
using Microsoft.Owin.Hosting;

namespace OwinHelloWorld
{
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
}