using NoMoreClick.Properties;
using System;
using System.Windows.Forms;

namespace NoMoreClick
{
    class SystemTrayIcon
    {
        private NotifyIcon ni;
        private WeakReference<MainForm> form;
        
        public SystemTrayIcon(MainForm form)
        {
            ni = new NotifyIcon();
            this.form = new WeakReference<MainForm>(form);
        }

        ~SystemTrayIcon()
        {
            ni.Dispose();
        }

        public void Display()
        {
            ni.Text = "NoMoreClick - active";
            ni.Visible = true;
            ni.Icon = Resources.AppIcon;
            ni.MouseClick += new MouseEventHandler((object o, MouseEventArgs args) =>
            {
                MainForm refForm;
                if (form.TryGetTarget(out refForm))
                {
                    refForm.BackToSettingWindow();
                }
            });
        }
    }
}
