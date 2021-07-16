using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Imaging;
using FFmpeg.AutoGen.Example;
using FFmpeg.AutoGen;
using log4net.Config;
using System.Windows.Forms;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net")]

namespace Image_en_decoing
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Form1));
        public Form1()
        {
            InitializeComponent();
            FFmpegBinariesHelper.RegisterFFmpegBinaries();
            SetupLogging();
            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.xml"));
        }

        private unsafe void button1_Click(object sender, EventArgs e)
        {
            /*Image img = Image.FromFile("C://test//cat.jpg");
            pictureBox1.Image = img;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            byte[] picture = ImgToByteArray(pictureBox1.Image);*/

            

            SetupLogging();

            EncodeAllFrameImage();
            label1.Text = "인코딩 완료";
        }

        private byte[] ImgToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

        private static unsafe void EncodeAllFrameImage()
        {
            string[] frameFiles = Directory.GetFiles(".", "frame.*.jpg").OrderBy(x => x).ToArray();// 파일 이름을 검색하여 framFiles에 담는다.
            Image firstFrameImage = Image.FromFile(frameFiles.First()); // frameFiles의 첫번째로 검색된 파일을 이미지로 저장한다.
            /*string frameFiles = "C://test//cat.jpg";
            Image firstFrameImage = Image.FromFile(frameFiles);*/ // framFiles의 이미지 파일을 저장


            string outputFileName = "out.h263"; // 인코딩할 파일 이름
            int fps = 25; // fps 변수값 초기화
            var sourceSize = firstFrameImage.Size;
            var sourcePixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24; // PixelFormat : 이미지의 각 픽셀에 대한 색 데이터의 형식을 지정
            var destinationSize = new Size(352,288); // h263 cif 사이즈
            var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_YUV420P;
            using (VideoFrameConverter vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
            {
                using (FileStream fs = File.Open(outputFileName, FileMode.Create)) // be advise only ffmpeg based player (like ffplay or vlc) can play this file, for the others you need to go through muxing
                {
                    using (H264VideoStreamEncoder vse = new H264VideoStreamEncoder(fs, fps, destinationSize))
                    {
                        int frameNumber = 0;
                        foreach (string frameFile in frameFiles)
                        {
                            byte[] bitmapData;

                            using (Image frameImage = Image.FromFile(frameFile))
                            using (Bitmap frameBitmap = frameImage is Bitmap bitmap ? bitmap : new Bitmap(frameImage))
                            {
                                bitmapData = GetBitmap(frameBitmap);
                            }

                            fixed (byte* pBitmapData = bitmapData)
                            {
                                var data = new byte_ptrArray8 { [0] = pBitmapData };
                                var linesize = new int_array8 { [0] = bitmapData.Length / sourceSize.Height };
                                var frame = new AVFrame
                                {
                                    data = data,
                                    linesize = linesize,
                                    height = sourceSize.Height
                                };
                                var convertedFrame = vfc.Convert(frame);
                                convertedFrame.pts = frameNumber * fps;
                                vse.Encode(convertedFrame);
                            }
                            Console.WriteLine($"frame :{frameNumber}");
                            frameNumber++;
                            log.Debug("frame : " + frameNumber);
                            if (frameNumber > 1000) break;
                        }
                    }
                }
            }
        }

        private static byte[] GetBitmap(Bitmap frameBitmap)
        {
            var bitmapData = frameBitmap.LockBits(new Rectangle(Point.Empty, frameBitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                var length = bitmapData.Stride * bitmapData.Height;
                byte[] data = new byte[length];
                Marshal.Copy(bitmapData.Scan0, data, 0, length);
                return data;
            }
            finally
            {
                frameBitmap.UnlockBits(bitmapData);
            }
        }

        private static unsafe void SetupLogging()
        {
            ffmpeg.av_log_set_level(ffmpeg.AV_LOG_VERBOSE);

            av_log_set_callback_callback logCallback = (p0, level, format, vl) =>
            {
                if (level > ffmpeg.av_log_get_level()) return;

                int lineSize = 1024;
                var lineBuffer = stackalloc byte[lineSize];
                var printPrefix = 1;
                ffmpeg.av_log_format_line(p0, level, format, vl, lineBuffer, lineSize, &printPrefix);
                var line = Marshal.PtrToStringAnsi((IntPtr)lineBuffer);
            };
            ffmpeg.av_log_set_callback(logCallback);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FFmpegBinariesHelper.RegisterFFmpegBinaries();


            SetupLogging();

            DecodeAllFrameToImage();
            label2.Text = "디코딩 완료";
        }

        private static unsafe void DecodeAllFrameToImage()
        {
            string File = "video.mpeg"; // be advised this file holds 1440 frames
            using (VideoStreamDecoder vsd = new VideoStreamDecoder(File)) // using범위를 벗어나면 dispose를 쓰는것과 같다.
            {
                Console.WriteLine($"codec name: {vsd.CodecName}");

                var info = vsd.GetContextInfo();
                info.ToList().ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));

                var sourceSize = vsd.FrameSize;
                var sourcePixelFormat = vsd.PixelFormat;
                var destinationSize = sourceSize;
                var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
                using (VideoFrameConverter vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
                {
                    int frameNumber = 0;
                    while (vsd.TryDecodeNextFrame(out AVFrame frame))
                    {
                        AVFrame convertedFrame = vfc.Convert(frame);

                        using (Bitmap bitmap = new Bitmap(convertedFrame.width, convertedFrame.height, convertedFrame.linesize[0], PixelFormat.Format24bppRgb, (IntPtr)convertedFrame.data[0]))
                            bitmap.Save($"file.{frameNumber:D8}.jpg", ImageFormat.Jpeg);

                        Console.WriteLine($"frame: {frameNumber}");
                        frameNumber++;
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*Object selectedItem = comboBox1.SelectedItem;*/
            if(comboBox1.SelectedItem.ToString() == "H263Encode")
            {

                Encode_H263();
                Decode_H263();
            }
            else if (comboBox1.SelectedItem.ToString() == "Mpeg4Encode")
            {
                Encode_Mpeg4();
                Decode_Mpeg4();
            }
        }

        private unsafe void Decode_Mpeg4()
        {
            string File = "video.mpg";
            using (VideoStreamDecoder vsd = new VideoStreamDecoder(File)) // using범위를 벗어나면 dispose를 쓰는것과 같다.
            {
                Console.WriteLine($"codec name: {vsd.CodecName}");

                var info = vsd.GetContextInfo();
                info.ToList().ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));

                var sourceSize = vsd.FrameSize;
                var sourcePixelFormat = vsd.PixelFormat;
                var destinationSize = sourceSize;
                var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
                using (VideoFrameConverter vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
                {
                    int frameNumber = 0;
                    while (vsd.TryDecodeNextFrame(out AVFrame frame))
                    {
                        AVFrame convertedFrame = vfc.Convert(frame);

                        using (Bitmap bitmap = new Bitmap(convertedFrame.width, convertedFrame.height, convertedFrame.linesize[0], PixelFormat.Format24bppRgb, (IntPtr)convertedFrame.data[0]))
                            bitmap.Save($"File.{frameNumber:D8}.jpg", ImageFormat.Jpeg);

                        frameNumber++;
                        log.Debug("Decode frame : " + frameNumber);
                    }
                }
            }
        }

        private unsafe void Encode_Mpeg4()
        {
            string[] frameFiles = Directory.GetFiles(".", "frame.*.jpg").OrderBy(x => x).ToArray();//frame.*.jpg의 이름을 가진 이미지 파일을 얻어온다.
            var firstFrameImage = Image.FromFile(frameFiles.First());
            /*string frameFiles = "C://test//cat.jpg";
            Image firstFrameImage = Image.FromFile(frameFiles);*/ // framFiles의 이미지 파일을 저장

            string outputFileName = "video.mpg"; // 인코딩한 파일 이름
            int fps = 25; // fps 변수값 초기화
            var sourceSize = firstFrameImage.Size;
            var sourcePixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24; // PixelFormat : 이미지의 각 픽셀에 대한 색 데이터의 형식을 지정
            var destinationSize = new Size(352, 288);
            var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_YUV420P;
            using (VideoFrameConverter vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
            {
                using (FileStream fs = File.Open(outputFileName, FileMode.Create)) // be advise only ffmpeg based player (like ffplay or vlc) can play this file, for the others you need to go through muxing
                {
                    using (Mpeg4VideoStreamEncoder vse = new Mpeg4VideoStreamEncoder(fs, fps, destinationSize))
                    {
                        int frameNumber = 0;
                        foreach (var frameFile in frameFiles)
                        {
                            byte[] bitmapData;

                            using (Image frameImage = Image.FromFile(frameFile))
                            using (Bitmap frameBitmap = frameImage is Bitmap bitmap ? bitmap : new Bitmap(frameImage))
                            {
                                bitmapData = GetBitmap(frameBitmap);
                            }

                            fixed (byte* pBitmapData = bitmapData)
                            {
                                var data = new byte_ptrArray8 { [0] = pBitmapData };
                                var linesize = new int_array8 { [0] = bitmapData.Length / sourceSize.Height };
                                var frame = new AVFrame
                                {
                                    data = data,
                                    linesize = linesize,
                                    height = sourceSize.Height
                                };
                                var convertedFrame = vfc.Convert(frame);
                                convertedFrame.pts = frameNumber * fps;
                                vse.Encode(convertedFrame);
                            }
                            Console.WriteLine($"Image :{frameNumber}");
                            frameNumber++;
                            log.Debug("frame : " + frameNumber);
                            if (frameNumber > 1000) break;
                        }
                    }
                }
            }
        }

        private static unsafe void Encode_H263()
        {
            string[] frameFiles = Directory.GetFiles(".", "frame.*.jpg").OrderBy(x => x).ToArray();//frame.*.jpg의 이름을 가진 이미지 파일을 얻어온다.
            var firstFrameImage = Image.FromFile(frameFiles.First());
            /*string frameFiles = "C://test//cat.jpg";
            Image firstFrameImage = Image.FromFile(frameFiles);*/ // framFiles의 이미지 파일을 저장

            string outputFileName = "video.h263"; // 인코딩한 파일 이름
            int fps = 25; // fps 변수값 초기화
            var sourceSize = firstFrameImage.Size;
            var sourcePixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24; // PixelFormat : 이미지의 각 픽셀에 대한 색 데이터의 형식을 지정
            var destinationSize = new Size(352, 288);
            var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_YUV420P;
            using (VideoFrameConverter vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
            {
                using (FileStream fs = File.Open(outputFileName, FileMode.Create)) // be advise only ffmpeg based player (like ffplay or vlc) can play this file, for the others you need to go through muxing
                {
                    using (H263VideoStreamEncoder vse = new H263VideoStreamEncoder(fs, fps, destinationSize))
                    {
                        int frameNumber = 0;
                        foreach (var frameFile in frameFiles)
                        {
                            byte[] bitmapData;

                            using (Image frameImage = Image.FromFile(frameFile))
                            using (Bitmap frameBitmap = frameImage is Bitmap bitmap ? bitmap : new Bitmap(frameImage))
                            {
                                bitmapData = GetBitmap(frameBitmap);
                            }

                            fixed (byte* pBitmapData = bitmapData)
                            {
                                var data = new byte_ptrArray8 { [0] = pBitmapData };
                                var linesize = new int_array8 { [0] = bitmapData.Length / sourceSize.Height };
                                var frame = new AVFrame
                                {
                                    data = data,
                                    linesize = linesize,
                                    height = sourceSize.Height
                                };
                                var convertedFrame = vfc.Convert(frame);
                                convertedFrame.pts = frameNumber * fps;
                                vse.Encode(convertedFrame);
                            }
                            Console.WriteLine($"Image :{frameNumber}");
                            frameNumber++;
                            log.Debug("frame : " + frameNumber);
                            if (frameNumber > 1000) break;
                        }
                    }
                }
            }
        }
        private static unsafe void Decode_H263()
        {
            string File = "video.h263";
            using (VideoStreamDecoder vsd = new VideoStreamDecoder(File)) // using범위를 벗어나면 dispose를 쓰는것과 같다.
            {
                Console.WriteLine($"codec name: {vsd.CodecName}");

                var info = vsd.GetContextInfo();
                info.ToList().ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));

                var sourceSize = vsd.FrameSize;
                var sourcePixelFormat = vsd.PixelFormat;
                var destinationSize = sourceSize;
                var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
                using (VideoFrameConverter vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
                {
                    int frameNumber = 0;
                    while (vsd.TryDecodeNextFrame(out AVFrame frame))
                    {
                        AVFrame convertedFrame = vfc.Convert(frame);

                        using (Bitmap bitmap = new Bitmap(convertedFrame.width, convertedFrame.height, convertedFrame.linesize[0], PixelFormat.Format24bppRgb, (IntPtr)convertedFrame.data[0]))
                            bitmap.Save($"File.{frameNumber:D8}.jpg", ImageFormat.Jpeg);

                        frameNumber++;
                        log.Debug("Decode frame : " + frameNumber);
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

