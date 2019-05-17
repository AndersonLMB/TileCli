using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace TileCli
{
    public class TaskOfX
    {
        public string Url { get; set; }

        public int Y { get; set; }
        public int Z { get; set; }
        public int X { get; set; }

        public string Filename { get; set; }

        public event SingleTaskFinished Finished;


        public void Download()
        {
            var attemp = 10;
            Task.Factory.StartNew(() =>
            {

                if (File.Exists(Filename)) File.Delete(Filename);
                var wc = new WebClient();
                wc.DownloadFileCompleted += (a, b) =>
                {
                    //成功
                    if (b.Error == null)
                    {
                        var filesize = new FileInfo(Filename).Length;
                        Finished?.Invoke(this, filesize);
                    }
                    //失败
                    else
                    {
                        attemp--;
                        if (attemp != 0)
                        {
                            wc.DownloadFileAsync(new Uri(Url), Filename);
                        }
                    }
                };

                wc.DownloadFileAsync(new System.Uri(Url), Filename);

            });

        }
    }
}