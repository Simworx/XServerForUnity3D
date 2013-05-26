using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServer
{
    public class DefaultController : Controller
    {
        public DefaultController()
            : base("",false)
        {

        }

        public override int OnConnection(HttpRequest request, HttpResponse response)
        {
            response.SetBody("Hello World");
            return base.OnConnection(request, response);
        }

    }
}
