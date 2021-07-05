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

        Thread th;
        bool receiveCheck;
        public Form1()
        {
            InitializeComponent();

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

        private void Receive()
        {
            byte[] receivedata = new byte[10000 * 10000]; // 이미지를 담을 data 그릇
            byte[] imageSize = new byte[4]; // imageSize = 4byte
            byte[] imageData = null;

            receiveCheck = true;

            while (receiveCheck)
            {
                if (!receiveCheck)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                byte[] temp = newsock.Receive(ref iPEndPoint); // 클라이언트에서 Receive해온걸 temp 바이트에 저장
                int tempSize = temp.Length;
                if (tempSize < 4)
                {
                    continue;
                }
                else // tempsize가 4바이트를 넘으면 
                {
                    Buffer.BlockCopy(temp, 0, receivedata, pos, tempSize); // temp에 0번째부터 tempsize만큼 receivedata의 pos지점부터 복사
                    pos += tempSize; // pos = pos+tempsize = 4byte;
                    log.Debug("pos : " + pos); // 로그

                    if (nImgSize == 0)
                    {
                        Buffer.BlockCopy(temp, 0, imageSize, 0, 4); // 4byte만큼 imageSize에 복사
                        nImgSize = BitConverter.ToInt32(imageSize, 0); //imageSize를 int로 변환
                        log.Debug("nImgSize : " + nImgSize);
                    }
                    else
                    {
                        if (nImgSize == (pos - 4))
                        {
                            imageData = new byte[nImgSize]; // nImgsize !== 0일경우 imageData는 nImgSize만큼 데이터를 담는다
                            Buffer.BlockCopy(receivedata, 4, imageData, 0, nImgSize); // pos = 4byte이므로 receivedata 는 4바이트째부터 nImgSize만큼 imageData에 복사

                            pictureBoxIpl1.Image = ByteArrayToImage(imageData);// 픽쳐박스에 이미지를 띄우기
                            Array.Clear(imageData, 0x0, imageData.Length); // imageData의 길이만큼 imageData를 지운다.
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            th.Abort();
            receiveCheck = false;
        }

        private Image ByteArrayToImage(byte[] imageData)
        {
            MemoryStream ms = new MemoryStream(imageData);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}
