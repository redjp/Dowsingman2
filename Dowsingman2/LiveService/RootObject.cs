using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchRootObject
{
    public class Preview
    {
        public string small { get; set; }
        public string medium { get; set; }
        public string large { get; set; }
        public string template { get; set; }
    }

    public class Links
    {
        public string self { get; set; }
        public string follows { get; set; }
        public string commercial { get; set; }
        public string stream_key { get; set; }
        public string chat { get; set; }
        public string features { get; set; }
        public string subscriptions { get; set; }
        public string editors { get; set; }
        public string teams { get; set; }
        public string videos { get; set; }
    }

    public class Channel
    {
        public bool mature { get; set; }
        public bool partner { get; set; }
        public string status { get; set; }
        public string broadcaster_language { get; set; }
        public string display_name { get; set; }
        public string game { get; set; }
        public string language { get; set; }
        public int _id { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public object delay { get; set; }
        public string logo { get; set; }
        public object banner { get; set; }
        public string video_banner { get; set; }
        public object background { get; set; }
        public string profile_banner { get; set; }
        public string profile_banner_background_color { get; set; }
        public string url { get; set; }
        public int views { get; set; }
        public int followers { get; set; }
        public Links _links { get; set; }
    }

    public class Links2
    {
        public string self { get; set; }
    }

    public class Stream
    {
        public object _id { get; set; }
        public string game { get; set; }
        public int viewers { get; set; }
        public int video_height { get; set; }
        public double average_fps { get; set; }
        public int delay { get; set; }
        public DateTime created_at { get; set; }
        public bool is_playlist { get; set; }
        public string stream_type { get; set; }
        public Preview preview { get; set; }
        public Channel channel { get; set; }
        public Links2 _links { get; set; }
    }

    public class Links3
    {
        public string self { get; set; }
        public string next { get; set; }
        public string featured { get; set; }
        public string summary { get; set; }
        public string followed { get; set; }
    }

    public class RootObject
    {
        public int _total { get; set; }
        public List<Stream> streams { get; set; }
        public Links3 _links { get; set; }
    }
}

namespace Fc2RootObject
{
    public class Channel
    {
        public string id { get; set; }
        public int fc2id { get; set; }
        public string bid { get; set; }
        public int video { get; set; }
        public int app { get; set; }
        public int category { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public int type { get; set; }
        public string image { get; set; }
        public string start { get; set; }
        public object start_time { get; set; }
        public string sex { get; set; }
        public int pay { get; set; }
        public int interval { get; set; }
        public int amount { get; set; }
        public string lang { get; set; }
        public int total { get; set; }
        public int count { get; set; }
        public int login { get; set; }
        public int comment_l { get; set; }
        public int tid { get; set; }
        public int price { get; set; }
        public int official { get; set; }
        public int comment_score { get; set; }
    }

    public class RootObject
    {
        public string link { get; set; }
        public int is_adult { get; set; }
        public int time { get; set; }
        public List<Channel> channel { get; set; }
    }
}
