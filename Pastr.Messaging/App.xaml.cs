using NamedPipeWrapper;
using System;
using System.Linq;
using System.Windows;

namespace Pastr.Messaging
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var message = string.Join("\n", Environment.GetCommandLineArgs().Skip(1));
            
            var client = new NamedPipeClient<string>("jaywick.labs.pastr.messaging");
            client.Start();
            client.WaitForConnection();
            client.PushMessage(message);

            Shutdown(0);
        }
    }
}
