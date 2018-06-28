using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RakuLand.DBHelper {
    class NesageLogDBHelper {
        public class NesageLog {
            public int DBId;
            public string itemid;
            public int nesage_cnt;
            public string itemname;
            public string exhibit_datetime;
            public string nesage_datetime;
            public int oldprice;
            public int newprice;
            public int likecnt;
            public string nickname;
        }

        public const string DBname = "database.db";
        private SQLiteConnection conn;
        public NesageLogDBHelper() {
            this.conn = new SQLiteConnection("Data Source=" + DBname + ";foreign keys = true;");
        }

        public void onCreate() {
            conn.Open();
            string commandText = "CREATE TABLE IF NOT EXISTS nesagelog ( id INTEGER PRIMARY KEY AUTOINCREMENT,"
                                + "nickname TEXT, exhibit_datetime TEXT, nesage_datetime TEXT, oldprice INTEGER, newprice INTEGER,"
                                + "itemname TEXT, itemid TEXT, likecnt INTEGER, nesage_cnt INTEGER);";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }

        private void addNesageLogDB(NesageLog log) {
            try {
                conn.Open();
                string commandText = "INSERT INTO nesagelog (nickname, exhibit_datetime, nesage_datetime, oldprice, newprice, itemname, itemid, likecnt, nesage_cnt) VALUES"
                                + "('" + log.nickname.Replace("'", "''") + "',"
                                + "'" + log.exhibit_datetime.Replace("'", "''") + "',"
                                + "'" + log.nesage_datetime.Replace("'", "''") + "',"
                                + log.oldprice.ToString() + ","
                                + log.newprice.ToString() + ","
                                + "'" + log.itemname.Replace("'", "''") + "',"
                                + "'" + log.itemid.Replace("'", "''") + "',"
                                + log.likecnt.ToString() + ","
                                + log.nesage_cnt.ToString() + ");";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            } catch (Exception ex) {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                Log.Logger.Error("NesageLogへのログ追加失敗" + ex.Message);
            }
        }

        public void addNesageLog(FrilItem item, string nickname, int oldprice, int newprice, int nesage_cnt) {
            NesageLog log = new NesageLog();
            log.nickname = nickname;
            log.exhibit_datetime = item.created_date.ToString();
            log.nesage_datetime = DateTime.Now.ToString();
            log.oldprice = oldprice;
            log.newprice = newprice;
            log.itemname = item.item_name;
            log.itemid = item.item_id;
            log.likecnt = item.likes_count;
            log.nesage_cnt = nesage_cnt;
            addNesageLogDB(log);
        }

        public List<NesageLog> loadNesageLog() {
            List<NesageLog> rst = new List<NesageLog>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select * from nesagelog;", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    var log = new NesageLog();
                    log.nickname = sQLiteDataReader["nickname"].ToString();
                    log.exhibit_datetime = sQLiteDataReader["exhibit_datetime"].ToString();
                    log.nesage_datetime = sQLiteDataReader["nesage_datetime"].ToString();
                    log.itemid = sQLiteDataReader["itemid"].ToString();
                    log.itemname = sQLiteDataReader["itemname"].ToString();
                    log.oldprice = int.Parse(sQLiteDataReader["oldprice"].ToString());
                    log.newprice = int.Parse(sQLiteDataReader["newprice"].ToString());
                    log.likecnt = int.Parse(sQLiteDataReader["likecnt"].ToString());
                    log.nesage_cnt = int.Parse(sQLiteDataReader["nesage_cnt"].ToString());
                    log.DBId = int.Parse(sQLiteDataReader["Id"].ToString());
                    rst.Add(log);
                } catch (Exception ex) {
                    Log.Logger.Error("ログ読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }
        public bool deleteNesageLog(List<int> DBIdList) {
            if (DBIdList.Count == 0) return true;
            try {
                string text = string.Join(",", DBIdList.ToArray());
                this.conn.Open();
                string commandText = "DELETE FROM nesagelog Where Id in (" + text + ");";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("値下げログDBからのログ削除成功");
                return true;
            } catch (Exception ex) {
                conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("値下げログDBからのログ削除失敗");
                return false;
            }
        }
    }
}
