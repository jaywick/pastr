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
        public event Action OnInvokeShunt;
        public event Action OnInvokePoke;
        public event Action OnInvokePeek;
        public event Action OnInvokePop;
        public event Action OnInvokeExpire;
        public event Action OnInvokePinch;
        public event Action OnInvokeWipe;
        public event Action OnInvokeSwap;
        public event Action OnInvokeRotateLeft;
        public event Action OnInvokeRotateRight;
        public event Action OnInvokeReverse;

        private Dictionary<string, Action> _shortcutCallbackMap;

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
            _shortcutCallbackMap = new Dictionary<string, Action>();
            ReserveHotKey("^+z", () => OnInvokeDrop.SafeInvoke());
            ReserveHotKey("^+x", () => OnInvokeShunt.SafeInvoke());
            ReserveHotKey("^+c", () => OnInvokePush.SafeInvoke());
            ReserveHotKey("^+v", () => OnInvokePeek.SafeInvoke());
            ReserveHotKey("^+b", () => OnInvokePop.SafeInvoke());
            ReserveHotKey("^+a", () => OnInvokeExpire.SafeInvoke());
            ReserveHotKey("^+s", () => OnInvokePinch.SafeInvoke());
            ReserveHotKey("^+d", () => OnInvokeWipe.SafeInvoke());
            ReserveHotKey("^+w", () => OnInvokeSwap.SafeInvoke());
            ReserveHotKey("^+f", () => OnInvokePoke.SafeInvoke());
            ReserveHotKey("^+q", () => OnInvokeRotateLeft.SafeInvoke());
            ReserveHotKey("^+e", () => OnInvokeRotateRight.SafeInvoke());
            ReserveHotKey("^+r", () => OnInvokeReverse.SafeInvoke());
            RegisterHotKeys();
        }

        ~Events()
        {
            if (_server != null)
                _server.Stop();
        }

        private void ReserveHotKey(string shortcut, Action callback)
        {
            _shortcutCallbackMap.Add(shortcut, callback);
        }

        private void RegisterHotKeys()
        {
            var script = new StringBuilder();

            foreach (var hotkey in _shortcutCallbackMap.Keys)
                script.AppendLine(String.Format("{0}::Run,Pastr.Messaging.exe {0}", hotkey));

            _ahkEngine.ExecRaw(script.ToString());
        }

        private void server_ClientMessage(NamedPipeConnection<string, string> connection, string message)
        {
            if (!_shortcutCallbackMap.ContainsKey(message))
            {
                switch (message)
                {
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

            _shortcutCallbackMap[message].Invoke();
        }

        public async Task InvokeCopyAsync()
        {
            _sentCopyToken = new CancellationTokenSource();
            _ahkEngine.ExecRaw("SendInput ^c\nRun,Pastr.Messaging.exe sentcopy");

            await _sentCopyToken.Wait();
        }

        public async Task InvokeCutAsync()
        {
            _sentCutToken = new CancellationTokenSource();
            _ahkEngine.ExecRaw("SendInput ^x\nRun,Pastr.Messaging.exe sentcut");

            await _sentCutToken.Wait();
        }

        public async Task InvokePasteAsync()
        {
            _sentPasteToken = new CancellationTokenSource();
            _ahkEngine.ExecRaw("SendInput ^v\nRun,Pastr.Messaging.exe sentpaste");

            await _sentPasteToken.Wait();
        }
    }
}
