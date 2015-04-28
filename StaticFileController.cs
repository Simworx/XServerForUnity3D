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
            : base("/static/", false)
        {
            Directories = dirs.Select(d => d.Replace("{{CWD}}", Directory.GetCurrentDirectory())).Where(Directory.Exists);
        }

        public string ReadFile(string fileName)
        {
            foreach (var dir in Directories)
            {
                var path = Path.Combine(dir, fileName);
                if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }
            }
            return null;
        }

        public override void OnConnection(XServer.HttpRequest request, XServer.HttpResponse response)
        {
            var fileName = request.Url.LocalPath.Replace(this.Route, "");
            foreach (var dir in Directories)
            {
                var path = Path.Combine(dir, fileName);
                if (File.Exists(path))
                {
                    response.SetFile(path);
                    return;
                }
            }
            response.SetCode(404);
        }
    }
}
