using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// 非同期処理に書き換え＆StaticClassを統合
/// 課題1：配信サイトごとのクラスを1つのコレクションで管理してforeachを使えるようにしたい
/// 課題2：List,All,UpdateListAsync()が完全に同じものなので継承かなにかでまとめたい
/// </summary>
namespace Dowsingman2
{
    /// <summary>
    /// List:登録チャンネルリスト
    /// All:配信一覧
    /// UpdateListAsync():更新メソッド
    /// </summary>
    public static class Kukulu
    {
        #region プロパティ
        /// <summary>
        /// 登録チャンネルリスト
        /// </summary>
        public static List<StreamClass> List { get; set; } = new List<StreamClass>();
        /// <summary>
        /// 配信一覧
        /// </summary>
        public static List<StreamClass> All { get; set; } = new List<StreamClass>();
        /// <summary>
        /// 変更可能ステータス
        /// </summary>
        public static Boolean EnableChange { get; set; } = true;
        #endregion

        private static Regex specialCharsRegex_ = new Regex("[\\00-\\x08\\x0b\\x0c\\x0e-\\x1f\\x7f]", RegexOptions.Compiled);

        /// <summary>
        /// 配信サイトから配信一覧を取得してList<StreamClass>に入れて返す
        /// </summary>
        /// <returns>配信一覧</returns>
        private static async Task<List<StreamClass>> GetAllAsync()
        {
            //戻り値
            var kukuluall = new List<StreamClass>();

            string kukulu_str;
            List<string[]> list = null;

            using (WebClient client = new WebClient()) {
                try
                {
                    //エンコード設定（UTF8）
                    client.Encoding = System.Text.Encoding.UTF8;

                    //kukuluLiveから配信一覧を取得
                    string url = @"http://dwsrod.kuku.lu/xml/stream/popular/search/all?limit=100";
                    kukulu_str = await client.DownloadStringTaskAsync(url);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return kukuluall;
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
                kukuluall.Add(new StreamClass(list[i][0], list[i][3], list[i][1], FormatDate(list[i][2])));
            }

            //配信一覧を返す
            return kukuluall;
        }

        /// <summary>
        /// GetAllAsync()を呼び出してList,Allを最新の状態に更新
        /// 追加の配信通知スタックをList<StreamClass>に入れて返す
        /// </summary>
        /// <returns>配信通知スタック</returns>
        public static async Task<List<StreamClass>> UpdateListAsync()
        #region
        {
            //戻り値
            List<StreamClass> stackStreamNote = new List<StreamClass>();

            //取得した配信一覧を入れる変数
            var streamAll = new List<StreamClass>();
            try
            {
                //配信一覧を取得（別スレッドで実行）
                streamAll = await GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<StreamClass>();
            }

            //ちゃんと取得できていれば
            if (streamAll.Count > 0)
            {
                //List,Allの変更不可
                EnableChange = false;
                try
                {
                    if (List.Count > 0)
                    {
                        //現在の登録チャンネルリストを取得
                        List<StreamClass> streamList = new List<StreamClass>(List);
                        var statusList = streamList.Select(x => x.StreamStatus).ToList();
                        //配信情報を初期化
                        streamList = streamList.Select(x => new StreamClass(x.Owner)).ToList();

                        foreach (StreamClass st in streamAll)
                        {
                            //一致する配信者名があった場合indexを取得
                            int index = streamList.FindIndex(item => item.Owner == st.Owner);
                            if (index >= 0)
                            {
                                //配信情報を上書き
                                streamList[index] = st;

                                //新しく開始した配信なら通知スタックに追加
                                if (!statusList[index])
                                {
                                    stackStreamNote.Add(st);
                                    statusList[index] = true;
                                }
                            }
                        }

                        List = streamList;
                    }

                    All = streamAll;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new List<StreamClass>();
                }
                finally
                {
                    //List,Allの変更可
                    EnableChange = true;
                }
            }

            //追加の通知スタックを返す
            return stackStreamNote;
        }
        #endregion


        public static string RemoveSpecialChars(string value)
        {
            return specialCharsRegex_.Replace(value, "");
        }

        public static DateTime? FormatDate(string dateString)
        {
            if (dateString == "")
            {
                return null;
            }
            DateTime dateTime = DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm:ss", null);
            dateTime = dateTime.AddHours(17.0);
            return dateTime;
        }
    }


    /// <summary>
    /// List:登録チャンネルリスト
    /// All:配信一覧
    /// UpdateListAsync():更新メソッド
    /// </summary>
    public class Twitch
    {
        #region プロパティ
        /// <summary>
        /// 登録チャンネルリスト
        /// </summary>
        public static List<StreamClass> List { get; set; } = new List<StreamClass>();
        /// <summary>
        /// 配信一覧
        /// </summary>
        public static List<StreamClass> All { get; set; } = new List<StreamClass>();
        /// <summary>
        /// 変更可能ステータス
        /// </summary>
        public static Boolean EnableChange { get; set; } = true;
        #endregion

        const int OFFSET = 80;
        const int RATE_LIMIT = 25;

        /// <summary>
        /// 配信サイトから配信一覧を取得してList<StreamClass>に入れて返す
        /// </summary>
        /// <returns>配信一覧</returns>
        private static async Task<List<StreamClass>> GetAllAsync()
        {
            //戻り値用
            List<StreamClass> result = new List<StreamClass>();

            using (WebClient client = new WebClient())
            {
                string json = string.Empty;

                for (int i = 0; i < RATE_LIMIT; i++)
                {
                    try
                    {
                        //エンコード設定（UTF8）
                        client.Encoding = System.Text.Encoding.UTF8;

                        //参考URL
                        //https://stackoverflow.com/questions/45622188/get-value-from-twitch-api
                        //日本語の配信を100配信ずつ（仕様上最大）取得
                        string url = @"https://api.twitch.tv/kraken/streams/?limit=100&broadcaster_language=ja&client_id=snk7w6raevojktexzkvf2ixy66gxtn&offset=" + i * OFFSET;
                        json = await client.DownloadStringTaskAsync(url);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        json = null;
                    }

                    //ちゃんと取得出来ているか
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

        /// <summary>
        /// GetAllAsync()を呼び出してList,Allを最新の状態に更新
        /// 追加の配信通知スタックをList<StreamClass>に入れて返す
        /// </summary>
        /// <returns>配信通知スタック</returns>
        public static async Task<List<StreamClass>> UpdateListAsync()
        #region
        {
            //戻り値
            List<StreamClass> stackStreamNote = new List<StreamClass>();

            //取得した配信一覧を入れる変数
            var streamAll = new List<StreamClass>();
            try
            {
                //配信一覧を取得（別スレッドで実行）
                streamAll = await GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<StreamClass>();
            }

            //ちゃんと取得できていれば
            if (streamAll.Count > 0)
            {
                //List,Allの変更不可
                EnableChange = false;
                try
                {
                    if (List.Count > 0)
                    {
                        //現在の登録チャンネルリストを取得
                        List<StreamClass> streamList = new List<StreamClass>(List);
                        var statusList = streamList.Select(x => x.StreamStatus).ToList();
                        //配信情報を初期化
                        streamList = streamList.Select(x => new StreamClass(x.Owner)).ToList();

                        foreach (StreamClass st in streamAll)
                        {
                            //一致する配信者名があった場合indexを取得
                            int index = streamList.FindIndex(item => item.Owner == st.Owner);
                            if (index >= 0)
                            {
                                //配信情報を上書き
                                streamList[index] = st;

                                //新しく開始した配信なら通知スタックに追加
                                if (!statusList[index])
                                {
                                    stackStreamNote.Add(st);
                                    statusList[index] = true;
                                }
                            }
                        }

                        List = streamList;
                    }

                    All = streamAll;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new List<StreamClass>();
                }
                finally
                {
                    //List,Allの変更可
                    EnableChange = true;
                }
            }

            //追加の通知スタックを返す
            return stackStreamNote;
        }
        #endregion
    }

    /// <summary>
    /// List:登録チャンネルリスト
    /// All:配信一覧
    /// UpdateListAsync():更新メソッド
    /// </summary>
    public class Fc2
    {
        #region プロパティ
        /// <summary>
        /// 登録チャンネルリスト
        /// </summary>
        public static List<StreamClass> List { get; set; } = new List<StreamClass>();
        /// <summary>
        /// 配信一覧
        /// </summary>
        public static List<StreamClass> All { get; set; } = new List<StreamClass>();
        /// <summary>
        /// 変更可能ステータス
        /// </summary>
        public static Boolean EnableChange { get; set; } = true;
        #endregion

        /// <summary>
        /// 配信サイトから配信一覧を取得してList<StreamClass>に入れて返す
        /// </summary>
        /// <returns>配信一覧</returns>
        private static async Task<List<StreamClass>> GetAllAsync()
        {
            //戻り値用
            List<StreamClass> fc2all = new List<StreamClass>();

            //FC2の配信一覧（一般）を取得
            string json = string.Empty;
            using (WebClient client = new WebClient())
            {
                try
                {
                    //エンコード設定（UTF8）
                    client.Encoding = System.Text.Encoding.UTF8;
                    json = await client.DownloadStringTaskAsync("https://live.fc2.com/contents/allchannellist.php");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    json = string.Empty;
                }
            }

            //ちゃんと取得出来ているか
            if (json != string.Empty)
            {
                //jsonをパースしてRootObjectに変換
                var r = JsonConvert.DeserializeObject<Fc2RootObject.RootObject>(json);

                //一覧を収納
                foreach (var channel in r.channel)
                {
                    if (channel.name != string.Empty && channel.name != "匿名")
                        fc2all.Add(new StreamClass(channel.title, "https://live.fc2.com/" + channel.id + '/', channel.name, DateTime.Parse(channel.start)));
                }
            }

            //配信一覧を返す
            return fc2all;
        }

        /// <summary>
        /// GetAllAsync()を呼び出してList,Allを最新の状態に更新
        /// 追加の配信通知スタックをList<StreamClass>に入れて返す
        /// </summary>
        /// <returns>配信通知スタック</returns>
        public static async Task<List<StreamClass>> UpdateListAsync()
        #region
        {
            //戻り値
            List<StreamClass> stackStreamNote = new List<StreamClass>();

            //取得した配信一覧を入れる変数
            var streamAll = new List<StreamClass>();
            try
            {
                //配信一覧を取得（別スレッドで実行）
                streamAll = await GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<StreamClass>();
            }

            //ちゃんと取得できていれば
            if (streamAll.Count > 0)
            {
                //List,Allの変更不可
                EnableChange = false;
                try
                {
                    if (List.Count > 0)
                    {
                        //現在の登録チャンネルリストを取得
                        List<StreamClass> streamList = new List<StreamClass>(List);
                        var statusList = streamList.Select(x => x.StreamStatus).ToList();
                        //配信情報を初期化
                        streamList = streamList.Select(x => new StreamClass(x.Owner)).ToList();

                        foreach (StreamClass st in streamAll)
                        {
                            //一致する配信者名があった場合indexを取得
                            int index = streamList.FindIndex(item => item.Owner == st.Owner);
                            if (index >= 0)
                            {
                                //配信情報を上書き
                                streamList[index] = st;

                                //新しく開始した配信なら通知スタックに追加
                                if (!statusList[index])
                                {
                                    stackStreamNote.Add(st);
                                    statusList[index] = true;
                                }
                            }
                        }

                        List = streamList;
                    }

                    All = streamAll;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new List<StreamClass>();
                }
                finally
                {
                    //List,Allの変更可
                    EnableChange = true;
                }
            }

            //追加の通知スタックを返す
            return stackStreamNote;
        }
        #endregion
    }

    /// <summary>
    /// List:登録チャンネルリスト
    /// All:配信一覧
    /// UpdateListAsync():更新メソッド
    /// </summary>
    public class Cavetube
    {
        #region プロパティ
        /// <summary>
        /// 登録チャンネルリスト
        /// </summary>
        public static List<StreamClass> List { get; set; } = new List<StreamClass>();
        /// <summary>
        /// 配信一覧
        /// </summary>
        public static List<StreamClass> All { get; set; } = new List<StreamClass>();
        /// <summary>
        /// 変更可能ステータス
        /// </summary>
        public static Boolean EnableChange { get; set; } = true;
        #endregion

        /// <summary>
        /// 配信サイトから配信一覧を取得してList<StreamClass>に入れて返す
        /// </summary>
        /// <returns>配信一覧</returns>
        private static async Task<List<StreamClass>> GetAllAsync()
        {
            //戻り値用
            List<StreamClass> cavetubeall = new List<StreamClass>();

            //参考URL
            //http://www.atmarkit.co.jp/fdotnet/dotnettips/753rssfeed/rssfeed.html
            var feed = new SyndicationFeed();
            try
            {
                //読み込み部分を非同期化
                await Task.Run(() =>
                {
                    using (XmlReader reader = XmlReader.Create("http://rss.cavelis.net/index_live.xml"))
                    {
                        feed = SyndicationFeed.Load(reader);
                    }
                });
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                feed = null;
            }

            //ちゃんと取得出来ているか
            if (feed != null)
            {
                //一覧を収納
                foreach (var item in feed.Items)
                {
                    cavetubeall.Add(new StreamClass(item.Title.Text, item.Id, item.Authors[0].Name, item.PublishDate.ToLocalTime().DateTime));
                }
            }
            else
            {
                return new List<StreamClass>();
            }

            //配信一覧を返す
            return cavetubeall;
        }

        /// <summary>
        /// GetAllAsync()を呼び出してList,Allを最新の状態に更新
        /// 追加の配信通知スタックをList<StreamClass>に入れて返す
        /// </summary>
        /// <returns>配信通知スタック</returns>
        public static async Task<List<StreamClass>> UpdateListAsync()
        #region
        {
            //戻り値
            List<StreamClass> stackStreamNote = new List<StreamClass>();

            //取得した配信一覧を入れる変数
            var streamAll = new List<StreamClass>();
            try
            {
                //配信一覧を取得（別スレッドで実行）
                streamAll = await GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<StreamClass>();
            }

            //ちゃんと取得できていれば
            if (streamAll.Count > 0)
            {
                //List,Allの変更不可
                EnableChange = false;
                try
                {
                    if (List.Count > 0)
                    {
                        //現在の登録チャンネルリストを取得
                        List<StreamClass> streamList = new List<StreamClass>(List);
                        var statusList = streamList.Select(x => x.StreamStatus).ToList();
                        //配信情報を初期化
                        streamList = streamList.Select(x => new StreamClass(x.Owner)).ToList();

                        foreach (StreamClass st in streamAll)
                        {
                            //一致する配信者名があった場合indexを取得
                            int index = streamList.FindIndex(item => item.Owner == st.Owner);
                            if (index >= 0)
                            {
                                //配信情報を上書き
                                streamList[index] = st;

                                //新しく開始した配信なら通知スタックに追加
                                if (!statusList[index])
                                {
                                    stackStreamNote.Add(st);
                                    statusList[index] = true;
                                }
                            }
                        }

                        List = streamList;
                    }
                    
                    All = streamAll;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new List<StreamClass>();
                }
                finally
                {
                    //List,Allの変更可
                    EnableChange = true;
                }
            }

            //追加の通知スタックを返す
            return stackStreamNote;
        }
        #endregion
    }
}
