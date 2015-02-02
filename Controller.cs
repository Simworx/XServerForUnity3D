using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XServer
{
    public abstract class Controller
    {
        public string Route { get; private set; }
        public bool ExactMatch { get; private set; }

        public Controller(string route,bool exact)
        {
            this.Route = route;
            this.ExactMatch = exact;
        }

        public abstract int OnConnection(HttpRequest request, HttpResponse response);

        
    }
}
