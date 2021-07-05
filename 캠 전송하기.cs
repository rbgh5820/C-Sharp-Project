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
using log4net;
using log4net.Config;
using System.Net;
using System.Net.Sockets;
using OpenCvSharp.Extensions;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net")]

namespace 캠_저장하기
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Form1));

        public UdpClient server = null;

        Thread th;
        bool SendCheck; // bool 변수 선언

        public Form1()
        {
            InitializeComponent();

            server = new UdpClient("10.40.10.69", 8888);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.xml"));
        }
        CvCapture capture; // 객체 생성
        IplImage src; // 객체 생성2

        private void timer1_Tick(object sender, EventArgs e)
        {
            src = capture.QueryFrame(); // src에 frame을 받아온다.
            pictureBoxIpl1.ImageIpl = src; //picturboxipl박스에 src 영상을 출력
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                capture = CvCapture.FromCamera(CaptureDevice.DShow, 0); // 0 : 일반적으로 노트북 카메라 장치번호
                capture.SetCaptureProperty(CaptureProperty.FrameWidth, 800); //영상 넓이
                capture.SetCaptureProperty(CaptureProperty.FrameHeight, 600); // 영상 높이
            }
            catch
            {
                timer1.Enabled = false;
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //폼을 닫으면서 메모리 할당 해제
            Cv.ReleaseImage(src);
            if (src != null) src.Dispose();
            SendCheck = false; // 폼을 닫으면 false값을 출력하고 Thread를 종료한다.
        }

        private void button1_Click(object sender, EventArgs e)
        {
            th = new Thread(Send_data);
            th.Start();
            /*Image captureImage = ConvertToBitmap(pictureBoxIpl1.ImageIpl); // pictureboxipl에 있는 imageIpl을 bitmap으로 변환
            Image dataImage = new Bitmap(captureImage); // captureImage 값을 dataImage에 저장
            byte[] data2 = new byte[5000]; // data2 byte 배열 값 초기화
            byte[] data = imageToBytes(dataImage); // dataImage값을 byte배열로 변환후 data에 저장
            byte[] size = new byte[4]; // size 4바이트 초기화

            size = BitConverter.GetBytes(data.Length);


            server.Send(size, size.Length);
            log.Debug("size : " + size.Length);
            int sended = 0;

            while ((data.Length - sended) > data2.Length) //data2.length = 5000
            {
                data2 = new byte[5000];
                Buffer.BlockCopy(data, sended, data2, 0, data2.Length);
                server.Send(data2, data2.Length);
                sended += data2.Length; // sended = sended+data2.length
                log.Debug("sended : " + data2.Length);
            }
            data2 = new byte[data.Length - sended]; // while문에서 5000씩 데이터를 받고 남은 데이터
            Buffer.BlockCopy(data, sended, data2, 0, data2.Length);
            server.Send(data2, data2.Length);
            log.Debug("sended : " + data2.Length);
            Thread.Sleep(1000);*/

            /*FileStream fs = new FileStream("C://test//byte2Test.jpg", FileMode.Create, FileAccess.Write);
            fs.Write(data, 0, data.Length);
            fs.Close();*/
        }

        private void Send_data()
        {
            SendCheck = true;
            while (SendCheck)
            {
                Image captureImage = ConvertToBitmap(pictureBoxIpl1.ImageIpl); // pictureboxipl에 있는 imageIpl을 bitmap으로 변환
                Image dataImage = new Bitmap(captureImage); // captureImage 값을 dataImage에 저장
                byte[] data2 = new byte[5000]; // data2 byte 배열 값 초기화
                byte[] data = imageToBytes(dataImage); // dataImage값을 byte배열로 변환후 data에 저장
                byte[] size = new byte[4]; // size 4바이트 초기화

                size = BitConverter.GetBytes(data.Length);


                server.Send(size, size.Length);
                log.Debug("size : " + size.Length);
                int sended = 0;

                while ((data.Length - sended) > data2.Length) //data2.length = 5000
                {
                    data2 = new byte[5000];
                    Buffer.BlockCopy(data, sended, data2, 0, data2.Length);
                    server.Send(data2, data2.Length);
                    sended += data2.Length; // sended = sended+data2.length
                    log.Debug("sended : " + data2.Length);
                }
                data2 = new byte[data.Length - sended]; // while문에서 5000씩 데이터를 받고 남은 데이터
                Buffer.BlockCopy(data, sended, data2, 0, data2.Length);
                server.Send(data2, data2.Length);
                log.Debug("sended : " + data2.Length);
                Thread.Sleep(1000);
            }
        }

        private byte[] imageToBytes(Image dataImage)
        {
            using (var ms = new MemoryStream())
            {
                dataImage.Save(ms, ImageFormat.Jpeg); // 이미지 파일형식 지정하는 ImageFormat 사용
                return ms.ToArray();
            }
        }

        private Image ConvertToBitmap(IplImage imageIpl)
        {
            Bitmap bitmap = src.ToBitmap();
            return bitmap;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendCheck = false; // 스레드 종료
        }


        //FileMode.Create : 파일을 생성할 때, FileMode.Open : 파일 읽을 때, FileAccess.Write(Read) : 파일의 액세스 권한
        /* FileStream fs = new FileStream("C://test//byte2Test", FileMode.Create, FileAccess.Write);

            fs.Write(data, 0, data.Length);
            fs.Close();
            log.Debug("byte2Test.Length : " + data.Length);*/
    }
}
        //파일 저장
        /*Image captureImage = ConvertToBitmap(pictureBoxIpl1.ImageIpl); // pictureboxIpl에있는 이미지를 비트맵으로 전환하는 객체 선언
        Image dataImage = new Bitmap(captureImage); // bitmap으로 전환된 captureImage를 dataImage에 저장하는 객체 선언
        byte[] data = imageToBytes(dataImage); // dataImage를 byte배열로 전환

        //FileMode.Create : 파일을 생성할 때, FileMode.Open : 파일 읽을 때, FileAccess.Write(Read) : 파일의 액세스 권한
        FileStream fs = new FileStream("C://test//byte2Test", FileMode.Create, FileAccess.Write);

        fs.Write(data, 0, data.Length);
        fs.Close();
        log.Debug("byte2Test.Length : " + data.Length); */
