using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using log4net.Config;
using System.Net.Sockets;
using System.Net;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net")]


namespace ByteArray
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Form1));

        public Form1()
        {
            InitializeComponent();
            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.xml"));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //이미지를 바이트 배열로 변환
            Image img = Image.FromFile("C://test//600.jpg");
            byte[] bArr = imgToByteArray(img);

            //바이트 배열을 이미지로 전환
            Image img1 = byteArrayToImage(bArr);
            pictureBox1.Image = img;
        }

        private Image byteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream mStream = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(mStream);
            }
        }

        private byte[] imgToByteArray(Image img)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                img.Save(mStream, img.RawFormat);
                return mStream.ToArray();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "다른 이름으로 저장";
            dlg.DefaultExt = "jpg";
            dlg.Filter = "JPEG (*.jpg)|*.jpg";
            dlg.FilterIndex = 0;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(dlg.FileName);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.button2.Enabled = false;

            Socket sListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 9999);

            sListener.Bind(ipEndPoint); // 주소를 할당한다.
            sListener.Listen(20);

            Socket sClient = sListener.Accept(); //소켓에서 요청이 올때까지 대기
            IPEndPoint ip = (IPEndPoint)sClient.RemoteEndPoint;
            Console.WriteLine("주소 {0} 에서 접속", ip.Address);

            Byte[] _data = imgToByteArray(this.pictureBox1.Image);
            sClient.Send(BitConverter.GetBytes(_data.Length));

            sClient.Send(_data);

            sListener.Close();
        }
    }
}