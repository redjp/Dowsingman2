using Dowsingman2.BaseClass;
using Dowsingman2.UtilityClass;
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
    public class LogManager : AbstractManager
    {
        private static LogManager instance_ = new LogManager();
        public static LogManager GetInstance() { return instance_; }

        public int maxLogSize { get; private set; }

        private LogManager() : base("Log", "log.xml")
        {
            maxLogSize = 100;
        }

        public override ReadOnlyCollection<StreamClass> GetFavoriteStreamClassList()
        {
            return favoriteStreamClassList_.OrderByDescending(x => x.Start_Time).ToList().AsReadOnly();
        }

        public override bool AddFavorite(StreamClass newFavorite)
        {
            lock (lockobject_)
            {
                if (favoriteStreamClassList_.Exists(x =>
                                x.Start_Time == newFavorite.Start_Time
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
            lock (lockobject_)
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
#if DEBUG
            MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[").Append(FileName).Append("] ファイル読み込み開始").ToString());
#endif
            try
            {
                List<StreamClass> list = MySerializer.Deserialize<List<StreamClass>>(FilePath);
                lock(lockobject_)
                    favoriteStreamClassList_ = list;
            }
            catch (DirectoryNotFoundException)
            {
#if DEBUG
                MyTraceSource.TraceEvent(TraceEventType.Error, new StringBuilder(40).Append("[").Append(FileName).Append("] Favoriteフォルダが見つかりません").ToString());
#endif
                Directory.CreateDirectory("favorite");
                InitStreamClassList();
            }
            catch (FileNotFoundException)
            {
#if DEBUG
                MyTraceSource.TraceEvent(TraceEventType.Error, new StringBuilder(40).Append("[").Append(FileName).Append("] ファイルが見つかりません").ToString());
#endif
                InitStreamClassList();
            }
            catch (Exception ex)
            {
                MyTraceSource.TraceEvent(TraceEventType.Error, ex);
                InitStreamClassList();
            }

            if (favoriteStreamClassList_ == null)
                InitStreamClassList();

#if DEBUG
            MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[").Append(FileName).Append("] ファイル読み込み完了").ToString());
#endif
        }

        public override void Save()
        {
#if DEBUG
            MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[").Append(FileName).Append("] ファイル保存開始").ToString());
#endif
            List<StreamClass> list;
            lock (lockobject_)
                list = GetFavoriteStreamClassList().ToList();

            while (true)
            {
                try
                {
                    MySerializer.Serialize<List<StreamClass>>(list, FilePath);
                }
                catch (DirectoryNotFoundException)
                {
#if DEBUG
                    MyTraceSource.TraceEvent(TraceEventType.Error, new StringBuilder(40).Append("[").Append(FileName).Append("] Favoriteフォルダが見つかりません").ToString());
#endif
                    Directory.CreateDirectory("favorite");
                    continue;
                }
                catch (Exception ex)
                {
                    MyTraceSource.TraceEvent(TraceEventType.Error, ex);
                }
                break;
            }
#if DEBUG
            MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[").Append(FileName).Append("] ファイル保存完了").ToString());
#endif
        }

        protected override void InitStreamClassList()
        {
            lock(lockobject_)
                favoriteStreamClassList_ = new List<StreamClass>();
        }
    }
}
