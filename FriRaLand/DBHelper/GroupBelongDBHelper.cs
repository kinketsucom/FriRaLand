using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace FriLand.DBHelper {
    class GroupBelongDBHelper {

        public const string DBname = "database.db";
        private SQLiteConnection conn;
        public GroupBelongDBHelper() {
            this.conn = new SQLiteConnection("Data Source=" + DBname + ";foreign keys = true;");
        }

        public void onCreate() {
            conn.Open();
            string commandText = "CREATE TABLE IF NOT EXISTS groupbelong ( id INTEGER PRIMARY KEY AUTOINCREMENT,"
                                + "groupid INTEGER, accountid INTEGER);";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }

        public class GroupBelong {
            public int DBId;
            public int GroupID;
            public int AccountID;
        }
        //グループ配属を追加する
        public void addGroupBelong(GroupBelong gb) {
            conn.Open();
            string commandText = "INSERT INTO groupbelong (groupid, accountid) VALUES (" + gb.GroupID.ToString() + "," + gb.AccountID.ToString() + ");";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }
        //グループ配属を読み込み
        public List<GroupBelong> loadGroupKind() {
            List<GroupBelong> rst = new List<GroupBelong>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.id, i.groupid, i.accountid from groupbelong i;", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    GroupBelong gb = new GroupBelong();
                    gb.DBId = int.Parse(sQLiteDataReader["id"].ToString());
                    gb.GroupID = int.Parse(sQLiteDataReader["groupid"].ToString());
                    gb.AccountID = int.Parse(sQLiteDataReader["accountid"].ToString());
                    rst.Add(gb);
                } catch (Exception ex) {
                    Log.Logger.Error("グループ配属読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }
        //グループ配属をdictionaryにして返す Dictionary<GroupID, List<AccountDBId>>
        public Dictionary<int, List<int>> loadGroupBelongDictionary() {
            Dictionary<int, List<int>> rst = new Dictionary<int, List<int>>();
            List<GroupBelong> loadrst = loadGroupKind();
            //Dictionaryつくる
            foreach (var gb in loadrst) {
                if (rst.ContainsKey(gb.GroupID) == false) rst[gb.GroupID] = new List<int>();
                rst[gb.GroupID].Add(gb.AccountID);
            }
            return rst;
        }

        public List<GroupBelong> selectItemByGroupID(int GroupID) {
            List<GroupBelong> rst = new List<GroupBelong>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.id, i.groupid, i.accountid from groupbelong i where i.groupid = " + GroupID.ToString() + ";", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    GroupBelong gb = new GroupBelong();
                    gb.DBId = int.Parse(sQLiteDataReader["id"].ToString());
                    gb.GroupID = int.Parse(sQLiteDataReader["groupid"].ToString());
                    gb.AccountID = int.Parse(sQLiteDataReader["accountid"].ToString());
                    rst.Add(gb);
                } catch (Exception ex) {
                    Log.Logger.Error("グループ配属読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        public bool deleteGroupBelongByGroupID(int GroupID) {
            //グループIDを指定してグループ配属情報を削除
            try {
                this.conn.Open();
                string commandText = "DELETE FROM groupbelong Where groupid = " + GroupID.ToString() + ";";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("グループ配属DBからのグループ配属削除成功");
                return true;
            } catch (Exception ex) {
                conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("グループ配属DBからのグループ配属削除失敗");
                return false;
            }
        }

        public bool deleteGroupBelongByAccountID(int AccountID) {
            //グループIDを指定してグループ配属情報を削除
            try {
                this.conn.Open();
                string commandText = "DELETE FROM groupbelong Where accountid = " + AccountID.ToString() + ";";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("グループ配属DBからのグループ配属削除成功");
                return true;
            } catch (Exception ex) {
                conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("グループ配属DBからのグループ配属削除失敗");
                return false;
            }
        }
    }
}
