
using Aspose.Imaging.ImageFilters.FilterOptions;
using FFMpegCore.Pipes;
using FFMpegCore;
using SixLabors.ImageSharp.Formats.Webp;

using SkiaSharp;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;





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
            //CompressAndReadStream();//Encode2();

            ///image = ByteToImage(res);

            //res = GetMaxCompressedWebP("C:\\Users\\okhobot\\Downloads\\ege\\00038.png");
            ///Console.WriteLine(res.Length);

            ///string picturesDirectory = "C:\\Oleg\\git\\NGM\\NGM_Client\\NGM_Client\\";
            ///image.Save(picturesDirectory + "test2.png", ImageFormat.Jpeg);

            //image.Save(picturesDirectory+"test.png", ImageFormat.Png);
            //Console.WriteLine("saved");


            return res;
        }




        public static void CompressAndReadStream()
        {
            // Путь к ffmpeg.exe
            string ffmpegPath = "ffmpeg.exe";

            // Настройки процесса FFmpeg
            var startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = "-i pipe:0 -c:v libx264 -preset fast -f matroska pipe:1",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // Запуск процесса FFmpeg
            using (var ffmpegProcess = new Process { StartInfo = startInfo })
            {
                ffmpegProcess.Start();

                // Поток для записи входных данных в FFmpeg
                var inputStream = ffmpegProcess.StandardInput.BaseStream;

                // Поток для чтения сжатых выходных данных из FFmpeg
                var outputStream = ffmpegProcess.StandardOutput.BaseStream;

                // Поток для чтения ошибок (может быть полезен для отладки)
                var errorStream = ffmpegProcess.StandardError.BaseStream;

                // Пример: асинхронное чтение ошибок
                var errorTask = ffmpegProcess.StandardError.ReadToEndAsync();

                // Пример обработки входных и выходных данных
                // 1. Запись входных данных (например, из другого потока)
                byte[] inputData = GetInputData(); // Ваш метод получения данных
                inputStream.Write(inputData, 0, inputData.Length);
                inputStream.Close();

                // 2. Чтение сжатых выходных данных
                using (var memoryStream = new MemoryStream())
                {
                    outputStream.CopyTo(memoryStream);
                    byte[] compressedData = memoryStream.ToArray();

                    // Далее можно работать с compressedData
                    Console.WriteLine($"Получено {compressedData.Length} байт сжатых данных");
                }

                // Ожидание завершения FFmpeg
                ffmpegProcess.WaitForExit();

                // Проверка кода выхода
                if (ffmpegProcess.ExitCode != 0)
                {
                    Console.WriteLine($"FFmpeg завершился с ошибкой: {errorTask.Result}");
                }
            }
        }

        private static byte[] GetInputData()
        {
            // Здесь должен быть ваш код для получения входных данных
            // Например, чтение из файла или другого потока
            return File.ReadAllBytes("i2.mp4");
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
                    Arguments = "-f gdigrab -framerate 60 -i desktop -pix_fmt yuv420p -f segment -segment_time 1 -reset_timestamps 1 -vcodec libx264 -crf 28 -g 60 pipe:1",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            ffmpegProcess.Start();

            byte[] data=File.ReadAllBytes("tmp.mp4");

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

                if (watch.Elapsed.TotalSeconds >= 1)
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
                    Arguments = "-i pipe:0 -c:v libx264 -preset fast -f matroska pipe:1",//$"-i input.mp4 -vcodec libx264 -crf 28 -f mp4 pipe:1",//pipe:0
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            
            ffmpegProcess.Start();

            
            using (var inputFstream = File.OpenRead("input.mp4"))
            {
                byte[] data=new byte[inputFstream.Length];
                inputFstream.Read(data,0, data.Length);

                ffmpegProcess.StandardInput.BaseStream.Write(data, 0, data.Length);
                //Console.OpenStandardOutput().Write(data, 0, data.Length);
                ffmpegProcess.StandardInput.BaseStream.Close();
            }


            // Пример: записываем выходной поток (можно заменить на любой Stream)
            using (var memoryStream = new MemoryStream())
            {
                ffmpegProcess.StandardOutput.BaseStream.CopyTo(memoryStream);
                byte[] data = memoryStream.ToArray();
                File.WriteAllBytes("out.mp4", data);
                // Далее можно работать с compressedData
                Console.WriteLine($"Получено {data.Length} байт сжатых данных");
            }
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
