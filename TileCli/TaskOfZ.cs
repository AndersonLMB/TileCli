using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TileCli
{
    public class TaskOfZ : TaskOfMany
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
                            Console.WriteLine($"FINISHED {Z}");
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
}