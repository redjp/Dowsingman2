using System;
using System.Media;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace Dowsingman2.UtilityClass
{
    internal class MyUtility
    {
        private static Regex htmlRegex_;
        private static Regex specialCharsRegex_;

        static MyUtility()
        {
            htmlRegex_ = new Regex("<.+?>", RegexOptions.Compiled);
            specialCharsRegex_ = new Regex("[\\00-\\x08\\x0b\\x0c\\x0e-\\x1f\\x7f]", RegexOptions.Compiled);
        }

        /// <summary>
        /// 改行コードを削除する
        /// </summary>
        public static string RemoveCRLF(string value)
        {
            return value.Replace("\r", string.Empty).Replace("\n", string.Empty);
        }

        /// <summary>
        /// HTMLタグを削除する
        /// </summary>
        public static string RemoveHtmlTag(string value)
        {
            return htmlRegex_.Replace(value, string.Empty);
        }

        /// <summary>
        /// 制御文字を削除する
        /// </summary>
        public static string RemoveSpecialChars(string value)
        {
            return specialCharsRegex_.Replace(value, string.Empty);
        }

        /// <summary>
        /// ブラウザを開く
        /// </summary>
        public static void OpenBrowser(string url)
        {
            if (url != string.Empty)
                try
                {
                    //規定のブラウザで配信URLを開く
                    Process.Start(url);
                }
                catch (Exception ex)
                {
                    MyTraceSource.TraceEvent(TraceEventType.Error, ex);
                }
        }

        /// <summary>
        /// 音を鳴らす
        /// </summary>
        public static void PlaySound(string path)
        {
            using (var player = new SoundPlayer(path))
            {
                player.Play();
            }
        }
    }
}
