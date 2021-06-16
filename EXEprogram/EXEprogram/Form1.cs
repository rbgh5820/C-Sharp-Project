using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace EXEprogram
{
    public partial class Form1 : Form
    {
        System.Diagnostics.Process pc;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "EXE File (*.exe) | *.exe";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                label1.Text = ofd.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //파일이 없다면...
            if (!System.IO.File.Exists(label1.Text)) return;

            //다른 응용 프로그램 실행 시키기...
            pc = System.Diagnostics.Process.Start(label1.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (pc == null) return;

            //실행 시킨 프로그램 죽이기...
            pc.Kill();

        }
    }
}