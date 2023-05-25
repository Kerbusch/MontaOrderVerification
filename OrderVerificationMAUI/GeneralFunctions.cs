using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderVerificationMAUI
{
    internal class GeneralFunctions
    {
        // Constructer
        public GeneralFunctions() { }

        // Returns the base path of the repo directory
        public string getPath(string folder)
        {
            string path = Path.GetDirectoryName(AppContext.BaseDirectory);
            string[] paths = path.Split('\\');
            path = "";
            for (int i = 0; i < paths.Length; i++) {
                path += paths[i] + "\\";
                if (paths[i] == folder) {
                    break;
                }
            }
            return path;
        }

        // Returns a list with all the vendors
        public List<string> getVendors()
        {
            string path = getPath("OrderVerificationMAUI");
            string text = File.ReadAllText(path + "\\Vendors.txt");
            var vec = new List<string>();
            foreach (string item in text.Split("\n"))
            {
                if (item.Length > 0)
                {
                    vec.Add(item);
                }
            }
            vec.Add("Add new vendor");
            return vec;
        }
    }
}
