using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TileCli;

namespace TileUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TaskOfALLExt TaskOfAll { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Dispatcher.Invoke(() =>
            {

                GenButton.IsEnabled = false;
                lock (DownButton)
                {
                    DownButton.IsEnabled = false;
                }
            });


            var toa = new TaskOfALLExt();
            this.TaskOfAll = toa;
            toa.GenerateTasks(urlTemplate: UrlTemplate.Text,
                levels: Levels.Text,
                servers: ""
                );
            toa.AllTasksGeneratedEvent += (a) =>
            {

                //DownButton.IsEnabled = true;
                //MessageBox.Show("ALL GENERATED");
                Dispatcher.Invoke(() =>
                {
                    lock (DownButton)
                    {
                        GenButton.IsEnabled = true;
                        DownButton.IsEnabled = true;
                    }
                    var thi = this;
                    //var ccc = TaskOfAll.TaskOfZs.Select(t => (TaskOfZExt)t).ToList();


                    var ttt = TaskOfAll.TaskOfZs.Select((item) =>
                    {
                        return new TaskOfZExt()
                        {
                            UrlTemplate = item.UrlTemplate,
                            Z = item.Z
                        };
                    }).ToList();
                    TileUIListView.ItemsSource = ttt;// TaskOfAll.TaskOfZs/*.Select((c) => { return (TaskOfZExt)c; })*/;
                    TileUIListView.Items.Refresh();
                });

                //TileUIListView.Items.Add()



                //toa.AllTasksFinishedEvent += (b) =>
                //{
                //    Console.WriteLine("ALL DOWNLOADED");
                //};
                //toa.DownTasks();
            };


        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                lock (GenButton)
                {
                    GenButton.IsEnabled = false;
                }

                lock (DownButton)
                {
                    DownButton.IsEnabled = false;
                }
            });
            TaskOfAll.DownTasks();
            TaskOfAll.AllTasksFinishedEvent += (a) =>
            {
                Dispatcher.Invoke(() =>
                {
                    lock (DownButton)
                    {
                        DownButton.IsEnabled = true;
                    }
                    lock (GenButton)
                    {
                        GenButton.IsEnabled = true;
                    }
                });

                MessageBox.Show("Finished");

            };

        }
    }

    public class TaskOfALLExt : TaskOfAll
    {

    }
    public class TaskOfZExt : TaskOfZ
    {


    }

}
