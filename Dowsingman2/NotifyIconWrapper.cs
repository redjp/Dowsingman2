using Dowsingman2.BaseClass;
using Dowsingman2.LiveService;
using Dowsingman2.MyUtility;
using Dowsingman2.SubManager;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
        public static event EventHandler RefreshStarting;
        public static event EventHandler RefreshCompleted;

        private object lockobject = new object();

        private List<AbstractManager> managers_ = new List<AbstractManager>();
        //配信通知スタック
        private List<StreamClass> balloonStacks_ = new List<StreamClass>();
        //コンテキストメニュー用
        private List<StreamClass> contextMenuItems_ = new List<StreamClass>();

        private string balloonClickUrl_ = string.Empty;

        const string SOUND_LOCAL_PATH = ".\\resource\\favorite.wav";
        const string ICON_P_LOCAL_PATH = ".\\resource\\icon_P.ico";
        const string ICON_G_LOCAL_PATH = ".\\resource\\icon_G.ico";
        const int balloontime = 3200;

        public string SoundFilePath { get; }
        public string IconPFilePath { get; }
        public string IconGFilePath { get; }

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
            SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding);
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;

            managers_.Add(KukuluManager.GetInstance());
            managers_.Add(CavetubeManager.GetInstance());
            managers_.Add(Fc2Manager.GetInstance());
            managers_.Add(TwitchManager.GetInstance());

            SoundFilePath = Path.GetFullPath(SOUND_LOCAL_PATH);
            IconPFilePath = Path.GetFullPath(ICON_P_LOCAL_PATH);
            IconGFilePath = Path.GetFullPath(ICON_G_LOCAL_PATH);
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
        /// アイコンをダブルクリックしたときに呼ばれます。
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
                var wnd = new MainWindow();
                wnd.Show();
                wnd.Activate();
                wnd.UpdateDispList();
            }
            else
            {
                temp.Activate();
            }
        }

        /// <summary>
        /// Windowsがログオフ、シャットダウンしたときに呼び出されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            LogManager.GetInstance().Save();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// コンテキストメニュー "終了" を選択したとき呼ばれます。
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            LogManager.GetInstance().Save();
            Application.Current.Shutdown();
        }



        /// <summary>
        /// 一定時間起きにタイマーイベントで一覧取得（非同期）
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private async void timer1_Tick(object sender, EventArgs e)
        {
            await RefreshNotifyIconAsync();
        }

        /// <summary>
        /// 各リストとコンテキストメニューの更新
        /// </summary>
        public async Task RefreshNotifyIconAsync()
        {
            OnRefreshStarting();

            var tasks = new List<Task>();
            var lockObject = new Object();

            foreach (AbstractManager manager in managers_)
            {
                //サイトごとにタスクを作り、配信一覧からお気に入りの更新まで済ませる
                tasks.Add(manager.RefreshLiveAsync().ContinueWith(task =>
                {
                    if (task.Result)
                        lock (lockObject)
                            balloonStacks_.AddRange(manager.CheckFavorite());
                }));
            }
            await Task.WhenAll(tasks);


            if (!timer2.Enabled)
            {
                //通知スタック1つ目を即座に処理
                MyBalloon.ExcuteBalloonStack(balloonStacks_, notifyIcon1, balloontime, SoundFilePath);
                timer2.Enabled = balloonStacks_.Count > 0;
            }

            ContextMenuUpdate();

            OnRefreshCompleted();
        }

        private void OnRefreshStarting()
        {
            RefreshStarting?.Invoke(this, EventArgs.Empty);
        }

        private void OnRefreshCompleted()
        {
            RefreshCompleted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// タイマーイベントで通知スタックを処理
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            MyBalloon.ExcuteBalloonStack(balloonStacks_, notifyIcon1, balloontime, SoundFilePath);
            timer2.Enabled = balloonStacks_.Count > 0;
        }

        /// <summary>
        /// バルーン通知クリック時の処理
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            //規定のブラウザで配信URLを開く
            MyTools.OpenBrowser(balloonClickUrl_);
        }



        /// <summary>
        /// コンテキストメニューの再描画
        /// </summary>
        public void ContextMenuUpdate()
        {
            //コンテキストメニューの配信情報を初期化
            contextMenuItems_ = new List<StreamClass>();
            //コンテキストメニューの3項目目以降を削除
            while (this.contextMenuStrip1.Items.Count > 2)
            {
                this.contextMenuStrip1.Items.RemoveAt(0);
            }

            foreach (AbstractManager manager in managers_)
            {
                contextMenuItems_.AddRange(manager.GetFavoriteLiveOnly());
            }

            //配信が1つ以上あるなら
            if (contextMenuItems_.Count > 0)
            {
                //セパレータを追加
                this.contextMenuStrip1.Items.Insert(0, new System.Windows.Forms.ToolStripSeparator());

                foreach (StreamClass sc in contextMenuItems_)
                {
                    //メニューの先頭に配信を追加
                    System.Windows.Forms.ToolStripMenuItem tsi = new System.Windows.Forms.ToolStripMenuItem(sc.Owner, null, ContextMenu_Clicked);
                    this.contextMenuStrip1.Items.Insert(0, tsi);
                }

                //コンテキストメニューに配信が1つ以上あるならアイコンの色を変える
                notifyIcon1.Icon = new System.Drawing.Icon(ICON_P_LOCAL_PATH);
            }
            else
            {
                notifyIcon1.Icon = new System.Drawing.Icon(ICON_G_LOCAL_PATH);
            }
        }

        /// <summary>
        /// 追加コンテキストメニューがクリックされたとき処理
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void ContextMenu_Clicked(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolStripMenuItem item = (System.Windows.Forms.ToolStripMenuItem)sender;
            if (item.Text == "開く" || item.Text == "終了") return;
            
            //クリックされた名前と同じ名前をリストから探してURLを開く
            foreach (StreamClass st in contextMenuItems_)
            {
                if (st.Owner == item.Text)
                {
                    MyTools.OpenBrowser(st.Url);
                    return;
                }
            }
        }

        private async void timer3_Tick(object sender, EventArgs e)
        {
            timer3.Enabled = false;
            await RefreshNotifyIconAsync();
            timer1.Enabled = true;
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Resume)
            {
                timer1.Enabled = false;
                System.Threading.Thread.Sleep(3000);
                if (!timer1.Enabled && !timer3.Enabled)
                    timer3.Start();
            }
        }
    }
}
