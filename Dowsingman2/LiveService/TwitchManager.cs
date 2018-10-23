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
    class TwitchManager : AbstractManager
    {
        private static TwitchManager instance_ = new TwitchManager();
        public static TwitchManager GetInstance()
        {
            return instance_;
        }

        const int OFFSET = 80;
        const int RATE_LIMIT = 25;

        private string url_;

        private TwitchManager() : base("Twitch", "twitch.xml")
        {
            url_ = "https://api.twitch.tv/kraken/streams/?limit=100&broadcaster_language=ja&client_id=snk7w6raevojktexzkvf2ixy66gxtn&offset=";
        }

        public override async Task<List<StreamClass>> DownloadLiveAsync()
        {
            //戻り値用
            List<StreamClass> result = new List<StreamClass>();

            using (WebClient client = new WebClient())
            {
                //エンコード設定（UTF8）
                client.Encoding = System.Text.Encoding.UTF8;

                string json;
                for (int i = 0; i < RATE_LIMIT; i++)
                {
                    json = string.Empty;

                    try
                    {
                        //参考URL
                        //https://stackoverflow.com/questions/45622188/get-value-from-twitch-api
                        //日本語の配信を100配信ずつ（仕様上最大）取得
                        string url = url_ + i * OFFSET;
                        json = await client.DownloadStringTaskAsync(url);
                    }
                    catch (Exception ex)
                    {
                        MyTraceSource.TraceEvent(TraceEventType.Error, ex);
                        json = null;
                    }

                    if (json != null && json != string.Empty)
                    {
                        //jsonをパースしてRootObjectに変換
                        var r = JsonConvert.DeserializeObject<TwitchRootObject.RootObject>(json);

                        //一覧を収納
                        foreach (var s in r.streams)
                        {
                            if (!result.Exists(item => item.Owner == s.channel.name))
                                result.Add(new StreamClass(s.channel.status, "https://www.twitch.tv/" + s.channel.name + '/', s.channel.name, s.created_at.ToLocalTime()));
                        }

                        //配信がなくなったらループを抜ける
                        if (r.streams.Count < OFFSET) break;
                    }
                }
            }

            //配信一覧を返す
            return result;
        }
    }
}
