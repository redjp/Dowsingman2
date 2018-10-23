using System;

namespace Dowsingman2.BaseClass
{
    public class StreamClass
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Owner { get; set; }
        public DateTime? Start_Time { get; set; }
        public Boolean StreamStatus { get; set; }

        public StreamClass()
        {
            this.Title = string.Empty;
            this.Url = string.Empty;
            this.Owner = string.Empty;
            this.Start_Time = null;
            this.StreamStatus = false;
        }
        public StreamClass(string owner)
        {
            this.Title = string.Empty;
            this.Url = string.Empty;
            this.Owner = owner;
            this.Start_Time = null;
            this.StreamStatus = false;
        }

        public StreamClass(string title, string url, string owner, DateTime? start_time)
        {
            this.Title = title;
            this.Url = url;
            this.Owner = owner;
            this.Start_Time = start_time;
            this.StreamStatus = true;
        }
    }
}