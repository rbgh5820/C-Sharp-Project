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

namespace ByteArray2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Socket sClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081);
            sClient.Connect(ipEndPoint);
            Byte[] _data = new byte[500];
            
            int nsize = sClient.Receive(_data); // Receive의 리턴값을 nsize에 반환
            Byte[] size = new byte[nsize];
            Buffer.BlockCopy(_data, 0, size, 0, nsize); // _data
            int iLength = BitConverter.ToInt32(size, 0);

            Byte[] _data2 = new byte[iLength];
            sClient.Receive(_data2);
            this.pictureBox1.Image = byteArrayToImage(_data2);
            sClient.Close();
        }
        private Image byteArrayToImage(byte[] data2)
        {
            MemoryStream ms = new MemoryStream(data2);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}
