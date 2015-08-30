using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Pastr
{
    public static class Workspace
    {
        public static DirectoryInfo Root { get; private set; }

        public static string LogFilePath
        {
            get { return Path.Combine(Root.FullName, "log.txt"); }
        }

        static Workspace()
        {
            var appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Root = new DirectoryInfo(Path.Combine(appdataPath, "Jay Wick Labs", "Pastr"));

            if (!Root.Exists)
                Root.Create();
        }
    }
}
