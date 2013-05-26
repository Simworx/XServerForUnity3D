using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServer
{
    public class QueryString
    {
        public Dictionary<string, string> Entries { get; set; }

        public QueryString(string s)
        {
            Entries = new Dictionary<string, string>();

            if (s[0] == '?')
            {
                s = s.Remove(0, 1);

                var combos = s.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var c in combos)
                {
                    string f = "";
                    string f2 = "";

                    var str = c.Split(new string[] { "=" }, 2, StringSplitOptions.None);

                    if (str.Length >= 1)
                        f = str[0];

                    if (str.Length == 2)
                        f = str[1];

                }
            }
        }
    }
}
