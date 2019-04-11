using Autodesk.Revit.Attributes;
using System.Threading;
namespace FirstPlugin
{
    public class Request
    {
        private int myRequest = 0;
        public RequestId Take()
        {
            return (RequestId)Interlocked.Exchange(ref this.myRequest, 0);
        }
        public void Make(RequestId request)
        {
            Interlocked.Exchange(ref this.myRequest, (int)request);
        }
    }
}
