using Dowsingman2.BaseClass;
using Dowsingman2.LiveService;
using Dowsingman2.SubManager;
using Dowsingman2.MyUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

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
        public static event EventHandler UpdateCompleted;

        private List<AbstractManager> managers_ = new List<AbstractManager>();
        //配信通知スタック
        private List<StreamClass> stackStreamNote = new List<StreamClass>();
        //コンテキストメニュー用
        private List<StreamClass> contextMenuNote = new List<StreamClass>();

        private MainWindow wnd = null;
        private string balloonClickUrl = string.Empty;

        const string SOUND_LOCAL_PATH = ".\\resource\\favorite.wav";
        const string ICON_P_LOCAL_PATH = ".\\resource\\icon_P.ico";
        const string ICON_G_LOCAL_PATH = ".\\resource\\icon_G.ico";
        const int balloontime = 3200;
        const int MAX_LOG = 100;

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
                wnd = new MainWindow();
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
            Application.Current.Shutdown();
        }

        /// <summary>
        /// コンテキストメニュー "終了" を選択したとき呼ばれます。
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// バルーン表示メソッド
        /// </summary>
        /// <param name="sc">配信情報</param>
        /// <param name="time">表示時間</param>
        private void balloonNotifyIcon(StreamClass sc, int time)
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
            using (var player = new System.Media.SoundPlayer(SoundFilePath))
            {
                player.Play();
            }
        }

        /// <summary>
        /// 通知スタック処理
        /// </summary>
        private void updateBalloonStack()
        {
            if (stackStreamNote.Count > 0)
            {
                try
                {
                    //履歴を追加
                    while (!LogManager.GetInstance().AddFavorite(stackStreamNote[0]))
                    {
                        //かぶっていれば削除
                        stackStreamNote.RemoveAt(0);
                        //スタックがなければループを抜ける
                        if (stackStreamNote.Count <= 0)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MyTraceSource.TraceEvent(TraceEventType.Error, ex);
                    return;
                }
            }

            //通知スタックがあるか
            if (stackStreamNote.Count > 0)
            {
                LogManager.GetInstance().Save();

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
            Dictionary<AbstractManager, Task<bool>> taskdictionary = new Dictionary<AbstractManager, Task<bool>>();

            foreach (AbstractManager manager in managers_)
            {
                taskdictionary[manager] = manager.RefreshLiveAsync();
            }

            foreach (AbstractManager manager in managers_)
            {
                if (await taskdictionary[manager])
                {
                    stackStreamNote.AddRange(manager.CheckFavorite());
                }
            }

            //ウィンドウが開いていれば更新
            if (Application.Current.Windows.OfType<Window>().FirstOrDefault() != null)
                wnd.UpdateDispList();

            if (!timer2.Enabled)
            {
                //通知スタック1つ目を即座に処理
                updateBalloonStack();

                //通知が残っていればタイマーをオン
                if (stackStreamNote.Count > 0)
                    timer2.Enabled = true;
            }

            ContextMenuUpdate();

            OnUpdateCompleted();
        }

        private void OnUpdateCompleted()
        {
            UpdateCompleted?.Invoke(this, EventArgs.Empty);
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
        /// 配信中の配信をListで返す
        /// </summary>
        /// <param name="list">登録チャンネルリスト</param>
        private List<StreamClass> ExtractOnLive(ReadOnlyCollection<StreamClass> list)
        {
            return list.Where(x => x.StreamStatus).ToList();
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
                foreach(AbstractManager manager in managers_)
                {
                    contextMenuNote.AddRange(ExtractOnLive(manager.GetFavoriteStreamClassList()));
                }
            }
            catch (Exception ex)
            {
                MyTraceSource.TraceEvent(TraceEventType.Error, ex);
                return;
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
                notifyIcon1.Icon = new System.Drawing.Icon(ICON_P_LOCAL_PATH);
            }
            else
                notifyIcon1.Icon = new System.Drawing.Icon(ICON_G_LOCAL_PATH);
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

        private async void timer3_Tick(object sender, EventArgs e)
        {
            timer3.Enabled = false;
            await UpdateListAndMenu();
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
