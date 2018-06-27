using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace FriLand.DBHelper {
    class ZaikoDBHelper {

        public const string DBname = "database.db";
        private SQLiteConnection conn;
        public ZaikoDBHelper() {
            this.conn = new SQLiteConnection("Data Source=" + DBname + ";foreign keys = true;");
        }

        public void onCreate() {
            conn.Open();
            string commandText = "CREATE TABLE IF NOT EXISTS zaiko ("
                                + "parent_id TEXT, zaikonum INTEGER, PRIMARY KEY(parent_id));";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }

        public class ZaikoInfo {
            public string parent_id;
            public int zaikonum;
        }
        //public : 在庫情報を新規作成または更新する
        public void updateZaikoInfo(string parent_id, int zaikonum) {
            parent_id = parent_id.ToLower();
            //該当データがあるか調べる
            int nownum = getZaikoNum(parent_id);
            bool parent_exist = isexistParentid(parent_id);
            if (parent_exist == false) {
                addZaikoDB(parent_id, zaikonum);
            } else {
                updateZaikoDB(parent_id, zaikonum);
            }
        }
        //private :在庫情報を追加する 在庫数はデフォルト値の0
        private void addZaikoDB(string parent_id, int zaikonum = 0) {
            parent_id = parent_id.ToLower();
            conn.Open();
            string commandText = "INSERT INTO zaiko (parent_id, zaikonum) VALUES ('" + parent_id + "', " + zaikonum.ToString() + ");";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }
        //private :在庫情報を更新する
        private void updateZaikoDB(string parent_id, int zaikonum) {
            parent_id = parent_id.ToLower();
            try {
                conn.Open();
                string commandText = "update zaiko set zaikonum = " + zaikonum.ToString() + " where parent_id = '" + parent_id + "';";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("在庫情報の更新に成功");
            } catch (Exception ex) {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("在庫情報の更新に失敗");
            }
        }
        //在庫情報をロードする
        public List<ZaikoInfo> loadZaikoData() {
            List<ZaikoInfo> rst = new List<ZaikoInfo>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.parent_id, i.zaikonum from zaiko i;", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    ZaikoInfo zi = new ZaikoInfo();
                    zi.parent_id = sQLiteDataReader["parent_id"].ToString().ToLower();
                    zi.zaikonum = int.Parse(sQLiteDataReader["zaikonum"].ToString());
                    rst.Add(zi);
                } catch (Exception ex) {
                    Log.Logger.Error("在庫データ読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        //指定したparent_idの在庫数を取得する　該当レコードがなければint_minをかえす
        public int getZaikoNum(string parent_id) {
            parent_id = parent_id.ToLower();
            int rst = int.MinValue;
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.zaikonum from zaiko i where parent_id = '" + parent_id + "';", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    rst = int.Parse(sQLiteDataReader["zaikonum"].ToString());
                } catch (Exception ex) {
                    Log.Logger.Error(string.Format("parent_idが{0}の在庫情報存在せず: ", parent_id) + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }
        //parent_idが存在するか
        public bool isexistParentid(string parent_id) {
            parent_id = parent_id.ToLower();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.zaikonum from zaiko i where parent_id = '" + parent_id + "';", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            bool res = false;
            while (sQLiteDataReader.Read()) {
                try {
                    int tmp = int.Parse(sQLiteDataReader["zaikonum"].ToString());
                    res = true;
                } catch (Exception ex) {
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return res;
        }

        public bool deleteZaikoInfo(string parent_id) {
            parent_id = parent_id.ToLower();
            try {
                conn.Open();
                string commandText = "delete from zaiko where parent_id = '" + parent_id + "';";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("在庫情報の削除に成功");
                return true;
            } catch (Exception ex) {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("在庫情報の削除に失敗");
                return false;
            }
        }
    }
}