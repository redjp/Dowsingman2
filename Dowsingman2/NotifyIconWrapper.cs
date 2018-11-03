using Dowsingman2.BaseClass;
using Dowsingman2.Dialog;
using Dowsingman2.LiveService;
using Dowsingman2.SubManager;
using Dowsingman2.UtilityClass;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        public static event EventHandler RefreshStarting;
        public static event EventHandler RefreshCompleted;

        public List<AbstractManager> EnableManagers { get; } = new List<AbstractManager>();
        public BalloonManager BalloonManager { get; }
        public ContextMenuManager ContextMenuManager { get; }

        private string balloonClickUrl_ = string.Empty;

        const string SOUND_LOCAL_PATH = "resource\\favorite.wav";
        const string ICON_P_LOCAL_PATH = "resource\\icon_P.ico";
        const string ICON_G_LOCAL_PATH = "resource\\icon_G.ico";
        const int balloontime = 3200;

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

            RefreshBridge.RefreshEvent += RefreshBridge_RefreshEvent;

            EnableManagers.Add(KukuluManager.GetInstance());
            EnableManagers.Add(CavetubeManager.GetInstance());
            EnableManagers.Add(Fc2Manager.GetInstance());
            EnableManagers.Add(TwitchManager.GetInstance());

            BalloonManager = new BalloonManager(myNotifyIcon);
            ContextMenuManager = new ContextMenuManager(myContextMenuStrip);

            var t = RefreshNotifyIconAsync();   //警告を消すために変数に代入
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
        private void myNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
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
            Window temp = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            //ウィンドウが2つ以上開かないようにチェック
            if (temp == null)
            {
                // MainWindow を生成、表示
                var wnd = new MainWindow();
                wnd.Show();
                wnd.Activate();
            }
            else
            {
                temp.Activate();
                temp = Application.Current.Windows.OfType<SettingWindow>().FirstOrDefault();
                temp?.Activate();
            }
        }

        /// <summary>
        /// Windowsがログオフ、シャットダウンしたときに呼び出されます。
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            Shutdown();
        }

        /// <summary>
        /// コンテキストメニュー "終了" を選択したとき呼ばれます。
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            Shutdown();
        }

        /// <summary>
        /// 保存とシャットダウンを行う
        /// </summary>
        private void Shutdown()
        {
            Window temp = Application.Current.Windows.OfType<SettingWindow>().FirstOrDefault();
            temp?.Close();
            temp = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            temp?.Close();

            SettingManager.GetInstance().Save();
            LogManager.GetInstance().Save();

            Application.Current.Shutdown();
        }

        /// <summary>
        /// 一定時間起きにタイマーイベントで一覧取得（非同期）
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private async void refreshTimer_Tick(object sender, EventArgs e)
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

            foreach (AbstractManager manager in EnableManagers)
            {
                //サイトごとにタスクを作り、配信一覧からお気に入りの更新まで済ませる
                tasks.Add(manager.RefreshLiveAsync().ContinueWith(task =>
                {
                    if (task.Result)
                        BalloonManager.AddRange(manager.CheckFavorite());
                }));
            }
            await Task.WhenAll(tasks);

            if (!balloonTimer.Enabled)
            {
                //通知スタック1つ目を即座に処理
                balloonTimer.Enabled = BalloonManager.ExcuteBalloonQueue(balloontime, SOUND_LOCAL_PATH) > 0;
            }

            RefreshMenuAndIcon();

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

        private void RefreshBridge_RefreshEvent(AbstractManager manager)
        {
            RefreshNotifyIconLite(manager);
        }

        /// <summary>
        /// 1箇所だけ更新があったとき用
        /// </summary>
        public void RefreshNotifyIconLite(AbstractManager manager)
        {
            BalloonManager.AddRange(manager.CheckFavorite());

            if (!balloonTimer.Enabled)
            {
                //通知スタック1つ目を即座に処理
                balloonTimer.Enabled = BalloonManager.ExcuteBalloonQueue(balloontime, SOUND_LOCAL_PATH) > 0;
            }

            RefreshMenuAndIcon();
        }

        /// <summary>
        /// タイマーイベントで通知スタックを処理
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void balloonTimer_Tick(object sender, EventArgs e)
        {
            balloonTimer.Enabled = BalloonManager.ExcuteBalloonQueue(balloontime, SOUND_LOCAL_PATH) > 0;
        }

        /// <summary>
        /// バルーン通知クリック時の処理
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void myNotifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            //規定のブラウザで配信URLを開く
            MyUtility.OpenBrowser(balloonClickUrl_);
        }

        /// <summary>
        /// コンテキストメニューの再描画
        /// </summary>
        public void RefreshMenuAndIcon()
        {
            if (ContextMenuManager.RefreshContextMenu(EnableManagers))
            {
                //コンテキストメニューに配信が1つ以上あるならアイコンの色を変える
                myNotifyIcon.Icon = new System.Drawing.Icon(ICON_P_LOCAL_PATH);
            }
            else
            {
                myNotifyIcon.Icon = new System.Drawing.Icon(ICON_G_LOCAL_PATH);
            }
        }

        private async void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Resume)
            {
                refreshTimer.Enabled = false;
                System.Threading.Thread.Sleep(1000);

                await RefreshNotifyIconAsync();
                refreshTimer.Enabled = true;
            }
        }
    }
}
