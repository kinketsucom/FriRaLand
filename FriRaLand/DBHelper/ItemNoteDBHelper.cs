using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriRaLand.DBHelper {
    class ItemNoteDBHelper {
        public const string DBname = "database.db";
        private SQLiteConnection conn;
        public ItemNoteDBHelper() {
            this.conn = new SQLiteConnection("Data Source=" + DBname + ";foreign keys = true;");
        }
        public class ItemNoteClass {
            public string itemid;
            public string bikou = "";
            public bool address_copyed = false; //住所転機済
        }
        public void onCreate() {
            conn.Open();
            string commandText = "CREATE TABLE IF NOT EXISTS itemnotes ( Id INTEGER PRIMARY KEY AUTOINCREMENT, itemid TEXT, bikou TEXT, address_copyed TEXT);";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }

        private void updateDB(ItemNoteClass itemnote) {
            try {
                conn.Open();
                string commandText = "update itemnotes set bikou = '" + itemnote.bikou.Replace("'", "''") + "', address_copyed = '" + itemnote.address_copyed.ToString() + "' where itemid = '" + itemnote.itemid + "';";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("商品備考DBの更新に成功");
            } catch (Exception ex) {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("商品備考DBの更新に失敗");
            }
        }

        private void insertDB(ItemNoteClass itemnote) {
            try {
                conn.Open();
                string commandText = "INSERT INTO itemnotes (itemid, bikou, address_copyed) VALUES ('" + itemnote.itemid + "','" + itemnote.bikou.Replace("'", "''") + "','" + itemnote.address_copyed.ToString() + "');";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("商品備考DBの挿入に成功");
            } catch (Exception ex) {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("商品備考DBの挿入に失敗");
            }
        }

        public void updateItemNote(ItemNoteClass itemnote) {
            //もしすでに商品備考があればupdate,なければinsert
            ItemNoteClass rst = getItemNote(itemnote.itemid);
            if (rst == null) insertDB(itemnote);
            else updateDB(itemnote);
        }

        //商品IDから商品備考を取得する　DBに商品IDがなければnull
        public ItemNoteClass getItemNote(string itemid) {
            ItemNoteClass rst = new ItemNoteClass();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.bikou, i.address_copyed from itemnotes i where itemid = '" + itemid + "';", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            bool ok = false;
            while (sQLiteDataReader.Read()) {
                try {
                    rst.bikou = sQLiteDataReader["bikou"].ToString();
                    rst.address_copyed = (sQLiteDataReader["address_copyed"].ToString() == "True");
                    rst.itemid = itemid;
                    ok = true;
                } catch (Exception ex) {
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            if (ok) return rst;
            else return null;
        }

        //出品履歴をすべてとってくる
        public List<ItemNoteClass> loadNotes() {
            List<ItemNoteClass> rst = new List<ItemNoteClass>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("SELECT * from itemnotes", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    ItemNoteClass itemnote = new ItemNoteClass();
                    itemnote.bikou = sQLiteDataReader["bikou"].ToString();
                    itemnote.address_copyed = (sQLiteDataReader["address_copyed"].ToString() == "True");
                    itemnote.itemid = sQLiteDataReader["itemid"].ToString();
                    rst.Add(itemnote);
                } catch (Exception ex) {
                    Log.Logger.Error("商品備考読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        public Dictionary<string, ItemNoteClass> loadItemNotesDictionary() {
            Dictionary<string, ItemNoteClass> rst = new Dictionary<string, ItemNoteClass>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("SELECT * from itemnotes", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    ItemNoteClass itemnote = new ItemNoteClass();
                    itemnote.bikou = sQLiteDataReader["bikou"].ToString();
                    itemnote.address_copyed = (sQLiteDataReader["address_copyed"].ToString() == "True");
                    itemnote.itemid = sQLiteDataReader["itemid"].ToString();
                    rst[itemnote.itemid] = itemnote;
                } catch (Exception ex) {
                    Log.Logger.Error("商品備考読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }
        //商品備考を削除する
        public bool deleteItemNote(string itemid) {
            try {
                conn.Open();
                string commandText = "delete from itemnotes where itemid = '" + itemid + "';";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("商品備考削除成功: " + itemid);
                return true;
            } catch (Exception ex) {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                Log.Logger.Error("商品備考削除失敗: " + itemid);
                return false;
            }
        }
    }
}
