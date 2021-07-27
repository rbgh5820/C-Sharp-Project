using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FFmpeg.AutoGen.Example
{
    public sealed unsafe class VideoStreamDecoder : IDisposable
    {
        private readonly AVCodecContext* _pCodecContext;
        private readonly AVFormatContext* _pFormatContext;
        private readonly int _streamIndex;
        private readonly AVFrame* _pFrame;
        private readonly AVPacket* _pPacket;

        public string CodecName { get; }
        public Size FrameSize { get; }
        public AVPixelFormat PixelFormat { get; }


        public VideoStreamDecoder(Size frameSize)
        {

            _pFormatContext = ffmpeg.avformat_alloc_context();
            AVCodecID codecId;
            if (frameSize.Equals(new Size(704, 576)))
            {
                codecId = AVCodecID.AV_CODEC_ID_H263;
            }
            else
            {
                codecId = AVCodecID.AV_CODEC_ID_H263;
                //codecId = AVCodecID.AV_CODEC_ID_H264;
            }
            var pCodec = ffmpeg.avcodec_find_decoder(codecId);
            if (pCodec == null) throw new InvalidOperationException("Unsupported codec.");
            _pCodecContext = ffmpeg.avcodec_alloc_context3(pCodec);

            ffmpeg.avcodec_open2(_pCodecContext, pCodec, null).ThrowExceptionIfError();
            CodecName = ffmpeg.avcodec_get_name(codecId);
            FrameSize = frameSize;
            PixelFormat = AVPixelFormat.AV_PIX_FMT_YUV420P;
            _pPacket = ffmpeg.av_packet_alloc();
            _pFrame = ffmpeg.av_frame_alloc();
        }

        public void Dispose()
        {
            ffmpeg.av_frame_unref(_pFrame);
            ffmpeg.av_free(_pFrame);

            ffmpeg.av_packet_unref(_pPacket);
            ffmpeg.av_free(_pPacket);

            ffmpeg.avcodec_close(_pCodecContext);
            var pFormatContext = _pFormatContext;
            ffmpeg.avformat_close_input(&pFormatContext);
        }
        public bool Decode(out AVFrame frame, byte* pImgByte, int imgLenth)
        {
            ffmpeg.av_frame_unref(_pFrame); //프레임 초기화
            int error;
            do
            {
                try
                {
                    _pPacket->data = pImgByte;
                    _pPacket->size = imgLenth;
                    ffmpeg.avcodec_send_packet(_pCodecContext, _pPacket).ThrowExceptionIfError();
                }
                finally
                {
                    ffmpeg.av_packet_unref(_pPacket);
                }
                error = ffmpeg.avcodec_receive_frame(_pCodecContext, _pFrame);
            } while (error == ffmpeg.AVERROR(ffmpeg.EAGAIN));

            error.ThrowExceptionIfError();
            frame = *_pFrame;
            return true;
        }
    }
}