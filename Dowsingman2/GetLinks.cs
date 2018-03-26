using HtmlAgilityPack;
using System;

/// <summary>
/// Attributeがnull以外ならValueを返す
/// </summary>
public static class HtmlAgilityExtender
{
    public static String ValueOrDefault(this HtmlAttribute attr)
    {
        return (attr != null) ? attr.Value : String.Empty;
    }
}

/*
/// <summary>
/// ここからコピペしたもの
/// http://www.atmarkit.co.jp/fdotnet/dotnettips/687nondispbrowser/nondispbrowser.html
/// </summary>
public class NonDispBrowser : WebBrowser
{

    bool done;

    // タイムアウト時間（10秒）
    TimeSpan timeout = new TimeSpan(0, 0, 10);

    protected override void OnDocumentCompleted(
                  WebBrowserDocumentCompletedEventArgs e)
    {
        // ページにフレームが含まれる場合にはフレームごとに
        // このメソッドが実行されるため実際のURLを確認する
        if (e.Url == this.Url)
        {
            done = true;
        }
    }

    protected override void OnNewWindow(CancelEventArgs e)
    {
        // ポップアップ・ウィンドウをキャンセル
        e.Cancel = true;
    }

    public NonDispBrowser()
    {
        // スクリプト・エラーを表示しない
        this.ScriptErrorsSuppressed = true;
    }

    public bool NavigateAndWait(string url)
    {

        base.Navigate(url); // ページの移動

        done = false;
        DateTime start = DateTime.Now;

        while (done == false)
        {
            if (DateTime.Now - start > timeout)
            {
                // タイムアウト
                return false;
            }
            Application.DoEvents();
        }
        return true;
    }
}

public class GetLinks
{

    /// <summary>
    /// くくるの配信一覧を取得する
    /// </summary>
    /// <returns>List<StreamClass>型 配信一覧</returns>
    public List<StreamClass> GetkukuluList()
    {
        //kukuluLiveのスマホ用ページから配信一覧のHTMLを取得
        NonDispBrowser ndb = new NonDispBrowser();
        ndb.NavigateAndWait("http://live.kukulu.erinn.biz/smphone.live.php");

        //HtmlAgilityPack用に取得したHTMLを変換
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(new StringReader(ndb.Document.Body.OuterHtml));

        //Xpathで配信タイトルの書かれたAタグを抜き出す
        HtmlAgilityPack.HtmlNodeCollection streamNode = doc.DocumentNode.SelectNodes("//div[@id='area_datalist']//td[@class='sub1' or @class='sub2']/div/a");
        //Xpathで太字の配信者名を抜き出す
        HtmlAgilityPack.HtmlNodeCollection streamerNode = doc.DocumentNode.SelectNodes("//div[@id='area_datalist']//td[@class='sub1' or @class='sub2']//td[1]/div[1]/a//b");

        //ここは外にクラスを作って書き換えたい
        List<string> title = new List<string>();
        List<string> url = new List<string>();
        List<string> owner = new List<string>();

        //ノードの数を数えるカウンタ
        int count = 0;

        foreach (var node in streamNode)
        {
            //配信タイトルを取得
            title.Add(node.InnerText);
            //配信URLを取得（そのままだとスマホ用URLなのでsmを外す）
            url.Add(node.Attributes["href"].ValueOrDefault().Replace("smlive", "live"));

            count++;
        }

        foreach (var node in streamerNode)
        {
            //配信者名を取得
            owner.Add(node.InnerText);
        }

        //戻り値用
        List<Stream> nowKukuluList = new List<Stream>();

        //一覧を収納
        for (int i = 0; i < count; i++)
        {
            nowKukuluList.Add(new Stream(title[i], url[i], owner[i]));
        }

        return nowKukuluList;
    }
}
*/