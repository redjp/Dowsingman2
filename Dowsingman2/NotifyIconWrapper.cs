using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

/// <summary>
/// 参考URL
/// https://garafu.blogspot.jp/2015/06/dev-tasktray-residentapplication.html
/// </summary>
namespace Dowsingman2
{
    /// <summary>
    /// タスクトレイ通知アイコン
    /// </summary>
    public partial class NotifyIconWrapper : Component
    {
        //配信通知スタック
        private List<StreamClass> stackStreamNote = new List<StreamClass>();
        //コンテキストメニュー用
        private List<StreamClass> contextMenuNote = new List<StreamClass>();

        private MainWindow wnd = null;
        private string balloonClickUrl = "";

        const string SoundFile = "./resource/favorite.wav";
        const string IconPFile = "./resource/icon_P.ico";
        const string IconGFile = "./resource/icon_G.ico";
        const int balloontime = 3200;
        const int MAX_LOG = 100;

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
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void notifyIcon1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            openMainWindow();
        }

        /// <summary>
        /// メインウィンドウを1つだけ開く
        /// 参考URL
        /// http://blog.okazuki.jp/entry/20101215/1292384080
        /// http://aitos.jp/blogs/ito/?p=1338
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
        /// <param name="sc">配信情報</param>
        /// <param name="time">表示時間</param>
        public void balloonNotifyIcon(StreamClass sc, int time)
        {
            notifyIcon1.BalloonTipTitle = sc.Title;
            notifyIcon1.BalloonTipText = sc.Owner;
            balloonClickUrl = sc.Url;
            notifyIcon1.ShowBalloonTip(time);
        }

        /// <summary>
        /// 音を鳴らす
        /// </summary>
        private void PlaySound()
        {
            var player = new System.Media.SoundPlayer(SoundFile);
            player.Play();
        }

        /// <summary>
        /// 通知スタック処理
        /// </summary>
        private void updateBalloonStack()
        {
            //通知スタックがあるか
            if (stackStreamNote.Count > 0)
            {
                try
                {
                    //ついでに履歴に追加
                    if (!StaticClass.logList.Exists(item => item.Start_Time - stackStreamNote[0].Start_Time < new TimeSpan(0,1,0) && item.Owner == stackStreamNote[0].Owner))
                    {
                        StaticClass.logList.Insert(0, stackStreamNote[0]);
                        if (StaticClass.logList.Count > MAX_LOG)
                            StaticClass.logList.RemoveAt(MAX_LOG);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

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
        /// 一定時間起きにタイマーイベントで一覧取得（非同期）
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private async void timer1_Tick(object sender, EventArgs e)
        {
            await UpdateListAndMenu();
        }

        /// <summary>
        /// 各リストとコンテキストメニューの更新
        /// </summary>
        public async Task UpdateListAndMenu()
        {
            //各リストの更新（配信通知スタックを受け取る）
            try
            {
                stackStreamNote.AddRange(await Kukulu.UpdateListAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            /*
            try
            {
                stackStreamNote.AddRange(await Cavetube.UpdateListAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            */
            try
            {
                stackStreamNote.AddRange(await Fc2.UpdateListAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                stackStreamNote.AddRange(await Twitch.UpdateListAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //ウィンドウが開いていれば更新
            if (Application.Current.Windows.OfType<Window>().FirstOrDefault() != null)
                wnd.UpdateDispList();

            //通知スタック1つ目を即座に処理
            updateBalloonStack();

            //通知が残っていればタイマーをオン
            if (stackStreamNote.Count > 0)
                timer2.Enabled = true;

            ContextMenuUpdate();
        }

        /// <summary>
        /// タイマーイベントで通知スタックを処理
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            updateBalloonStack();
        }

        /// <summary>
        /// バルーン通知クリック時の処理
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
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
        private void ListToContextMenu(ReadOnlyCollection<StreamClass> list)
        {
            foreach (StreamClass sc in list)
            {
                if (sc.StreamStatus)
                {
                    //コンテキストメニューに配信情報を追加
                    contextMenuNote.Add(sc);
                }

            }
        }

        /// <summary>
        /// コンテキストメニューの再描画
        /// </summary>
        public void ContextMenuUpdate()
        {
            try
            {
                //コンテキストメニューの配信情報を初期化
                contextMenuNote = new List<StreamClass>();
                //コンテキストメニューの3項目目以降を削除
                while (this.contextMenuStrip1.Items.Count > 2)
                {
                    this.contextMenuStrip1.Items.RemoveAt(0);
                }

                //配信中の配信を探す
                ListToContextMenu(new ReadOnlyCollection<StreamClass>(Kukulu.List));
                ListToContextMenu(new ReadOnlyCollection<StreamClass>(Fc2.List));
                ListToContextMenu(new ReadOnlyCollection<StreamClass>(Twitch.List));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //配信が1つ以上あるなら
            if (contextMenuNote.Count > 0)
            {
                //セパレータを追加
                this.contextMenuStrip1.Items.Insert(0, new System.Windows.Forms.ToolStripSeparator());

                foreach (StreamClass sc in contextMenuNote)
                {
                    //メニューの先頭に配信を追加
                    System.Windows.Forms.ToolStripMenuItem tsi = new System.Windows.Forms.ToolStripMenuItem(sc.Owner, null, ContextMenu_Clicked);
                    this.contextMenuStrip1.Items.Insert(0, tsi);
                }

                //コンテキストメニューに配信が1つ以上あるならアイコンの色を変える
                notifyIcon1.Icon = new System.Drawing.Icon(IconPFile);
            }
            else
                notifyIcon1.Icon = new System.Drawing.Icon(IconGFile);
        }

        /// <summary>
        /// 追加コンテキストメニューがクリックされたとき処理
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void ContextMenu_Clicked(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolStripMenuItem item = (System.Windows.Forms.ToolStripMenuItem)sender;

            //クリックされた名前と同じ名前をリストから探してURLを開く
            foreach (StreamClass st in contextMenuNote)
            {
                if (st.Owner == item.Text)
                    if (st.Url != "")
                        try
                        {
                            //規定のブラウザで配信URLを開く
                            System.Diagnostics.Process.Start(st.Url);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
            }
        }
    }
}
