using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XServer
{
    /// <summary>
    /// Serves static files
    /// </summary>
    public class StaticFileController : XServer.Controller
    {
        public IEnumerable<string> Directories { get; private set; }

        public StaticFileController(string[] dirs)
            : base("", false)
        {
            Directories = dirs.Select(d => d.Replace("{{CWD}}", Directory.GetCurrentDirectory())).Where(Directory.Exists);
        }

        public override int OnConnection(XServer.HttpRequest request, XServer.HttpResponse response)
        {
            var fileName = request.Url.LocalPath.TrimStart('/');
            if (fileName == string.Empty)
            {
                fileName = "index.html";
            }

            foreach (var dir in Directories)
            {
                var path = Path.Combine(dir, fileName);
                if (File.Exists(path))
                {
                    response.SetFile(path);
                    return 200;
                }
            }
            return 404;
        }
    }
}
