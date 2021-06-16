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

namespace UDP_Client
{
    public partial class Form1 : Form
    {
        public UdpClient newsock = null;
        public IPEndPoint iPEndPoint = null;

        public Form1()
        {
            InitializeComponent();
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 8888);
            newsock = new UdpClient(ipep);
            iPEndPoint = new IPEndPoint(IPAddress.Any, 0); // IPAddress.Any :  모든 클라이언트에서 오는 요청을 받겠다는 의미
        }
        private void button1_Click(object sender, EventArgs e)
        {
            byte[] data = new byte[1000 * 1000];
            data = newsock.Receive(ref iPEndPoint); // ref : 변수에 대한 참조를 전달하는 기능

            //File.WriteAllBytes("J://Image.jpg", data); // 클라이언트에서 전송한 이미지를 J드라이브 img라는 이름의 jpg로 저장

            pictureBox1.Image = byteArrayToImage(data) // Client에 받은 데이터를 이미지로 변환해 PictureBox에 담는 코드 설정

        }
        private Image byteArrayToImage(byte[] data)
        {
           //byte->이미지로 변환하는 코딩
           MemoryStream ms = new MemoryStream(data)
           Image returnImage = Image.FromStream(ms);
           return returnImage;
        }
    }
}
