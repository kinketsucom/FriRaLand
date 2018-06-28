using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RakuLand.DBHelper {
    class ShuppinRirekiDBHelper {

        public const string DBname = "database.db";
        private SQLiteConnection conn;
        public ShuppinRirekiDBHelper() {
            this.conn = new SQLiteConnection("Data Source=" + DBname + ";foreign keys = true;");
        }

        public void onCreate() {
            conn.Open();
            string commandText = "CREATE TABLE IF NOT EXISTS shuppinrireki ( id INTEGER PRIMARY KEY AUTOINCREMENT,"
                                + "itemDBId INTEGER, accountDBId INTEGER, item_id TEXT);";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }
        private int LoadUserVersion() {
            //バージョン情報読み取り
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("pragma user_version", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            int user_version = -1;
            while (sQLiteDataReader.Read()) {
                try {
                    user_version = int.Parse(sQLiteDataReader["user_version"].ToString());
                } catch (Exception ex) {
                    Log.Logger.Error("DBのuser_versionの読み込み失敗");
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return user_version;
        }
        public void addCreatedDateColumn() {
            int user_version = LoadUserVersion();
            //更新
            if (user_version < 6) {
                conn.Open();
                string commandText = "alter table shuppinrireki add created_date TEXT; update shuppinrireki set created_date = '" + DateTime.Now.ToString() + "'; pragma user_version = 6;";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
        }
        public void addUretaColumn() {//Ver4.1で廃止
            int user_version = LoadUserVersion();
            //更新
            if (user_version < 7) {
                conn.Open();
                string commandText = "alter table shuppinrireki add ureta TEXT; update shuppinrireki set ureta = 'False'; pragma user_version = 7;";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
        }

        public void addReexhibitFlag() {
            int user_version = LoadUserVersion();
            //更新
            if (user_version < 10) {
                conn.Open();
                string commandText = "alter table shuppinrireki add reexhibit_flag TEXT; update shuppinrireki set reexhibit_flag = 'False'; pragma user_version = 10;";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
        }

        public class ShuppinRireki {
            public int itemDBId;
            public int accountDBId;
            public string item_id;
            public DateTime created_date;
            //public bool ureta = false; Ver4.1で廃止
            public bool reexhibit_flag = false;
            public ShuppinRireki() {
                //this.ureta = false;
            }
            public ShuppinRireki(int itemDBId, int accountDBId, string item_id, bool reexhibit_flag = false) {
                this.itemDBId = itemDBId;
                this.accountDBId = accountDBId;
                this.item_id = item_id;
                //this.ureta = false;
                this.reexhibit_flag = reexhibit_flag;
            }
        }
        //出品履歴を追加する 売れたかどうかはデフォルトではFalse
        public void addShuppinRireki(ShuppinRireki sr) {
            conn.Open();
            string commandText = "INSERT INTO shuppinrireki (itemDBId, accountDBId, item_id, created_date, ureta, reexhibit_flag) VALUES (" + sr.itemDBId.ToString() + "," + sr.accountDBId.ToString() + ",'" + sr.item_id + "','" + DateTime.Now.ToString() + "','dummy','" + sr.reexhibit_flag.ToString() + "');";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }

        //出品履歴をすべてとってくる
        public List<ShuppinRireki> loadRireki() {
            List<ShuppinRireki> rst = new List<ShuppinRireki>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("SELECT * from shuppinrireki", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    ShuppinRireki sr = new ShuppinRireki();
                    sr.accountDBId = int.Parse(sQLiteDataReader["accountDBId"].ToString());
                    sr.itemDBId = int.Parse(sQLiteDataReader["itemDBId"].ToString());
                    sr.item_id = sQLiteDataReader["item_id"].ToString();
                    sr.created_date = DateTime.Parse(sQLiteDataReader["created_date"].ToString());
                    sr.reexhibit_flag = Boolean.Parse(sQLiteDataReader["reexhibit_flag"].ToString());
                    //sr.ureta = (sQLiteDataReader["ureta"].ToString() == "True");
                    rst.Add(sr);
                } catch (Exception ex) {
                    Log.Logger.Error("出品履歴読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }
        //再出品フラグの立っている出品履歴をすべてとってくる
        public List<ShuppinRireki> loadReExhibitRireki() {
            List<ShuppinRireki> rst = new List<ShuppinRireki>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("SELECT * from shuppinrireki where reexhibit_flag = 'True'", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    ShuppinRireki sr = new ShuppinRireki();
                    sr.accountDBId = int.Parse(sQLiteDataReader["accountDBId"].ToString());
                    sr.itemDBId = int.Parse(sQLiteDataReader["itemDBId"].ToString());
                    sr.item_id = sQLiteDataReader["item_id"].ToString();
                    sr.created_date = DateTime.Parse(sQLiteDataReader["created_date"].ToString());
                    sr.reexhibit_flag = Boolean.Parse(sQLiteDataReader["reexhibit_flag"].ToString());
                    //sr.ureta = (sQLiteDataReader["ureta"].ToString() == "True");
                    rst.Add(sr);
                } catch (Exception ex) {
                    Log.Logger.Error("出品履歴読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        public Dictionary<string, ShuppinRireki> loadRirekiDictionary() {
            var shuppinrirekiList = loadRireki();
            Dictionary<string, ShuppinRireki> rst = new Dictionary<string, ShuppinRireki>();
            foreach (var rireki in shuppinrirekiList) {
                rst[rireki.item_id] = rireki;
            }
            return rst;
        }

        public bool deleteRireki(string item_id) {
            try {
                conn.Open();
                string commandText = "delete from shuppinrireki where item_id = '" + item_id + "';";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("出品履歴削除成功: " + item_id);
                return true;
            } catch (Exception ex) {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                Log.Logger.Error("出品履歴削除失敗: " + item_id);
                return false;
            }
        }

        //出品履歴をすべて削除
        public bool resetShuppinRireki() {
            try {
                conn.Open();
                string commandText = "delete from shuppinrireki;";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                return true;
            } catch (Exception ex) {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                return false;
            }
        }

        //出品履歴が存在するか
        public bool existShuppinRireki(string item_id) {
            List<ShuppinRireki> rst = new List<ShuppinRireki>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("SELECT * from shuppinrireki where item_id = '" + item_id + "';", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    ShuppinRireki sr = new ShuppinRireki();
                    sr.accountDBId = int.Parse(sQLiteDataReader["accountDBId"].ToString());
                    sr.itemDBId = int.Parse(sQLiteDataReader["itemDBId"].ToString());
                    sr.item_id = sQLiteDataReader["item_id"].ToString();
                    sr.created_date = DateTime.Parse(sQLiteDataReader["created_date"].ToString());
                    sr.reexhibit_flag = Boolean.Parse(sQLiteDataReader["reexhibit_flag"].ToString());
                    //sr.ureta = (sQLiteDataReader["ureta"].ToString() == "True");
                    rst.Add(sr);
                } catch (Exception ex) {
                    Log.Logger.Error("出品履歴読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();

            if (rst.Count == 0) return false;
            else return true;
        }
    }
}
