using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Codecs;

namespace Recoder
{
    class ALawCodec
    {
        /// <summary>
        /// A-Law 코덱
        /// </summary>
        /// <returns></returns>
        public byte[] Encode(byte[] data, int offset, int length)
        {
            byte[] encoded = new byte[length / 2]; // 오디오 파일 크기를 2로 나눈만큼 encoded에 초기화
            int outIndex = 0; 
            for (int n = 0; n < length; n += 2)
            {
                encoded[outIndex++] = ALawEncoder.LinearToALawSample(BitConverter.ToInt16(data, offset + n));
            }
            return encoded;
        }
        public byte[] Decode(byte[] data, int offset, int length)
        {
            byte[] decoded = new byte[length * 2];
            int outIndex = 0;
            for (int n = 0; n < length; n++)
            {
                short decodedSample = ALawDecoder.ALawToLinearSample(data[n + offset]);
                decoded[outIndex++] = (byte)(decodedSample & 0xFF);
                decoded[outIndex++] = (byte)(decodedSample >> 8);
            }
            return decoded;
        }
    }
}
