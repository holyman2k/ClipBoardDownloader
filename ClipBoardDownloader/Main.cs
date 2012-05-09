using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ClipBoardDownloader.Services;
using System.Text.RegularExpressions;

namespace ClipBoardDownloader
{
    public partial class Main : Form
    {
        protected bool started = false;

        protected bool skipFirst = false;

        private ClipBoardTextService clipBoardTextService;

        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        IntPtr nextClipboardViewer;

        public Main()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    DisplayClipboardData();
                    SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                        nextClipboardViewer = m.LParam;
                    else
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        protected void DisplayClipboardData()
        {
            if (!skipFirst)
            {
                skipFirst = true;
                return;
            }
            try
            {
                IDataObject iData = new DataObject();
                iData = Clipboard.GetDataObject();

                if (iData.GetDataPresent(DataFormats.Text))
                {
                    string text = (string)iData.GetData(DataFormats.Text);
                    txtClipBoard.Text = text;
                    int next = clipBoardTextService.process(text);
                    txtStartNumber.Text = next.ToString();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        protected void NumberInput_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text;
            string exp = @"^([0-9])+";
            textBox.Text = Regex.Match(text, exp).Value;

        }

        protected void btnStart_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (!started)
            {
                int numberLength = int.Parse(txtNumberLength.Text);
                int startNumber = int.Parse(txtStartNumber.Text);
                clipBoardTextService = new ClipBoardTextService(txtPrefix.Text, txtExtensions.Text, numberLength, startNumber);
                nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
                btn.Text = "Stop";
            }
            else
            {
                skipFirst = false;
                ChangeClipboardChain(this.Handle, nextClipboardViewer);
                btn.Text = "Start Monitor";
            }
            started = !started;
            setView(started);
        }

        protected void setView(bool start)
        {
            txtPrefix.Enabled = !start;
            txtExtensions.Enabled = !start;
            txtNumberLength.Enabled = !start;
            txtStartNumber.Enabled = !start;
        }
    }
}
