using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pastr
{
    public class Events
    {
        public event Action OnInvokeDrop;
        public event Action OnInvokePush;
        public event Action OnInvokePoke;
        public event Action OnInvokePeek;
        public event Action OnInvokePop;

        private NamedPipeServer<string> _server;
        private AutoHotkey.Interop.AutoHotkeyEngine _ahkEngine;

        public Events()
        {
            _server = new NamedPipeServer<string>("io-jaywick-labs-pastr-messaging");
            _server.ClientMessage += server_ClientMessage;
            _server.Start();

            _ahkEngine = new AutoHotkey.Interop.AutoHotkeyEngine();
            _ahkEngine.ExecRaw("^+z::Run,Pastr.Messaging.exe drop");
            _ahkEngine.ExecRaw("^+x::Run,Pastr.Messaging.exe push");
            _ahkEngine.ExecRaw("^+c::Run,Pastr.Messaging.exe poke");
            _ahkEngine.ExecRaw("^+v::Run,Pastr.Messaging.exe peek");
            _ahkEngine.ExecRaw("^+b::Run,Pastr.Messaging.exe pop");
        }

        ~Events()
        {
            if (_server != null)
                _server.Stop();
        }

        private void server_ClientMessage(NamedPipeConnection<string, string> connection, string message)
        {
            switch (message)
            {
                case "drop":
                    OnInvokeDrop.SafeInvoke();
                    break;
                case "push":
                    OnInvokePush.SafeInvoke();
                    break;
                case "poke":
                    OnInvokePoke.SafeInvoke();
                    break;
                case "peek":
                    OnInvokePeek.SafeInvoke();
                    break;
                case "pop":
                    OnInvokePop.SafeInvoke();
                    break;
                default:
                    break;
            }
        }

        public void InvokeCopy()
        {
            _ahkEngine.ExecRaw("SendInput ^c");
        }

        public void InvokeCut()
        {
            _ahkEngine.ExecRaw("SendInput ^x");
        }

        public void InvokePaste()
        {
            _ahkEngine.ExecRaw("SendInput ^v");
        }
    }
}
