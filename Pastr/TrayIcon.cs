using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WinForms = System.Windows.Forms;

namespace Pastr
{
    public class TrayIcon
    {
        private MainWindow mainWindow;
        private WinForms.NotifyIcon notifyIcon;
        private string _versionInfo;

        public TrayIcon(MainWindow parent)
        {
            mainWindow = parent;

            _versionInfo = String.Format("Pastr {0} by Jay Wick Labs", Common.GetVersionInfo());

            notifyIcon = new WinForms.NotifyIcon();
            notifyIcon.MouseUp += Notify_Click;
            notifyIcon.Icon = CreateIcon(mainWindow.Icon);
            notifyIcon.Visible = true;
            notifyIcon.Text = _versionInfo;
            notifyIcon.ContextMenu = CreateSystemTrayContextMenu();
            notifyIcon.Disposed += (s, e) => Remove();
        }

        private WinForms.ContextMenu CreateSystemTrayContextMenu()
        {
            var contextMenu = new WinForms.ContextMenu();
            contextMenu.MenuItems.Add(new WinForms.MenuItem(_versionInfo) { Enabled = false, DefaultItem = true });
            contextMenu.MenuItems.Add(new WinForms.MenuItem("Open Pastr", Notify_LaunchVizr) { });
            contextMenu.MenuItems.Add(new WinForms.MenuItem("-"));
            contextMenu.MenuItems.Add(new WinForms.MenuItem("Exit", Notify_Exit));

            return contextMenu;
        }

        private void Notify_Click(object sender, WinForms.MouseEventArgs e)
        {
            if (e.Button != WinForms.MouseButtons.Left)
                return;

            mainWindow.ShowApp();
        }

        private void Notify_LaunchVizr(object sender, EventArgs e)
        {
            mainWindow.ShowApp();
        }

        private void Notify_Exit(object sender, EventArgs e)
        {
            mainWindow.ForceExit();
        }

        // thanks to @StanislavKniazev http://stackoverflow.com/a/430909/80428
        private System.Drawing.Icon CreateIcon(ImageSource source)
        {
            string path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

            try
            {
                return System.Drawing.Icon.ExtractAssociatedIcon(path);
            }
            catch (Exception)
            {
                Common.Log("Failed to get icon from" + path);
                return null;
            }
        }

        public void Remove()
        {
            notifyIcon.Visible = false;
        }
    }
}
