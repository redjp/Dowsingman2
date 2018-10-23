using Dowsingman2.BaseClass;
using Dowsingman2.MyUtility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace Dowsingman2.LiveService
{
    public class CavetubeManager : AbstractManager
    {
        private static CavetubeManager instance_ = new CavetubeManager();
        public static CavetubeManager GetInstance()
        {
            return instance_;
        }

        private string url_;

        private CavetubeManager() : base("Cavetube", "Cavetube.xml")
        {
            url_ = "http://rss.cavelis.net/index_live.xml";
        }

        public override async Task<List<StreamClass>> DownloadLiveAsync()
        {
            List<StreamClass> result = new List<StreamClass>();

            //参考URL
            //http://www.atmarkit.co.jp/fdotnet/dotnettips/753rssfeed/rssfeed.html
            var feed = new SyndicationFeed();
            try
            {
                //読み込み部分を非同期化
                await Task.Run(() =>
                {
                    using (XmlReader reader = XmlReader.Create(url_))
                    {
                        feed = SyndicationFeed.Load(reader);
                    }
                });
            }
            catch (Exception ex)
            {
                MyTraceSource.TraceEvent(TraceEventType.Error, ex);
                return result;
            }

            //ちゃんと取得出来ているか
            if (feed != null)
            {
                //一覧を収納
                foreach (var item in feed.Items)
                {
                    result.Add(new StreamClass(item.Title.Text, item.Id, item.Authors[0].Name, item.PublishDate.ToLocalTime().DateTime));
                }
            }

            //配信一覧を返す
            return result;
        }
    }
}
