using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Pastr.Messaging
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var message = String.Join("\n", Environment.GetCommandLineArgs().Skip(1));
            //MessageBox.Show(message);

            var client = new NamedPipeClient<string>("io-jaywick-labs-pastr-messaging");
            client.Start();
            client.WaitForConnection();
            client.PushMessage(message);

            Shutdown(0);
        }
    }
}
