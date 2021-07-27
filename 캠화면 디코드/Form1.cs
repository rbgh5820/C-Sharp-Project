using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using log4net;
using log4net.Config;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using FFmpeg.AutoGen.Example;
using FFmpeg.AutoGen;
using System.Runtime.InteropServices;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net")]

namespace 캠_수신하기
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Form1));

        public UdpClient newsock = null;
        public IPEndPoint iPEndPoint = null;
        int pos = 0;
        int nImgSize = 0;

        public static Size imgSize = new Size(704, 576);
        public VideoStreamDecoder vsd = new VideoStreamDecoder(imgSize);

        Thread th;
        bool receiveCheck;
        public Form1()
        {
            InitializeComponent();
            FFmpegBinariesHelper.RegisterFFmpegBinaries();
            SetupLogging();

            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 8888);
            newsock = new UdpClient(ipep);
            iPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.xml"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            th = new Thread(Receive);
            th.Start();
        }
        private static unsafe void SetupLogging()
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
        /// <summary>
        /// Thread를 사용하여 데이터를 수신하는 메소드
        /// </summary>
        private void Receive()
        {
            byte[] receivedata = new byte[100 * 1000]; // 이미지를 담을 data 그릇
            byte[] imageSize = new byte[4]; // imageSize = 4byte
            byte[] imageData = null;

            receiveCheck = true;
            while (receiveCheck)
            {
                if (!receiveCheck)
                {
                    Thread.Sleep(50);
                    continue;
                }
                byte[] temp = newsock.Receive(ref iPEndPoint); // 클라이언트에서 Receive해온걸 temp 바이트에 저장
                int tempSize = temp.Length;
                if (tempSize < 4)
                {
                    continue;
                }
                else
                {
                    Buffer.BlockCopy(temp, 0, receivedata, pos, tempSize); // temp에 0번째부터 tempsize만큼 receivedata의 pos지점부터 복사
                    pos += tempSize; // 받은 데이터의 사이즈가 누적된다.
                    log.Debug("pos : " + pos);

                    if (nImgSize == 0)
                    {
                        Buffer.BlockCopy(temp, 0, imageSize, 0, 4); // 4byte만큼 imageSize에 복사
                        nImgSize = BitConverter.ToInt32(imageSize, 0); //imageSize를 int로 변환
                        log.Debug("nImgSize : " + nImgSize);
                    }
                    else
                    {
                        if (nImgSize <= (pos - 4))
                        {
                            imageData = new byte[nImgSize]; // 이미지 데이터만큼 초기화
                            Buffer.BlockCopy(receivedata, 4, imageData, 0, nImgSize); // 이미지 사이즈의 크기만큼 imageData에 복사(receivedata 4byte부문부터)
                            Image decode = DecodeFrameToImage(imageData, imgSize); // 인코딩된 파일을 읽어온 imageData를 byte배열에 담는다
                            pictureBoxIpl1.Image = decode; // 픽쳐박스에 이미지를 띄우기
                            Array.Clear(imageData, 0x0, imageData.Length); // imageData의 길이만큼 imageData를 초기화.
                            pos = 0;
                            nImgSize = 0;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }

        private unsafe Image DecodeFrameToImage(byte[] encodeFrame, Size imgSize)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                fixed (byte* pImgByte = encodeFrame)
                {
                    var sourceSize = imgSize;
                    var imgLenth = encodeFrame.Length;
                    var sourcePixelFormat = AVPixelFormat.AV_PIX_FMT_YUV420P;
                    var destinationSize = sourceSize;
                    var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
                    using (var vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
                    {
                        while (vsd.Decode(out AVFrame frame, pImgByte, imgLenth))
                        {
                            AVFrame convertedFrame = vfc.Convert(frame);
                            using (var bitmap = new Bitmap(convertedFrame.width, convertedFrame.height, convertedFrame.linesize[0], PixelFormat.Format24bppRgb, (IntPtr)convertedFrame.data[0]))
                                bitmap.Save(ms, ImageFormat.Jpeg);
                            Image deImage = ByteArrayToImage(ms.ToArray());
                            return deImage;
                        }
                        return null;
                    }
                }
            }
        }

        /*private unsafe byte[] DecodeFrameToImage(byte[] encodeFrame, Size imgSize)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                fixed (byte* pImgByte = encodeFrame)
                {
                    var sourceSize = imgSize;
                    var imgLenth = encodeFrame.Length;
                    var sourcePixelFormat = AVPixelFormat.AV_PIX_FMT_YUV420P;
                    var destinationSize = sourceSize;
                    var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
                    using (var vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
                    {
                        while (vsd.Decode(out AVFrame frame, pImgByte, imgLenth))
                        {
                            AVFrame convertedFrame = vfc.Convert(frame);
                            using (var bitmap = new Bitmap(convertedFrame.width, convertedFrame.height, convertedFrame.linesize[0], PixelFormat.Format24bppRgb, (IntPtr)convertedFrame.data[0]))
                                bitmap.Save(ms, ImageFormat.Jpeg);
                            Image deImage = ByteArrayToImage(ms.ToArray());
                            return ms.ToArray();
                        }
                        return null;
                    }
                }
            }
        }*/

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            receiveCheck = false;
        }

        private Image ByteArrayToImage(byte[] encodeFrame)
        {
            MemoryStream ms = new MemoryStream(encodeFrame);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}
