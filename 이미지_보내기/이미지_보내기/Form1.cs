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
using System.Text.RegularExpressions;

namespace 이미지_보내기
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Image img = Image.FromFile("C://test//1.jpg");
            pictureBox1.Image = img;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //이미지를 byte배열로 전환하고 파일로 저장하는 코드
            byte[] data = imgToByteArray(pictureBox1.Image);

            // FileMode.Create : 파일을 만들때 사용하는 FileStream코드, FileAccess.Write = 파일쓰기
            FileStream fs = new FileStream("C:\\test\\byteTest", FileMode.Create, FileAccess.Write);

            fs.Write(data, 0, data.Length);
            fs.Close();
        }
        private byte[] imgToByteArray(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, img.RawFormat);
                return ms.ToArray();
            }
        }
    }
}
