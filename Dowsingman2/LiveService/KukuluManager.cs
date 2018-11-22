﻿using Dowsingman2.BaseClass;
using Dowsingman2.Error;
using Dowsingman2.UtilityClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dowsingman2.LiveService
{
    class KukuluManager : AbstractManager
    {
        private static KukuluManager instance_ = new KukuluManager();
        public static KukuluManager GetInstance() { return instance_; }

        private string url_;
        private string dateFormat_;
        private TimeZoneInfo timeZoneInfo_;

        private KukuluManager() : base("Kukulu", "kukulu.xml")
        {
            url_ = "http://dwsrod.kuku.lu/xml/stream/popular/search/all?limit=100";
            dateFormat_ = "yyyy-MM-dd HH:mm:ss";
            timeZoneInfo_ = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
        }

        public override async Task<List<StreamClass>> DownloadLiveAsync()
        {
            var result = new List<StreamClass>();

            try
            {
                XDocument xDocument = XDocument.Parse(MyUtility.RemoveSpecialChars(await new MyHttpClient().GetStringAsync(url_, null)));
                result = (from array in xDocument.Root.Element("results").Elements("array")
                          let owner = array.Element("userName").Value
                          let title = array.Element("title").Value
                          let description = array.Element("description").Value
                          let start_time = MyUtility.FormatDate(array.Element("createdAt").Value, dateFormat_, timeZoneInfo_)
                          let url = array.Element("url").Value
                          let listener = MyUtility.TryParseOrDefault<string, int>(int.TryParse, array.Element("currentNumberOfViewers").Value)
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
