using Dowsingman2.BaseClass;
using Dowsingman2.UtilityClass;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Dowsingman2.SubManager
{
    public class BalloonManager
    {
        private object lockObject_ = new object();

        public Queue<StreamClass> BalloonQueue { get; private set; }
        public NotifyIcon NotifyIcon { get; }

        public BalloonManager(NotifyIcon notifyIcon)
        {
            BalloonQueue = new Queue<StreamClass>();
            NotifyIcon = notifyIcon;
        }

        public void AddRange(IEnumerable<StreamClass> streams)
        {
            lock (lockObject_)
            {
                foreach (StreamClass stream in streams)
                {
                    BalloonQueue.Enqueue(stream);
                }
            }
        }

        /// <summary>
        /// 通知スタック処理（残りのキューの数を返す）
        /// </summary>
        public int ExcuteBalloonQueue(int time, string soundFilePath)
        {
            StreamClass streamClass;
            while (BalloonQueue.Count > 0)
            {
                lock (lockObject_)
                    streamClass = BalloonQueue.Dequeue();

                //履歴を追加（重複チェックを兼ねる）
                if (!LogManager.GetInstance().AddFavorite(streamClass)) continue;
                
                ShowBalloon(streamClass, time);
                MyUtility.PlaySound(soundFilePath);
                break;
            }
            return BalloonQueue.Count;
        }

        /// <summary>
        /// バルーンを表示する
        /// </summary>
        public string ShowBalloon(StreamClass sc, int time)
        {
            NotifyIcon.BalloonTipTitle = sc.Title;
            NotifyIcon.BalloonTipText = sc.Owner;
            NotifyIcon.ShowBalloonTip(time);
            return sc.Url;
        }
    }
}
