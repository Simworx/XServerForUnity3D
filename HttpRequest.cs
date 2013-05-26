using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace XServer
{
    public class HttpRequest
    {
        public enum Methods
        {
            GET,
            POST
        }

        public Methods Method{get; protected set;}
        public Uri Url { get; protected set; }
        public Dictionary<string, string> Headers { get; protected set; }
        public NetworkStream Stream { get; protected set; }
        public Socket Socket { get; protected set; }

        public string RemoteIp { get; private set; }

        public QueryString QueryString { get; set; }

        protected HttpRequest()
        {
            
        }

        public HttpResponse CreateRepsonse()
        {
            return new HttpResponse(this);
        }

        public static HttpRequest Parse(Socket socket,NetworkStream stream)
        {
            try
            {
                HttpRequest request = new HttpRequest();
                request.Socket = socket;
                request.RemoteIp = request.Socket.RemoteEndPoint.ToString();

                request.Stream = stream;

                StreamReader sReader = new StreamReader(stream);

                bool read = true;

                Dictionary<string, string> headers = new Dictionary<string, string>();

                string head = sReader.ReadLine();

                if (head == null)
                {
                    return null;
                }

                //Console.WriteLine(head);
                //Logger.Log(head);

                string[] hS = head.Split(' ');

                switch (hS[0])
                {
                    case "GET":
                        request.Method = Methods.GET;
                        break;
                    default:
                        request.Method = Methods.GET;
                        break;
                }

                Uri ur = new Uri("http://localhost" + hS[1]);

                request.QueryString = new QueryString(ur.Query);
                //Console.WriteLine("URI: "+ur.LocalPath);


                request.Url = ur;

                while (read)
                {
                    string line = sReader.ReadLine();
                    //Console.WriteLine(line);
                    //Logger.Log(line);
                    if (line == "")
                        break;

                    string[] spl = line.Split(new string[] { ": " }, StringSplitOptions.None);

                    if (spl.Length == 1)
                        spl = line.Split(new string[] { ":" }, StringSplitOptions.None);

                    headers.Add(spl[0], spl[1]);
                }

                request.Headers = headers;



                return request;
            }
            catch (Exception ex)
            {
                Logger.LogError("Error parsing package \n"+ex.Message+" \n "+ex.StackTrace);
                
                return null;
            }
        }
    }
}
