using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// 参考URL
/// https://garafu.blogspot.jp/2015/06/dev-tasktray-residentapplication.html
/// </summary>
namespace Dowsingman2
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Windows;
    using System.Threading.Tasks;

    /// <summary>
    /// タスクトレイ通知アイコン
    /// </summary>
    public partial class NotifyIconWrapper : Component
    {
        private MainWindow wnd = null;
        //配信通知スタック
        private List<StreamClass> stackStreamNote = new List<StreamClass>();
        private string balloonClickUrl = "";

        private System.Media.SoundPlayer player = null;
        const string SoundFile = "./resource/favorite.wav";
        const string IconPFile = "./resource/icon_P.ico";
        const string IconGFile = "./resource/icon_G.ico";
        const int balloontime = 3500;

        /// <summary>
        /// NotifyIconWrapper クラス を生成、初期化します。
        /// </summary>
        public NotifyIconWrapper()
        {
            // コンポーネントの初期化
            this.InitializeComponent();

            // コンテキストメニューのイベントを設定
            this.toolStripMenuItem_Open.Click += this.toolStripMenuItem_Open_Click;
            this.toolStripMenuItem_Exit.Click += this.toolStripMenuItem_Exit_Click;
        }

        /// <summary>
        /// コンテナ を指定して NotifyIconWrapper クラス を生成、初期化します。
        /// </summary>
        /// <param name="container">コンテナ</param>
        public NotifyIconWrapper(IContainer container)
        {
            container.Add(this);

            this.InitializeComponent();
        }

        /// <summary>
        /// コンテキストメニュー "表示" を選択したとき呼ばれます。
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void toolStripMenuItem_Open_Click(object sender, EventArgs e)
        {
            openMainWindow();
        }

        /// <summary>
        /// アイコンをダブルクリックしたときに開かれます
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            openMainWindow();
        }

        /// <summary>
        /// メインウィンドウを1つだけ開く
        /// </summary>
        private void openMainWindow()
        {
            //現在開いているウィンドウの情報を取得
            Window temp = Application.Current.Windows.OfType<Window>().FirstOrDefault();
            //ウィンドウが2つ以上開かないようにチェック
            if (temp == null)
            {
                // MainWindow を生成、表示
                wnd = new MainWindow();
                wnd.Show();
                wnd.Activate();
            }
            else
            {
                temp.Activate();
            }
        }

        /// <summary>
        /// コンテキストメニュー "終了" を選択したとき呼ばれます。
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            // 現在のアプリケーションを終了
            Application.Current.Shutdown();
        }

        /// <summary>
        /// バルーン表示メソッド
        /// </summary>
        /// <param name="message1"></param>
        /// <param name="message2"></param>
        public void balloonNotifyIcon(StreamClass st, int time)
        {
            notifyIcon1.BalloonTipTitle = st.Title;
            notifyIcon1.BalloonTipText = st.Owner;
            balloonClickUrl = st.Url;
            notifyIcon1.ShowBalloonTip(time);
        }

        /// <summary>
        /// 通知スタック処理
        /// </summary>
        private void updateBalloonStack()
        {
            //通知スタックがあるか
            if (stackStreamNote.Count > 0)
            {
                //ついでに履歴に追加
                StaticClass.logList.Add(stackStreamNote[0]);
                if (StaticClass.logList.Count > 100)
                    StaticClass.logList.RemoveAt(0);

                //スタックがあるなら1つ目を処理
                balloonNotifyIcon(stackStreamNote[0], balloontime);
                stackStreamNote.RemoveAt(0);

                //音を鳴らす
                PlaySound();
            }
            else
            {
                timer2.Enabled = false;
            }
        }

        /// <summary>
        /// 一定時間起きにタイマーイベントで一覧取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            checkStreamIcon();
        }

        public void checkStreamIcon()
        {
            //配信があればアイコンの色を変える
            CheckkukuluList();
            ChecktwitchList();
            Checkfc2List();
            
            //ウィンドウが開いていれば更新
            if (wnd != null && Application.Current.Windows.OfType<Window>().FirstOrDefault() != null)
                wnd.UpdateDispList();

            //通知スタック1つ目を即座に処理
            updateBalloonStack();

            //通知が残っていればタイマーをオン
            if (stackStreamNote.Count > 0)
                timer2.Enabled = true;

            contextMenuUpdate();
        }

        /// <summary>
        /// タイマーイベントで通知スタックを処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            updateBalloonStack();
        }

        /// <summary>
        /// 音を鳴らす
        /// </summary>
        private void PlaySound()
        {
            player = new System.Media.SoundPlayer(SoundFile);
            player.Play();
        }

        /// <summary>
        /// バルーン通知クリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            if (balloonClickUrl != "")
                //規定のブラウザで配信URLを開く
                System.Diagnostics.Process.Start(balloonClickUrl);
        }

        /// <summary>
        /// 配信中の配信をListからContextMenuに追加
        /// </summary>
        /// <param name="list">登録チャンネルリスト</param>
        private void ListToMenu(List<StreamClass> list)
        {
            foreach (StreamClass st in list)
            {
                if (st.StreamStatus)
                {
                    //配信を初めて追加するとき
                    if (this.contextMenuStrip1.Items.Count < 3)
                        //セパレータを追加
                        this.contextMenuStrip1.Items.Insert(0, new System.Windows.Forms.ToolStripSeparator());

                    //メニューの先頭に配信を追加
                    System.Windows.Forms.ToolStripMenuItem tsi = new System.Windows.Forms.ToolStripMenuItem(st.Owner, null, ContextMenu_Clicked);
                    this.contextMenuStrip1.Items.Insert(0, tsi);
                }

            }
        }

        /// <summary>
        /// コンテキストメニューの再描画
        /// </summary>
        public void contextMenuUpdate()
        {
            //コンテキストメニューの3項目目以降を削除
            while (this.contextMenuStrip1.Items.Count > 2)
            {
                this.contextMenuStrip1.Items.RemoveAt(0);
            }

            //配信中の配信を探すkukulu
            ListToMenu(StaticClass.kukuluList);
            //配信中の配信を探すfc2
            ListToMenu(StaticClass.fc2List);
            //配信中の配信を探すtwitch
            ListToMenu(StaticClass.twitchList);

            //コンテキストメニューが3つ以上あるならアイコンの色を変える
            if (this.contextMenuStrip1.Items.Count > 2)
                notifyIcon1.Icon = new System.Drawing.Icon(IconPFile);
            else
                notifyIcon1.Icon = new System.Drawing.Icon(IconGFile);
        }

        //追加コンテキストメニューがクリックされたとき処理
        private void ContextMenu_Clicked(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolStripMenuItem item = (System.Windows.Forms.ToolStripMenuItem)sender;
            
            //クリックされた名前と同じ名前をリストから探してURLを開く
            foreach (StreamClass st in StaticClass.kukuluList)
            {
                if(st.Owner == item.Text)
                    if (st.Url != "")
                        //規定のブラウザで配信URLを開く
                        System.Diagnostics.Process.Start(st.Url);
            }
            //クリックされた名前と同じ名前をリストから探してURLを開く
            foreach (StreamClass st in StaticClass.fc2List)
            {
                if (st.Owner == item.Text)
                    if (st.Url != "")
                        //規定のブラウザで配信URLを開く
                        System.Diagnostics.Process.Start(st.Url);
            }
            foreach (StreamClass st in StaticClass.twitchList)
            {
                if (st.Owner == item.Text)
                    if (st.Url != "")
                        //規定のブラウザで配信URLを開く
                        System.Diagnostics.Process.Start(st.Url);
            }

        }


        /// <summary>
        /// くくるの配信一覧を取得する
        /// </summary>
        /// <returns>List<StreamClass>型 配信一覧</returns>
        public static List<StreamClass> GetkukuluList()
        {
            HtmlAgilityPack.HtmlWeb hweb = new HtmlAgilityPack.HtmlWeb();
            var doc = new HtmlAgilityPack.HtmlDocument();

            //kukuluLiveから配信一覧を取得
            try
            {
                doc = hweb.Load("http://live.kukulu.erinn.biz/_live.ajax.php");
            }
            //取得に失敗したら前の一覧を返す
            catch (WebException)
            {
                return StaticClass.kukuluAll;
            }
           
            //Xpathで配信タイトルの書かれたAタグを抜き出す
            HtmlAgilityPack.HtmlNodeCollection streamNode = doc.DocumentNode.SelectNodes("//a[@class='a_live']");
            //Xpathで太字の配信者名を抜き出す
            HtmlAgilityPack.HtmlNodeCollection streamerNode = doc.DocumentNode.SelectNodes("//td[contains(@id,'livecom')]//div/a/b");

            //ここは外にクラスを作って書き換えたい
            List<string> title = new List<string>();
            List<string> url = new List<string>();
            List<string> owner = new List<string>();

            //ノードの数を数えるカウンタ
            int count = 0;

            //ちゃんと取得出来ているか
            if (streamNode != null && streamerNode != null)
            {
                foreach (var node in streamNode)
                {
                    //配信タイトルを取得
                    title.Add(node.InnerText);
                    //配信URLを取得
                    url.Add("http://live.kukulu.erinn.biz/" + node.Attributes["href"].ValueOrDefault());

                    count++;
                }

                foreach (var node in streamerNode)
                {
                    //配信者名を取得
                    owner.Add(node.InnerText);
                }
            }

            //戻り値用
            List<StreamClass> nowKukuluList = new List<StreamClass>();

            //一覧を収納
            for (int i = 0; i < count; i++)
            {
                nowKukuluList.Add(new StreamClass(title[i], url[i], owner[i]));
            }

            return nowKukuluList;
        }

        /// <summary>
        /// くくる配信一覧とお気に入りを比較する
        /// </summary>
        public void CheckkukuluList()
        {
            List<Boolean> statusList = new List<Boolean>();
            List<string> loadList = new List<string>();

            //くくる配信一覧を取得（別スレッドで実行）
            var task = Task.Run(() => GetkukuluList());

            //kukuluタブの情報を保存
            foreach (StreamClass st in StaticClass.kukuluList)
            {
                statusList.Add(st.StreamStatus);
                loadList.Add(st.Owner);
            }

            //保存した情報を元にリセット
            StaticClass.kukuluList = new List<StreamClass>();
            foreach (string str in loadList)
            {
                StaticClass.kukuluList.Add(new StreamClass(str));
            }

            //ここで合流
            StaticClass.kukuluAll = task.Result;

            foreach (StreamClass st in StaticClass.kukuluAll)
            {
                //一致する配信者名があった場合
                int index = StaticClass.kukuluList.FindIndex(item => item.Owner == st.Owner);
                if (index != -1)
                {
                    //配信情報を上書き
                    StaticClass.kukuluList[index] = st;

                    //新しく開始した配信なら通知スタックに追加
                    if (!statusList[index])
                    {
                        stackStreamNote.Add(st);
                    }
                }
            }
        }

        /// <summary>
        /// Twitchで配信があればタイトルを返す
        /// </summary>
        /// <returns></returns>
        private List<StreamClass> GettwitchList()
        {
            //戻り値用
            List<StreamClass> allLive = new List<StreamClass>();

            int total = 1000;
            for (int i = 0; i*100 < total; i++)
            {
                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.UTF8;
                string json;

                try
                {
                    //参考URL
                    //https://stackoverflow.com/questions/45622188/get-value-from-twitch-api
                    //日本語の配信を100配信ずつ（仕様上最大）取得
                    string url = @"https://api.twitch.tv/kraken/streams/?limit=100&broadcaster_language=ja&client_id=snk7w6raevojktexzkvf2ixy66gxtn&offset=" + i*100;
                    json = client.DownloadString(url);
                }
                catch (WebException)
                {
                    json = null;
                }
                finally
                {
                    Com.ComRelease.FinalReleaseComObjects(client);
                    client.Dispose();
                }

                //ちゃんと取得出来ているか
                if (json != null)
                {
                    //jsonをパースしてRootObjectに変換
                    var r = JsonConvert.DeserializeObject<TwitchRootObject.RootObject>(json);
                    //最大値を入れる
                    total = r._total;

                    //一覧を収納
                    foreach (var s in r.streams)
                    {
                        allLive.Add(new StreamClass(s.channel.status, "https://www.twitch.tv/" + s.channel.name + '/', s.channel.name));
                    }
                }
            }
            return allLive;
        }

        /// <summary>
        /// twitchのお気に入り配信があるか調べる
        /// </summary>
        public void ChecktwitchList()
        {
            List<Boolean> statusList = new List<Boolean>();
            List<string> loadList = new List<string>();
            StaticClass.twitchAll = new List<StreamClass>();

            //twitch配信一覧を取得（別スレッドで実行）
            var task = Task.Run(() => GettwitchList());
            
            //twitchタブの情報を保存
            foreach (StreamClass st in StaticClass.twitchList)
            {
                statusList.Add(st.StreamStatus);
                loadList.Add(st.Owner);
            }

            //保存した情報を元にリセット
            StaticClass.twitchList = new List<StreamClass>();
            foreach (string str in loadList)
            {
                StaticClass.twitchList.Add(new StreamClass(str));
            }

            //ここで合流
            StaticClass.twitchAll = task.Result;

            foreach (StreamClass st in StaticClass.twitchAll)
            {
                //一致する配信者名があった場合
                int index = StaticClass.twitchList.FindIndex(item => item.Owner == st.Owner);
                if (index != -1)
                {
                    //配信情報を上書き
                    StaticClass.twitchList[index] = st;

                    //新しく開始した配信なら通知スタックに追加
                    if (!statusList[index])
                    {
                        stackStreamNote.Add(st);
                    }
                }
            }
        }

        /// <summary>
        /// FC2の配信一覧を取得する
        /// </summary>
        /// <returns>List<StreamClass>型 配信一覧</returns>
        public static List<StreamClass> Getfc2List()
        {
            //エンコードを設定
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            string json = null;

            try
            {
                json = client.DownloadString("https://live.fc2.com/contents/allchannellist.php");
            }
            catch (WebException)
            {
                json = null;
            }
            finally
            {
                Com.ComRelease.FinalReleaseComObjects(client);
                client.Dispose();
            }

            //戻り値用
            List<StreamClass> allLive = new List<StreamClass>();

            //ちゃんと取得出来ているか
            if (json != null)
            {
                //jsonをパースしてRootObjectに変換
                var r = JsonConvert.DeserializeObject<Fc2RootObject.RootObject>(json);

                //一覧を収納
                foreach (var channel in r.channel)
                {
                    if (channel.name != "" && channel.name != "匿名")
                        allLive.Add(new StreamClass(channel.title, "https://live.fc2.com/" + channel.id + '/', channel.name));
                }
            }

            return allLive;
        }

        /// <summary>
        /// FC2配信一覧とお気に入りを比較する
        /// </summary>
        public void Checkfc2List()
        {
            List<Boolean> statusList = new List<Boolean>();
            List<string> loadList = new List<string>();

            //FC2配信一覧を取得（別スレッドで実行）
            var task = Task.Run(() => Getfc2List());

            //FC2タブの情報を保存
            foreach (StreamClass st in StaticClass.fc2List)
            {
                statusList.Add(st.StreamStatus);
                loadList.Add(st.Owner);
            }

            //保存した情報を元にリセット
            StaticClass.fc2List = new List<StreamClass>();
            foreach (string str in loadList)
            {
                StaticClass.fc2List.Add(new StreamClass(str));
            }

            //ここで合流
            StaticClass.fc2All = task.Result;

            foreach (StreamClass st in StaticClass.fc2All)
            {
                //一致する配信者名があった場合
                int index = StaticClass.fc2List.FindIndex(item => item.Owner == st.Owner);
                if (index != -1)
                {
                    //配信情報を上書き
                    StaticClass.fc2List[index] = st;

                    //新しく開始した配信なら通知スタックに追加
                    if (!statusList[index])
                    {
                        stackStreamNote.Add(st);
                    }
                }
            }
        }
    }
}
