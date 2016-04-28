using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace OwinHelloWorld
{
    /// <summary>
    /// Buffers the request stream to allow for reading multiple times.
    /// The Katana (OWIN implementation) implementation of request streams
    /// is different than that of IIS.
    /// </summary>
    public class RequestBufferingMiddleware : OwinMiddleware
    {
        public RequestBufferingMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        // Explanation of why this is necessary: http://stackoverflow.com/a/25607448/4780595
        // Implementation inspiration: http://stackoverflow.com/a/26216511/4780595
        public override Task Invoke(IOwinContext context)
        {
            var requestStream = context.Request.Body;
            var requestMemoryBuffer = new MemoryStream();
            requestStream.CopyTo(requestMemoryBuffer);
            requestMemoryBuffer.Seek(0, SeekOrigin.Begin);

            context.Request.Body = requestMemoryBuffer;

            return Next.Invoke(context);
        }
    }
}