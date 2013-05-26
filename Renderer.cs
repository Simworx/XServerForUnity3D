using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServer
{
    public class Renderer
    {
        public static string TemplateDirectory = "";

        public static string Render(string templateName, Dictionary<string, string> values)
        {
            if (File.Exists(TemplateDirectory + templateName + ".html"))
            {
                var sR = new StreamReader(File.OpenRead(TemplateDirectory + templateName + ".html"));

                string fileContent = sR.ReadToEnd();

                foreach (var d in values)
                {
                    fileContent = fileContent.Replace("{"+d.Key+"}",d.Value);
                }

                sR.Close();
                sR.Dispose();

                return fileContent;
            }
            else
            {
                Logger.LogError("Can't find template " + templateName);
            }

            return "";
        }

        public static string RenderMain(string template, string body)
        {
            var d = new Dictionary<string, string>();
            d["BODY"] = body;

            return Render(template, d);
        }
    }
}
