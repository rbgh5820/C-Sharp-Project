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

namespace UDP_server
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Form1));

        public UdpClient server = null;
        public Form1()
        {
            InitializeComponent();
            server = new UdpClient("10.40.10.69", 8888);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.xml"));
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            int sendsize = 1000;
            int sended = 0; //시작위치
            int sendlength = 0;
            //byte[] headerpkt = new byte[7];
            byte[] size = new byte[4];
            //byte[] header = new byte[headerpkt.Length + size.Length];

            //headerpkt = StringToByte("headpkt");

            //Buffer.BlockCopy(headerpkt, 0,header,0,headerpkt.Length);

            
            //Buffer.BlockCopy(size,0,header,headerpkt.Length,size.Length);

            //server.Send(header, header.Length);
            

            byte[] data = File.ReadAllBytes("C://test//cat.jpg"); // 파일의 용량만큼 배열에 담는다.

            log.Debug("data.length : " + data.Length);
            size = BitConverter.GetBytes(data.Length);

            byte[] data2 = new byte[1000];

            server.Send(size, size.Length);

            //File.ReadAllBytes("C://test//cat.jpg");

            while ((data.Length-sended) > sendsize)
            {
                sendlength = 1000;
                if (sended > data.Length) // sended가 data.length보다 클경우 break로 빠져나온다.
                {
                    break;

                }
                
                Buffer.BlockCopy(data, sended, data2, 0, sendlength); // data의 sended 위치부터 sendlength 크기만큼 data2의 0부터 복사한다. data2 = data에서 0~1000까지 짜른것을 복사한것 data = 복사할 데이터    
                server.Send(data2, sendlength); // data2를 sendlength만큼 보낸다.
                sended += sendlength; // send = sended+sendlength
                log.Debug("sended = "+ sended);

            }
            Buffer.BlockCopy(data, sended, data2,0,data.Length - sended);
            server.Send(data2, data.Length-sended); // 배열에 있는 데이터를 서버에 전송한다.
            log.Debug("data.length - sended : " + (data.Length-sended));
            
            label1.Text = "파일을 전송했습니다.";
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            log.Debug("Form1 Loaded Complete~!");
        }
        private byte[] StringToByte(string str)
        {
            byte[] StrByte = Encoding.UTF8.GetBytes(str);
            return StrByte;
        }
    }
}
