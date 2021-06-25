using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace 이미지_받기_test
{
    public partial class Form1 : Form
    {
        public UdpClient newsock = null;
        public IPEndPoint iPEndPoint = null;

        public Form1()
        {
            InitializeComponent();

            /*IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 8888);
            newsock = new UdpClient(ipep);
            iPEndPoint = new IPEndPoint(IPAddress.Any, 0);*/
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //파일 읽어오기
            FileStream fs = File.OpenRead(@"C://test//byteTest");


            byte[] data = new byte[fs.Length];

            fs.Read(data, 0, data.Length);
            fs.Close();

            this.pictureBox1.Image = byteArrayToImage(data);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

        }

        private Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}
