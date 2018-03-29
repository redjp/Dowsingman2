using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

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
        /// System.Windows.Application.Startup イベント を発生させます。
        /// </summary>
        /// <param name="e">イベントデータ を格納している StartupEventArgs</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            this.notifyIcon = new NotifyIconWrapper();

            //起動時にXMLファイルから読み込み
            StaticClass.kukuluList = FileToListString(System.IO.Path.GetFullPath(@".\favorite\kukulu.xml"));
            StaticClass.twitchList = FileToListString(System.IO.Path.GetFullPath(@".\favorite\twitch.xml"));
            StaticClass.fc2List = FileToListString(System.IO.Path.GetFullPath(@".\favorite\fc2.xml"));
            
            //起動時にXMLファイルから読み込み（履歴）
            StaticClass.logList = FileToList(System.IO.Path.GetFullPath(@".\favorite\log.xml"));

            //起動時に配信をチェック
            notifyIcon.UpdateListAndMenu();
        }

        /// <summary>
        /// System.Windows.Application.Exit イベント を発生させます。
        /// </summary>
        /// <param name="e">イベントデータ を格納している ExitEventArgs</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Com.ComRelease.FinalReleaseComObjects(notifyIcon);
            this.notifyIcon.Dispose();

            //終了時にXMLファイルへ保存
            ListToFileString(StaticClass.kukuluList, System.IO.Path.GetFullPath(@".\favorite\kukulu.xml"));
            ListToFileString(StaticClass.twitchList, System.IO.Path.GetFullPath(@".\favorite\twitch.xml"));
            ListToFileString(StaticClass.fc2List, System.IO.Path.GetFullPath(@".\favorite\fc2.xml"));

            //終了時にXMLファイルへ保存（履歴）
            ListToFile(StaticClass.logList, System.IO.Path.GetFullPath(@".\favorite\log.xml"));
        }

        /// <summary>
        /// XMLファイルからリストを読み込み
        /// </summary>
        /// <param name="filePath">XMLファイルのフルパス</param>
        /// <returns>読み込まれたリスト</returns>
        private List<StreamClass> FileToList(string filePath)
        {
            //戻り値
            var list = new List<StreamClass>();

            if (File.Exists(filePath))
            {
                //https://dobon.net/vb/dotnet/file/xmlserializer.html
                XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath, new UTF8Encoding(false)))
                {
                    try
                    {
                        List<string> loadList = new List<string>();
                        loadList = (List<string>)serializer.Deserialize(sr);

                        foreach (string str in loadList)
                        {
                            list.Add(new StreamClass(str));
                        }
                    }
                    catch
                    {
                        list = new List<StreamClass>();
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// XMLファイルからリストを読み込み
        /// </summary>
        /// <param name="filePath">XMLファイルのフルパス</param>
        /// <returns>読み込まれたリスト</returns>
        private List<StreamClass> FileToListString(string filePath)
        {
            //戻り値
            var list = new List<StreamClass>();

            if (File.Exists(filePath))
            {
                //https://dobon.net/vb/dotnet/file/xmlserializer.html
                XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath, new UTF8Encoding(false)))
                {
                    try
                    {
                        List<string> loadList = new List<string>();
                        loadList = (List<string>)serializer.Deserialize(sr);

                        foreach (string str in loadList)
                        {
                            list.Add(new StreamClass(str));
                        }
                    }
                    catch
                    {
                        list = new List<StreamClass>();
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// リストをXMLファイルへ保存
        /// </summary>
        /// <param name="saveList">保存するリスト</param>
        /// <param name="filePath">XMLファイルのフルパス</param>
        private void ListToFile(List<StreamClass> saveList, string filePath)
        {
            //https://dobon.net/vb/dotnet/file/xmlserializer.html
            XmlSerializer serializer = new XmlSerializer(typeof(List<StreamClass>));
            using (StreamWriter sw = new StreamWriter(filePath, false, new UTF8Encoding(false)))
            {
                try
                {
                    serializer.Serialize(sw, saveList);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// リストをXMLファイルへ保存
        /// </summary>
        /// <param name="saveList">保存するリスト</param>
        /// <param name="filePath">XMLファイルのフルパス</param>
        private void ListToFileString(List<StreamClass> saveList, string filePath)
        {
            List<string> saveOwnerList = new List<string>();
            foreach (StreamClass st in saveList)
            {
                saveOwnerList.Add(st.Owner);
            }
            //https://dobon.net/vb/dotnet/file/xmlserializer.html
            XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
            using (StreamWriter sw = new StreamWriter(filePath, false, new UTF8Encoding(false)))
            {
                try
                {
                    serializer.Serialize(sw, saveOwnerList);
                }
                catch
                {
                }
            }
        }
    }
}
