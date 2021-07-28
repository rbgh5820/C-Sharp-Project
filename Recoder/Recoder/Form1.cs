using System.Diagnostics;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Data;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Recoder
{
    public partial class Form1 : Form
    {
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int record(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);
        public Form1()
        {
            InitializeComponent();
        }

        private void Record_Audio_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Start();
            record("open new Type waveaudio Alias recsound", "", 0, 0);
            record("record recsound", "", 0, 0);
        }

        private void Save_Stop_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Enabled = false;
            record("save recsound C://test//audio.wav", "", 0, 0);
            record("close recsound", "", 0, 0);
        }
    }
}
