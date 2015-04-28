using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace XServer
{
    public class WebServer
    {
        public int Port { get; private set; }
        public bool IsRunning { get; private set; }
        public Dictionary<string, Controller> Controllers { get; protected set; }

        TcpListener _tcpListener;
        Thread _listeningThread;
        bool _allowGzipCompression = true;

        public WebServer(int port, bool allowGzipCompression = true)
        {
            _allowGzipCompression = allowGzipCompression;

            //Logger.Init();
            Http.Init();

            Renderer.TemplateDirectory = Directory.GetCurrentDirectory() + "\\templates\\";

            Controllers = new Dictionary<string, Controller>();            

            this.RegisterController(new NotFoundController());
            this.RegisterController(new FaviconController());
            this.RegisterController(new DefaultController());

            this.Port = port;
        }

        public void RegisterController(Controller ctrl)
        {
            Controllers[ctrl.Route] =  ctrl;
        }

        public void Start()
        {
            IsRunning = true;
            _listeningThread = new Thread(new ThreadStart(Listening));
            _listeningThread.Start();
        }

        public void Stop()
        {
            if (_listeningThread != null)
            {
                IsRunning = false;
                _listeningThread.Join(500);
                _listeningThread = null;
            }
        }

        void Listening()
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Any,Port);
                _tcpListener.Start();

                while (IsRunning)
                {
                    // check if new connections are pending, if not, be nice and sleep 100ms
                    if (!_tcpListener.Pending())
                    {
                        Thread.Sleep(100);
                    }
                    else
                    {
                        var client = _tcpListener.AcceptTcpClient();

                        new Thread(new ThreadStart(delegate()
                        {
                            ProcessConnection(client);

                        })).Start();
                    }

                }
            }
            catch (ThreadAbortException)
            {
                
            }
            finally
            {
                IsRunning = false;
                _tcpListener.Stop();                
            }            
        }

        void ProcessConnection(TcpClient client)
        {
            HttpRequest request = HttpRequest.Parse(client.Client,client.GetStream());

            if (request != null)
            {
                //Logger.Log(request.Method + " " + request.Url,request);

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

                var response = request.CreateRepsonse();
                response.AllowGzipCompression = _allowGzipCompression;

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
                    //Logger.LogWarning(ex.ToString());
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
    }
}
