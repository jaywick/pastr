﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Pastr
{
    // thanks to Eric Ouellet
    // http://stackoverflow.com/a/9330358/80428
    public class HotKey : IDisposable
    {
        private Dictionary<int, HotKey> dictHotKeyToCalBackProc;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, UInt32 fsModifiers, UInt32 vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        
        public const int WmHotKey = 0x0312;

        private bool disposed = false;

        public Key Key { get; private set; }
        public KeyModifier KeyModifiers { get; private set; }
        public int Id { get; set; }

        public delegate void ActivatedHandler();
        public event ActivatedHandler Activated;

        public HotKey(Key k, KeyModifier keyModifiers)
        {
            Key = k;
            KeyModifiers = keyModifiers;

            if (!Register())
                MessageBox.Show("Failed to register " + k.ToString());
        }

        public bool Register()
        {
            int virtualKeyCode = KeyInterop.VirtualKeyFromKey(Key);
            Id = virtualKeyCode + ((int)KeyModifiers * 0x10000);
            bool result = RegisterHotKey(IntPtr.Zero, Id, (UInt32)KeyModifiers, (UInt32)virtualKeyCode);

            if (dictHotKeyToCalBackProc == null)
            {
                dictHotKeyToCalBackProc = new Dictionary<int, HotKey>();
                ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(ComponentDispatcherThreadFilterMessage);
            }

            dictHotKeyToCalBackProc.Add(Id, this);
            
            return result;
        }

        public void Unregister()
        {
            HotKey hotKey;

            if (dictHotKeyToCalBackProc.TryGetValue(Id, out hotKey))
                UnregisterHotKey(IntPtr.Zero, Id);
        }

        private void ComponentDispatcherThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (!handled)
            {
                if (msg.message == WmHotKey)
                {
                    HotKey hotKey;

                    if (dictHotKeyToCalBackProc.TryGetValue((int)msg.wParam, out hotKey))
                    {
                        if (Activated != null)
                            Activated.Invoke();

                        handled = true;
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                    Unregister();

                disposed = true;
            }
        }
    }

    [Flags]
    public enum KeyModifier
    {
        None = 0x0000,
        Alt = 0x0001,
        Ctrl = 0x0002,
        NoRepeat = 0x4000,
        Shift = 0x0004,
        Win = 0x0008
    }
}