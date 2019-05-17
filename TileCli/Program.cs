using System;
using System.Net;

namespace TileCli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // url levels servers
            //0-z 1-y 2-x
            //3-s
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //string url, levels, servers = "";
            string url = "https://services.arcgisonline.com/arcgis/rest/services/World_Imagery/MapServer/tile/{0}/{1}/{2}";
            try
            {
                url = args[0];
            }
            catch (Exception)
            {
            }
            string levels = "2";
            try
            {
                levels = args[1];
            }
            catch (Exception)
            {
            }
            string server = "";
            try
            {
                server = args[2];
            }
            catch (Exception)
            {
            }
            //var url = args[0];
            //var levels = args[1];
            //var servers = args[2];
            //var kdsf = string.Format("dsafadsfs{1}{2}a", "x", "y", "z");

            var toa = new TaskOfAll();
            toa.GenerateTasks(url, levels, server);

            toa.AllTasksGeneratedEvent += (a) =>
            {
                Console.WriteLine("ALL GENERATED");
                toa.AllTasksFinishedEvent += (b) =>
                {
                    Console.WriteLine("ALL DOWNLOADED");
                };
                toa.DownTasks();
            };
            Console.ReadLine();
        }
    }

    public delegate void AllTasksGeneratedEventHandler(object sender);

    public delegate void AllTasksFinishedEventHandler(object sender);

    public delegate void TasksFininshedIncreasedBy(object sender, int num, long size);
    public delegate void TasksGeneratedIncreasedBy(object sender, int num);
    public delegate void TasksDownloadedSizeIncreasedBy(object sender, int num);

    public delegate void SingleTaskFinished(object sender, long size);
    public interface TaskOfMany
    {
        string UrlTemplate { get; set; }

        string DirPath { get; set; }

        void GenerateTasks();

        event AllTasksGeneratedEventHandler AllTasksGeneratedEvent;
        event AllTasksFinishedEventHandler AllTasksFinishedEvent;

        //AllTasksGeneratedEventHandler AllTasksGeneratedEvent { get; set; }
    }
    public static class StringUtils
    {
        public static string XyzTo012(string xyzString)
        {
            var temp = xyzString;
            temp = temp.Replace(@"{x}", @"{0}");
            temp = temp.Replace(@"{y}", @"{1}");
            temp = temp.Replace(@"{z}", @"{2}");
            temp = temp.Replace(@"{s}", @"{3}");
            return temp;
        }
        public static string SizeToReadable(long size)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = size;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            string result = String.Format("{0:0.##} {1}", len, sizes[order]);
            return result;
        }

    }

}