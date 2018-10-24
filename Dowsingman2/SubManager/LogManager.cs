using Dowsingman2.BaseClass;
using Dowsingman2.MyUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dowsingman2.SubManager
{
    class LogManager : AbstractManager
    {
        private static LogManager instance_ = new LogManager();
        public static LogManager GetInstance()
        {
            return instance_;
        }

        public int maxLogSize { get; private set; }

        private LogManager() : base("Log", "log.xml")
        {
            maxLogSize = 100;

            Load();
        }

        public override bool AddFavorite(StreamClass newFavorite)
        {
            lock (lockobject)
            {
                if (favoriteStreamClassList_.Exists(x =>
                                new TimeSpan(0, -1, 0) < x.Start_Time - newFavorite.Start_Time
                                && x.Start_Time - newFavorite.Start_Time < new TimeSpan(0, 1, 0)
                                && x.Owner == newFavorite.Owner))
                {
                    return false;
                }
                else
                {
                    favoriteStreamClassList_.Add(newFavorite);
                    //最大サイズを超えていれば古い方から削除
                    while (favoriteStreamClassList_.Count > maxLogSize)
                    {
                        favoriteStreamClassList_.RemoveAt(0);
                    }
                    return true;
                }
            }
        }

        public override bool RemoveFavorite(StreamClass target)
        {
            lock (lockobject)
            {
                if (favoriteStreamClassList_.Exists(x => x == target))
                {
                    favoriteStreamClassList_.Remove(target);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override Task<List<StreamClass>> DownloadLiveAsync()
        {
            throw new NotImplementedException();
        }

        public override void Load()
        {
            try
            {
                List<StreamClass> list = MySerializer.Deserialize<List<StreamClass>>(FilePath);
                lock(lockobject)
                    favoriteStreamClassList_ = list;
            }
            catch (DirectoryNotFoundException)
            {
                MyTraceSource.TraceEvent(TraceEventType.Error, new StringBuilder(40).Append("[").Append(FileName).Append("] Favoriteフォルダが見つかりません").ToString());
                Directory.CreateDirectory(".\\favorite");
                InitStreamClassList();
            }
            catch (FileNotFoundException)
            {
                MyTraceSource.TraceEvent(TraceEventType.Error, new StringBuilder(40).Append("[").Append(FileName).Append("] ファイルが見つかりません").ToString());
                InitStreamClassList();
            }
            catch (Exception ex)
            {
                MyTraceSource.TraceEvent(TraceEventType.Error, ex);
                InitStreamClassList();
            }

            if (favoriteStreamClassList_ == null)
                InitStreamClassList();
        }

        public override void Save()
        {
            while (true)
            {
                List<StreamClass> list = GetFavoriteStreamClassList().ToList();
                try
                {
                    MySerializer.Serialize<List<StreamClass>>(list, FilePath);
                }
                catch (DirectoryNotFoundException)
                {
                    MyTraceSource.TraceEvent(TraceEventType.Error, new StringBuilder(40).Append("[").Append(FileName).Append("] Favoriteフォルダが見つかりません").ToString());
                    Directory.CreateDirectory(".\\favorite");
                    continue;
                }
                catch (Exception ex)
                {
                    MyTraceSource.TraceEvent(TraceEventType.Error, ex);
                }
                break;
            }
        }

        protected override void InitStreamClassList()
        {
            lock(lockobject)
                favoriteStreamClassList_ = new List<StreamClass>();
        }
    }
}
