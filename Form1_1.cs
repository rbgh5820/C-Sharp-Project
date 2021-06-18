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
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace Capture
{
    public partial class Form1 : Form
    {
        CvCapture capture;
        IplImage src;

        Thread th;
        bool Run;

        public UdpClient server = null;

        public Form1()
        {
            InitializeComponent();
            server = new UdpClient("10.40.10.69", 8888);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (Run) return;
            try
            {
                    Thread.Sleep(500);
                    th = new Thread(data);
                    th.Start();
                    Run = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                button1.Enabled = true;
                if (Run)
                    Run = false;
            }
        }

        private void data()
        {


            Byte[] data2 = new byte[40000];
            server.Send(data2, data2.Length);

        }

        private byte[] ByteMove()
        {
            throw new NotImplementedException();
        }

        private byte[] imgToByteArray(Image img)
        {
            // 이미지를 바이트 배열로 전환하는 코드
            using (MemoryStream mStream = new MemoryStream())
            {
                return mStream.ToArray();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                capture = CvCapture.FromCamera(CaptureDevice.DShow, 0);
                capture.SetCaptureProperty(CaptureProperty.FrameWidth, 700);
                capture.SetCaptureProperty(CaptureProperty.FrameHeight, 500);
            }
            catch
            {
                timer1.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            src = capture.QueryFrame();
            pictureBoxIpl1.ImageIpl = src;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cv.ReleaseImage(src);
            if (src != null) src.Dispose();

            Run = false;
            try
            {
                if (server != null) server.Close();
            }
            catch (Exception)
            {
                //소켓 종료 오류
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            th.Abort();
        }
    }
}
