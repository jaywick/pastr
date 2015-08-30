﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pastr.Extensions;

namespace Pastr
{
    public class Storage
    {
        private LinkedList<string> _items = new LinkedList<string>();
        private Events _events;

        public Storage(Events events)
        {
            _events = events;
        }

        public async void Poke()
        {
            var value = await Copy();

            if (!_items.Any())
            {
                Push();
                return;
            }

            _items.First.Value = value;
        }

        public async void Shunt()
        {
            var value = await Cut();
            _items.AddFirst(value);
        }

        public async void Push()
        {
            var value = await Copy();
            _items.AddFirst(value);
        }

        public async void Peek()
        {
            if (!_items.Any())
                return;

            var value = _items.First.Value;
            await Paste(value);
        }

        public void Pop()
        {
            Peek();
            _items.RemoveFirst();
        }

        public void Drop()
        {
            if (_items.Any())
                _items.RemoveFirst();
        }

        public void Expire()
        {
            if (_items.Any())
                _items.RemoveLast();
        }

        public void Swap()
        {
            var first = _items.First.Value;
            var second = _items.First.Next.Value;

            _items.First.Value = second;
            _items.First.Value = first;
        }

        public void RotateRight()
        {
            var first = _items.First;
            _items.RemoveFirst();
            _items.AddLast(first);
        }

        public void RotateLeft()
        {
            var last = _items.Last;
            _items.RemoveLast();
            _items.AddFirst(last);
        }

        public void Reverse()
        {
            _items = new LinkedList<string>(_items.Reverse());
        }

        public void Wipe()
        {
            _items.Clear();
        }

        public void Pinch()
        {
            _items = new LinkedList<string>(_items.Take(1));
        }

        public IEnumerable<string> Items
        {
            get { return _items; }
        }

        private async Task<string> Copy()
        {
            await _events.InvokeCopyAsync();
            return GetCurrentClipboard();
        }

        private async Task<string> Cut()
        {
            await _events.InvokeCutAsync();
            return GetCurrentClipboard();
        }

        private async Task Paste(string data)
        {
            SetCurrentClipboard(data);
            await _events.InvokePasteAsync();
        }

        private string GetCurrentClipboard()
        {
            return Common.RunInSTA(() => Clipboard.GetText());
        }

        private void SetCurrentClipboard(string value)
        {
            Common.RunInSTA(() =>
            {
                if (String.IsNullOrEmpty(value))
                {
                    Clipboard.Clear();
                    return "";
                }

                Clipboard.SetText(value);
                return value;
            });
        }
    }
}
