using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace XServer
{
    public class HttpResponse
    {
        public Dictionary<string, string> Headers { get; protected set; }
        public HttpRequest Request { get; protected set; }
        public string RespCode { get; set; }

        

        string file = "";
        string body = "";
        Stream stream;

        FileStream fStream;

        byte[] writeBuffer = new byte[1];

        int fStart = -1;
        int fEnd = -1;

        bool closed = false;

        Bitmap ic = null;

        public HttpResponse(HttpRequest request)
        {
            this.Request = request;
            this.stream = request.Stream;
            this.Headers = new Dictionary<string, string>();
            this.RespCode = "UNKNOWN";

            Headers.Add("Content-Type", "text/html");
        }

        public string GetBody()
        {
            return body;
        }

        public void SetBitmap(Bitmap ic)
        {
            this.ic = ic;
        }

        public void SetBody(string body)
        {
            this.body = body;
        }

        public void SetFile(string file,bool partial = false)
        {
            this.file = file;

            if (partial && Request.Headers.ContainsKey("Range"))
            {
                string[] range = Request.Headers["Range"].Replace("bytes=", "").Split('-');

                int start = Int32.Parse(range[0]);
                int end = -1;

                if (range[1] != "")
                {
                    end = Int32.Parse(range[1]);
                }

                fStart = start;
                fEnd = end;

                RespCode = "206 Partial Content";
            }

        }

        /// <summary>
        /// Sets partial files
        /// </summary>
        /// <param name="file"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void SetFile(string file, int start, int end)
        {


            this.file = file;
            fStart = start;
            fEnd = end;

            RespCode = "206 Partial Content";
        }

        public void Redirect(string url)
        {
            this.SetCode(307);
            this.Headers["Location"] = url;
        }

        public void SetCode(int code)
        {
            this.RespCode = Http.GetFullCode(code);
        }

        public void Send404Response()
        {
            this.RespCode = "404 Not Found";
            this.Send();
        }

        public void Send()
        {
            if (closed)
            {
                Logger.LogError("Sending already sent response",Request);
                return;
            }


            SendHeader();
            SendBody();
            Close();

            closed = true;
        }

        void SendHeader()
        {
                StringBuilder sb = new StringBuilder();
                

                Headers["Content-Length"] = PrepareBody().ToString();
                Headers["Accept-Ranges"] = "bytes";
                Headers["Date"] = DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss ")+"GMT+3";

                sb.Append("HTTP/1.1 " + RespCode + "\r\n");

                foreach (var k in Headers)
                {
                    sb.Append(k.Key + ":" + k.Value + "\r\n");
                }

                sb.Append("\r\n");

                byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());


                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            

            

        }


        long PrepareBody()
        {
            if (ic != null)
            {
                MemoryStream ms = new MemoryStream();
                ic.MakeTransparent();
                ic.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                Headers["Content-Type"] = "image/png";

                ms.Position = 0;

                writeBuffer = new byte[ms.Length];

                ms.Read(writeBuffer, 0, (int)ms.Length);
                ms.Close();
                ms.Dispose();

                return writeBuffer.Length;
            }
            else

            if (file != "")
            {
                if (!File.Exists(file))
                {
                    this.SetCode(404);
                    return 0;

                }

                FileInfo fi = new FileInfo(file);

                if (Request.Headers.ContainsKey("Accept-Encoding") && Request.Headers["Accept-Encoding"].Contains("gzip"))
                {
                    Headers["Content-Encoding"] = "gzip";
                }

                Headers["Last-Modified"] = fi.LastWriteTime.ToString("ddd, dd MMM yyyy HH:mm:ss ")+"GMT+3";
                Headers["Expires"] = DateTime.Now.AddDays(7).ToString("ddd, dd MMM yyyy HH:mm:ss ")+"GMT+3";

                Headers["Content-Type"] = TdxF.MIMETypes.GetMimeType(fi.Extension);
                fStream = new FileStream(file, FileMode.Open, FileAccess.Read);

                int start = 0;
                int end = (int)fStream.Length;

                if (fStart != -1)
                {
                    start = fStart;
                    RespCode = "206 Partial Content";
                }


                if (fEnd == -1)
                    end = end - start;
                else
                {
                    end = fEnd;
                }

                if (RespCode.StartsWith("206"))
                {
                    Headers.Add("Content-Range", "bytes " + start.ToString() + "-" + end.ToString() + "/" + fStream.Length.ToString());
                } 

                //Console.WriteLine("File: Start-"+start+" End: "+end);

                writeBuffer = new byte[end];
                fStream.Position = start;
                fStream.Read(writeBuffer, 0, end);
                

                fStream.Close();
                fStream.Dispose();

                

                return end;
            }
            else if (body != "")
            {
                /*if (Request.Headers.ContainsKey("Accept-Encoding") && Request.Headers["Accept-Encoding"].Contains("gzip"))
                {
                    Headers["Content-Encoding"] = "gzip";
                }*/

                
                writeBuffer = Encoding.UTF8.GetBytes(body);
                return writeBuffer.Length;
            }

            return 0;
        }

        void SendBody()
        {
            try
            {
                /*if (Headers.ContainsKey("Content-Encoding") && Headers["Content-Encoding"] == "gzip")
                {
                    stream = new GZipStream(stream);
                    
                }*/

                    stream.Write(writeBuffer, 0, writeBuffer.Length);
                    stream.Flush();
                

                

                
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Close()
        {

                //Request.Stream.Close();

            try
            {
                stream.Close();
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.Message);
            }

            //stream.Dispose();
            writeBuffer = null;

            GC.Collect();
        }
    }
}
