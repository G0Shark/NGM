using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System;

using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
//using SixLabors.ImageSharp;
//using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
namespace NGM_Client
{
    class ScreenCatcher
    {
        Rectangle screen;
        int x;
        int y;
        int w;
        int h;


        WebpEncoder webpEncoder;

        public ScreenCatcher()
        {
            screen = System.Windows.Forms.SystemInformation.VirtualScreen;
            x = screen.Left;
            y = screen.Top;
            w = screen.Width;
            h = screen.Height ;

            webpEncoder = new WebpEncoder
            {
                Quality = 10,                     // Минимальное качество
                Method = WebpEncodingMethod.Level6, // Максимальное сжатие
                UseAlphaCompression = false,       // Отключить, если нет прозрачности
                SkipMetadata = true,               // Удалить EXIF, комментарии и т.д.
                EntropyPasses = 5,                // Максимальное количество проходов
                SpatialNoiseShaping = 100,        // Агрессивное подавление шума
                FilterStrength = 100              // Максимальная фильтрация
            };

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

            byte[] res= FastWebPCompress(PreprocessImage(ConvertBitmapToImageSharp(image),1920 / 2,1080 / 2));
            //res = GetMaxCompressedWebP("C:\\Users\\okhobot\\Downloads\\ege\\00038.png");
            Console.WriteLine(res.Length);

            ///string picturesDirectory = "C:\\Oleg\\git\\NGM\\NGM_Client\\NGM_Client\\";
            ///image.Save(picturesDirectory + "test.png", System.Drawing.Imaging.ImageFormat.Png);

            //image.Save(picturesDirectory+"test.png", ImageFormat.Png);
            //Console.WriteLine("saved");


            return res;
        }
        public byte[] GetMaxCompressedWebP(string imagePath)
        {
            using var image = SixLabors.ImageSharp.Image.Load(imagePath);
            var processed = PreprocessImage(image,1920,1080);
            return FastWebPCompress(processed);
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


        public SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> ConvertBitmapToImageSharp(Bitmap bitmap)
        {
            using var memoryStream = new MemoryStream();

            // Сохраняем Bitmap в поток как PNG
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Загружаем из потока в ImageSharp
            return SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(memoryStream);
        }

        /*
        public byte[] AdaptiveCompress(SixLabors.ImageSharp.Image image, int targetSizeKB)
        {
            int quality = 90;
            byte[] result;

            do
            {
                result = FastWebPCompress(image, quality);
                quality -= 5;
            }
            while (result.Length > targetSizeKB * 1024 && quality > 10);

            return result;
        }
        */

        

        public byte[] FastWebPCompress(SixLabors.ImageSharp.Image image)
        {
            using var ms = new MemoryStream();
            image.Save(ms, webpEncoder); // Переиспользуем encoder
            return ms.ToArray();
        }
        public SixLabors.ImageSharp.Image PreprocessImage(SixLabors.ImageSharp.Image image, int maxWidth, int maxHeight)
        {

            // Автоматическое уменьшение разрешения
            if (image.Width > maxWidth || image.Height > maxHeight)
            {
                var options = new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new SixLabors.ImageSharp.Size(maxWidth, maxHeight)
                };

                return image.Clone(x => x.Resize(options));
            }

            return image;
        }

    }
}
