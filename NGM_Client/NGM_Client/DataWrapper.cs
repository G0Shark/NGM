using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using OpusSharp.Core;
using System.Threading.Channels;
using System.Security.Cryptography;
namespace NGM_Client
{
    class DataWrapper
    {
        private OpusEncoder Encoder;
        private OpusDecoder Decoder;

        int sampleRate;
        int channels;
        int frameSize=20;

        public DataWrapper(int _sampleRate, int _bitrate, int _frameSize, int _channels)
        {
            sampleRate = _sampleRate;
            frameSize = _frameSize;
            channels = _channels;

            Encoder = new OpusEncoder(sampleRate, channels, OpusSharp.Core.Enums.PreDefCtl.OPUS_APPLICATION_AUDIO);//OpusSharp.Core.Enums.PreDefCtl.OPUS_APPLICATION_AUDIO
                                                                                                                   //Encoder.Bitrate = _bitrate;//  64000; // Set bitrate
            Decoder = new OpusDecoder(sampleRate, channels);
        }

        public byte[] Encode(byte[] buffer)
        {
            var encodeBuffer = new byte[1000]; // Allocate buffer for encoded data
            var encodedBytes = Encoder.Encode(buffer, frameSize, encodeBuffer); // Perform encoding
            byte[] output = encodeBuffer.Take(encodedBytes).ToArray(); // Trim to actual size
            //Console.WriteLine(output.Length);
            return output; // Return number of bytes encoded
        }

        public byte[] Decode(byte[] input)
        {
            //Decoder = new OpusDecoder(sampleRate, channels);
            byte[] decoded = new byte[frameSize]; // Allocate buffer for decoded audio    sampleRate / 1000 * 320 * channels
            var bytesRead = Decoder.Decode(input, input.Length, decoded, frameSize); // Perform decoding   decoded.Length
            
            //Console.WriteLine("d: "+decoded.Length);
            return decoded; // Return number of bytes decoded
        }
    

        //static

        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }

        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
        public static string GetHashString(string input)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(input);


            byte[] hashValue = SHA256.HashData(messageBytes); 
            Console.WriteLine(Convert.ToHexString(hashValue));

            return Convert.ToHexString(hashValue);
        }

    }
}
