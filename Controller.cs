using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServer
{
    public class Controller
    {
        public string Route { get; private set; }
        public bool ExactMatch { get; private set; }

        public Controller(string route,bool exact)
        {
            this.Route = route;
            this.ExactMatch = exact;
        }

        public virtual int OnConnection(HttpRequest request, HttpResponse response)
        {


            return 200;
        }

        
    }
}
