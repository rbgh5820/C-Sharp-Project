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

namespace TCPserver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) // 이미지를 가져오는 버튼
        {
            OpenFileDialog dia = new OpenFileDialog();
            dia.Multiselect = false;
            dia.Filter = "|*.jpg"; // 이미지 파일 타입은 .jpg 파일

            if (dia.ShowDialog() == DialogResult.OK)
            {
                this.pictureBox1.ImageLocation = dia.FileName; // 이미지를 가져올시 픽쳐박스에 생성
            }
        }
        public byte[] ImageToByteArray(System.Drawing.Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray(); // 이미지를 배열에 반환한다.
        }

        private void button2_Click(object sender, EventArgs e) // 클라이언트와의 연결 대기하는 버튼
        {
            this.button2.Enabled = false;

            Socket sListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 9999);

            sListener.Bind(ipEndPoint);
            sListener.Listen(20);

            Console.WriteLine("클라이언트 연결을 대기합니다.");

            Socket sClient = sListener.Accept(); //소켓에서 요청이 올때까지 대기
            IPEndPoint ip = (IPEndPoint)sClient.RemoteEndPoint;
            Console.WriteLine("주소 {0} 에서 접속", ip.Address);

            Byte[] _data = ImageToByteArray(this.pictureBox1.Image);
            sClient.Send(BitConverter.GetBytes(_data.Length));
            sClient.Send(_data);

            sListener.Close();
        }
    }
}
