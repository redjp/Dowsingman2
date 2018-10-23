using Dowsingman2.BaseClass;
using Dowsingman2.MyUtility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dowsingman2.LiveService
{
    class KukuluManager : AbstractManager
    {
        private static KukuluManager instance_ = new KukuluManager();
        public static KukuluManager GetInstance()
        {
            return instance_;
        }

        private TimeZoneInfo timeZoneInfo_;
        private Regex specialCharsRegex_;
        private string url_;

        private KukuluManager() : base("Kukulu", "kukulu.xml")
        {
            timeZoneInfo_ = TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time");
            specialCharsRegex_ = new Regex("[\\00-\\x08\\x0b\\x0c\\x0e-\\x1f\\x7f]", RegexOptions.Compiled);
            url_ = "http://dwsrod.kuku.lu/xml/stream/popular/search/all?limit=100";
        }

        public override async Task<List<StreamClass>> DownloadLiveAsync()
        {
            var result = new List<StreamClass>();

            string kukulu_str = string.Empty;
            List<string[]> list = null;

            using (WebClient client = new WebClient())
            {
                try
                {
                    //エンコード設定（UTF8）
                    client.Encoding = System.Text.Encoding.UTF8;

                    //kukuluLiveから配信一覧を取得
                    kukulu_str = await client.DownloadStringTaskAsync(url_);
                }
                catch (Exception ex)
                {
                    MyTraceSource.TraceEvent(TraceEventType.Error, ex);
                    return result;
                }
            }

            XDocument xDocument = XDocument.Parse(RemoveSpecialChars(kukulu_str));

            list = (from array in xDocument.Root.Element("results").Elements("array")
                    let user = array.Element("userName").Value
                    let title = array.Element("title").Value
                    let date = array.Element("streamStartedAt").Value
                    select new string[4]
                    {
                title,
                user,
                date,
                array.Element("url").Value,
                    }).ToList();

            //一覧を収納
            for (int i = 0; i < list.Count; i++)
            {
                result.Add(new StreamClass(list[i][0], list[i][3], list[i][1], FormatDate(list[i][2])));
            }

            //配信一覧を返す
            return result;
        }

        public string RemoveSpecialChars(string value)
        {
            return specialCharsRegex_.Replace(value, "");
        }

        public DateTime? FormatDate(string dateString)
        {
            if (dateString == "")
            {
                return null;
            }
            DateTime dateTime = DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm:ss", null);
            dateTime = ((!timeZoneInfo_.IsDaylightSavingTime(dateTime)) ? dateTime.AddHours(17.0) : dateTime.AddHours(16.0));
            return dateTime;
        }
    }
}
