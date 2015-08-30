using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pastr
{
    public class Common
    {
        public static string GetVersionInfo()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

            if (versionInfo == "DEBUG")
                return versionInfo + "-" + new FileInfo(assembly.Location).LastWriteTime.ToString("yyyyMMdd_HHmmss");

            return versionInfo;
        }

        public static void Log(string message, params string[] args)
        {
            File.AppendAllLines(Workspace.LogFilePath, new[] { String.Format(message, args) });
        }

        public static TResult RunInSTA<TResult>(Func<TResult> target) where TResult : class
        {
            TResult returnValue = default(TResult);
            Exception threadEx = null;

            var staThread = new Thread(() =>
            {
                try
                {
                    returnValue = target();
                }
                catch (Exception ex)
                {
                    threadEx = ex;
                }
            });

            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();

            return returnValue;
        }
    }
}
