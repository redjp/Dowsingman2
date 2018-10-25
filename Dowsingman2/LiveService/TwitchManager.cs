using Dowsingman2.BaseClass;
using Dowsingman2.UtilityClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

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
        private Dictionary<string, string> param_;

        private TwitchManager() : base("Twitch", "twitch.xml")
        {
            url_ = "https://api.twitch.tv/kraken/streams/";
            param_ = new Dictionary<string, string>()
            {
                { "limit", "100" },
                { "broadcaster_language", "ja" },
                { "client_id", "snk7w6raevojktexzkvf2ixy66gxtn" },
                { "offset", string.Empty }
            };
        }

        public override async Task<List<StreamClass>> DownloadLiveAsync()
        {
            //戻り値用
            List<StreamClass> result = new List<StreamClass>();

            for (int i = 0; i < RATE_LIMIT; i++)
            {
                //参考URL
                //https://stackoverflow.com/questions/45622188/get-value-from-twitch-api
                //日本語の配信を100配信ずつ（仕様上最大）取得
                param_["offset"] = (i * OFFSET).ToString();
                string json = MyUtility.RemoveSpecialChars(await new MyHttpClient().GetStringAsync(url_, param_));
                using (XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(json), XmlDictionaryReaderQuotas.Max))
                {
                    XDocument xDocument = XDocument.Load(reader);

                    result.AddRange((from item in xDocument.Root.Element("streams").Elements("item")
                                    let channel = item.Element("channel")
                                    let owner = channel.Element("name").Value
                                    let title = MyUtility.RemoveCRLF(channel.Element("status").Value)
                                    let start_time = FormatDate(item.Element("created_at").Value)
                                    let url = "https://www.twitch.tv/" + channel.Element("name") + '/'
                                    let listener = item.Element("viewers").Value
                                    where !result.Exists(x => x.Owner == owner)
                                    select new StreamClass(title, url, owner, start_time)).ToList());
                    
                    ////配信がなくなったらループを抜ける
                    if (xDocument.Root.Element("streams").Elements("item").Count() < OFFSET) break;
                }
            }

            return result;
        }

        public DateTime? FormatDate(string dateString)
        {
            if (dateString == "")
            {
                return null;
            }
            DateTime dateTime = DateTime.ParseExact(dateString, "yyyy-MM-ddTHH:mm:ssZ", null);
            dateTime = dateTime.ToLocalTime();
            return dateTime;
        }
    }
}
