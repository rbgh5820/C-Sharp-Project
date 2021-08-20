using NAudio.Wave;
using System;
using NAudio.Codecs;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recoder
{
    class G722codec
    {
        private readonly G722CodecState encoderState = new G722CodecState(64000, G722Flags.SampleRate8000);
        private readonly G722Codec codec = new G722Codec();

        /// <summary>
        /// G722 코덱 인코딩
        /// </summary>
        /// <returns></returns>
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
        private byte[] Decode(byte[] data, int offset, int length)
        {
            if (offset != 0)
            {
                throw new ArgumentException("G722 does not yet support non-zero offsets");
            }
            int decodedLength = length * 2;
            var outputBuffer = new byte[decodedLength];
            var wb = new WaveBuffer(outputBuffer);
            int decoded = codec.Decode(encoderState, wb.ShortBuffer, data, length);
            return outputBuffer;
        }
    }
}
