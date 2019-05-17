namespace TileCli
{
    public class TaskOfX
    {
        public string Url { get; set; }

        public int Y { get; set; }
        public int Z { get; set; }
        public int X { get; set; }

        public string Filename { get; set; }

        public event AllTasksFinishedEventHandler AllTasksFinishedEvent;


        public void Download()
        {


        }
    }
}