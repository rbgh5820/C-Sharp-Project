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
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace DB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void MariaDB_Select()
        {
            string connstr = "Server=127.0.0.1; Port=3306; Database=test;Uid=root; Pwd=12431243a";
            MySqlConnection conn = new MySqlConnection(connstr);
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "Select * from movie";
            cmd.CommandText = sql;
            try
            {
                conn.Open();
            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                textMovie.Text = reader["movie_name"].ToString();
                textDi.Text = reader["director"].ToString();
                textType.Text = reader["Types"].ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
            ILog log = LogManager.GetLogger(typeof(Form1));
            while (true)
            {
                log.Info("Log4Net Info");
                log.Info("Log4Net debug");
                log.Info("Log4Net Warn");
                log.Info("Log4Net Error");
                log.Info("Log4Net Fatal");
            }
            MariaDB_Select();
        }

        private void textType_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
