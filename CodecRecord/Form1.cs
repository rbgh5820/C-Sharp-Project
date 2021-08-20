using System.Diagnostics;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Data;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Net.Sockets;
using System.Net;
using log4net.Config;
using NAudio.Wave;
using NAudio.Codecs;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net")]

namespace Recoder
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Form1));

        byte[] data;
        public UdpClient server = null;

        private readonly G722CodecState encoderState = new G722CodecState(64000, G722Flags.SampleRate8000);
        private readonly G722Codec codec = new G722Codec();

        public Form1()
        {
            InitializeComponent();

            server = new UdpClient("127.0.0.1", 8888);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            XmlConfigurator.Configure(new FileInfo("log4net.xml"));
        }
        WaveIn waveIn;
        WaveFileWriter waveWriter;
        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveWriter != null && G722.Checked == true)
            {
                data = Encode(e.Buffer, 0, e.BytesRecorded);
                byte[] data2 = new byte[5000];
                byte[] size = new byte[4];
                size = BitConverter.GetBytes(data.Length);
                server.Send(size, size.Length);
                log.Debug("data : " + data.Length);

                int sended = 0;

                while ((data.Length - sended) > data2.Length)
                {
                    data2 = new byte[5000];
                    Buffer.BlockCopy(data, sended, data2, 0, data2.Length);
                    sended += data2.Length;
                    log.Debug("size : " + data2.Length);
                    server.Send(data2, data2.Length);
                }
                data2 = new byte[data.Length - sended];
                Buffer.BlockCopy(data, sended, data2, 0, data2.Length);
                server.Send(data2, data2.Length);
                log.Debug("size : " + data2.Length);

                /*waveWriter.Write(e.Buffer,0,e.BytesRecorded);
                 waveWriter.Flush();*/
            }

            if(waveWriter != null && ALaw.Checked == true)
            {
                byte[] bufer = new byte[e.Buffer.Length];
                byte[] data2 = new byte[1600];
                byte[] size = new byte[4];

                data = ALawEncoding(e.Buffer, 0, e.BytesRecorded);
                waveWriter.Write(data, 0, data.Length);
                waveWriter.Flush();
            }

            if(waveWriter != null && muLaw.Checked == true)
            {
                data = MuLawEncoding(e.Buffer, 0, e.BytesRecorded);
                byte[] size = new byte[4];
                size = BitConverter.GetBytes(data.Length);
                server.Send(size, size.Length);
                server.Send(data, data.Length);
                log.Debug("data : " + data.Length);
            }
        }

        /// <summary>
        /// Mu-Law 코덱 인코딩
        /// </summary>
        private byte[] MuLawEncoding(byte[] data, int offset, int length)
        {
            byte[] encoded = new byte[length / 2];
            int outIndex = 0;
            for (int n = 0; n < length; n += 2)
            {
                encoded[outIndex++] = MuLawEncoder.LinearToMuLawSample(BitConverter.ToInt16(data, offset + n));
            }
            return encoded;
        }

        /// <summary>
        /// A-Law 코덱 인코딩
        /// </summary>
        private byte[] ALawEncoding(byte[] data, int offset, int length)
        {
            byte[] encoded = new byte[length / 2];
            int outIndex = 0;
            for (int n = 0; n < length; n += 2)
            {
                encoded[outIndex++] = ALawEncoder.LinearToALawSample(BitConverter.ToInt16(data, offset + n));
            }
            return encoded;
        }
        /// <summary>
        /// 라디오 버튼으로 코덱 선택후 녹화
        /// </summary>
        private void Record_Audio_Click(object sender, EventArgs e)
        {
            waveIn = new WaveIn();
            waveIn.DeviceNumber = 0;
            waveIn.WaveFormat = new WaveFormat(44100, WaveIn.GetCapabilities(waveIn.DeviceNumber).Channels); // sampleRate : 1초당 들리는 sample갯수(44100 = 44.1khz)
            waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(WaveIn_DataAvailable);

            if(G722.Checked == true)
            {
                MemoryStream ms = new MemoryStream();
                waveWriter = new WaveFileWriter(ms, waveIn.WaveFormat);
                waveIn.StartRecording();
            }
            if(ALaw.Checked == true)
            {
                MemoryStream ms = new MemoryStream();
                waveWriter = new WaveFileWriter(ms, waveIn.WaveFormat);
                waveIn.StartRecording();
            }
            if(muLaw.Checked == true)
            {
                MemoryStream ms = new MemoryStream();
                waveWriter = new WaveFileWriter(ms, waveIn.WaveFormat);
                waveIn.StartRecording();
            }
        }
        /// <summary>
        /// G722 코덱 인코딩
        /// </summary>
        private byte[] Encode(byte[] data, int offset, int length)
        {
            if (offset != 0)
            {
                throw new ArgumentException("G722 does not yet support non-zero offsets");
            }
            int encodeLength = length / 2;
            byte[] outputBuffer = new byte[encodeLength];
            WaveBuffer wb = new WaveBuffer(data);
            int encoded = codec.Encode(encoderState, outputBuffer, wb.ShortBuffer, length / 2);
            return outputBuffer;
        }
        /// <summary>
        /// 오디오 녹화 중지
        /// </summary>
        private void Save_Stop_Click(object sender, EventArgs e)
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