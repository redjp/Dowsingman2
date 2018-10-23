using Dowsingman2.MyUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dowsingman2.BaseClass
{
    public abstract class AbstractManager
    {
        protected object lockobject = new object();
        protected List<StreamClass> liveStreamClassList_;
        protected List<StreamClass> favoriteStreamClassList_;

        public string Name { get; }
        public string FileName { get; }
        public string FilePath { get; }
        public bool IsRequest { get; protected set; }

        protected AbstractManager(string name, string fileName)
        {
            Name = name;
            FileName = fileName;
            FilePath = Path.GetFullPath(".\\favorite\\" + FileName);
            IsRequest = false;
            liveStreamClassList_ = new List<StreamClass>();

            Load();
        }

        public virtual ReadOnlyCollection<StreamClass> GetLiveStreamClassList()
        {
            lock (lockobject)
                return liveStreamClassList_.AsReadOnly();
        }

        public virtual ReadOnlyCollection<StreamClass> GetFavoriteStreamClassList()
        {
            lock (lockobject)
                return favoriteStreamClassList_.AsReadOnly();
        }

        public virtual bool AddFavorite(StreamClass newFavorite)
        {
            lock (lockobject)
            {
                if (!favoriteStreamClassList_.Exists(x => x.Owner == newFavorite.Owner))
                {
                    favoriteStreamClassList_.Add(newFavorite);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public virtual bool RemoveFavorite(StreamClass target)
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

        public abstract Task<List<StreamClass>> DownloadLiveAsync();
        public virtual async Task<bool> RefreshLiveAsync()
        {
            if (IsRequest)
            {
                MyTraceSource.TraceEvent(TraceEventType.Error, new StringBuilder(40).Append("[").Append(Name).Append("] RefreshLiveが重複").ToString());
                return false;
            }

            IsRequest = true;

            try
            {
                List<StreamClass> list = await DownloadLiveAsync();
                lock(lockobject)
                    liveStreamClassList_ = list;
                return true;
            }
            catch (Exception ex)
            {
                MyTraceSource.TraceEvent(TraceEventType.Error, ex);
                return false;
            }
            finally
            {
                IsRequest = false;
            }
        }

        public virtual List<StreamClass> CheckFavorite()
        {
            List<StreamClass> result = new List<StreamClass>();

            lock (lockobject)
            {
                if (liveStreamClassList_.Count > 0 && favoriteStreamClassList_.Count > 0)
                {
                    //現在の登録チャンネルリストを取得
                    List<StreamClass> streamList = new List<StreamClass>(favoriteStreamClassList_);
                    var statusList = streamList.Select(x => x.StreamStatus).ToList();
                    //配信情報を初期化
                    streamList = streamList.Select(x => new StreamClass(x.Owner)).ToList();

                    foreach (StreamClass st in GetLiveStreamClassList())
                    {
                        //一致する配信者名があった場合indexを取得
                        int idx = streamList.FindIndex(item => item.Owner == st.Owner);
                        if (idx >= 0)
                        {
                            //配信情報を上書き
                            streamList[idx] = st;

                            //新しく開始した配信なら通知スタックに追加
                            if (!statusList[idx])
                            {
                                result.Add(st);
                                statusList[idx] = true;
                            }
                        }
                    }

                    favoriteStreamClassList_ = streamList;
                }
            }

            //追加の通知スタックを返す
            return result;
        }

        public virtual void Load()
        {
            try
            {
                List<string> list = MySerializer.Deserialize<List<string>>(FilePath);
                lock (lockobject)
                    favoriteStreamClassList_ = list.Select(x => new StreamClass(x)).ToList();
            }
            catch(DirectoryNotFoundException)
            {
                MyTraceSource.TraceEvent(TraceEventType.Error, new StringBuilder(40).Append("[").Append(FileName).Append("] Favoriteフォルダが見つかりません").ToString());
                Directory.CreateDirectory(".\\favorite");
                InitStreamClassList();
            }
            catch(FileNotFoundException)
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

        public virtual void Save()
        {
            while (true)
            {
                List<string> list = GetFavoriteStreamClassList().Select(x => x.Owner).ToList();
                try
                {
                    MySerializer.Serialize<List<string>>(list, FilePath);
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

        protected virtual void InitStreamClassList()
        {
            lock (lockobject)
                favoriteStreamClassList_ = new List<StreamClass>();
        }
    }
}
