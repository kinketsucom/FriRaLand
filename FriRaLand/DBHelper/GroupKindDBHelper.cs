using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriRaLand.DBHelper {
    class GroupKindDBHelper {
        //public const string DBname = "database.db";
        //private SQLiteConnection conn;
        //public GroupKindDBHelper() {
        //    this.conn = new SQLiteConnection("Data Source=" + DBname + ";foreign keys = true;");
        //}

        //public void onCreate() {
        //    conn.Open();
        //    string commandText = "CREATE TABLE IF NOT EXISTS groupkind ( groupid INTEGER PRIMARY KEY AUTOINCREMENT,"
        //                        + "groupname TEXT);";
        //    SQLiteCommand command = conn.CreateCommand();
        //    command.CommandText = commandText;
        //    command.ExecuteNonQuery();
        //    command.Dispose();
        //    conn.Close();
        //}

        //public class GroupKind {
        //    public int GroupId;
        //    public string GroupName;
        //}
        ////グループを追加して追加したグループのID返す
        //public int addGroupKind(string groupname) {
        //    //addするときはまずmax(number)を取得して+1の値をいれる
        //    int max_number = getMaxNumber();
        //    int new_number = 0;
        //    if (max_number < 0) {
        //        //はじめてアカウントを追加する
        //        new_number = 1;
        //    } else {
        //        new_number = max_number + 1;
        //    }
        //    conn.Open();
        //    string commandText = "INSERT INTO groupkind (groupname, number) VALUES ('" + groupname.Replace("'", "''") + "'," + new_number.ToString() + ");";
        //    SQLiteCommand command = conn.CreateCommand();
        //    command.CommandText = commandText;
        //    command.ExecuteNonQuery();
        //    command.Dispose();
        //    int rst = -1;
        //    SQLiteCommand sQLiteCommand = new SQLiteCommand("select * from groupkind where groupid = last_insert_rowid();", this.conn);
        //    SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
        //    while (sQLiteDataReader.Read()) {
        //        try {
        //            rst = int.Parse(sQLiteDataReader["groupid"].ToString());
        //        } catch (Exception ex) {
        //        }
        //    }
        //    sQLiteDataReader.Close();
        //    this.conn.Close();
        //    sQLiteCommand.Dispose();
        //    return rst;

        //}

        //public List<GroupKind> loadGroupKind() {
        //    List<GroupKind> rst = new List<GroupKind>();
        //    this.conn.Open();
        //    SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.groupid, i.groupname from groupkind i order by number;", this.conn);
        //    SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
        //    while (sQLiteDataReader.Read()) {
        //        try {
        //            GroupKind gk = new GroupKind();
        //            gk.GroupId = int.Parse(sQLiteDataReader["groupid"].ToString());
        //            gk.GroupName = sQLiteDataReader["groupname"].ToString();
        //            rst.Add(gk);
        //        } catch (Exception ex) {
        //            Log.Logger.Error("グループリスト読み込み中エラー : " + ex.Message);
        //        }
        //    }
        //    sQLiteDataReader.Close();
        //    this.conn.Close();
        //    sQLiteCommand.Dispose();
        //    return rst;
        //}

        //public Dictionary<int, string> loadGroupKindDictionary() { //Dictionary<GroupID, GroupName>
        //    Dictionary<int, string> rst = new Dictionary<int, string>();
        //    var loadrst = loadGroupKind();
        //    foreach (var gb in loadrst) {
        //        rst[gb.GroupId] = gb.GroupName;
        //    }
        //    return rst;
        //}

        //public List<GroupKind> selectItem(List<int> GroupIdList) {
        //    List<GroupKind> rst = new List<GroupKind>();
        //    if (GroupIdList.Count == 0) return rst;
        //    string text = string.Join(",", GroupIdList.ToArray());
        //    this.conn.Open();
        //    SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.groupid, i.groupname from groupkind i where i.groupid in (" + text + ");", this.conn);
        //    SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
        //    while (sQLiteDataReader.Read()) {
        //        try {
        //            GroupKind gk = new GroupKind();
        //            gk.GroupId = int.Parse(sQLiteDataReader["groupid"].ToString());
        //            gk.GroupName = sQLiteDataReader["groupname"].ToString();
        //            rst.Add(gk);
        //        } catch (Exception ex) {
        //            Log.Logger.Error("グループリスト読み込み中エラー : " + ex.Message);
        //        }
        //    }
        //    sQLiteDataReader.Close();
        //    this.conn.Close();
        //    sQLiteCommand.Dispose();
        //    return rst;
        //}

        //public bool deleteGroupKind(int GroupID) {
        //    //グループIDを指定してグループ情報を削除
        //    try {
        //        this.conn.Open();
        //        string commandText = "DELETE FROM groupkind Where groupid = " + GroupID.ToString() + ";";
        //        SQLiteCommand command = conn.CreateCommand();
        //        command.CommandText = commandText;
        //        command.ExecuteNonQuery();
        //        command.Dispose();
        //        conn.Close();
        //        Log.Logger.Info("グループDBからのグループ削除成功");
        //        return true;
        //    } catch (Exception ex) {
        //        conn.Close();
        //        Log.Logger.Error(ex.Message);
        //        Log.Logger.Error("グループDBからのグループ削除失敗");
        //        return false;
        //    }
        //}
        //private int getMaxNumber() {
        //    //numberの最大値を取得する
        //    this.conn.Open();
        //    SQLiteCommand sQLiteCommand = new SQLiteCommand("select max(number) from groupkind;", this.conn);
        //    SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
        //    int max_number = -1;
        //    while (sQLiteDataReader.Read()) {
        //        try {
        //            max_number = int.Parse(sQLiteDataReader["max(number)"].ToString());
        //        } catch (Exception ex) {
        //            Log.Logger.Error("DBのnumberの最大値取得失敗");
        //        }
        //    }
        //    sQLiteDataReader.Close();
        //    this.conn.Close();
        //    sQLiteCommand.Dispose();
        //    Console.WriteLine(max_number);
        //    return max_number;
        //}
        //private int LoadUserVersion() {
        //    //バージョン情報読み取り
        //    this.conn.Open();
        //    SQLiteCommand sQLiteCommand = new SQLiteCommand("pragma user_version", this.conn);
        //    SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
        //    int user_version = -1;
        //    while (sQLiteDataReader.Read()) {
        //        try {
        //            user_version = int.Parse(sQLiteDataReader["user_version"].ToString());
        //        } catch (Exception ex) {
        //            Log.Logger.Error("DBのuser_versionの読み込み失敗");
        //        }
        //    }
        //    sQLiteDataReader.Close();
        //    this.conn.Close();
        //    sQLiteCommand.Dispose();
        //    return user_version;
        //}
        //public void addNumberColumn() {
        //    int user_version = LoadUserVersion();
        //    //更新
        //    if (user_version < 11) {
        //        conn.Open();
        //        string commandText = "pragma user_version = 11; alter table groupkind add number INTEGER; update groupkind set number = rowid;";
        //        SQLiteCommand command = conn.CreateCommand();
        //        command.CommandText = commandText;
        //        command.ExecuteNonQuery();
        //        command.Dispose();
        //        conn.Close();
        //    }
        //    Console.WriteLine(user_version);
        //    getMaxNumber();
        //}

        //public void swapNumber(int DBId1, int DBId2) {
        //    //それぞれのnumberを取得する
        //    int number1, number2;
        //    number1 = number2 = -1;

        //    this.conn.Open();
        //    SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.number from groupkind i where i.groupid = " + DBId1.ToString() + ";", this.conn);
        //    SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
        //    while (sQLiteDataReader.Read()) {
        //        try {
        //            number1 = int.Parse(sQLiteDataReader["number"].ToString());
        //        } catch (Exception ex) {
        //            Log.Logger.Error("swapNumberにてDBId1のnumber取得失敗: " + ex.Message);
        //        }
        //    }
        //    sQLiteDataReader.Close();
        //    this.conn.Close();
        //    sQLiteCommand.Dispose();

        //    this.conn.Open();
        //    sQLiteCommand = new SQLiteCommand("select i.number from groupkind i where i.groupid = " + DBId2.ToString() + ";", this.conn);
        //    sQLiteDataReader = sQLiteCommand.ExecuteReader();
        //    while (sQLiteDataReader.Read()) {
        //        try {
        //            number2 = int.Parse(sQLiteDataReader["number"].ToString());
        //        } catch (Exception ex) {
        //            Log.Logger.Error("swapNumberにてDBId2のnumber取得失敗: " + ex.Message);
        //        }
        //    }
        //    sQLiteDataReader.Close();
        //    this.conn.Close();
        //    sQLiteCommand.Dispose();

        //    //swap実行
        //    if (number1 > 0 && number2 > 0) {
        //        conn.Open();
        //        string commandText = "update groupkind set number = " + number2.ToString() + " where groupid = " + DBId1.ToString() + ";";
        //        SQLiteCommand command = conn.CreateCommand();
        //        command.CommandText = commandText;
        //        command.ExecuteNonQuery();
        //        command.Dispose();
        //        conn.Close();

        //        conn.Open();
        //        commandText = "update groupkind set number = " + number1.ToString() + " where groupid = " + DBId2.ToString() + ";";
        //        command = conn.CreateCommand();
        //        command.CommandText = commandText;
        //        command.ExecuteNonQuery();
        //        command.Dispose();
        //        conn.Close();
        //    }
        //}
        ////グループ名を更新する
        //public void updateGroupName(int groupid, string groupname) {
        //    string commandText = "update groupkind set groupname = '" + groupname + "' where groupid = " + groupid.ToString() + ";";
        //    conn.Open();
        //    SQLiteCommand command = conn.CreateCommand();
        //    command.CommandText = commandText;
        //    command.ExecuteNonQuery();
        //    command.Dispose();
        //    conn.Close();
        //}

    }
}
