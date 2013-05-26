using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServer
{
    public class FaviconController : Controller
    {
        public FaviconController()
            : base("favicon.ico",true)
        {

        }

        public override int OnConnection(HttpRequest request, HttpResponse response)
        {
            response.SetFile(@"favicon.png");

            return base.OnConnection(request, response);
        }
    }
}
