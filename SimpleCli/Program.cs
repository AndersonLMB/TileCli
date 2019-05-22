using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleCli
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var minlevel = int.Parse(args[0]);
            var maxlevel = int.Parse(args[1]);
            var urlTemplate = args[2];
            var inputAttempt = int.Parse(args[3]);
            Console.WriteLine(int.MaxValue);
            Console.WriteLine((long)Math.Pow((long)Math.Pow(2, 12), 2));

            for (int z = minlevel; z <= maxlevel; z++)
            {
                var width = (long)Math.Pow(2, z);
                var totalCount = (long)Math.Pow(width, 2);
                var currentZ = z;

                if (Directory.Exists(currentZ.ToString()) == false)
                {
                    Directory.CreateDirectory(currentZ.ToString());
                }

                Task.Factory.StartNew(() =>
                {
                    for (int y = 0; y < width; y++)
                    {
                        var currentY = y;
                        if (Directory.Exists($"{currentZ}\\{currentY}") == false)
                        {
                            Directory.CreateDirectory($"{currentZ}\\{currentY}");
                        }


                        Task.Factory.StartNew(() =>
                        {
                            for (int x = 0; x < width; x++)
                            {
                                var currentX = x;
                                if (File.Exists($"{currentZ}\\{currentY}\\{currentX}.png") == true)
                                {
                                    File.Delete($"{currentZ}\\{currentY}\\{currentX}.png");
                                }

                                Task.Factory.StartNew(() =>
                                {
                                    var attempt = inputAttempt;
                                    var wc = new WebClient();
                                    wc.DownloadFileCompleted += (s, e) =>
                                    {
                                        if (e.Error != null)
                                        {
                                            if (attempt != 0)
                                            {
                                                attempt--;
                                                totalCount--;
                                                wc.DownloadFileAsync(new Uri(string.Format(urlTemplate, currentZ, currentY, currentX)), $"{currentZ}\\{currentY}\\{currentX}.png");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"{string.Format(urlTemplate, currentZ, currentY, currentX)}");
                                            totalCount--;
                                            if (totalCount == 0)
                                            {
                                                MessageBox.Show($"level {currentZ} downloaded");
                                            }
                                        }
                                    };

                                    wc.DownloadFileAsync(new Uri(string.Format(urlTemplate, currentZ, currentY, currentX)), $"{currentZ}\\{currentY}\\{currentX}.png");




                                });

                            }


                        });




                    }

                });


            }
            Console.ReadLine();
        }
    }
}
