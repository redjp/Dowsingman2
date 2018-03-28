using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 非同期処理に書き換え＆StaticClassを統合
/// </summary>
namespace Dowsingman2
{
    /// <summary>
    /// 雛形クラス
    /// </summary>
    public class StreamList
    {
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
    }

    /// <summary>
    /// List:登録チャンネルリスト
    /// All:配信一覧
    /// UpdateListAsync():更新メソッド
    /// </summary>
    public class Kukulu : StreamList
    {
        /// <summary>
        /// 配信サイトから配信一覧を取得してList<StreamClass>に入れて返す
        /// </summary>
        /// <returns>配信一覧</returns>
        private static async Task<List<StreamClass>> GetAllAsync()
        {
            //戻り値用
            List<StreamClass> nowKukuluList = new List<StreamClass>();


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
                return nowKukuluList;
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
                nowKukuluList.Add(new StreamClass(streamNode[i].InnerText, "http://live.kukulu.erinn.biz/" + streamNode[i].Attributes["href"].ValueOrDefault(), streamerNode[i].InnerText));
            }

            return nowKukuluList;
        }

        /// <summary>
        /// GetAllAsync()を呼び出してList,Allを最新の状態に更新
        /// 追加の配信通知スタックをList<StreamClass>に入れて返す
        /// </summary>
        /// <returns>配信通知スタック</returns>
        public static async Task<List<StreamClass>> UpdateListAsync()
        {
            //戻り値用
            List<StreamClass> stackStreamNote = new List<StreamClass>();

            //取得した配信一覧を入れる変数
            var kukuluAll = new List<StreamClass>();

            try
            {
                //くくる配信一覧を取得（別スレッドで実行）
                kukuluAll = await GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return stackStreamNote;
            }

            //List,Allの変更不可
            EnableChange = false;
            try
            {
                //現在のお気に入り配信一覧を取得
                List<StreamClass> kukuluList = List;

                //配信中ステータスと配信者名保存用
                var statusList = new List<Boolean>();
                var loadList = new List<string>();

                //kukuluタブの情報を保存
                foreach (StreamClass st in kukuluList)
                {
                    statusList.Add(st.StreamStatus);
                    loadList.Add(st.Owner);
                }

                //保存した情報を元にリセット
                kukuluList = new List<StreamClass>();
                foreach (string str in loadList)
                {
                    kukuluList.Add(new StreamClass(str));
                }

                foreach (StreamClass st in kukuluAll)
                {
                    //一致する配信者名があった場合indexを取得
                    int index = kukuluList.FindIndex(item => item.Owner == st.Owner);
                    if (index != -1)
                    {
                        //配信情報を上書き
                        kukuluList[index] = st;

                        //新しく開始した配信なら通知スタックに追加
                        if (!statusList[index])
                        {
                            stackStreamNote.Add(st);
                        }
                    }
                }

                //最後に結果を入れる
                List = kukuluList;
                All = kukuluAll;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return stackStreamNote;
            }
            finally
            {
                //List,Allの変更化
                EnableChange = true;
            }

            //追加の通知スタックを返す
            return stackStreamNote;
        }
    }

    /// <summary>
    /// List:登録チャンネルリスト
    /// All:配信一覧
    /// UpdateListAsync():更新メソッド
    /// </summary>
    public class Twitch : StreamList
    {
    }
}
