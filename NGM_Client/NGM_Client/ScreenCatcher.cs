using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System;

using System.Drawing;
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


        public ScreenCatcher()
        {
            screen = System.Windows.Forms.SystemInformation.VirtualScreen;
            x = screen.Left;
            y = screen.Top;
            w = screen.Width;
            h = screen.Height ;

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
            //Console.WriteLine(ImageToByte(image).Length);

            Console.WriteLine(ImageToByte(image).Length);

            byte[] res= Encode(image,25L);

            //res = GetMaxCompressedWebP("C:\\Users\\okhobot\\Downloads\\ege\\00038.png");
            Console.WriteLine(res.Length);

            ///string picturesDirectory = "C:\\Oleg\\git\\NGM\\NGM_Client\\NGM_Client\\";
            ///image.Save(picturesDirectory + "test.png", System.Drawing.Imaging.ImageFormat.Png);

            //image.Save(picturesDirectory+"test.png", ImageFormat.Png);
            //Console.WriteLine("saved");


            return res;
        }


        public byte[] Encode(Bitmap image, long quality = 70L)
        {
            MemoryStream data =new MemoryStream();
            var jpegEncoder = GetEncoder(ImageFormat.Jpeg);
            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality); // 0-100 (меньше = сильнее сжатие)

            image.Save("C:\\Oleg\\git\\NGM\\NGM_Client\\NGM_Client\\test.jpeg", jpegEncoder, encoderParams);
            image.Save(data, jpegEncoder, encoderParams);
            return data.ToArray();
            
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static byte[] ImageToByte(Bitmap img)
        {

            ImageConverter converter = new ImageConverter();

            byte[] res = (byte[])converter.ConvertTo(img, typeof(byte[]));



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
