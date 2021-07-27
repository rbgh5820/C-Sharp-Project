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
using log4net.Config;
using System.Net;
using System.Net.Sockets;
using OpenCvSharp.Extensions;
using FFmpeg.AutoGen;
using FFmpeg.AutoGen.Example;
using System.Runtime.InteropServices;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net")]

namespace 캠_저장하기
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Form1));

        public UdpClient server = null;

        Thread th;
        Boolean SendCheck;

        public Form1()
        {
            InitializeComponent();

            FFmpegBinariesHelper.RegisterFFmpegBinaries();
            SetupLogging();

            server = new UdpClient("127.0.0.1", 8888);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.xml"));
        }

        private unsafe void SetupLogging()
        {
            ffmpeg.av_log_set_level(ffmpeg.AV_LOG_VERBOSE);

            av_log_set_callback_callback logCallback = (p0, level, format, vl) =>
            {
                if (level > ffmpeg.av_log_get_level()) return;

                int lineSize = 1024;
                var lineBuffer = stackalloc byte[lineSize];
                var printPrefix = 1;
                ffmpeg.av_log_format_line(p0, level, format, vl, lineBuffer, lineSize, &printPrefix);
                var line = Marshal.PtrToStringAnsi((IntPtr)lineBuffer);
            };
            ffmpeg.av_log_set_callback(logCallback);
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
            SendCheck = false;
        }
        /// <summary>
        /// 이미지의 데이터를 나눠서 전송
        /// </summary>
        /// <param name="encordFrame">pictureboxIpl1의 이미지 데이터</param>
        /// <param name="size">이미지 정보가 들어있는 헤더패킷</param>
        private void button1_Click(object sender, EventArgs e)
        {
            th = new Thread(Send_data);
            th.Start();
        }

        private unsafe byte[] EncodeFrameImage(Image imageData)
        {
            int fps = 25; // fps 변수값 초기화
            var sourceSize = imageData.Size;
            var sourcePixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24; // PixelFormat : 이미지의 각 픽셀에 대한 색 데이터의 형식을 지정
            var destinationSize = new Size(704, 576); // h263 cif 사이즈(352,288)
            var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_YUV420P;
            byte[] returnData = null;
            using (VideoFrameConverter vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (H263VideoStreamEncoder vse = new H263VideoStreamEncoder(ms, fps, destinationSize))
                    {
                        if(imageData != null)
                        {
                            byte[] bitmapData;

                            using (Bitmap frameBitmap = imageData is Bitmap bitmap ? bitmap : new Bitmap(imageData))
                            {
                                bitmapData = GetBitmap(frameBitmap);
                            }

                            fixed (byte* pBitmapData = bitmapData) // * 포인터
                            {
                                var data = new byte_ptrArray8 { [0] = pBitmapData };
                                var linesize = new int_array8 { [0] = bitmapData.Length / sourceSize.Height };
                                var frame = new AVFrame
                                {
                                    data = data,
                                    linesize = linesize,
                                    height = sourceSize.Height
                                };
                                var convertedFrame = vfc.Convert(frame); // convert : 인코드를 하는 사이즈를 맞춰준다.
                                convertedFrame.pts = fps;
                                vse.Encode(convertedFrame);
                            }
                            returnData = ms.ToArray(); // 메모리 값이 returnData안에 들어간다.
                            return returnData;
                        }
                    }
                }
            }
            return returnData;
        }

        private byte[] GetBitmap(Bitmap frameBitmap)
        {
            var bitmapData = frameBitmap.LockBits(new Rectangle(Point.Empty, frameBitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                var length = bitmapData.Stride * bitmapData.Height;
                byte[] data = new byte[length];
                Marshal.Copy(bitmapData.Scan0, data, 0, length);
                return data;
            }
            finally
            {
                frameBitmap.UnlockBits(bitmapData);
            }
        }
        /// <summary>
        /// Thread를 사용하여 이미지 데이터 전송하는 메소드
        /// </summary>
        private void Send_data()
        {
            SendCheck = true;
            while (SendCheck)
            {
                /*string frameFiles = "C://test//cat.jpg";// 파일 이름을 검색하여 framFiles에 담는다.
                Image dataImage = Image.FromFile(frameFiles);*/ // frameFiles의 첫번째로 검색된 파일을 이미지로 저장한다.
                Image captureImage = ConvertToBitmap(pictureBoxIpl1.ImageIpl); // pictureboxipl에 있는 imageIpl을 bitmap으로 변환
                Image dataImage = new Bitmap(captureImage); // captureImage 값을 dataImage에 초기화
                /*byte[] data = imageToBytes(dataImage);*/
                byte[] encodeFrame = EncodeFrameImage(dataImage); // dataImage 값을 바이트배열 encodeFrame에 초기화
                byte[] data2 = new byte[20000]; // data2 byte 배열 값 초기화
                byte[] size = new byte[4]; // size 4바이트 초기화 해더패킷
                size = BitConverter.GetBytes(encodeFrame.Length);

                server.Send(size, size.Length);
                /*byte[] senddata = new byte[size.Length + encodeFrame.Length]; // size값 4바이트 + 이미지값을 초기화
                Buffer.BlockCopy(size,0,senddata,0,size.Length); // 사이즈값 4byte를 senddata에 복사
                Buffer.BlockCopy(senddata, 0, senddata, size.Length, encodeFrame.Length);*/ // 이미지 데이터 값을 4byte부터 복사.
                log.Debug("size : " + encodeFrame.Length);
                int sended = 0;

                while ((encodeFrame.Length - sended) > data2.Length) //data2.length = 20000
                {
                    data2 = new byte[20000]; // data2의 크기만큼 서버에 전송
                    Buffer.BlockCopy(encodeFrame, sended, data2, 0, data2.Length); // data의 sended부분부터 data2의 크기만큼 data2 배열에 복사
                    server.Send(data2, data2.Length);
                    sended += data2.Length; // sended = sended+data2.length
                    log.Debug("sended : " + data2.Length);
                }
                data2 = new byte[encodeFrame.Length - sended]; // while문에서 5000씩 데이터를 받고 남은 데이터 
                Buffer.BlockCopy(encodeFrame, sended, data2, 0, data2.Length);
                server.Send(data2, data2.Length);
                log.Debug("sended : " + data2.Length);
                Thread.Sleep(50);
            }
        }

        /*private byte[] imageToBytes(Image dataImage)
        {
            using (var ms = new MemoryStream())
            {
                dataImage.Save(ms, ImageFormat.Jpeg); // 이미지 파일형식 지정하는 ImageFormat 사용
                return ms.ToArray();
            }
        }*/

        private Image ConvertToBitmap(IplImage imageIpl)
        {
            Bitmap bitmap = src.ToBitmap();
            return bitmap;
        }

        /// <summary>
        /// 바이트를 뒤로 이동시켜주는 메서드
        /// </summary>
        /// <param name="afterByte">처음 바이트</param>
        /// <param name="movePoint">이동할 시작위치</param>
        /// <param name="moveCount">이동할 마지막위치</param>
        /// <returns></returns>
        private byte[] ByteMove(byte[] afterByte, int movePoint, int moveCount)
        {
            byte[] resultByte = new byte[moveCount];
            Buffer.BlockCopy(afterByte, movePoint, resultByte, 0, moveCount);
            return resultByte;
        }
    }
}
