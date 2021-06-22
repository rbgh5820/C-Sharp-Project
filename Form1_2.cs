using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.IO;
using log4net;
using log4net.Config;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net")]

namespace UDP_Client
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Form1));

        public UdpClient newsock = null;
        public IPEndPoint iPEndPoint = null;
        public int pos = 0;
        public int nImgSize = 0;

        public Form1()
        {
            InitializeComponent();
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 8888);
            newsock = new UdpClient(ipep);
            iPEndPoint = new IPEndPoint(IPAddress.Any, 0); // IPAddress.Any :  모든 클라이언트에서 오는 요청을 받겠다는 의미

            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.xml"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] receivedata = new byte[1000 * 1000]; // 이미지를 담을 data 그릇
            byte[] imageSize = new byte[4]; // imageSize = 4byte 
            byte[] imageData = null; // imageData = null로 지정 (이미지 사이즈를 모르기때문)

            while (true)
            {
                byte[] temp = newsock.Receive(ref iPEndPoint); // 클라이언트에서 Receive해온걸 temp 바이트에 저장
                int tempSize = temp.Length;
                if (tempSize < 4)
                {
                    continue;
                }
                else // tempsize가 4바이트를 넘으면 
                {
                    Buffer.BlockCopy(temp, 0, receivedata, pos, tempSize); // temp에 0번째부터 tempsize만큼 receivedata의 pos시작부터 복사
                    pos += tempSize; // pos = pos+tempsize = 4byte;
                    log.Debug("pos : " + pos); // 로그

                    if(nImgSize == 0) {
                        Buffer.BlockCopy(temp, 0, imageSize, 0, 4); // 4byte만큼 imageSize에 복사
                        nImgSize = BitConverter.ToInt32(imageSize); //imageSize를 int로 변환
                        log.Debug("nImgSize : " + nImgSize);
                    }
                    else
                    {
                        if( nImgSize == (pos - 4))
                        {
                            imageData = new byte[nImgSize]; // mImgsize !== 0일경우 imageData는 nImgSize만큼 데이터를 담는다
                            Buffer.BlockCopy(receivedata, 4, imageData, 0, nImgSize); // pos = 4byte이므로 receivedata 는 4바이트째부터 nImgSize만큼 imageData에 복사

                            this.pictureBox1.Image = byteArrayToImage(imageData); // 픽쳐박스에 이미지를 띄우기
                            //File.WriteAllBytes("J://Image.jpg", imageData);
                            break; // 빠져나오기
                        }
                        else
                        {
                            continue;
                        }
                    }

                }
                
            }
        }
        //byte를 이미지로 변환
        private Image byteArrayToImage(byte[] data) 
        {
            //byte배열->이미지 전환하는 코딩
            MemoryStream ms = new MemoryStream(data);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}



//data = newsock.Receive(ref iPEndPoint); // ref : 변수에 대한 참조를 전달하는 기능

