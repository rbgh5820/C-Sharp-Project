using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;

namespace 캠_저장하기
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        CvCapture capture;
        IplImage src;
        private void timer1_Tick(object sender, EventArgs e)
        {
            src = capture.QueryFrame(); //src에 frame을 받아온다.
            pictureBoxIpl1.ImageIpl = src; // picturboxipl박스에 src 영상을 출력
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                capture = CvCapture.FromCamera(CaptureDevice.DShow, 0);
                capture.SetCaptureProperty(CaptureProperty.FrameWidth, 900); // 영상 넓이
                capture.SetCaptureProperty(CaptureProperty.FrameHeight, 700); // 영상 높이
            }
            catch
            {
                timer1.Enabled = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //폼을 닫으면서 메모리 할당 해제
            Cv.ReleaseImage(src); // 이미지의 메모리 할당 해제
            if (src != null) src.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Image captureImage = ConvertToBitmap(pictureBoxIpl1.ImageIpl); // pictureboxIpl에있는 이미지를 비트맵으로 전환하는 객체 선언
            Image dataImage = new Bitmap(captureImage); // bitmap으로 전환된 captureImage를 dataImage에 저장하는 객체 선언
            byte[] data = imageToBytes(dataImage); // dataImage를 배열에 저장
            
            //FileMode.Create : 파일을 생성할 때, FileMode.Open : 파일 읽을 때, FileAccess.Write(Read) : 파일의 액세스 권한
            FileStream fs = new FileStream("C:\\test\\byte2Test", FileMode.Create, FileAccess.Write);

            fs.Write(data, 0, data.Length);
            fs.Close();
        }
        public static byte[] imageToBytes(Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg); // 이미지 파일형식 지정하는 ImageFormat 사용
                return ms.ToArray();
            }
        }
        public Bitmap ConvertToBitmap(IplImage src)
        {
            //Bitmap으로 전환
            Bitmap bitmap = src.ToBitmap();

            return bitmap;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            
        }
    }
}

