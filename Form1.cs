using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZoomIn
{
    public partial class Form1 : Form
    {
        private Image img;
        private bool IsClicked = false;
        private Point recLoc;
        private Point choosingPoint;

        public Form1()
        {
            InitializeComponent();
            this.pictureBox1.MouseWheel += pictureBox1_MouseWheel; // 마우스 휠 이벤트 생성
            this.pictureBox1.Paint += pictureBox1_Paint; // 이벤트 생성
            this.pictureBox1.MouseDown += pictureBox1_MouseDown; // 마우스 이벤트 생성
            this.pictureBox1.MouseUp += pictureBox1_MouseUp;
            this.pictureBox1.MouseMove += pictureBox1_MouseMove;
        }


        // 이미지를 이동시키는 이벤트
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) 
        {
            if (IsClicked)
            {
                recLoc.X = recLoc.X + e.X - choosingPoint.X;
                recLoc.Y = recLoc.Y + e.Y - choosingPoint.Y;
                choosingPoint = e.Location;
                pictureBox1.Invalidate();
            }
        }


        //마우스버튼 뗄 경우 발생하는 이벤트
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            IsClicked = false;
        }

        //마우스버튼을 클릭했을때 발생하는 이벤트
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            IsClicked = true;
            choosingPoint.X = e.X;
            choosingPoint.Y = e.Y;


        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (IsClicked)
            {
                e.Graphics.Clear(Color.White); // 이미지 이동시 배경화면 하얀색
                e.Graphics.DrawImage(img, recLoc);
            }
        }

        //// MouseWheel로 확대,축소하는 이벤트
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e) 
        {
            if(e.Delta > 0) // delta = 마우스 휠 함수
            {
                pictureBox1.Width = pictureBox1.Width + 60; // x축 확대
                pictureBox1.Height = pictureBox1.Height + 60; // y축 확대
            }
            else
            {
                pictureBox1.Width = pictureBox1.Width - 60; // x축 축소
                pictureBox1.Height = pictureBox1.Height - 60; // y축 축소
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //버튼클릭시 openFileDialog 디자인으로 파일을 골라서 열 수 있다.
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) 
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                img = pictureBox1.Image;
            }
        }

        // Form을 닫을 시 종료
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }
        }
    }
}
