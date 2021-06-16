using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using log4net;
using log4net.Config;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.xml")]

namespace DB_insert_Form
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Form1));

        public Form1()
        {
            InitializeComponent();
            XmlConfigurator.Configure(new System.IO.FileInfo("log4net"));
        }
        public void MariaDB_insert()
        {
            string connstr = "Server=10.40.10.69; Port=3306; Database=test; Uid=root; Pwd=12431243a";
            MySqlConnection conn = new MySqlConnection(connstr);
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "insert into movie(movie_name,director,types) values('영화1','감독4','???2')";
            cmd.CommandText = sql;
            try
            {
                conn.Open();
            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            MySqlDataReader reader = cmd.ExecuteReader();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            MariaDB_insert();
            lbDBinsert.Text=("DB가 추가 되었습니다");

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            log.Debug("Form1 Loaded Complete~!");
        }
    }
}
