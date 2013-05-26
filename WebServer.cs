using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XServer
{
    public class WebServer
    {
        public int Port { get; private set; }
        public bool Running { get; private set; }

        TcpListener listener;
        Thread listeningThread;

        public Dictionary<string, Controller> Controllers { get; protected set; }

        public WebServer(int port)
        {
            Logger.Init();
            Http.Init();

            Renderer.TemplateDirectory = Directory.GetCurrentDirectory() + "\\templates\\";

            Controllers = new Dictionary<string, Controller>();
            

            this.RegisterController(new NotFoundController());
            this.RegisterController(new FaviconController());
            this.RegisterController(new DefaultController());

            this.Port = port;

            listener = new TcpListener(IPAddress.Any,port);
            OnConnectionReceived += WebServer_OnConnectionReceived;
            
        }

        void WebServer_OnConnectionReceived(TcpClient client,HttpRequest request)
        {
            
        }

        public void RegisterController(Controller ctrl)
        {
            Controllers[ctrl.Route] =  ctrl;
        }

        public void Start()
        {
            Running = true;
            listener.Start();


            listeningThread = new Thread(new ThreadStart(Listening));
            listeningThread.Start();
        }

        void Listening()
        {
            while (Running)
            {
                var client = listener.AcceptTcpClient();

                new Thread(new ThreadStart(delegate()
                {
                    ProcessConnection(client);

                })).Start();

            }
        }

        void ProcessConnection(TcpClient client)
        {
            HttpRequest request = HttpRequest.Parse(client.Client,client.GetStream());

            

            if (request != null)
            {
                Logger.Log(request.Method + " " + request.Url,request);

                Controller ctrlr = Controllers["404"];

                string rUrl = request.Url.LocalPath.Remove(0, 1);

                if (Controllers.ContainsKey(rUrl))
                {
                    ctrlr = Controllers[rUrl];
                }
                else if(Controllers.ContainsKey(rUrl.Remove(rUrl.Length-1,1)))
                {
                    ctrlr = Controllers[rUrl.Remove(rUrl.Length-1,1)];
                }
                else
                {
                    List<Controller> matchedCtrl = new List<Controller>();

                    foreach (var ctrl in Controllers)
                    {
                        if (!ctrl.Value.ExactMatch && rUrl.StartsWith(ctrl.Key))
                        {
                            matchedCtrl.Add(ctrl.Value);
                        }
                    }

                    if (matchedCtrl.Count > 1)
                    {
                        List<int> dff = new List<int>();
                        Dictionary<int, Controller> rt = new Dictionary<int, Controller>();

                        foreach (var ctrl in matchedCtrl)
                        {
                            dff.Add(Compute(rUrl, ctrl.Route));
                            rt.Add(dff[dff.Count - 1], ctrl);
                            
                        }

                        dff.Sort();

                        ctrlr = rt[dff[0]];

                    }
                    else if (matchedCtrl.Count == 1)
                    {
                        ctrlr = matchedCtrl[0];
                    }
                }

                Console.WriteLine("Route: "+ctrlr.Route);

                //OnConnectionReceived(client, request);

                var response = request.CreateRepsonse();

                int code = 500;

                try
                {
                    code = ctrlr.OnConnection(request, response);

                }
                catch (UnauthorizedAccessException ex)
                {
                    response.SetCode(403);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex.ToString());
                }

                


                if (response.RespCode == "UNKNOWN")
                    response.SetCode(code);

                response.Send();

            }
                
        }

        public static int Compute(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }

        public delegate void ConnectionReceivedHandler(TcpClient client, HttpRequest request);
        public event ConnectionReceivedHandler OnConnectionReceived;

    }
}
