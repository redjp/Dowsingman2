﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

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
    public class Kukulu
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
            //戻り値
            var kukuluall = new List<StreamClass>();

            HtmlAgilityPack.HtmlWeb hweb = new HtmlAgilityPack.HtmlWeb();
            var doc = new HtmlAgilityPack.HtmlDocument();

            try
            {
                //kukuluLiveから配信一覧を取得
                doc = await hweb.LoadFromWebAsync("http://live.kukulu.erinn.biz/_live.ajax.php");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return kukuluall;
            }

            //Xpathで配信タイトルの書かれたAタグを抜き出す
            HtmlAgilityPack.HtmlNodeCollection streamNode = doc.DocumentNode.SelectNodes("//a[@class='a_live']");
            //Xpathで太字の配信者名を抜き出す
            HtmlAgilityPack.HtmlNodeCollection streamerNode = doc.DocumentNode.SelectNodes("//td[contains(@id,'livecom')]//div/a/b");

            //要素数
            int count = streamNode.Count;
            if (streamerNode.Count < count) count = streamerNode.Count;

            //一覧を収納
            for (int i = 0; i < count; i++)
            {
                kukuluall.Add(new StreamClass(streamNode[i].InnerText, "http://live.kukulu.erinn.biz/" + streamNode[i].Attributes["href"].ValueOrDefault(), streamerNode[i].InnerText));
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
                //くくる配信一覧を取得（別スレッドで実行）
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
                    //現在の登録チャンネルリストを取得
                    List<StreamClass> streamList = new List<StreamClass>(List);

                    //配信中ステータスと配信者名保存用
                    var statusList = new List<Boolean>();
                    var loadList = new List<string>();

                    //kukuluタブの情報を保存
                    foreach (StreamClass st in streamList)
                    {
                        statusList.Add(st.StreamStatus);
                        loadList.Add(st.Owner);
                    }

                    //保存した情報を元にリセット
                    streamList = new List<StreamClass>();
                    foreach (string str in loadList)
                    {
                        streamList.Add(new StreamClass(str));
                    }

                    foreach (StreamClass st in streamAll)
                    {
                        //一致する配信者名があった場合indexを取得
                        int index = streamList.FindIndex(item => item.Owner == st.Owner);
                        if (index != -1)
                        {
                            //配信情報を上書き
                            streamList[index] = st;

                            //新しく開始した配信なら通知スタックに追加
                            if (!statusList[index])
                            {
                                stackStreamNote.Add(st);
                            }
                        }
                    }

                    //最後に結果を入れる
                    List = streamList;
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

        /// <summary>
        /// 配信サイトから配信一覧を取得してList<StreamClass>に入れて返す
        /// </summary>
        /// <returns>配信一覧</returns>
        private static async Task<List<StreamClass>> GetAllAsync()
        {
            //戻り値用
            List<StreamClass> twitchall = new List<StreamClass>();

            //配信数をとりあえず100に設定
            int total = 100;
            for (int i = 0; i * 100 < total; i++)
            {
                string json = "";
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        //エンコード設定（UTF8）
                        client.Encoding = System.Text.Encoding.UTF8;

                        //参考URL
                        //https://stackoverflow.com/questions/45622188/get-value-from-twitch-api
                        //日本語の配信を100配信ずつ（仕様上最大）取得
                        string url = @"https://api.twitch.tv/kraken/streams/?limit=100&broadcaster_language=ja&client_id=snk7w6raevojktexzkvf2ixy66gxtn&offset=" + i * 100;
                        json = await client.DownloadStringTaskAsync(url);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        json = null;
                    }
                }

                //ちゃんと取得出来ているか
                if (json != null)
                {
                    //jsonをパースしてRootObjectに変換
                    var r = JsonConvert.DeserializeObject<TwitchRootObject.RootObject>(json);
                    //配信数を入れる
                    total = r._total;

                    //一覧を収納
                    foreach (var s in r.streams)
                    {
                        twitchall.Add(new StreamClass(s.channel.status, "https://www.twitch.tv/" + s.channel.name + '/', s.channel.name));
                    }
                }
                else
                {
                    return new List<StreamClass>();
                }
            }

            //配信一覧を返す
            return twitchall;
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
                //くくる配信一覧を取得（別スレッドで実行）
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
                    //現在の登録チャンネルリストを取得
                    List<StreamClass> streamList = new List<StreamClass>(List);

                    //配信中ステータスと配信者名保存用
                    var statusList = new List<Boolean>();
                    var loadList = new List<string>();

                    //kukuluタブの情報を保存
                    foreach (StreamClass st in streamList)
                    {
                        statusList.Add(st.StreamStatus);
                        loadList.Add(st.Owner);
                    }

                    //保存した情報を元にリセット
                    streamList = new List<StreamClass>();
                    foreach (string str in loadList)
                    {
                        streamList.Add(new StreamClass(str));
                    }

                    foreach (StreamClass st in streamAll)
                    {
                        //一致する配信者名があった場合indexを取得
                        int index = streamList.FindIndex(item => item.Owner == st.Owner);
                        if (index != -1)
                        {
                            //配信情報を上書き
                            streamList[index] = st;

                            //新しく開始した配信なら通知スタックに追加
                            if (!statusList[index])
                            {
                                stackStreamNote.Add(st);
                            }
                        }
                    }

                    //最後に結果を入れる
                    List = streamList;
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

            //配信数をとりあえず100に設定
            
            string json = "";
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
                    json = null;
                }
            }

            //ちゃんと取得出来ているか
            if (json != null)
            {
                //jsonをパースしてRootObjectに変換
                var r = JsonConvert.DeserializeObject<Fc2RootObject.RootObject>(json);

                //一覧を収納
                foreach (var channel in r.channel)
                {
                    if (channel.name != "" && channel.name != "匿名")
                        fc2all.Add(new StreamClass(channel.title, "https://live.fc2.com/" + channel.id + '/', channel.name));
                }
            }
            else
            {
                return new List<StreamClass>();
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
                //くくる配信一覧を取得（別スレッドで実行）
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
                    //現在の登録チャンネルリストを取得
                    List<StreamClass> streamList = new List<StreamClass>(List);

                    //配信中ステータスと配信者名保存用
                    var statusList = new List<Boolean>();
                    var loadList = new List<string>();

                    //kukuluタブの情報を保存
                    foreach (StreamClass st in streamList)
                    {
                        statusList.Add(st.StreamStatus);
                        loadList.Add(st.Owner);
                    }

                    //保存した情報を元にリセット
                    streamList = new List<StreamClass>();
                    foreach (string str in loadList)
                    {
                        streamList.Add(new StreamClass(str));
                    }

                    foreach (StreamClass st in streamAll)
                    {
                        //一致する配信者名があった場合indexを取得
                        int index = streamList.FindIndex(item => item.Owner == st.Owner);
                        if (index != -1)
                        {
                            //配信情報を上書き
                            streamList[index] = st;

                            //新しく開始した配信なら通知スタックに追加
                            if (!statusList[index])
                            {
                                stackStreamNote.Add(st);
                            }
                        }
                    }

                    //最後に結果を入れる
                    List = streamList;
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
