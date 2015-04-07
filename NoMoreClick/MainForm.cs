using NoMoreClick.Properties;
using System;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;

namespace NoMoreClick
{
    public partial class MainForm : Form
    {
        private SystemTrayIcon systemTrayIcon;
        private bool capturingMouse = false;
        private System.Timers.Timer clickTimer;
        private uint lastX, lastY;

        public MainForm()
        {
            InitializeComponent();

            clickTimer = new System.Timers.Timer();
            clickTimer.AutoReset = false;
            clickTimer.Elapsed += new ElapsedEventHandler((o, arg) => {
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, lastX, lastY, 0, UIntPtr.Zero);
            });
            clickTimer.Stop();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            Settings.Default.Save();
            clickTimer.Interval = (int)Settings.Default.WaitTimeMs;
            if (!MouseHook.StartHook(OnMouseMove))
            {
                MessageBox.Show("Failed to capture mouse.");
                return;
            }
            if (systemTrayIcon == null)
            {
                systemTrayIcon = new SystemTrayIcon(this);
            }
            systemTrayIcon.Display();
            capturingMouse = true;
            Hide();
        }

        internal void BackToSettingWindow()
        {
            Show();
            capturingMouse = false;
            if (!MouseHook.StopHook())
            {
                MessageBox.Show("Failed to stop capturing mouse.");
            }
        }

        private void OnMouseMove(int x, int y)
        {
            if (!capturingMouse)
                return;
            if (clickTimer.Enabled)
            {
                int diff = (int)Settings.Default.IgnoreMotion;
                if (Math.Abs(lastX - x) >= diff || Math.Abs(lastY - y) >= diff) {
                    clickTimer.Stop();
                    StartTick(x, y);
                }
            }
            else
            {
                StartTick(x, y);
            }
        }

        private void StartTick(int x, int y)
        {
            lastX = (uint)x;
            lastY = (uint)y;
            clickTimer.Start();
        }

        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
    }
}
