using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace Dowsingman2
{
    using System.Windows;

    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// タスクトレイに表示するアイコン
        /// </summary>
        private NotifyIconWrapper notifyIcon;

        /// <summary>
        /// 多重起動を防止する為のミューテックス。
        /// </summary>
        private static Mutex _mutex;
        /// <summary>
        /// ミューテックス用のGUID
        /// </summary>
        private const string APPNAME_GUID = "Dowsingman2-{FAB552E3-0F8F-446F-90A0-EE05BDE8C8D1}";

        private static readonly string KUKULU_PATH = Path.GetFullPath(@".\favorite\kukulu.xml");
        private static readonly string TWITCH_PATH = Path.GetFullPath(@".\favorite\twitch.xml");
        private static readonly string FC2_PATH = Path.GetFullPath(@".\favorite\fc2.xml");
        private static readonly string CAVETUBE_PATH = Path.GetFullPath(@".\favorite\cavetube.xml");
        private static readonly string LOG_PATH = Path.GetFullPath(@".\favorite\log.xml");

        /// <summary>
        /// System.Windows.Application.Startup イベント を発生させます。
        /// </summary>
        /// <param name="e">イベントデータ を格納している StartupEventArgs</param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            //多重起動チェック
            App._mutex = new Mutex(false, APPNAME_GUID);
            if (!App._mutex.WaitOne(0, false))
            {
                MessageBox.Show("すでに起動しています！");
                App._mutex.Close();
                App._mutex = null;
                this.Shutdown();
                return;
            }

            //起動時にXMLファイルから読み込み
            Kukulu.List = (File.Exists(KUKULU_PATH)) ? (await DeserializeAsync<List<string>>(KUKULU_PATH))
                .Select(x => new StreamClass(x)).ToList() : new List<StreamClass>();
            Twitch.List = (File.Exists(TWITCH_PATH)) ? (await DeserializeAsync<List<string>>(TWITCH_PATH))
                .Select(x => new StreamClass(x)).ToList() : new List<StreamClass>();
            Fc2.List = (File.Exists(FC2_PATH)) ? (await DeserializeAsync<List<string>>(FC2_PATH))
                .Select(x => new StreamClass(x)).ToList() : new List<StreamClass>();
            Cavetube.List = (File.Exists(CAVETUBE_PATH)) ? (await DeserializeAsync<List<string>>(CAVETUBE_PATH))
                .Select(x => new StreamClass(x)).ToList() : new List<StreamClass>();
            StaticClass.logList = (File.Exists(LOG_PATH)) ? await DeserializeAsync<List<StreamClass>>(LOG_PATH)
                : new List<StreamClass>();

            base.OnStartup(e);
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            this.notifyIcon = new NotifyIconWrapper();
            //起動時に配信をチェック
            await notifyIcon.UpdateListAndMenu();
        }

        /// <summary>
        /// System.Windows.Application.Exit イベント を発生させます。
        /// </summary>
        /// <param name="e">イベントデータ を格納している ExitEventArgs</param>
        protected override void OnExit(ExitEventArgs e)
        {
            if (App._mutex == null) { base.OnExit(e); return; }

            //フォルダがなければ作成
            if (!Directory.Exists(@".\favorite"))
                Directory.CreateDirectory(@".\favorite");

            SaveAll();

            // ミューテックスの解放
            App._mutex.ReleaseMutex();
            App._mutex.Close();
            App._mutex = null;

            this.notifyIcon.Dispose();
            base.OnExit(e);
        }

        static void SaveAll()
        {
            //XMLファイルへ保存
            Serialize(Kukulu.List.Select(x => x.Owner).ToList(), Path.GetFullPath(@".\favorite\kukulu.xml"));
            Serialize(Twitch.List.Select(x => x.Owner).ToList(), Path.GetFullPath(@".\favorite\twitch.xml"));
            Serialize(Fc2.List.Select(x => x.Owner).ToList(), Path.GetFullPath(@".\favorite\fc2.xml"));
            Serialize(Cavetube.List.Select(x => x.Owner).ToList(), Path.GetFullPath(@".\favorite\cavetube.xml"));
            Serialize(StaticClass.logList, Path.GetFullPath(@".\favorite\log.xml"));
        }

        // 排他ロックに使うSemaphoreSlimオブジェクト
        // （プロセス間の排他が必要なときはSemaphoreオブジェクトに変える）
        static System.Threading.SemaphoreSlim _semaphore
          = new System.Threading.SemaphoreSlim(1, 1);

        // シリアライズする
        static async Task SerializeAsync<T>(T data, string filePath)
        {
            await _semaphore.WaitAsync(); // ロックを取得する
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                using (var streamWriter = new StreamWriter(filePath, false, new UTF8Encoding(false)))
                {
                    await Task.Run(() => xmlSerializer.Serialize(streamWriter, data));
                    await streamWriter.FlushAsync();  // .NET Framework 4.5以降
                }
            }
            finally
            {
                _semaphore.Release(); // ロックを解放する
            }
        }

        static void Serialize<T>(T data, string filePath)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var streamWriter = new StreamWriter(filePath, false, new UTF8Encoding(false)))
            {
                xmlSerializer.Serialize(streamWriter, data);
            }
        }

        // デシリアライズする
        static async Task<T> DeserializeAsync<T>(string filePath)
        {
            await _semaphore.WaitAsync(); // ロックを取得する
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                var xmlSettings = new System.Xml.XmlReaderSettings()
                {
                    CheckCharacters = false,
                };
                using (var streamReader = new StreamReader(filePath, new UTF8Encoding(false)))
                using (var xmlReader = System.Xml.XmlReader.Create(streamReader, xmlSettings))
                {
                    return await Task.Run(() => (T)xmlSerializer.Deserialize(xmlReader));
                }
            }
            finally
            {
                _semaphore.Release(); // ロックを解放する
            }
        }
    }
}
