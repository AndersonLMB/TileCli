using System.Collections.Generic;
using System.IO;

namespace TileCli
{
    public class TaskOfAll
    {
        public List<TaskOfZ> TaskOfZs { get; set; } = new List<TaskOfZ>();

        public void GenerateTasks(string urlTemplate, string levels, string servers)
        {
            var split = levels.Split(',');
            var levelsShouldBeDone = split.Length;
            var levelsShouldBeGenerated = split.Length;
            foreach (var level in split)
            {
                var lev = int.Parse(level);
                var taskOfZ_0 = new TaskOfZ()
                {
                    UrlTemplate = string.Format(urlTemplate, lev.ToString(), "{1}", "{2}"),
                    Z = lev
                };
                lock (this.TaskOfZs)
                {
                    TaskOfZs.Add(taskOfZ_0);
                }

                taskOfZ_0.AllTasksGeneratedEvent += (sender) =>
                {
                    var tz = (TaskOfZ)sender;
                    //Directory.CreateDirectory($"{tz.Z}");
                    if (!Directory.Exists($"{tz.Z}"))
                    {
                        tz.DirPath = Directory.CreateDirectory($"{tz.Z}").FullName;
                    }
                    tz.DirPath = new DirectoryInfo($"{tz.Z}").FullName;

                    levelsShouldBeGenerated--;
                    if (levelsShouldBeGenerated == 0)
                    {
                        this.AllTasksGeneratedEvent?.Invoke(this);
                    }

                    #region COMMENT IF YOU DO NOT WANT DOWNLOAD
                    //tz.AllTasksFinishedEvent += (a) =>
                    //{
                    //    var taskofz = (TaskOfZ)a;
                    //    //Console.WriteLine($"Z: {taskofz}")
                    //    ;
                    //};
                    //tz.DownTasks();
                    #endregion
                };
                taskOfZ_0.GenerateTasks();

            }
        }
        public void DownTasks()
        {
            var levelsShouldBeDone = TaskOfZs.Count;
            foreach (var tz in TaskOfZs)
            {
                tz.AllTasksFinishedEvent += (a) =>
                {
                    var taskofz = (TaskOfZ)a;
                    //Console.WriteLine($"Z: {taskofz}")

                    levelsShouldBeDone--;
                    if (levelsShouldBeDone == 0)
                    {
                        AllTasksFinishedEvent?.Invoke(this);
                    }
                };
                tz.DownTasks();
            }
        }
        public void GenerateTasksAndDownload(string urlTemplate, string levels, string servers)
        {
            var split = levels.Split(',');
            var levelsShouldBeDone = split.Length;
            var levelsShouldBeGenerated = split.Length;
            foreach (var level in split)
            {
                var lev = int.Parse(level);
                var taskOfZ_0 = new TaskOfZ()
                {
                    UrlTemplate = string.Format(urlTemplate, lev.ToString(), "{1}", "{2}"),
                    Z = lev
                };
                lock (this.TaskOfZs)
                {
                    TaskOfZs.Add(taskOfZ_0);
                }

                taskOfZ_0.TasksGeneratedIncreasedByEvent += (s, e) =>
                {
                    TasksGeneratedIncreasedByEvent?.Invoke(this, e);
                };


                //Directory.CreateDirectory($"{tz.Z}");
                if (!Directory.Exists($"{taskOfZ_0.Z}"))
                {
                    taskOfZ_0.DirPath = Directory.CreateDirectory($"{taskOfZ_0.Z}").FullName;
                }
                taskOfZ_0.DirPath = new DirectoryInfo($"{taskOfZ_0.Z}").FullName;


                taskOfZ_0.TasksFininshedIncreasedByEvent += (s, e, size) =>
                {
                    this.TasksFininshedIncreasedByEvent?.Invoke(this, e, size);
                };
                taskOfZ_0.GenerateTasksAndDownload();

            }

        }
        public event AllTasksFinishedEventHandler AllTasksGeneratedEvent;
        public event AllTasksFinishedEventHandler AllTasksFinishedEvent;
        public event TasksFininshedIncreasedBy TasksFininshedIncreasedByEvent;
        public event TasksGeneratedIncreasedBy TasksGeneratedIncreasedByEvent;
    }
}