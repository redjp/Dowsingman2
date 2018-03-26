using System.Collections.Generic;

namespace Dowsingman2
{
    public static class StaticClass
    {
        /// <summary>
        /// List : お気に入り配信一覧
        /// All : 配信一覧
        /// </summary>
        public static List<StreamClass> kukuluList { get; set; }
        public static List<StreamClass> kukuluAll { get; set; }
        public static List<StreamClass> fc2List { get; set; }
        public static List<StreamClass> fc2All { get; set; }
        public static List<StreamClass> twitchList { get; set; }
        public static List<StreamClass> twitchAll { get; set; }

        /// <summary>
        /// 履歴
        /// </summary>
        public static List<StreamClass> logList{ get; set; }
    }
}