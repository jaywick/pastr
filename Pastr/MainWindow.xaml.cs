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

namespace Pastr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Manager _manager;
        private SystemTrayIcon _trayIcon;
        private List<HotKey> _hotkeys;

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        ~MainWindow()
        {
            if (_trayIcon != null)
                _trayIcon.Remove();

            if (_hotkeys != null)
                _hotkeys.ForEach(x => x.Dispose());
        }

        private void Start()
        {
            //this.Hide();

            _manager = new Manager();
            _trayIcon = new SystemTrayIcon(this);
            _hotkeys = new List<HotKey>();
            
            RegisterHotKey(KeyModifier.Alt, Key.Z, _manager.Drop);
            RegisterHotKey(KeyModifier.Alt, Key.X, _manager.Push);
            RegisterHotKey(KeyModifier.Alt, Key.C, _manager.Poke);
            RegisterHotKey(KeyModifier.Alt, Key.V, _manager.Peek);
            RegisterHotKey(KeyModifier.Alt, Key.B, _manager.Pop);
        }

        private void RegisterHotKey(KeyModifier modifiers, Key key, Action action)
        {
            var hotkey = new HotKey(key, modifiers);
            hotkey.Activated += async () =>
            {
                action();
                await Task.Delay(150);
                textEvents.Text = _manager.DEBUG_GetItems();
            };

            _hotkeys.Add(hotkey);
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
