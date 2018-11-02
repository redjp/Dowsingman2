using Dowsingman2.BaseClass;
using Dowsingman2.Error;
using Dowsingman2.UtilityClass;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dowsingman2.LiveService
{
    public class CavetubeManager : AbstractManager
    {
        private static CavetubeManager instance_ = new CavetubeManager();
        public static CavetubeManager GetInstance() { return instance_; }

        private string url_;
        private string dateFormat_;
        private bool isUniversal_;

        private CavetubeManager() : base("Cavetube", "Cavetube.xml")
        {
            url_ = "http://rss.cavelis.net/index_live.xml";
            dateFormat_ = "ddd MMM d HH:mm:ss 'UTC' yyyy";
            isUniversal_ = true;
        }

        public override async Task<List<StreamClass>> DownloadLiveAsync()
        {
            var result = new List<StreamClass>();

            try
            {
                XDocument xDocument = XDocument.Parse(MyUtility.RemoveSpecialChars(await new MyHttpClient().GetStringAsync(url_, null)));
                XNamespace ns = "http://www.w3.org/2005/Atom";
                XNamespace ct = "http://gae.cavelis.net";
                result = (from entry in xDocument.Root.Elements(ns + "entry")
                        let owner = entry.Element(ns + "author").Element(ns + "name").Value
                        let title = entry.Element(ns + "title").Value
                        let description = entry.Element(ns + "summary").Value
                        let start_time = MyUtility.FormatDate(entry.Element(ct + "start_date").Value, dateFormat_, isUniversal_, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal)
                        let url = entry.Element(ns + "id").Value
                        let listener = MyUtility.TryParseOrDefault<string, int>(int.TryParse, entry.Element(ct + "listener").Value)
                        select new StreamClass(title, url, owner, listener, start_time)).ToList();
            }
            catch (HttpClientException innerException)
            {
                throw innerException;
            }
            catch (Exception innerException2)
            {
                throw innerException2;
            }

            return result;
        }
    }
}
