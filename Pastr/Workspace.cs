using System;
using System.IO;

namespace Pastr
{
    public static class Workspace
    {
        private static DirectoryInfo Root { get; set; }

        public static string LogFilePath => Path.Combine(Root.FullName, "log.txt");

        static Workspace()
        {
            var appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Root = new DirectoryInfo(Path.Combine(appdataPath, "Jay Wick Labs", "Pastr"));

            if (!Root.Exists)
                Root.Create();
        }
    }
}
