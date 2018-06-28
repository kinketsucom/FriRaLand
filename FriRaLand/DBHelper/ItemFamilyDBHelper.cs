using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace RakuLand.DBHelper {
    class ItemFamilyDBHelper {

        public const string DBname = "database.db";
        private SQLiteConnection conn;
        public ItemFamilyDBHelper() {
            this.conn = new SQLiteConnection("Data Source=" + DBname + ";foreign keys = true;");
        }
        public class ItemFamily {
            public string parent_id;
            public string child_id;
            public int itemDBId;
        }
        public void onCreate() {
            conn.Open();
            string commandText = "CREATE TABLE IF NOT EXISTS item_family ("
                                + "parent_id TEXT, child_id TEXT, itemDBId INTEGER,"
                                + "PRIMARY KEY(parent_id, child_id),"
                                + "foreign key(itemDBId) references items(Id) on delete cascade on update cascade "
                                + "foreign key(parent_id) references zaiko(parent_id) on delete cascade on update cascade);";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }
        //親子情報を追加する
        public bool addItemFamily(string parent_id, string child_id, int itemDBId) {
            parent_id = parent_id.ToLower();
            child_id = child_id.ToLower();
            //もしすでに同じ商品情報がある,あるいは存在しないitemDBIdに対しての情報を追加するとエラーになる
            try {
                conn.Open();
                string commandText = "INSERT INTO item_family (parent_id, child_id, itemDBId) VALUES ('" + parent_id + "','" + child_id + "'," + itemDBId.ToString() + ");";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("親子情報の挿入に成功");
                return true;
            } catch (Exception ex) {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                Log.Logger.Error("親子情報の挿入に失敗");
                return false;
            }
        }
        //親子情報を更新する
        public bool updateItemFamily(string old_parent_id, string old_child_id, string new_parent_id, string new_child_id) {
            old_parent_id = old_parent_id.ToLower();
            old_child_id = old_child_id.ToLower();
            new_parent_id = new_parent_id.ToLower();
            new_child_id = new_child_id.ToLower();
            try {
                conn.Open();
                string commandText = "update item_family set parent_id = '" + new_parent_id + "', child_id = '" + new_child_id + "' where parent_id = '" + old_parent_id + "' and child_id = '" + old_child_id + "';";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("親子情報の更新に成功");
                return true;
            } catch (Exception ex) {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("親子情報の更新に失敗");
                return false;
            }
        }
        //親子情報をロードする
        public Dictionary<string, List<ItemFamily>> loadItemFamily() {
            Dictionary<string, List<ItemFamily>> rst = new Dictionary<string, List<ItemFamily>>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.parent_id, i.child_id, i.itemDBId from item_family i;", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    ItemFamily itemfamily = new ItemFamily();
                    itemfamily.parent_id = sQLiteDataReader["parent_id"].ToString().ToLower();
                    itemfamily.child_id = sQLiteDataReader["child_id"].ToString().ToLower();
                    itemfamily.itemDBId = int.Parse(sQLiteDataReader["itemDBId"].ToString());
                    if (rst.ContainsKey(itemfamily.parent_id) == false) rst[itemfamily.parent_id] = new List<ItemFamily>();
                    rst[itemfamily.parent_id].Add(itemfamily);
                } catch (Exception ex) {
                    Log.Logger.Error("親子情報読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        //親情報一覧を取得
        public List<string> getParentList() {
            List<string> rst = new List<string>();
            var dic = loadItemFamily();
            foreach (var pair in dic) {
                rst.Add(pair.Key);
            }
            return rst;
        }
        //商品IDから親子情報を取得,該当データがなければnull
        public ItemFamily getItemFamilyFromItemDBId(int itemDBId) {
            ItemFamily rst = new ItemFamily();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.parent_id, i.child_id, i.itemDBId from item_family i where itemDBId =" + itemDBId.ToString() + ";", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            bool ok = false;
            while (sQLiteDataReader.Read()) {
                try {
                    rst.parent_id = sQLiteDataReader["parent_id"].ToString().ToLower();
                    rst.child_id = sQLiteDataReader["child_id"].ToString().ToLower();
                    rst.itemDBId = int.Parse(sQLiteDataReader["itemDBId"].ToString());
                    ok = true;
                } catch (Exception ex) {
                    Log.Logger.Error("親子情報読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            if (ok) return rst;
            else return null;
        }

        //親子情報から商品IDを取得 なければ-1
        public int getItemDBIdFromParentChild(string parent_id, string child_id) {
            parent_id = parent_id.ToLower();
            child_id = child_id.ToLower();
            int rst = -1;
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.itemDBId from item_family i where parent_id = '" + parent_id + "'" + "and child_id = '" + child_id + "';", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    rst = int.Parse(sQLiteDataReader["itemDBId"].ToString());
                } catch (Exception ex) {
                    Log.Logger.Error("親子情報読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        //在庫にあるitemidを取得
        public Dictionary<int, string> getItemIdToParentIdDictionary() {
            Dictionary<int, string> rst = new Dictionary<int, string>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.parent_id, i.child_id, i.itemDBId from item_family i;", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    ItemFamily itemfamily = new ItemFamily();
                    itemfamily.parent_id = sQLiteDataReader["parent_id"].ToString().ToLower();
                    itemfamily.child_id = sQLiteDataReader["child_id"].ToString().ToLower();
                    itemfamily.itemDBId = int.Parse(sQLiteDataReader["itemDBId"].ToString());
                    rst[itemfamily.itemDBId] = itemfamily.parent_id;
                } catch (Exception ex) {
                    Log.Logger.Error("親子情報読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        //親子情報を削除する
        public bool deleteItemFamily(string parent_id, string child_id) {
            parent_id = parent_id.ToLower();
            child_id = child_id.ToLower();
            try {
                conn.Open();
                string commandText = "delete from item_family where parent_id = '" + parent_id + "' and child_id = '" + child_id + "';";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("親子情報の削除に成功");
                return true;
            } catch (Exception ex) {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("親子情報の削除に失敗");
                return false;
            }
        }
    }
}