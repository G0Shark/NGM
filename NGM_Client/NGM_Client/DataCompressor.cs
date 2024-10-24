using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using OpusSharp.Core;
namespace NGM_Client
{
    class DataCompressor
    {
        private OpusEncoder Encoder;
        private OpusDecoder Decoder;

          int SampleRate = 48000; // 48 kHz
          int Channels = 2; // Stereo
          int FrameSize = 20; // 20 ms

        public DataCompressor(int _sampleRate,int _bitrate, int _channels)
        {
            SampleRate = _sampleRate;

            Encoder = new OpusEncoder(SampleRate, Channels, OpusSharp.Core.Enums.PreDefCtl.OPUS_APPLICATION_AUDIO);
            Encoder.Bitrate = _bitrate;//  64000; // Set bitrate

            Decoder = new OpusDecoder(SampleRate, Channels);
        }

        public byte[] Encode(byte[] buffer, int count)
        {
            var encodeBuffer = new byte[1000]; // Allocate buffer for encoded data
            var encodedBytes = Encoder.Encode(buffer, count, encodeBuffer); // Perform encoding
            byte[]  output = encodeBuffer.Take(encodedBytes).ToArray(); // Trim to actual size
            return output; // Return number of bytes encoded
        }

        public byte[] Decode(byte[] input, int count)
        {
            byte[]  decoded = new byte[SampleRate / 1000 * FrameSize * Channels]; // Allocate buffer for decoded audio
            var bytesRead = Decoder.Decode(input, count, decoded, decoded.Length); // Perform decoding
            return decoded; // Return number of bytes decoded
        }
    }
}
