using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XServer
{
    public class Http
    {
        static Dictionary<int, string> codes = new Dictionary<int, string>();

        public static void Init()
        {
            codes[404] = "404 Not Found";
            codes[200] = "200 OK";
            codes[206] = "206 Partial Content";
            codes[307] = "307 Temporary Redirect";
            codes[403] = "403 Forbidden";
        }

        public static string GetFullCode(int code)
        {
            if (codes.ContainsKey(code))
            {
                return codes[code];
            }
            else
            {
                return code.ToString() + " UNKNOWN";
            }

        }
    }
}
