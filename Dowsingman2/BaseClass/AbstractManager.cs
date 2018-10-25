using Dowsingman2.UtilityClass;
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

        /// <summary>
        /// お気に入りのうち配信中のものだけをリストで返す
        /// </summary>
        public virtual ReadOnlyCollection<StreamClass> GetFavoriteLiveOnly()
        {
            lock (lockobject)
                return favoriteStreamClassList_.Where(x => x.StreamStatus).ToList().AsReadOnly();
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
#if DEBUG
            MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[").Append(Name).Append("] Httpリクエスト開始").ToString());
#endif

            try
            {
                List<StreamClass> list = await DownloadLiveAsync();
                lock (lockobject)
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
#if DEBUG
                MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[").Append(Name).Append("] Httpリクエスト完了").ToString());
#endif
            }
        }

        /// <summary>
        /// 配信一覧とお気に入り一覧を比較してお気に入り一覧を変更する
        /// </summary>
        /// <returns>新しく開始した配信のリスト</returns>
        public virtual List<StreamClass> CheckFavorite()
        {
            List<StreamClass> result = new List<StreamClass>();

#if DEBUG
            MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[").Append(Name).Append("] お気に入りチェック開始").ToString());
#endif
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

#if DEBUG
            MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[").Append(Name).Append("] お気に入りチェック完了").ToString());
#endif

            //追加の通知スタックを返す
            return result;
        }

        public virtual void Load()
        {
#if DEBUG
            MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[").Append(FileName).Append("] ファイル読み込み開始").ToString());
#endif
            try
            {
                List<string> list = MySerializer.Deserialize<List<string>>(FilePath);
                lock (lockobject)
                    favoriteStreamClassList_ = list.Select(x => new StreamClass(x)).ToList();
            }
            catch (DirectoryNotFoundException)
            {
#if DEBUG
                MyTraceSource.TraceEvent(TraceEventType.Error, new StringBuilder(40).Append("[").Append(FileName).Append("] Favoriteフォルダが見つかりません").ToString());
#endif
                Directory.CreateDirectory(".\\favorite");
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

        public virtual void Save()
        {
#if DEBUG
            MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[").Append(FileName).Append("] ファイル保存開始").ToString());
#endif
            while (true)
            {
                List<string> list = GetFavoriteStreamClassList().Select(x => x.Owner).ToList();
                try
                {
                    MySerializer.Serialize<List<string>>(list, FilePath);
                }
                catch (DirectoryNotFoundException)
                {
#if DEBUG
                    MyTraceSource.TraceEvent(TraceEventType.Error, new StringBuilder(40).Append("[").Append(FileName).Append("] Favoriteフォルダが見つかりません").ToString());
#endif
                    Directory.CreateDirectory(".\\favorite");
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

        protected virtual void InitStreamClassList()
        {
            lock (lockobject)
                favoriteStreamClassList_ = new List<StreamClass>();
        }
    }
}
