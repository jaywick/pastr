using NamedPipeWrapper;
using System;
using System.Collections.Generic;
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

        private readonly Dictionary<string, Action> _shortcutCallbackMap;
        private readonly NamedPipeServer<string> _server;
        private readonly AutoHotkey.Interop.AutoHotkeyEngine _ahkEngine;

        private CancellationTokenSource _sentCopyToken;
        private CancellationTokenSource _sentCutToken;
        private CancellationTokenSource _sentPasteToken;

        public Events()
        {
            _server = new NamedPipeServer<string>("jaywick.labs.pastr.messaging");
            _server.ClientMessage += server_ClientMessage;
            _server.Start();

            _ahkEngine = AutoHotkey.Interop.AutoHotkeyEngine.Instance;
            _shortcutCallbackMap = new Dictionary<string, Action>();
            ReserveHotKey("^+z", () => OnInvokeDrop?.Invoke());
            ReserveHotKey("^+x", () => OnInvokeShunt?.Invoke());
            ReserveHotKey("^+c", () => OnInvokePush?.Invoke());
            ReserveHotKey("^+v", () => OnInvokePeek?.Invoke());
            ReserveHotKey("^+b", () => OnInvokePop?.Invoke());
            ReserveHotKey("^+a", () => OnInvokeExpire?.Invoke());
            ReserveHotKey("^+s", () => OnInvokePinch?.Invoke());
            ReserveHotKey("^+d", () => OnInvokeWipe?.Invoke());
            ReserveHotKey("^+w", () => OnInvokeSwap?.Invoke());
            ReserveHotKey("^+f", () => OnInvokePoke?.Invoke());
            ReserveHotKey("^+q", () => OnInvokeRotateLeft?.Invoke());
            ReserveHotKey("^+e", () => OnInvokeRotateRight?.Invoke());
            ReserveHotKey("^+r", () => OnInvokeReverse?.Invoke());
            RegisterHotKeys();
        }

        ~Events()
        {
            _server?.Stop();
        }

        private void ReserveHotKey(string shortcut, Action callback)
        {
            _shortcutCallbackMap.Add(shortcut, callback);
        }

        private void RegisterHotKeys()
        {
            var script = new StringBuilder();

            foreach (var hotkey in _shortcutCallbackMap.Keys)
                script.AppendLine($"{hotkey}::Run,Pastr.Messaging.exe {hotkey}");

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

            _shortcutCallbackMap.GetValueOrDefault(message)?.Invoke();
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
