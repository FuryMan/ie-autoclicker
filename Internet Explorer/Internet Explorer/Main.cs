using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Internet_Explorer
{
    public partial class Main : MaterialSkin.Controls.MaterialForm
    {
        public static int toggle = 0;
        public static int toggle1 = 0;

        [DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys key);

        Random random = new Random();
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        Process[] proc = Process.GetProcessesByName("iexplore");

        public Main()
        {
            InitializeComponent();
        }

        public virtual double nextGaussian()
        {
            Random rand = new Random();
            double v1 = 0;
            double v2 = 0;
            double s = 0;
            do
            {
                v1 = 2 * rand.NextDouble() - 1;
                v2 = 2 * rand.NextDouble() - 1;
                s = v1 * v1 + v2 * v2;
            } while (s >= 1 || s == 0);

            double multiplier = Math.Sqrt(-2 * Math.Log(s) / s);
            return v1 * multiplier;
        }

        public object nextGaussian(double Mean, double StdDev)
        {
            return nextGaussian() * StdDev + Mean;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe");

            System.Threading.Thread.Sleep(1500);

            Main mainform = new Main();

            mainform.Hide();
            base.TopMost = true;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("IExplore"))
            {
                process.Kill();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            double value = trackBar1.Value / 10f;

            materialLabel1.Text = "Average CPS: " + value.ToString();
        }

        private void CheckIE_Tick(object sender, EventArgs e)
        {
            var proc = Process.GetProcessesByName("IExplore");

            if (proc.Length == 0)
            {
                Application.Exit();
            }
        }

        private async void Autoclicker_Tick(object sender, EventArgs e)
        {
            double CPS = 10000 / trackBar1.Value;
            double average = (CPS + CPS) / 2;



            if (trackBar1.Value >= 13)
            {
                Autoclicker.Interval = Convert.ToInt32(nextGaussian(average, random.Next(5, 10)));
            } else
            {
                Autoclicker.Interval = Convert.ToInt32(nextGaussian(average, random.Next(5, 16)));
            }

            if (MouseButtons == MouseButtons.Left)
            {
                await Task.Delay(30);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                await Task.Delay(30);
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            }
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            toggle++;

            if (toggle == 1)
            {
                materialRaisedButton1.Text = "Toggle Off";
                Autoclicker.Enabled = true;
            } else
            {
                toggle = 0;
                materialRaisedButton1.Text = "Toggle On";
                Autoclicker.Enabled = false;
            }
        }

        private async void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            await Task.Delay(1000);

            Main mainfrm = new Main();
            mainfrm.Dispose();

            base.Dispose();
        }

        private async void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            await Task.Delay(1000);
            Hiding.Enabled = true;
            MessageBox.Show("RShift to retrieve window.");
        }

        private  void Hiding_Tick(object sender, EventArgs e)
        {
            if (Convert.ToBoolean(GetAsyncKeyState(Keys.RShiftKey)))
            {
                base.Opacity = 100;
                Hiding.Enabled = false;
            } else
            {
                base.Opacity = 0;
            }
        }
    }
}
