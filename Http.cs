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
            codes[100] = "100 Continue";
            codes[101] = "101 Switching Protocols";
            codes[200] = "200 OK";
            codes[201] = "201 Created";
            codes[202] = "202 Accepted";
            codes[203] = "203 Non-Authoritative Information";
            codes[204] = "204 No Content";
            codes[205] = "205 Reset Content";
            codes[206] = "206 Partial Content";
            codes[300] = "300 Multiple Choices";
            codes[301] = "301 Moved Permanently";
            codes[302] = "302 Found";
            codes[303] = "303 See Other";
            codes[304] = "304 Not Modified";
            codes[305] = "305 Use Proxy";
            codes[307] = "307 Temporary Redirect";
            codes[400] = "400 Bad Request";
            codes[401] = "401 Unauthorized";
            codes[402] = "402 Payment Required";
            codes[403] = "403 Forbidden";
            codes[404] = "404 Not Found";
            codes[405] = "405 Method Not Allowed";
            codes[406] = "406 Not Acceptable";
            codes[407] = "407 Proxy Authentication Required";
            codes[408] = "408 Request Time-out";
            codes[409] = "409 Conflict";
            codes[410] = "410 Gone";
            codes[411] = "411 Length Required";
            codes[412] = "412 Precondition Failed";
            codes[413] = "413 Request Entity Too Large";
            codes[414] = "414 Request-URI Too Large";
            codes[415] = "415 Unsupported Media Type";
            codes[416] = "416 Requested range not satisfiable";
            codes[417] = "417 Expectation Failed";
            codes[500] = "500 Internal Server Error";
            codes[501] = "501 Not Implemented";
            codes[502] = "502 Bad Gateway";
            codes[503] = "503 Service Unavailable";
            codes[504] = "504 Gateway Time-out";
            codes[505] = "505 HTTP Version not supported";

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
