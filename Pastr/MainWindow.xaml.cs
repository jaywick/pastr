using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Pastr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Storage _storage;
        private TrayIcon _trayIcon;
        private Events _events;
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            //this.Hide();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(10);
            _timer.Tick += _timer_Tick;
            _timer.Start();

            _events = new Events();
            _storage = new Storage(_events);
            _trayIcon = new TrayIcon(this);

            _events.OnInvokeDrop += _storage.Drop;
            _events.OnInvokeExpire += _storage.Expire;
            _events.OnInvokePeek += _storage.Peek;
            _events.OnInvokePoke += _storage.Poke;
            _events.OnInvokePinch += _storage.Pinch;
            _events.OnInvokePop += _storage.Pop;
            _events.OnInvokePush += _storage.Push;
            _events.OnInvokeShunt += _storage.Shunt;
            _events.OnInvokeReverse += _storage.Reverse;
            _events.OnInvokeRotateLeft += _storage.RotateLeft;
            _events.OnInvokeRotateRight += _storage.RotateRight;
            _events.OnInvokeSwap += _storage.Swap;
            _events.OnInvokeWipe += _storage.Wipe;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            listItems.Items.Clear();

            int i = 1;
            foreach (var item in _storage.Items)
            {
                listItems.Items.Add(i.ToString() + ". " + new String(item.Take(30).ToArray()));
                ++i;
            }
        }

        public void ShowApp()
        {
            this.Show();
            this.Activate();
        }

        public void ForceExit()
        {
            this.Close();
        }
    }
}
