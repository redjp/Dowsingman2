using System;
using System.Media;
using System.Diagnostics;


namespace Dowsingman2.MyUtility
{
    public static class MyTools
    {
        public static void OpenBrowser(string url)
        {
            if (url != "")
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
