using System.Web.Http;

namespace OwinHelloWorld
{
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
}