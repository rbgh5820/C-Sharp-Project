using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioPlay
{
    public partial class Form1 : Form
    {
        private BufferedWaveProvider bwp;
        public Form1()
        {
            InitializeComponent();
        }

        WaveIn waveIn;
        WaveFileWriter waveWriter;
        WaveOut waveOut;
        private void button1_Click(object sender, EventArgs e)
        {
            waveIn = new WaveIn();
            waveOut = new WaveOut();
            waveIn.DeviceNumber = 0;
            waveIn.WaveFormat = new WaveFormat(44100, WaveIn.GetCapabilities(waveIn.DeviceNumber).Channels); // sampleRate : 1초당 들리는 sample갯수(44100 = 44.1khz)
            waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(waveIn_DataAvailable);
            MemoryStream ms = new MemoryStream();
            waveWriter = new WaveFileWriter(ms, waveIn.WaveFormat);
            bwp = new BufferedWaveProvider(waveIn.WaveFormat);
            bwp.DiscardOnBufferOverflow = true;
            waveOut.Init(bwp);
            waveIn.StartRecording();
        }

        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveWriter != null)
            {
                waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
                waveWriter.Flush();
                bwp.AddSamples(e.Buffer, 0, e.BytesRecorded);
            }
            waveOut.Play();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null;
            }
            if (waveWriter != null)
            {
                waveWriter.Dispose();
                waveWriter = null;
            }
        }
    }
}
