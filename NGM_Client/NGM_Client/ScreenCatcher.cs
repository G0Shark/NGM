
using Aspose.Imaging.ImageFilters.FilterOptions;
using SixLabors.ImageSharp.Formats.Webp;

using SkiaSharp;
using System;
using System.Diagnostics;
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
        bool runStream = false;


        public ScreenCatcher()
        {
            screen = System.Windows.Forms.SystemInformation.VirtualScreen;
            x = screen.Left;
            y = screen.Top;
            imgSize.Width = screen.Width;
            imgSize.Height = screen.Height ;

        }




        public async Task<byte[]> CaptureScreen()
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

            int quality = 0;

            //Console.WriteLine(imgSize * 10 / (int)(Math.Sqrt(200 - quality)));
            //Console.WriteLine(90 - Math.Sqrt(5000 - quality * 50));

            //byte[] res= Encode(ResizeImage(image,imgSize*10/(int)(Math.Sqrt(150 - quality*1.5))), (long)(90-Math.Sqrt(9000 - quality*90)));
            byte[] res = null;
            //Encode2();
            //Encode();
            runStream = true;
            test();
             //await test1();

            ///image = ByteToImage(res);

            //res = GetMaxCompressedWebP("C:\\Users\\okhobot\\Downloads\\ege\\00038.png");
            ///Console.WriteLine(res.Length);

            ///string picturesDirectory = "C:\\Oleg\\git\\NGM\\NGM_Client\\NGM_Client\\";
            ///image.Save(picturesDirectory + "test2.png", ImageFormat.Jpeg);

            //image.Save(picturesDirectory+"test.png", ImageFormat.Png);
            //Console.WriteLine("saved");


            return res;
        }






        public void Encode()
        {
            string inputFile = "input.mp4";
            string outputFile = "output_compressed.mp4";

            // Простейшая команда FFmpeg для сжатия видео
            string ffmpegCommand = $"-i \"{inputFile}\" -vcodec libx264 -crf 28 \"{outputFile}\"";
            Console.WriteLine(ffmpegCommand);
            Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = ffmpegCommand,
                UseShellExecute = false,
                CreateNoWindow = false
            })?.WaitForExit();//*/
            //Process.Start("cmd.exe","ffmpeg "+ ffmpegCommand)?.WaitForExit();
            
            Console.WriteLine("Сжатие завершено!");

        }

        void test()
        {
            var ffmpegProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = "-f gdigrab -framerate 60 -i desktop -pix_fmt yuv420p -f segment -segment_time 1 -reset_timestamps 1 -vcodec libx264 -crf 28 tmp%03d.mp4",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            ffmpegProcess.Start();

            ffmpegProcess.WaitForExit();
        }

        async Task test1()
        {
            var ffmpegProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = "-f gdigrab -framerate 60 -i desktop -pix_fmt yuv420p " +// -vcodec libx264 -crf 28
                           "-f mpegts   pipe:1",//-movflags frag_keyframe+empty_moov
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            ffmpegProcess.Start();
            var outputStream = ffmpegProcess.StandardOutput.BaseStream;
            var buffer = new byte[1024 * 1024];
            var chunkStream = new MemoryStream();
            var watch = System.Diagnostics.Stopwatch.StartNew();

            while (true)
            {
                

                int bytesRead = await outputStream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                await chunkStream.WriteAsync(buffer, 0, bytesRead);

                if (watch.Elapsed.TotalSeconds >= 2)
                {
                    await SaveChunk(chunkStream.ToArray(), "mp4"); // Сохраняем как .ts
                    chunkStream.SetLength(0);
                    watch.Restart();
                }
                ///Console.WriteLine("qqq");
            }
            ffmpegProcess.WaitForExit();
        }

        static async Task SaveChunk(byte[] data, string format)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var filename = $"chunk_{timestamp}.{format}";
            await File.WriteAllBytesAsync(filename, data);
            Console.WriteLine($"Сохранён {filename} ({data.Length} байт)");
        }


        public void Encode2()
        {
            // Запускаем FFmpeg для сжатия потока
            var ffmpegProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = "-f gdigrab -framerate 60 -i desktop -t 00:00:02 -c:v libx264 -pix_fmt yuv420p -crf 28 output.mp4",//pipe:0
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = false
                }
            };

            ffmpegProcess.Start();
            /*
            // Пример: записываем выходной поток (можно заменить на любой Stream)
            using (var outputStream = File.OpenWrite("output_compressed.mp4"))
            {
                ffmpegProcess.StandardOutput.BaseStream.CopyTo(outputStream);
            }*/
            //Thread.Sleep(100);
            //ffmpegProcess.StandardInput.WriteLine("q");
            ffmpegProcess.WaitForExit();
            Console.WriteLine("Сжатие завершено!");
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
