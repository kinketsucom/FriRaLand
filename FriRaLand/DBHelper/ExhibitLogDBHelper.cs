using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriRaLand.DBHelper {
    class ExhibitLogDBHelper {
        //完全に他のDBとは独立させて保存
        public class ExhibitLog {
            public int DBId;
            public string nickname;
            public string exhibit_day;
            public string exhibit_time;
            public string sold_day;
            public string sold_time;
            public string parent_id;
            public string child_id;
            public string status;
            public string itemid;
            public ExhibitLog() {
                this.nickname = this.exhibit_day = this.sold_day = this.sold_time = this.parent_id = this.child_id = this.status = this.itemid = "";
            }
            public ExhibitLog(string nickname, string exhibit_day, string exhibit_time, string sold_day, string sold_time, string parent_id, string child_id, string status, string itemid) {
                this.nickname = nickname;
                this.exhibit_day = exhibit_day;
                this.exhibit_time = exhibit_time;
                this.sold_day = sold_day;
                this.sold_time = sold_time;
                this.parent_id = parent_id;
                this.child_id = child_id;
                this.status = status;
                this.itemid = itemid;
            }
        }

        public const string DBname = "database.db";
        private SQLiteConnection conn;
        public ExhibitLogDBHelper() {
            this.conn = new SQLiteConnection("Data Source=" + DBname + ";foreign keys = true;");
        }

        public void onCreate() {
            conn.Open();
            string commandText = "CREATE TABLE IF NOT EXISTS exhibitlog ( id INTEGER PRIMARY KEY AUTOINCREMENT,"
                                + "nickname TEXT, exhibit_day TEXT, exhibit_time TEXT, sold_day TEXT, sold_time TEXT,"
                                + "parent_id TEXT, child_id TEXT, status TEXT);";
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
        public void addItemIDColumn() {
            int user_version = LoadUserVersion();
            if (user_version < 15) {
                conn.Open();
                string commandText = "pragma user_version = 15;";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();

                conn.Open();
                commandText = "alter table exhibitlog add itemid TEXT;";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
        }

        private void addExhibitLogDB(ExhibitLog log) {
            try {
                conn.Open();
                string commandText = "INSERT INTO exhibitlog (nickname, exhibit_day, exhibit_time, sold_day, sold_time, parent_id, child_id, status, itemid) VALUES"
                                + "('" + log.nickname.Replace("'", "''") + "',"
                                + "'" + log.exhibit_day.Replace("'", "''") + "',"
                                + "'" + log.exhibit_time.Replace("'", "''") + "',"
                                + "'" + log.sold_day.Replace("'", "''") + "',"
                                + "'" + log.sold_time.Replace("'", "''") + "',"
                                + "'" + log.parent_id.Replace("'", "''") + "',"
                                + "'" + log.child_id.Replace("'", "''") + "',"
                                + "'" + log.status.Replace("'", "''") + "',"
                                + "'" + log.itemid + "');";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            } catch (Exception ex) {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                Log.Logger.Error("ExhibitLogへのログ追加失敗" + ex.Message);
            }
        }

        public void addExhibitLog(string nickname, DateTime exhibitDate, ItemFamilyDBHelper.ItemFamily itemfamily, string itemid) {
            ExhibitLog log = new ExhibitLog();
            log.nickname = nickname;
            log.exhibit_day = exhibitDate.ToString("yyyy/MM/dd");
            log.exhibit_time = exhibitDate.ToString("HH:mm");
            log.sold_day = "";
            log.sold_time = "";
            log.parent_id = (itemfamily == null ? "" : itemfamily.parent_id);
            log.child_id = (itemfamily == null ? "" : itemfamily.child_id);
            log.status = "出品";
            log.itemid = itemid;
            addExhibitLogDB(log);
        }
        public void addReexhibitLog(string nickname, DateTime exhibitDate, ItemFamilyDBHelper.ItemFamily itemfamily, string itemid) {
            ExhibitLog log = new ExhibitLog();
            log.nickname = nickname;
            log.exhibit_day = exhibitDate.ToString("yyyy/MM/dd");
            log.exhibit_time = exhibitDate.ToString("HH:mm");
            log.sold_day = "";
            log.sold_time = "";
            log.parent_id = (itemfamily == null ? "" : itemfamily.parent_id);
            log.child_id = (itemfamily == null ? "" : itemfamily.child_id);
            log.status = "再出品";
            log.itemid = itemid;
            addExhibitLogDB(log);
        }
        public void addSoldLog(string nickname, DateTime exhibitDate, DateTime soldDate, ItemFamilyDBHelper.ItemFamily itemfamily, string itemid) {
            ExhibitLog log = new ExhibitLog();
            log.nickname = nickname;
            log.exhibit_day = exhibitDate.ToString("yyyy/MM/dd");
            log.exhibit_time = exhibitDate.ToString("HH:mm");
            log.sold_day = soldDate.ToString("yyyy/MM/dd");
            log.sold_time = soldDate.ToString("HH:mm");
            log.parent_id = (itemfamily == null ? "" : itemfamily.parent_id);
            log.child_id = (itemfamily == null ? "" : itemfamily.child_id);
            log.status = "販売";
            log.itemid = itemid;
            addExhibitLogDB(log);
        }
        public bool deleteExhibitLog(List<int> DBIdList) {
            if (DBIdList.Count == 0) return true;
            try {
                string text = string.Join(",", DBIdList.ToArray());
                this.conn.Open();
                string commandText = "DELETE FROM exhibitlog Where Id in (" + text + ");";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("ログDBからのログ削除成功");
                return true;
            } catch (Exception ex) {
                conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("ログDBからのログ削除失敗");
                return false;
            }
        }

        public List<ExhibitLog> loadExhibitLog() {
            List<ExhibitLog> rst = new List<ExhibitLog>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select * from exhibitlog;", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    var log = new ExhibitLog();
                    log.nickname = sQLiteDataReader["nickname"].ToString();
                    log.exhibit_day = sQLiteDataReader["exhibit_day"].ToString();
                    log.exhibit_time = sQLiteDataReader["exhibit_time"].ToString();
                    log.sold_day = sQLiteDataReader["sold_day"].ToString();
                    log.sold_time = sQLiteDataReader["sold_time"].ToString();
                    log.parent_id = sQLiteDataReader["parent_id"].ToString();
                    log.child_id = sQLiteDataReader["child_id"].ToString();
                    log.status = sQLiteDataReader["status"].ToString();
                    log.DBId = int.Parse(sQLiteDataReader["Id"].ToString());
                    log.itemid = sQLiteDataReader["itemid"].ToString();
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

        //履歴からIDを指定して親IDを取得
        //返り値: 成功：親ID 失敗:空文字列
        public string getParentIDFromExhibitLog(string itemid) {
            string rst = "";
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select parent_id from exhibitlog where itemid = '" + itemid + "';", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    rst = sQLiteDataReader["parent_id"].ToString();
                } catch (Exception ex) {
                    Log.Logger.Error("ログ読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }
    }
}
