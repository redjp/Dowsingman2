using System;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using Dowsingman2.SubManager;


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

        public static string ReplaceUnderScore(string value)
        {
            return value.Replace("_", "__");
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

            String fileName = SettingManager.GetInstance().GetBrowserPath();

            if (String.IsNullOrEmpty(fileName))
            {
                BrowserProcessStart(url);
            }
            else
            {
                BrowserProcessStart(fileName, url);
            }
        }

        #region --BrowserProcessStart
        private static void BrowserProcessStart(string url)
        {
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

        private static void BrowserProcessStart(string fileName, string url)
        {
            try
            {
                //指定のブラウザで配信URLを開く
                Process.Start(fileName, url);
            }
            //ファイルを開いてるときにエラーが発生
            catch (Win32Exception ex)
            {
                MyTraceSource.TraceEvent(TraceEventType.Error, ex);

                //規定のブラウザで配信URLを開く
                BrowserProcessStart(url);
            }
            catch (Exception ex)
            {
                MyTraceSource.TraceEvent(TraceEventType.Error, ex);
            }
        }
        #endregion

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
        public static DateTime? FormatDate(string dateString, string dateFormat, TimeZoneInfo timeZoneInfo)
        {
            if (String.IsNullOrEmpty(dateString)) return null;

            try
            {
                DateTime dateTime = DateTime.ParseExact(dateString, dateFormat, null);
                dateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZoneInfo);
                return dateTime.ToLocalTime();
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

        /// <summary>
        /// TryParseを1行で書くために用意
        /// https://qiita.com/Temarin/items/9aac6c1f569fc2113e0d
        /// </summary>
        public delegate bool TryParse<T, TValue>(T input, out TValue output);

        public static TValue TryParseOrDefault<T, TValue>(TryParse<T, TValue> tryParse, T input)
        {
            if (tryParse == null)
                throw new ArgumentNullException(nameof(tryParse));
            TValue outvalue;
            return tryParse(input, out outvalue) ? outvalue : default(TValue);
        }
    }
}
