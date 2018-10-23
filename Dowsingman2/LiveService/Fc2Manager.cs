using Dowsingman2.BaseClass;
using Dowsingman2.MyUtility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Dowsingman2.LiveService
{
    class Fc2Manager : AbstractManager
    {
        private static Fc2Manager instance_ = new Fc2Manager();
        public static Fc2Manager GetInstance()
        {
            return instance_;
        }

        private string url_;

        public Fc2Manager() : base("Fc2", "fc2.xml")
        {
            url_ = "https://live.fc2.com/contents/allchannellist.php";
        }

        public override async Task<List<StreamClass>> DownloadLiveAsync()
        {
            List<StreamClass> result = new List<StreamClass>();

            string json = string.Empty;
            using (WebClient client = new WebClient())
            {
                //エンコード設定（UTF8）
                client.Encoding = System.Text.Encoding.UTF8;

                try
                {
                    json = await client.DownloadStringTaskAsync(url_);
                }
                catch (Exception ex)
                {
                    MyTraceSource.TraceEvent(TraceEventType.Error, ex);
                    return result;
                }
            }

            if (json != string.Empty)
            {
                //jsonをパースしてRootObjectに変換
                var r = JsonConvert.DeserializeObject<Fc2RootObject.RootObject>(json);

                //一覧を収納
                foreach (var channel in r.channel)
                {
                    if (channel.name != string.Empty && channel.name != "匿名")
                        result.Add(new StreamClass(channel.title, "https://live.fc2.com/" + channel.id + '/', channel.name, DateTime.Parse(channel.start)));
                }
            }

            //配信一覧を返す
            return result;
        }
    }
}
