using System;

namespace Dowsingman2.BaseClass
{
    public class StreamClass
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Owner { get; set; }
        public int Listener { get; set; }
        public DateTime? Start_Time { get; set; }
        public Boolean StreamStatus { get; set; }

        public StreamClass()
        {
            Title = string.Empty;
            Url = string.Empty;
            Owner = string.Empty;
            Listener = -1;
            Start_Time = null;
            StreamStatus = false;
        }
        public StreamClass(string owner)
        {
            Title = string.Empty;
            Url = string.Empty;
            Owner = owner;
            Listener = -1;
            Start_Time = null;
            StreamStatus = false;
        }

        public StreamClass(string title, string url, string owner, DateTime? start_time)
        {
            Title = title;
            Url = url;
            Owner = owner;
            Listener = -1;
            Start_Time = start_time;
            StreamStatus = false;
        }

        public StreamClass(string title, string url, string owner, int listener, DateTime? start_time)
        {
            Title = title;
            Url = url;
            Owner = owner;
            Listener = listener;
            Start_Time = start_time;
            StreamStatus = true;
        }
    }
}