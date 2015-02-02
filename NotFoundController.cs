﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XServer
{
    public class NotFoundController : Controller
    {
        public NotFoundController()
            : base("404",false)
        {

        }

        public override int OnConnection(HttpRequest request, HttpResponse response)
        {
            return 404;
        }
    }
}
