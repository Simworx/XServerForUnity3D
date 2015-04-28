using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XServer;

namespace XServer.TestApp
{
    class RootController : XServer.Controller
    {
        readonly XServer.StaticFileController _sfc = null;
        public RootController(XServer.StaticFileController sfc)
            : base("/", true)
        {
            _sfc = sfc;
        }

        public override void OnConnection(XServer.HttpRequest request, XServer.HttpResponse response)
        {
            var contents = _sfc.ReadFile("index.html");
            if (contents != null)
            {
                response.SetBody(contents);
            } else
            {
                response.SetCode(404);
            }
        }
    }

    class ExampleController : Controller
    {
        public ExampleController(string route, bool exact) : base(route, exact)
        {

        }

        public override void OnConnection(HttpRequest request, HttpResponse response)
        {
            response.SetBody("Hello, World!", 200);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var sfc = new StaticFileController(new string[] { "{{CWD}}" });
            var rc = new RootController(sfc);

            WebServer ws = new WebServer(8080, false);
            ws.RegisterController(rc);
            ws.RegisterController(sfc);
            ws.Start();
        }
    }
}
