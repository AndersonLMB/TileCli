using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace TileCli
{
    public class TaskOfY : TaskOfMany
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

                Task.Factory.StartNew(() =>
                {
                    tx.Filename = Path.Combine(DirPath, $"{tx.X}.png");

                    tx.Finished += (a, size) =>
                    {
                        taskCountOfThisTaskYShouldBeDone--;
                        if (taskCountOfThisTaskYShouldBeDone == 0)
                        {
                            AllTasksFinishedEvent?.Invoke(this);
                        }

                    };
                    tx.Download();



                    //if (File.Exists(tx.Filename)) File.Delete(tx.Filename);

                    //var wc = new WebClient();
                    //wc.DownloadFileCompleted += (a, b) =>
                    //{
                    //    if (b.Error != null)
                    //    {
                    //        //失败
                    //        wc.DownloadFileAsync(new Uri(tx.Url), tx.Filename);
                    //    }
                    //    else
                    //    {
                    //        taskCountOfThisTaskYShouldBeDone--;

                    //        //成功
                    //        //Console.WriteLine($"Done\n{tx.Url}\n{tx.Filename}");
                    //        if (taskCountOfThisTaskYShouldBeDone == 0)
                    //        {
                    //            Console.WriteLine($"FINISHED {Z}/{Y}");
                    //            AllTasksFinishedEvent?.Invoke(this);
                    //        }
                    //    }
                    //};
                    //wc.DownloadFileAsync(new Uri(tx.Url), tx.Filename);



                });

                //task.ContinueWith((a) => {

                //});

            });

        }

        public void GenerateTasksAndDownload()
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
                            Z = Z,
                            Filename = Path.Combine(DirPath, $"{currentX}.png")
                        };


                        taskOfX.Finished += (sender, size) =>
                        {
                            if (TasksFininshedIncreasedByEvent != null)
                            {
                                TasksFininshedIncreasedByEvent.Invoke(this, 1,size);
                            }
                        };


                        if (TasksGeneratedIncreasedByEvent != null)
                        {
                            TasksGeneratedIncreasedByEvent.Invoke(this, 1);
                        }

                        Task.Factory.StartNew(() =>
                        {
                            taskOfXes.Add(taskOfX);
                        });

                        taskOfX.Download();




                        //Console.WriteLine( taskOfX.Url);
                        //Console.WriteLine($"{taskOfXes.Count} / {countOofTaskXShouldBeGenerated} {taskOfX.Url}");
                        if (taskOfXes.Count == countOofTaskXShouldBeGenerated)
                        {
                            AllTasksGeneratedEvent?.Invoke(this);
                        }

                    });
                }
            }

        }
        public event AllTasksFinishedEventHandler AllTasksFinishedEvent;

        public event TasksFininshedIncreasedBy TasksFininshedIncreasedByEvent;
        public event TasksGeneratedIncreasedBy TasksGeneratedIncreasedByEvent;
        public event TasksDownloadedSizeIncreasedBy SizeIncreaseEvent;
    }
}