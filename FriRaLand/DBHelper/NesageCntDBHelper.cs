using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace FriRaLand.DBHelper {
    class NesageCntDBHelper {
        public const string DBname = "database.db";
        private SQLiteConnection conn;
        public NesageCntDBHelper() {
            this.conn = new SQLiteConnection("Data Source=" + DBname + ";foreign keys = true;");
        }

        public void onCreate() {
            conn.Open();
            string commandText = "CREATE TABLE IF NOT EXISTS nesagecnt ( id INTEGER PRIMARY KEY AUTOINCREMENT,"
                                + "itemid TEXT, cnt INTEGER);";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }

        public class NesageCnt {
            public string itemid;
            public int nesagecnt;
        }

        //値下げ回数を1増やす
        public void nesageCntIncrement(string itemid) {
            if (hasNesageCntRecord(itemid)) {
                int new_cnt = getNesageCnt(itemid) + 1;
                updateNesageCnt(itemid, new_cnt);
            } else {
                addNesageCntData(itemid);
            }
        }
        //値下げ回数を取得
        //該当レコードなし: 0
        //それ以外: 回数
        public int getNesageCnt(string itemid) {
            int rst = 0;
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select * from nesagecnt i where i.itemid = '" + itemid + "';", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    rst = int.Parse(sQLiteDataReader["cnt"].ToString());
                } catch (Exception ex) {
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        private bool hasNesageCntRecord(string itemid) {
            bool rst = false;
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select * from nesagecnt i where i.itemid = '" + itemid + "';", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    rst = true;
                } catch (Exception ex) {

                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }
        private void updateNesageCnt(string itemid, int new_cnt) {
            this.conn.Open();
            string commandText = "update nesagecnt set cnt = " + new_cnt.ToString() + " where itemid = '" + itemid + "';";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }
        //値下げ情報を追加
        //これが呼ばれるのは値下げが新規実行された後なので,値下げ回数は1でレコード追加
        private void addNesageCntData(string itemid) {
            this.conn.Open();
            string commandText = "insert into nesagecnt(itemid, cnt) VALUES ('" + itemid + "', 1);";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }

        public bool deleteNesageCnt(string itemid) {
            try {
                this.conn.Open();
                string commandText = "DELETE FROM nesagecnt Where itemid = '" + itemid + "';";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("値下げ回数DBからレコード削除成功");
                return true;
            } catch (Exception ex) {
                conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("値下げ回数DBからレコード削除失敗");
                return false;
            }
        }
    }
}
