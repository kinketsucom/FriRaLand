using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriLand.DBHelper {
    class SettingsDBHelper {
        public const string DBname = "database.db";
        private SQLiteConnection conn;
        public SettingsDBHelper() {
            this.conn = new SQLiteConnection("Data Source=" + DBname + ";foreign keys = true;");
        }

        public void onCreate() {
            conn.Open();
            string commandText = "CREATE TABLE IF NOT EXISTS settings ( Id INTEGER PRIMARY KEY AUTOINCREMENT, key TEXT, value TEXT);";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }

        public void updateSettings(string key, string value) {
            //もしすでにkeyが同じのがあればupdate, なければinsert
            string rst = getSettingValue(key);
            if (rst == null) insertDB(key, value);
            else updateDB(key, value);
        }

        private void updateDB(string key, string value) {
            try {
                conn.Open();
                string commandText = "update settings set value = '" + value.Replace("'", "''") + "' where key = '" + key.Replace("'", "''") + "';";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("設定DBの更新に成功");
            } catch (Exception ex) {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("設定DBの更新に失敗");
            }
        }

        private void insertDB(string key, string value) {
            try {
                conn.Open();
                string commandText = "INSERT INTO settings (key, value ) VALUES ('" + key.Replace("'", "''") + "','" + value.Replace("'", "''") + "');";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("設定DBの挿入に成功");
            } catch (Exception ex) {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("設定DBの挿入に失敗");
            }
        }
        //なければnull
        public string getSettingValue(string key) {
            string rst = null;
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.value from settings i where key = '" + key.Replace("'", "''") + "';", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    rst = sQLiteDataReader["value"].ToString();
                } catch (Exception ex) {
                    Log.Logger.Error("設定ファイル not found key : " + key);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }
    }
}
