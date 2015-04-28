using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XServer;

namespace XServer.TestApp
{
    class ExampleController : Controller
    {
        public ExampleController(string route, bool exact) : base(route, exact)
        {

        }

        public override int OnConnection(HttpRequest request, HttpResponse response)
        {
            response.SetBody("HELLO WORLD");
            return 200;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            WebServer ws = new WebServer(8080, false);
            ws.RegisterController(new StaticFileController(new string[] {"{{CWD}}"}));
            ws.Start();
        }
    }
}
