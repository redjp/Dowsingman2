using Dowsingman2.BaseClass;
using Dowsingman2.UtilityClass;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Dowsingman2.SubManager
{
    public class ContextMenuManager
    {
        public List<StreamClass> ContextMenuList { get; private set; }
        public ContextMenuStrip ContextMenuStrip { get; }

        public ContextMenuManager(ContextMenuStrip contextMenuStrip)
        {
            ContextMenuList = new List<StreamClass>();
            ContextMenuStrip = contextMenuStrip;
        }

        /// <summary>
        /// コンテキストメニューの再描画
        /// </summary>
        public bool RefreshContextMenu(IEnumerable<AbstractManager> managers)
        {
            //コンテキストメニューの3項目目以降を削除
            while (ContextMenuStrip.Items.Count > 2)
            {
                ContextMenuStrip.Items.RemoveAt(0);
            }

            //コンテキストメニューの配信情報を初期化
            ContextMenuList.Clear();
            //配信中のお気に入り配信を追加
            foreach (AbstractManager manager in managers)
            {
                ContextMenuList.AddRange(manager.GetFavoriteLiveOnly());
            }

            //配信がなければfalseを返す
            if (ContextMenuList.Count <= 0) return false;

            //セパレータを追加
            ContextMenuStrip.Items.Insert(0, new ToolStripSeparator());

            foreach (StreamClass sc in ContextMenuList)
            {
                //セパレータの上に配信を追加
                ToolStripMenuItem tsi = new ToolStripMenuItem(sc.Owner, null, ContextMenu_Clicked);
                ContextMenuStrip.Items.Insert(ContextMenuStrip.Items.Count - 3, tsi);
            }
        
            return true;
        }

        /// <summary>
        /// 追加コンテキストメニューがクリックされたとき処理
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void ContextMenu_Clicked(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;

            //クリックされた名前と同じ名前をリストから探してURLを開く
            foreach (StreamClass st in ContextMenuList)
            {
                if (st.Owner == item.Text)
                {
                    MyUtility.OpenBrowser(st.Url);
                    return;
                }
            }
        }
    }
}
