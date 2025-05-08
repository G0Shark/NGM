using Aspose.Imaging.ImageFilters.FilterOptions;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;





namespace NGM_Client
{
    class ScreenCatcher
    {
        Rectangle screen;
        int x;
        int y;
        Size imgSize;


        public ScreenCatcher()
        {
            screen = System.Windows.Forms.SystemInformation.VirtualScreen;
            x = screen.Left;
            y = screen.Top;
            imgSize.Width = screen.Width;
            imgSize.Height = screen.Height ;

        }




        public byte[] CaptureScreen()
        {


            Bitmap image = new Bitmap(imgSize.Width, imgSize.Height);
            using var graphics = Graphics.FromImage(image);
            graphics.CopyFromScreen(x, y, 0, 0, new Size(imgSize.Width, imgSize.Height));

            /*
            List<byte[]>  data=Encode(image);


            byte[] b= Decode(data).GetBytes();
            using var outputStream = File.Create("outim.png");
            outputStream.Write(b);
            outputStream.Close();

            */
            //Console.WriteLine(ImageToByte(image).Length);

            //Console.WriteLine(ImageToByte(image).Length);

            int quality = 50;

            //Console.WriteLine(imgSize * 10 / (int)(Math.Sqrt(200 - quality)));
            //Console.WriteLine(90 - Math.Sqrt(5000 - quality * 50));

            //byte[] res= Encode(ResizeImage(image,imgSize*10/(int)(Math.Sqrt(150 - quality*1.5))), (long)(90-Math.Sqrt(9000 - quality*90)));
            byte[] res = Encode((image),(long)quality);
            image = ByteToImage(res);

            //res = GetMaxCompressedWebP("C:\\Users\\okhobot\\Downloads\\ege\\00038.png");
            ///Console.WriteLine(res.Length);

            ///string picturesDirectory = "C:\\Oleg\\git\\NGM\\NGM_Client\\NGM_Client\\";
            ///image.Save(picturesDirectory + "test2.png", ImageFormat.Jpeg);

            //image.Save(picturesDirectory+"test.png", ImageFormat.Png);
            //Console.WriteLine("saved");


            return res;
        }


        public void test(ref Bitmap image2)
        {
             //реализация сглаживающего фильтра
            int[,] arrR = new int[image2.Width, image2.Height];
            int[,] arrG = new int[image2.Width, image2.Height];
            int[,] arrB = new int[image2.Width, image2.Height];

            for (int i = 0; i < image2.Width; i++)
            {
                for (int j = 0; j < image2.Height; j++)
                {
                    arrR[i, j] = image2.GetPixel(i, j).R;
                    arrG[i, j] = image2.GetPixel(i, j).G;
                    arrB[i, j] = image2.GetPixel(i, j).B;
                }
            }

            for (int i = 1; i < image2.Width - 1; i++)
            {
                for (int j = 1; j < image2.Height - 1; j++)
                {
                    int arrRSum = 0, arrGSum = 0, arrBSum = 0;
                    int arrsrR = 0, arrsrG = 0, arrsrB = 0;
                    for (int x = -1; x < 2; x++)
                    {
                        for (int y = -1; y < 2; y++)
                        {
                            float k = 0.5f;
                            if (x == 0 && y == 0) k = 5f;
                            arrRSum = arrRSum + (int)(arrR[i + x, j + y] * k);
                            arrGSum = arrGSum + (int)(arrG[i + x, j + y] * k);
                            arrBSum = arrBSum + (int)(arrB[i + x, j + y] * k);

                        }
                    }
                    
                    arrsrR = arrRSum / 9;
                    arrsrG =  arrGSum / 9 ;
                    arrsrB =  arrBSum / 9;
                    image2.SetPixel(i, j, Color.FromArgb(arrsrR, arrsrG, arrsrB));

                }
            }
        }



        public byte[] Encode(Bitmap image, long quality = 70L)
        {
            MemoryStream data =new MemoryStream();
            var jpegEncoder = GetEncoder(ImageFormat.Jpeg);
            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality); // 0-100 (меньше = сильнее сжатие)

            ///image.Save("C:\\Oleg\\git\\NGM\\NGM_Client\\NGM_Client\\test.jpeg", jpegEncoder, encoderParams);
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
        public static Bitmap ResizeImage(Bitmap imgToResize, Size size)
        {
            try
            {
                Bitmap b = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
                }
                return b;
            }
            catch
            {
                Console.WriteLine("Bitmap could not be resized");
                return imgToResize;
            }
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
                res = new Bitmap(Image.FromStream(ms));
            }


            return res;
        }
    }
}
