using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace OwinHelloWorld
{
    /// <summary>
    /// Sets the current <see cref="IOwinContext"/> for later access via <see cref="OwinCallContext.Current"/>.
    /// Inspiration: https://github.com/neuecc/OwinRequestScopeContext
    /// </summary>
    public class OwinContextMiddleware : OwinMiddleware
    {
        public OwinContextMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                OwinCallContext.Set(context);
                await Next.Invoke(context);
            }
            finally
            {
                OwinCallContext.Remove(context);
            }
        }
    }

    /// <summary>
    /// Helper class for setting and accessing the current <see cref="IOwinContext"/>
    /// </summary>
    public class OwinCallContext
    {
        private const string OwinContextKey = "owin.IOwinContext";

        public static IOwinContext Current
        {
            get { return (IOwinContext)CallContext.LogicalGetData(OwinContextKey); }
        }

        public static void Set(IOwinContext context)
        {
            CallContext.LogicalSetData(OwinContextKey, context);
        }

        public static void Remove(IOwinContext context)
        {
            CallContext.FreeNamedDataSlot(OwinContextKey);
        }
    }
}