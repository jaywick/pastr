using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            _items.AddFirst(value);
        }

        public async void Push()
        {
            var value = await Cut();
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

        private async Task<string> Copy()
        {
            _events.InvokeCopy();
            await Task.Delay(100);
            return GetCurrentClipboard();
        }

        private async Task<string> Cut()
        {
            _events.InvokeCut();
            await Task.Delay(100);
            return GetCurrentClipboard();
        }

        private async Task Paste(string data)
        {
            SetCurrentClipboard(data);
            await Task.Delay(100);
            _events.InvokePaste();
        }

        private string GetCurrentClipboard()
        {
            string data = null;
            Exception threadEx = null;

            var staThread = new Thread(() =>
            {
                try
                {
                    data = Clipboard.GetText();
                }
                catch (Exception ex)
                {
                    threadEx = ex;
                }
            });
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();

            return data;
        }

        private void SetCurrentClipboard(string value)
        {
            Exception threadEx = null;

            var staThread = new Thread(() =>
            {
                try
                {
                    if (String.IsNullOrEmpty(value))
                    {
                        Clipboard.Clear();
                        return;
                    }

                    Clipboard.SetText(value);
                }
                catch (Exception ex)
                {
                    threadEx = ex;
                }
            });
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
        }

        internal string DEBUG_GetItems()
        {
            var output = new StringBuilder();

            foreach (var item in _items)
                output.AppendLine(item ?? "");

            return output.ToString();
        }
    }
}
