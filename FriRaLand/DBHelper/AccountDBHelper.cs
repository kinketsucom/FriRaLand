using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace FriRaLand.DBHelper {
    class AccountDBHelper {

        public const string DBname = "database.db";
        private SQLiteConnection conn;
        public AccountDBHelper() {
            this.conn = new SQLiteConnection("Data Source=" + DBname + ";foreign keys = true;");
        }

        public void onCreate() {
            conn.Open();
            string commandText = "CREATE TABLE IF NOT EXISTS accounts ( Id INTEGER PRIMARY KEY AUTOINCREMENT,"
                                + "email TEXT, password TEXT,auth_token TEXT,"
                                + "user_id TEXT, nickname TEXT, number INTEGER,expiration_date,kengai_num INTEGER,exhibit_cnt INTEGER,"
                                + "hanbai_num INTEGER,lastExhibitDate TEXT,addSpecialTextToItemName TEXT, insertEmptyStrToItemName TEXT, " +
                                "defaultbankaddressId INTEGER);";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }

        public void addAccount(Common.Account account) {
            //addするときはまずmax(number)を取得して+1の値をいれる
            int max_number = getMaxNumber();
            int new_number = 0;
            if (max_number < 0) {
                //はじめてアカウントを追加する
                new_number = 1;
            } else {
                new_number = max_number + 1;
            }
            conn.Open();
            string commandText = "INSERT INTO accounts ("
                                + "email, password," +
                                " auth_token, user_id, nickname, " +
                                "number, expiration_date, kengai_num, " +
                                "exhibit_cnt, hanbai_num, lastExhibitDate, " +
                                "addSpecialTextToItemName, insertEmptyStrToItemName, " +
                                "defaultbankaddressId, expiration_date )"
                                + "VALUES ('" + account.email.Replace("'", "''") + "','"
                                + account.password.Replace("'", "''") + "','"
                                + account.auth_token.Replace("'", "''") + "','"
                                + account.userId.Replace("'", "''") + "','"
                                + account.nickname.Replace("'", "''") + "', " 
                                + new_number.ToString() + ",'" 
                                + Common.getUnixTimeStampFromDate(account.expiration_date) 
                                + "', -1, 0,0, '','False','False', " 
                                + account.defaultbankaddressId.ToString() + ",'" 
                                + Common.getUnixTimeStampFromDate(account.token_update_date) + "'); ";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }

        private int getMaxNumber() {
            //numberの最大値を取得する
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select max(number) from accounts;", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            int max_number = -1;
            while (sQLiteDataReader.Read()) {
                try {
                    if (!string.IsNullOrEmpty(sQLiteDataReader["max(number)"].ToString())) {
                        max_number = int.Parse(sQLiteDataReader["max(number)"].ToString());
                    }
                } catch (Exception ex) {
                    Log.Logger.Error("DBのnumberの最大値取得失敗");
                    Console.WriteLine(ex);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            Console.WriteLine("max_number:"+max_number.ToString());
            return max_number;
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
        public void addNumberColumn() {
            int user_version = LoadUserVersion();
            //更新
            if (user_version < 1) {
                conn.Open();
                string commandText = "pragma user_version = 1;";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();

                //conn.Open();
                //commandText = "alter table accounts add number INTEGER;";
                //command = conn.CreateCommand();
                //command.CommandText = commandText;
                //command.ExecuteNonQuery();
                //command.Dispose();
                //conn.Close();

                conn.Open();
                commandText = "update accounts set number = rowid;";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
            Console.WriteLine("user_version:"+user_version.ToString());
            getMaxNumber();
        }

        public void swapNumber(int DBId1, int DBId2) {
            //それぞれのnumberを取得する
            int number1, number2;
            number1 = number2 = -1;

            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.number from accounts i where i.id = " + DBId1.ToString() + ";", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    number1 = int.Parse(sQLiteDataReader["number"].ToString());
                } catch (Exception ex) {
                    Log.Logger.Error("swapNumberにてDBId1のnumber取得失敗: " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();

            this.conn.Open();
            sQLiteCommand = new SQLiteCommand("select i.number from accounts i where i.id = " + DBId2.ToString() + ";", this.conn);
            sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    number2 = int.Parse(sQLiteDataReader["number"].ToString());
                } catch (Exception ex) {
                    Log.Logger.Error("swapNumberにてDBId2のnumber取得失敗: " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();

            //swap実行
            if (number1 > 0 && number2 > 0) {
                conn.Open();
                string commandText = "update accounts set number = " + number2.ToString() + " where id = " + DBId1.ToString() + ";";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();

                conn.Open();
                commandText = "update accounts set number = " + number1.ToString() + " where id = " + DBId2.ToString() + ";";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
        }

        public void addExpirationDateColumn() {
            int user_version = LoadUserVersion();
            //更新
            if (user_version < 2) {
                conn.Open();
                string commandText = "pragma user_version = 2;";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();

                conn.Open();
                commandText = "alter table accounts add expiration_date TEXT;";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
        }

        public void addKengai_ExhibitCnt_LastExhibitTime_Column() {
            int user_version = LoadUserVersion();
            //更新
            if (user_version < 4) {
                conn.Open();
                string commandText = "pragma user_version = 4;";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();

                conn.Open();
                commandText = "update accounts set kengai_num = -1;update accounts set exhibit_cnt = 0;update accounts set lastExhibitDate = '';";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
        }
        public void addHanbaiNumColumn() {
            int user_version = LoadUserVersion();
            //更新
            if (user_version < 8) {
                conn.Open();
                string commandText = "pragma user_version = 8;";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();

                conn.Open();
                commandText = "update accounts set hanbai_num = 0;";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
        }
        public void addItemNameSpeccialSettingsColumn() {
            int user_version = LoadUserVersion();
            //更新
            if (user_version < 9) {
                conn.Open();
                string commandText = "pragma user_version = 9;";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();

                conn.Open();
                commandText = "update accounts set addSpecialTextToItemName = 'False'; update accounts set insertEmptyStrToItemName = 'False';";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
        }
        //方法2で使用するデフォルト銀行がどれかを指定するカラムを追加
        public void addDefaultBankAddressColumn() {
            int user_version = LoadUserVersion();
            //更新
            if (user_version < 12) {
                conn.Open();
                string commandText = "pragma user_version = 12;";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();

                conn.Open();
                commandText = "update accounts set defaultbankaddressId = -1";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
        }
        public void addTokenUpdateDateColumn() {
            int user_version = LoadUserVersion();
            //更新
            if (user_version < 13) {
                conn.Open();
                string commandText = "pragma user_version = 13;";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();

                conn.Open();
                commandText = "alter table accounts add TokenUpdateDate TEXT; update accounts set TokenUpdateDate = expiration_date";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
        }
        public List<Common.Account> loadAccounts() {
            List<Common.Account> rst = new List<Common.Account>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select * from accounts order by number;", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    Common.Account account = new Common.Account();
                    account.DBId = int.Parse(sQLiteDataReader["Id"].ToString());
                    account.email = sQLiteDataReader["email"].ToString();
                    account.password = sQLiteDataReader["password"].ToString();
                    account.auth_token = sQLiteDataReader["auth_token"].ToString();
                    account.userId = sQLiteDataReader["user_id"].ToString();
                    account.nickname = sQLiteDataReader["nickname"].ToString();
                    account.expiration_date = Common.getDateFromUnixTimeStamp(sQLiteDataReader["expiration_date"].ToString());
                    account.kengai_num = int.Parse(sQLiteDataReader["kengai_num"].ToString());
                    account.exhibit_cnt = int.Parse(sQLiteDataReader["exhibit_cnt"].ToString());
                    account.hanbai_num = int.Parse(sQLiteDataReader["hanbai_num"].ToString());
                    account.last_exhibitTime_str = sQLiteDataReader["lastExhibitDate"].ToString();
                    account.addSpecialTextToItemName = bool.Parse(sQLiteDataReader["addSpecialTextToItemName"].ToString());
                    account.insertEmptyStrToItemName = bool.Parse(sQLiteDataReader["insertEmptyStrToItemName"].ToString());
                    account.defaultbankaddressId = int.Parse(sQLiteDataReader["defaultbankaddressId"].ToString());
                    account.token_update_date = Common.getDateFromUnixTimeStamp(sQLiteDataReader["expiration_date"].ToString());
                    rst.Add(account);
                } catch (Exception ex) {
                    Log.Logger.Error("アカウントリスト読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        public List<Common.Account> selectItem(List<int> DBIdList) {
            List<Common.Account> rst = new List<Common.Account>();
            if (DBIdList.Count == 0) return rst;
            string text = string.Join(",", DBIdList.ToArray());
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select * from accounts i where i.id in (" + text + ") order by number;", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    Common.Account account = new Common.Account();
                    account.DBId = int.Parse(sQLiteDataReader["Id"].ToString());
                    account.email = sQLiteDataReader["email"].ToString();
                    account.password = sQLiteDataReader["password"].ToString();
                    account.auth_token = sQLiteDataReader["auth_token"].ToString();
                    account.sellerid = sQLiteDataReader["user_id"].ToString();
                    account.nickname = sQLiteDataReader["nickname"].ToString();
                    account.expiration_date = Common.getDateFromUnixTimeStamp(sQLiteDataReader["expiration_date"].ToString());
                    account.kengai_num = int.Parse(sQLiteDataReader["kengai_num"].ToString());
                    account.exhibit_cnt = int.Parse(sQLiteDataReader["exhibit_cnt"].ToString());
                    account.hanbai_num = int.Parse(sQLiteDataReader["hanbai_num"].ToString());
                    account.last_exhibitTime_str = sQLiteDataReader["lastExhibitDate"].ToString();
                    account.addSpecialTextToItemName = bool.Parse(sQLiteDataReader["addSpecialTextToItemName"].ToString());
                    account.insertEmptyStrToItemName = bool.Parse(sQLiteDataReader["insertEmptyStrToItemName"].ToString());
                    account.defaultbankaddressId = int.Parse(sQLiteDataReader["defaultbankaddressId"].ToString());
                    account.token_update_date = Common.getDateFromUnixTimeStamp(sQLiteDataReader["expiration_date"].ToString());
                    rst.Add(account);
                } catch (Exception ex) {
                    Log.Logger.Error("アカウントリスト読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        public bool updateAccount(int DBId, Common.Account account) {
            try {
                conn.Open();
                string commandText = "UPDATE accounts SET "
                                    + "email = '" + account.email.Replace("'", "''") + "',"
                                    + "password = '" + account.password.Replace("'", "''") + "',"
                                    + "auth_token = '" + account.auth_token + "',"
                                    + "sellerid = '" + account.sellerid + "',"
                                    + "nickname = '" + account.nickname.Replace("'", "''") + "',"
                                    + "expiration_date = '" + Common.getUnixTimeStampFromDate(account.expiration_date) + "',"
                                    + "kengai_num = " + account.kengai_num.ToString() + ", "
                                    + "hanbai_num = " + account.hanbai_num.ToString() + ", "
                                    + "exhibit_cnt = " + account.exhibit_cnt.ToString() + ", "
                                    + "lastExhibitDate = '" + account.last_exhibitTime_str + "' ,"
                                    + "addSpecialTextToItemName = '" + account.addSpecialTextToItemName.ToString() + "', "
                                    + "insertEmptyStrToItemName = '" + account.insertEmptyStrToItemName.ToString() + "', "
                                    + "defaultbankaddressId = " + account.defaultbankaddressId.ToString() + ","
                                    + "tokenupdatedate = '" + Common.getUnixTimeStampFromDate(account.token_update_date) + "'"
                                    + " WHERE Id = " + DBId.ToString() + ";";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("アカウントDBの更新に成功");
                return true;
            } catch (Exception ex) {
                conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("アカウントDBの更新に失敗");
                return false;
            }
        }

        public bool deleteAccount(List<int> DBIdList) {
            if (DBIdList.Count == 0) return true;
            try {
                string text = string.Join(",", DBIdList.ToArray());
                this.conn.Open();
                string commandText = "DELETE FROM accounts Where Id in (" + text + ");";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("アカウントDBからの商品削除成功");
                return true;
            } catch (Exception ex) {
                conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("アカウントDBからの商品削除失敗");
                return false;
            }
        }

        //アカウントDBにIDがあればDBを返す (なければ0を返す) emailが一致していれば同じとみなす！
        public int getAccountDBId(Common.Account account) {
            int rst = 0;
            this.conn.Open();
            try {
                SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.Id from accounts i where"
                                    + " i.email = '" + account.email.Replace("'", "''") + "';", this.conn);
                SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
                while (sQLiteDataReader.Read()) {
                    rst = int.Parse(sQLiteDataReader["Id"].ToString());
                }
                sQLiteDataReader.Close();
                this.conn.Close();
                sQLiteCommand.Dispose();
                return rst;
            } catch (Exception ex) {
                this.conn.Close();
                return rst;
            }
        }

        //アカウントDBにIDがあればDBを返す (なければ0を返す) emailが一致していれば同じとみなす！
        public int getAccountDBId(string email) {
            int rst = 0;
            this.conn.Open();
            try {
                SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.Id from accounts i where"
                                    + " i.email = '" + email.Replace("'", "''") + "';", this.conn);
                SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
                while (sQLiteDataReader.Read()) {
                    rst = int.Parse(sQLiteDataReader["Id"].ToString());
                }
                sQLiteDataReader.Close();
                this.conn.Close();
                sQLiteCommand.Dispose();
                return rst;
            } catch (Exception ex) {
                this.conn.Close();
                return rst;
            }
        }

        //なければ空を返す
        public string getAccountEmailFromSellerid(string sellerid) {
            string rst = "";
            this.conn.Open();
            try {
                SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.email from accounts i where"
                                    + " i.sellerid = '" + sellerid + "';", this.conn);
                SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
                while (sQLiteDataReader.Read()) {
                    rst = sQLiteDataReader["email"].ToString();
                }
                sQLiteDataReader.Close();
                this.conn.Close();
                sQLiteCommand.Dispose();
                return rst;
            } catch (Exception ex) {
                this.conn.Close();
                return rst;
            }
        }

        //なければnew Common.Accountを返す
        public Common.Account getAccountFromSellerid(string user_id) {
            Common.Account account = new Common.Account();
            this.conn.Open();
            try {
                SQLiteCommand sQLiteCommand = new SQLiteCommand("select * from accounts i where"
                                    + " i.user_id = '" + user_id + "';", this.conn);
                SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
                while (sQLiteDataReader.Read()) {
                    account.DBId = int.Parse(sQLiteDataReader["Id"].ToString());
                    account.email = sQLiteDataReader["email"].ToString();
                    account.password = sQLiteDataReader["password"].ToString();
                    account.auth_token = sQLiteDataReader["auth_token"].ToString();
                    account.sellerid = sQLiteDataReader["user_id"].ToString();
                    account.nickname = sQLiteDataReader["nickname"].ToString();
                    account.expiration_date = Common.getDateFromUnixTimeStamp(sQLiteDataReader["expiration_date"].ToString());
                    account.kengai_num = int.Parse(sQLiteDataReader["kengai_num"].ToString());
                    account.exhibit_cnt = int.Parse(sQLiteDataReader["exhibit_cnt"].ToString());
                    account.hanbai_num = int.Parse(sQLiteDataReader["hanbai_num"].ToString());
                    account.last_exhibitTime_str = sQLiteDataReader["lastExhibitDate"].ToString();
                    account.addSpecialTextToItemName = bool.Parse(sQLiteDataReader["addSpecialTextToItemName"].ToString());
                    account.insertEmptyStrToItemName = bool.Parse(sQLiteDataReader["insertEmptyStrToItemName"].ToString());
                    account.defaultbankaddressId = int.Parse(sQLiteDataReader["defaultbankaddressId"].ToString());
                    account.token_update_date = Common.getDateFromUnixTimeStamp(sQLiteDataReader["expiration_date"].ToString());//FIXIT:token_update_date関連はなおさないとね
                }
                sQLiteDataReader.Close();
                this.conn.Close();
                sQLiteCommand.Dispose();
                return account;
            } catch (Exception ex) {
                Dev.printE(ex);
                this.conn.Close();
                return null;
            }
        }
    }
}
