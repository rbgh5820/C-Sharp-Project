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

namespace UDP_server
{

   
    public partial class Form1 : Form
    {

        public UdpClient server = null;


        public Form1()
        {
            InitializeComponent();
            server = new UdpClient("10.40.10.69", 8888);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
        }
        
        private void button1_Click(object sender, EventArgs e)
        {

            byte[] data = new byte[1000 * 1000];

            data = File.ReadAllBytes("c://test//cat.jpg");
            server.Send(data, data.Length); // 배열에 있는 데이터를 서버에 전송한다.

            label1.Text = "파일을 전송했습니다.";
        }

        private byte[] imgToByteArray(Image img)
        {
            // 이미지를 바이트 배열로 전환
           using(MemoryStream mStream = new MemoryStream())
            {
                return mStream.ToArray();
            }
        }
    }
}
