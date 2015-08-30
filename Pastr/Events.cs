using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pastr.Extensions;

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

        private CancellationTokenSource _sentCopyToken;
        private CancellationTokenSource _sentCutToken;
        private CancellationTokenSource _sentPasteToken;

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
                case "sentcopy":
                    _sentCopyToken.Cancel();
                    break;
                case "sentcut":
                    _sentCutToken.Cancel();
                    break;
                case "sentpaste":
                    _sentPasteToken.Cancel();
                    break;
                default:
                    break;
            }
        }

        public async Task InvokeCopyAsync()
        {
            _sentCopyToken = new CancellationTokenSource();
            _ahkEngine.ExecRaw("SendInput ^c\nRun,Pastr.Messaging.exe sentcopy");
            await Task.Delay(100, _sentCopyToken.Token);
        }

        public async Task InvokeCutAsync()
        {
            _sentCutToken = new CancellationTokenSource();
            _ahkEngine.ExecRaw("SendInput ^x\nRun,Pastr.Messaging.exe sentcut");
            await Task.Delay(100, _sentCutToken.Token);
        }

        public async Task InvokePasteAsync()
        {
            _sentPasteToken = new CancellationTokenSource();
            _ahkEngine.ExecRaw("SendInput ^v\nRun,Pastr.Messaging.exe sentpaste");
            await Task.Delay(100, _sentPasteToken.Token);
        }
    }
}
