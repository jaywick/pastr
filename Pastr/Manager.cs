using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pastr
{
    public class Manager
    {
        private LinkedList<string> _items = new LinkedList<string>();

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
            SendKeys.SendWait("^{c}");
            await Task.Delay(100);
            return CurrentClipboard;
        }

        private async Task<string> Cut()
        {
            SendKeys.SendWait("^{x}");
            await Task.Delay(100);
            return CurrentClipboard;
        }

        private async Task Paste(string data)
        {
            CurrentClipboard = data;
            await Task.Delay(100);
            SendKeys.SendWait("^{v}");
        }

        private string CurrentClipboard
        {
            get { return Clipboard.GetText(); }
            set { Clipboard.SetText(value); }
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
