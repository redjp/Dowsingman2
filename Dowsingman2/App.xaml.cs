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

            StaticClass.kukuluList = new List<StreamClass>();
            StaticClass.kukuluAll = new List<StreamClass>();
            StaticClass.fc2List = new List<StreamClass>();
            StaticClass.fc2All = new List<StreamClass>();
            StaticClass.twitchList = new List<StreamClass>();
            StaticClass.twitchAll = new List<StreamClass>();

            StaticClass.logList = new List<StreamClass>();

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            this.notifyIcon = new NotifyIconWrapper();

            #region XMLファイル読み込み
            //起動時にXMLファイルから読み込み
            string filePath = System.IO.Path.GetFullPath(@".\favorite\kukulu.xml");
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
                            StaticClass.kukuluList.Add(new StreamClass(str));
                        }
                    }
                    catch
                    {
                        StaticClass.fc2List = new List<StreamClass>();
                    }
            }
            }

            //起動時にXMLファイルから読み込みその2
            filePath = System.IO.Path.GetFullPath(@".\favorite\twitch.xml");
            if (File.Exists(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath, new UTF8Encoding(false)))
                {
                    try
                    {
                        List<string> loadList = new List<string>();
                        loadList = (List<string>)serializer.Deserialize(sr);

                        foreach (string str in loadList)
                        {
                            StaticClass.twitchList.Add(new StreamClass(str));
                        }
                    }
                    catch
                    {
                        StaticClass.fc2List = new List<StreamClass>();
                    }
                }
            }

            //起動時にXMLファイルから読み込みその3
            filePath = System.IO.Path.GetFullPath(@".\favorite\fc2.xml");
            if (File.Exists(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath, new UTF8Encoding(false)))
                {
                    try
                    {
                        List<string> loadList = new List<string>();
                        loadList = (List<string>)serializer.Deserialize(sr);

                        foreach (string str in loadList)
                        {
                            StaticClass.fc2List.Add(new StreamClass(str));
                        }
                    }
                    catch
                    {
                        StaticClass.fc2List = new List<StreamClass>();
                    }
                }
            }

            //起動時にXMLファイルから読み込み（履歴）
            filePath = System.IO.Path.GetFullPath(@".\favorite\log.xml");
            if (File.Exists(filePath))
            {
                //https://dobon.net/vb/dotnet/file/xmlserializer.html
                XmlSerializer serializer = new XmlSerializer(typeof(List<StreamClass>));
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath, new UTF8Encoding(false)))
                {
                    try
                    {
                        List<StreamClass> loadList = new List<StreamClass>();
                        loadList = (List<StreamClass>)serializer.Deserialize(sr);

                        foreach (StreamClass st in loadList)
                        {
                            StaticClass.logList.Add(st);
                        }
                    }
                    catch
                    {
                        StaticClass.logList = new List<StreamClass>();
                    }
                }
            }
            #endregion

            //起動時に配信をチェック
            notifyIcon.checkStreamIcon();
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

            #region XMLファイル書き込み
            //終了時にXMLファイルへ保存
            List<string> favoriteList = new List<string>();
            string filePath = System.IO.Path.GetFullPath(@".\favorite\kukulu.xml");
            foreach (StreamClass st in StaticClass.kukuluList)
            {
                favoriteList.Add(st.Owner);
            }
            //https://dobon.net/vb/dotnet/file/xmlserializer.html
            XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
            using (StreamWriter sw = new StreamWriter(filePath, false, new UTF8Encoding(false)))
            {
                serializer.Serialize(sw, favoriteList);
            }

            //終了時にXMLファイルへ保存その2
            favoriteList = new List<string>();
            filePath = System.IO.Path.GetFullPath(@".\favorite\twitch.xml");
            foreach (StreamClass st in StaticClass.twitchList)
            {
                favoriteList.Add(st.Owner);
            }
            //https://dobon.net/vb/dotnet/file/xmlserializer.html
            serializer = new XmlSerializer(typeof(List<string>));
            using (StreamWriter sw = new StreamWriter(filePath, false, new UTF8Encoding(false)))
            {
                serializer.Serialize(sw, favoriteList);
            }

            //終了時にXMLファイルへ保存その3
            favoriteList = new List<string>();
            filePath = System.IO.Path.GetFullPath(@".\favorite\fc2.xml");
            foreach (StreamClass st in StaticClass.fc2List)
            {
                favoriteList.Add(st.Owner);
            }
            //https://dobon.net/vb/dotnet/file/xmlserializer.html
            serializer = new XmlSerializer(typeof(List<string>));
            using (StreamWriter sw = new StreamWriter(filePath, false, new UTF8Encoding(false)))
            {
                serializer.Serialize(sw, favoriteList);
            }

            //終了時にXMLファイルへ保存（履歴）
            List<StreamClass> logList = new List<StreamClass>();
            filePath = System.IO.Path.GetFullPath(@".\favorite\log.xml");
            foreach (StreamClass st in StaticClass.logList)
            {
                favoriteList.Add(st.Owner);
            }
            //https://dobon.net/vb/dotnet/file/xmlserializer.html
            serializer = new XmlSerializer(typeof(List<StreamClass>));
            using (StreamWriter sw = new StreamWriter(filePath, false, new UTF8Encoding(false)))
            {
                serializer.Serialize(sw, logList);
            }
            #endregion
        }
    }
}
