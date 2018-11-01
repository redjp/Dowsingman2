using Dowsingman2.BaseClass;
using Dowsingman2.UtilityClass;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Dowsingman2.LiveService
{
    class Fc2Manager : AbstractManager
    {
        private static Fc2Manager instance_ = new Fc2Manager();
        public static Fc2Manager GetInstance() { return instance_; }

        private string url_;
        private string dateFormat_;
        private bool isUniversal_;

        public Fc2Manager() : base("Fc2", "fc2.xml")
        {
            url_ = "https://live.fc2.com/contents/allchannellist.php";
            dateFormat_ = "yyyy-MM-dd HH:mm:ss";
            isUniversal_ = false;
        }

        public override async Task<List<StreamClass>> DownloadLiveAsync()
        {
            var result = new List<StreamClass>();

            string json = MyUtility.RemoveSpecialChars(await new MyHttpClient().GetStringAsync(url_, null));
            using (XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(json), XmlDictionaryReaderQuotas.Max))
            {
                XDocument xDocument = XDocument.Load(reader);

                result.AddRange((from item in xDocument.Root.Element("channel").Elements("item")
                                 let owner = item.Element("name").Value
                                 let title = item.Element("title").Value
                                 let start_time = MyUtility.FormatDate(item.Element("start").Value, dateFormat_, isUniversal_, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None)
                                 let url = "http://live.fc2.com/" + item.Element("id").Value + '/'
                                 let listener = item.Element("count").Value
                                 where owner.Trim() != string.Empty && owner != "匿名"
                                 select new StreamClass(title, url, owner, start_time)).ToList());
            }

            return result;
        }
    }
}
