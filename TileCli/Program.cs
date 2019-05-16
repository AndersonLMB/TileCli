using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

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
            string levels = "3";
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
            var kdsf = string.Format("dsafadsfs{1}{2}a", "x", "y", "z");

            foreach (var level in levels.Split(','))
            {
                var lev = int.Parse(level);
                var taskOfZ_0 = new TaskOfZ()
                {
                    UrlTemplate = string.Format(url, lev.ToString(), "{1}", "{2}"),
                    Z = lev
                };
                taskOfZ_0.GenerateTasks();
                taskOfZ_0.AllTasksGeneratedEvent += (sender) =>
                {
                    var tz = (TaskOfZ)sender;
                    //Directory.CreateDirectory($"{tz.Z}");
                    if (!Directory.Exists($"{tz.Z}"))
                    {
                        tz.DirPath = Directory.CreateDirectory($"{tz.Z}").FullName;
                    }
                    tz.DirPath = new DirectoryInfo($"{tz.Z}").FullName;

                    tz.AllTasksFinishedEvent += (a) =>
                    {

                        var taskofz = (TaskOfZ)a;

                        //Console.WriteLine($"Z: {taskofz}")
                        ;
                    };
                    tz.DownTasks();


                };
                ;
            }




            Console.ReadLine();
        }
    }

    internal class TaskOfZ : TaskOfMany
    {
        public string UrlTemplate { get; set; }
        public int Z { get; set; }

        public List<TaskOfY> taskOfYs { get; set; } = new List<TaskOfY>();
        public string DirPath { get; set; }

        public event AllTasksGeneratedEventHandler AllTasksGeneratedEvent;

        public void GenerateTasks()
        {
            var countOofTaskYShouldBeGenerated = (int)Math.Round(Math.Pow(2, Z));

            var countOfFullYTask = 0;
            for (int y = 0; y < countOofTaskYShouldBeGenerated; y++)
            {
                var currentY = y;
                lock (taskOfYs)
                {
                    Task.Factory.StartNew(() =>
                    {
                        var taskofY = new TaskOfY()
                        {
                            UrlTemplate = string.Format(UrlTemplate, "", currentY.ToString(), "{2}"),
                            Y = currentY,
                            Z = Z
                        };
                        taskofY.AllTasksGeneratedEvent += (sender) =>
                        {
                            countOfFullYTask++;
                            if (countOfFullYTask == countOofTaskYShouldBeGenerated)
                            {
                                AllTasksGeneratedEvent?.Invoke(this);
                            }
                        };
                        taskofY.GenerateTasks();
                        //Console.WriteLine(taskofY.Y);
                        //Console.WriteLine(taskofY.UrlTemplate);
                        taskOfYs.Add(taskofY);
                        ////该Z层的所有Y均完成创建
                        //if (taskOfYs.Count == countOofTaskYShouldBeGenerated)
                        //{
                        //    AllTasksGeneratedEvent?.Invoke(this);
                        //    //if (this.AllTasksGeneratedEvent != null)
                        //    //{
                        //    //    //this.AllTasksGeneratedEvent
                        //    //}
                        //}
                    });
                }
            }
            //throw new NotImplementedException();
        }

        public void DownTasks()
        {
            var taskCOuntOfThisTaskZShouldBeDone = taskOfYs.Count;
            taskOfYs.ForEach((ty) =>
            {
                Task.Factory.StartNew(() =>
                {
                    if (!Directory.Exists(Path.Combine(DirPath, $"{ty.Y}")))
                    {
                        ty.DirPath = Directory.CreateDirectory(Path.Combine(DirPath, $"{ty.Y}")).FullName;
                    }
                    ty.DirPath = new DirectoryInfo(Path.Combine(DirPath, $"{ty.Y}")).FullName;
                    ty.AllTasksFinishedEvent += (a) =>
                    {
                        taskCOuntOfThisTaskZShouldBeDone--;
                        if (taskCOuntOfThisTaskZShouldBeDone == 0)
                        {
                            AllTasksFinishedEvent?.Invoke(this);
                        }
                    };
                    ty.DownTasks();
                });


            })

            ;

        }

        public event AllTasksFinishedEventHandler AllTasksFinishedEvent;
    }

    public delegate void AllTasksGeneratedEventHandler(object sender);

    public delegate void AllTasksFinishedEventHandler(object sender);

    internal class TaskOfY : TaskOfMany
    {
        public string UrlTemplate { get; set; }
        public int Xs { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        public List<TaskOfX> taskOfXes { get; set; } = new List<TaskOfX>();
        public string DirPath { get; set; }

        public event AllTasksGeneratedEventHandler AllTasksGeneratedEvent;

        public void GenerateTasks()
        {
            var countOofTaskXShouldBeGenerated = (int)Math.Round(Math.Pow(2, Z));
            for (int x = 0; x < countOofTaskXShouldBeGenerated; x++)
            {
                var currentX = x;
                lock (taskOfXes)
                {
                    Task.Factory.StartNew(() =>
                    {
                        var taskOfX = new TaskOfX()
                        {
                            Url = string.Format(UrlTemplate, "", "", currentX.ToString()),
                            X = currentX,
                            Y = Y,
                            Z = Z
                        };
                        taskOfXes.Add(taskOfX);

                        //Console.WriteLine( taskOfX.Url);
                        //Console.WriteLine($"{taskOfXes.Count} / {countOofTaskXShouldBeGenerated} {taskOfX.Url}");
                        if (taskOfXes.Count == countOofTaskXShouldBeGenerated)
                        {
                            AllTasksGeneratedEvent?.Invoke(this);
                        }
                    });
                }
            }

            //throw new NotImplementedException();
        }

        public void DownTasks()
        {
            var taskCountOfThisTaskYShouldBeDone = taskOfXes.Count;
            taskOfXes.ForEach((tx) =>
            {
                tx.Filename = Path.Combine(DirPath, $"{tx.X}.png");
                if (File.Exists(tx.Filename)) File.Delete(tx.Filename);

                var wc = new WebClient();
                wc.DownloadFileCompleted += (a, b) =>
                {
                    if (b.Error != null)
                    {
                        //失败
                        wc.DownloadFileAsync(new Uri(tx.Url), tx.Filename);
                    }
                    else
                    {
                        taskCountOfThisTaskYShouldBeDone--;
                        //成功
                        Console.WriteLine($"Done\n{tx.Url}\n{tx.Filename}");
                        if (taskCountOfThisTaskYShouldBeDone == 0)
                        {
                            AllTasksFinishedEvent?.Invoke(this);
                        }
                    }
                };
                wc.DownloadFileAsync(new Uri(tx.Url), tx.Filename);
                //task.ContinueWith((a) => {

                //});

            });

        }
        public event AllTasksFinishedEventHandler AllTasksFinishedEvent;
    }

    public class TaskOfX
    {
        public string Url { get; set; }

        public int Y { get; set; }
        public int Z { get; set; }
        public int X { get; set; }

        public string Filename { get; set; }
    }

    public interface TaskOfMany
    {
        string UrlTemplate { get; set; }

        string DirPath { get; set; }

        void GenerateTasks();

        event AllTasksGeneratedEventHandler AllTasksGeneratedEvent;
        event AllTasksFinishedEventHandler AllTasksFinishedEvent;

        //AllTasksGeneratedEventHandler AllTasksGeneratedEvent { get; set; }
    }
}