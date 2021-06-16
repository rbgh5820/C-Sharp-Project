using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPclient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e) // 서버와 연결 후 이미지를 가져오는 버튼
        {
            Socket sClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("10.40.10.69"), 9999);
            sClient.Connect(ipEndPoint);
            Byte[] _data = new byte[1024];
            sClient.Receive(_data); // 클라이언트에서 데이터 수신
            int iLength = BitConverter.ToInt32(_data, 0);

            Byte[] _data2 = new byte[iLength];
            sClient.Receive(_data2);

            this.pictureBox1.Image = byteArrayToImage(_data2);

            sClient.Close();
        }
        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}
