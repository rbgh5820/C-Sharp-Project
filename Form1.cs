using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;


namespace WebCam2
{
    public partial class Form1 : Form
    {
        Bitmap btMain;
        VideoCapture video;
        Mat frame = new Mat();
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                video = new VideoCapture(0); // 카메라에서 영상을 
                video.FrameWidth = 700; // 폼 실행시 가로 사이즈
                video.FrameHeight = 550; // 폼 실행시 세로 사이즈
            }
            catch
            {
                timer1.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            video.Read(frame);
            pictureBoxIpl1.ImageIpl = frame;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            frame.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "다른 이름으로 저장";
            dlg.DefaultExt = "jpg";
            dlg.Filter = "JPEG (*.jpg)|*.jpg";
            dlg.FilterIndex = 0;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pictureBoxIpl1.Image.Save(dlg.FileName);
            }
        }
    }    
}


