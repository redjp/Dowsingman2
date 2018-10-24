using Dowsingman2.BaseClass;
using Dowsingman2.SubManager;
using System;
using System.Collections.Generic;

using System.Media;
using System.Windows.Forms;

namespace Dowsingman2.MyUtility
{
    public static class MyBalloon
    {
        /// <summary>
        /// 通知スタック処理
        /// </summary>
        public static void ExcuteBalloonStack(List<StreamClass> stack, NotifyIcon notifyIcon, int time, string soundFilePath)
        {
            while (stack.Count > 0)
            {
                StreamClass streamClass = stack[0];
                stack.RemoveAt(0);

                //履歴を追加（重複チェックを兼ねる）
                if (LogManager.GetInstance().AddFavorite(streamClass))
                {
                    ShowBalloon(notifyIcon, streamClass, time);
                    MyTools.PlaySound(soundFilePath);
                    break;
                }
            }
        }

        /// <summary>
        /// バルーンを表示する
        /// </summary>
        public static string ShowBalloon(NotifyIcon notifyIcon, StreamClass sc, int time)
        {
            notifyIcon.BalloonTipTitle = sc.Title;
            notifyIcon.BalloonTipText = sc.Owner;
            notifyIcon.ShowBalloonTip(time);
            return sc.Url;
        }
    }
}
