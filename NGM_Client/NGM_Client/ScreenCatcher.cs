using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.IO.Compression;
using H264Sharp;
using H264SharpBitmapExtentions;
using System.IO;
using System.Drawing.Imaging;
namespace NGM_Client
{
    class ScreenCatcher
    {
        Rectangle screen;
        int x;
        int y;
        int w;
        int h;
        RgbImage rgbIn;
        RgbImage rgbOut;
        H264Encoder encoder;
        H264Decoder decoder;




        public ScreenCatcher()
        {
            screen = System.Windows.Forms.SystemInformation.VirtualScreen;
            x = screen.Left;
            y = screen.Top;
            w = screen.Width;
            h = screen.Height ;
            
            encoder = new H264Encoder();
            decoder = new H264Decoder();
            decoder.Initialize();
            encoder.Initialize(w, h, 200_000_000, 30, ConfigType.ScreenCaptureAdvanced);

            rgbOut = new RgbImage(H264Sharp.ImageFormat.Rgb, w, h);


        }

        /*
        public void EncodeImageToH264(Bitmap bitmap)
        {
           

            // Преобразуем изображение в массив байтов (формат RGB24)
            var imageData = new byte[bitmap.Width * bitmap.Height * 3];
            int index = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    imageData[index++] = pixel.R;
                    imageData[index++] = pixel.G;
                    imageData[index++] = pixel.B;
                }
            }

            // Кодируем изображение как один кадр видео
            var encodedFrame = encoder.Encode(imageData, bitmap.Width, bitmap.Height);

            // Сохраняем результат в файл
           //File.WriteAllBytes(outputVideoPath, encodedFrame);
        }
        */

        private byte[] ConvertBitmapToRgb24(Bitmap bitmap)
        {
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            try
            {
                int bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;
                byte[] rgbValues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, rgbValues, 0, bytes);
                return rgbValues;
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        List<byte[]> Encode(Bitmap image)
        {
            
            //image = new Bitmap("C:\\Users\\okhobot\\Downloads\\ege\\00038.png");
            RgbImage source = new RgbImage(H264Sharp.ImageFormat.Rgb, w, h, BitmapExtensions.BitmapToRawBytes(image));

            List<byte[]> encodedFrames = new List<byte[]>();

            if (!encoder.Encode(source, out EncodedData[] ec))
            {
                foreach (var encoded in ec)
                {
                    encodedFrames.Add(encoded.GetBytes());
                }
            }
            return encodedFrames;
        }

        RgbImage Decode(List<byte[]> encodedFrames)
        {

            RgbImage rgb = new RgbImage(H264Sharp.ImageFormat.Rgb, w, h);
            foreach (byte[] encoded in encodedFrames)
            {
                if (decoder.Decode(encoded, 0, encoded.Length, noDelay: true, out DecodingState ds, ref rgb))
                {
                    // Do something with rgb
                }
            }
            return rgb;
        }
        


        public byte[] CaptureScreen()
        {


            Bitmap image = new Bitmap(w, h);
            using var graphics = Graphics.FromImage(image);
            graphics.CopyFromScreen(x, y, 0, 0, new Size(w, h));

            /*
            List<byte[]>  data=Encode(image);


            byte[] b= Decode(data).GetBytes();
            using var outputStream = File.Create("outim.png");
            outputStream.Write(b);
            outputStream.Close();




            */
            byte[] res = ( Decode(Encode(image)).GetBytes());

            Console.WriteLine(ImageToByte(image).Length);
            Console.WriteLine(res.Length);

            ///string picturesDirectory = "C:\\Oleg\\git\\NGM\\NGM_Client\\NGM_Client\\";
            ///res.Save(picturesDirectory + "test.png", System.Drawing.Imaging.ImageFormat.Png);

            //image.Save(picturesDirectory+"test.png", ImageFormat.Png);
            //Console.WriteLine("saved");


            return ImageToByte(image);
        }
        public static byte[] ImageToByte(Bitmap img)
        {

            ImageConverter converter = new ImageConverter();

            byte[] res=(byte[])converter.ConvertTo(img, typeof(byte[]));



            return res;
        }

        public static Bitmap ByteToImage(byte[] data)
        {
            Bitmap res;
            using (var ms = new MemoryStream(data))
            {
                res = new Bitmap(ms);
            }
            

            return res;
        }

        



    }
}
