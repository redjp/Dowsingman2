using System;
using System.Media;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;


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
            if (String.IsNullOrEmpty(url)) return;

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

        /// <summary>
        /// 日時をstringからDateTimeへ変換
        /// </summary>
        public static DateTime? FormatDate(string dateString, string dateFormat, bool isUniversal)
        {
            if (String.IsNullOrEmpty(dateString)) return null;

            try
            {
                DateTime dateTime = DateTime.ParseExact(dateString, dateFormat, null);
                return isUniversal ? dateTime.ToLocalTime() : dateTime;
            }
            catch (FormatException)
            {
#if DEBUG
                MyTraceSource.TraceEvent(TraceEventType.Error, new StringBuilder(40).Append("[").Append(dateString).Append("] is not [").Append(dateFormat).Append("]").ToString());
#endif
                return null;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 日時をstringからDateTimeへ変換
        /// </summary>
        public static DateTime? FormatDate(string dateString, string dateFormat, bool isUniversal, DateTimeFormatInfo formatInfo, DateTimeStyles styles)
        {
            if (String.IsNullOrEmpty(dateString)) return null;

            try
            {
                DateTime dateTime = DateTime.ParseExact(dateString, dateFormat, formatInfo, styles);
                return isUniversal ? dateTime.ToLocalTime() : dateTime;
            }
            catch (FormatException)
            {
#if DEBUG
                MyTraceSource.TraceEvent(TraceEventType.Error, new StringBuilder(40).Append("[").Append(dateString).Append("] is not [").Append(dateFormat).Append("]").ToString());
#endif
                return null;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
