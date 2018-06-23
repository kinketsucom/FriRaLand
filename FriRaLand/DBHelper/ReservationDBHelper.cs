using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Drawing;
using FriRaLand.Forms;

namespace FriRaLand.DBHelper{
    class ReservationDBHelper {

        public const string DBname = "database.db";
        private SQLiteConnection conn;
        public ReservationDBHelper() {
            this.conn = new SQLiteConnection("Data Source=" + DBname + ";foreign keys = true;");
        }

        public void onCreate() {
            conn.Open();
            string commandText = "CREATE TABLE IF NOT EXISTS reservations ( Id INTEGER PRIMARY KEY AUTOINCREMENT,"
                                + "itemDBId INTEGER, accountDBId INTEGER, status INTEGER, exhibitDate TEXT, deleteDate TEXT, item_id TEXT, "
                                + "check_favorite TEXT, check_comment TEXT);";
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

        public void addReexhibitFlagColumn() {
            int user_version = LoadUserVersion();
            //更新
            if (user_version < 3) {
                conn.Open();
                string commandText = "pragma user_version = 3;";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();

                conn.Open();
                commandText = "alter table reservations add reexhibit_flag TEXT;";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();

                conn.Open();
                commandText = "update reservations set reexhibit_flag = 'しない';";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
        }
        public void addDelete2Column() {
            int user_version = LoadUserVersion();
            //更新
            if (user_version < 5) {
                conn.Open();
                string commandText = "pragma user_version = 5;";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();

                conn.Open();
                commandText = "alter table reservations add deleteDate2 TEXT; alter table reservations add check_favorite2 TEXT; alter table reservations add check_comment2 TEXT;";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();

                conn.Open();
                commandText = "update reservations set deleteDate2 = '1970/01/01 0:00:00'; update reservations set check_favorite2 = '見る'; update reservations set check_comment2 = '見る';";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
        }

        public void addReservation(ReservationSettingForm.ReservationSetting reservation) {
            conn.Open();
            string check_favorite = reservation.consider_favorite ? "見る" : "見ない";
            string check_comment = reservation.consider_comment ? "見る" : "見ない";
            string check_favorite2 = reservation.consider_favorite2 ? "見る" : "見ない";
            string check_comment2 = reservation.consider_comment2 ? "見る" : "見ない";
            string reexhibit_str = reservation.reexhibit_flag ? "する" : "しない";
            string commandText = "INSERT INTO reservations ("
                                + "itemDBId, accountDBId,status, exhibitDate, deleteDate, item_id, check_favorite, check_comment, reexhibit_flag, deleteDate2, check_favorite2, check_comment2)"
                                + "VALUES ( " + reservation.itemDBId.ToString() + ","
                                + reservation.accountDBId.ToString() + ","
                                + reservation.exhibit_status + ",'"
                                + reservation.exhibitDate.ToString() + "','"
                                + reservation.deleteDate.ToString() + "','"
                                + reservation.item_id.ToString() + "','"
                                + check_favorite + "','"
                                + check_comment + "','"
                                + reexhibit_str + "','"
                                + reservation.deleteDate2.ToString() + "','"
                                + check_favorite2 + "','"
                                + check_comment2 + "');";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }

        public List<ReservationSettingForm.ReservationSetting> loadReservations() {
            List<ReservationSettingForm.ReservationSetting> rst = new List<ReservationSettingForm.ReservationSetting>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("SELECT r.Id, r.itemDBId, r.accountDBId, r.status, r.exhibitDate, r.deleteDate, r.deleteDate2, r.item_id, r.check_favorite, r.check_favorite2, r.check_comment, r.check_comment2, r.reexhibit_flag , i.item_name, i.Pic1, a.nickname from ((reservations r JOIN items i ON r.itemDBId = i.Id )  INNER JOIN accounts a ON a.Id = r.accountDBId);", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    ReservationSettingForm.ReservationSetting reservation = new ReservationSettingForm.ReservationSetting();
                    reservation.DBId = int.Parse(sQLiteDataReader["Id"].ToString());
                    reservation.itemDBId = int.Parse(sQLiteDataReader["itemDBId"].ToString());
                    reservation.accountDBId = int.Parse(sQLiteDataReader["accountDBId"].ToString());
                    reservation.exhibit_status = int.Parse(sQLiteDataReader["status"].ToString());
                    reservation.exhibit_status_str = ReservationSettingForm.get_exhibit_status_str(reservation.exhibit_status);
                    reservation.exhibitDateString = sQLiteDataReader["exhibitDate"].ToString();
                    reservation.exhibitDate = DateTime.Parse(reservation.exhibitDateString);
                    reservation.deleteDateString = sQLiteDataReader["deleteDate"].ToString();
                    reservation.deleteDate = DateTime.Parse(reservation.deleteDateString);
                    reservation.deleteDateString2 = sQLiteDataReader["deleteDate2"].ToString();
                    reservation.deleteDate2 = DateTime.Parse(reservation.deleteDateString2);
                    reservation.item_id = sQLiteDataReader["item_id"].ToString();
                    reservation.consider_comment_str = sQLiteDataReader["check_comment"].ToString();
                    reservation.consider_comment_str2 = sQLiteDataReader["check_comment2"].ToString();
                    reservation.consider_favorite_str = sQLiteDataReader["check_favorite"].ToString();
                    reservation.consider_favorite_str2 = sQLiteDataReader["check_favorite2"].ToString();
                    reservation.reexhibit_flag_str = sQLiteDataReader["reexhibit_flag"].ToString();
                    if (reservation.consider_comment_str == "見る") reservation.consider_comment = true;
                    else reservation.consider_comment = false;
                    if (reservation.consider_favorite_str == "見る") reservation.consider_favorite = true;
                    else reservation.consider_favorite = false;
                    if (reservation.consider_comment_str2 == "見る") reservation.consider_comment2 = true;
                    else reservation.consider_comment2 = false;
                    if (reservation.consider_favorite_str2 == "見る") reservation.consider_favorite2 = true;
                    else reservation.consider_favorite2 = false;

                    if (reservation.reexhibit_flag_str == "する") reservation.reexhibit_flag = true;
                    else reservation.reexhibit_flag = false;
                    reservation.accountNickName = sQLiteDataReader["nickname"].ToString();
                    reservation.item_name = sQLiteDataReader["item_name"].ToString();
                    reservation.imagepath = sQLiteDataReader["Pic1"].ToString();
                    rst.Add(reservation);
                } catch (Exception ex) {
                    Log.Logger.Error("予約読み込み中エラー : " + ex.Message);
                    Console.WriteLine("予約読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        public List<ReservationSettingForm.ReservationSetting> selectReservation(List<int> DBIdList) {
            List<ReservationSettingForm.ReservationSetting> rst = new List<ReservationSettingForm.ReservationSetting>();
            if (DBIdList.Count == 0) return rst;
            string text = string.Join(",", DBIdList.ToArray());
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("SELECT r.Id, r.itemDBId, r.accountDBId, r.status, r.exhibitDate, r.deleteDate, r.deleteDate2, r.item_id, r.check_favorite, r.check_favorite2, r.check_comment, r.check_comment2, r.reexhibit_flag , i.ItemName, i.Pic1, a.nickname from ((reservations r JOIN items i ON r.itemDBId = i.Id )  INNER JOIN accounts a ON a.Id = r.accountDBId) where r.Id in (" + text + ");", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    ReservationSettingForm.ReservationSetting reservation = new ReservationSettingForm.ReservationSetting();
                    reservation.DBId = int.Parse(sQLiteDataReader["Id"].ToString());
                    reservation.itemDBId = int.Parse(sQLiteDataReader["itemDBId"].ToString());
                    reservation.accountDBId = int.Parse(sQLiteDataReader["accountDBId"].ToString());
                    reservation.exhibit_status = int.Parse(sQLiteDataReader["status"].ToString());
                    reservation.exhibit_status_str = ReservationSettingForm.get_exhibit_status_str(reservation.exhibit_status);
                    reservation.exhibitDateString = sQLiteDataReader["exhibitDate"].ToString();
                    reservation.exhibitDate = DateTime.Parse(reservation.exhibitDateString);
                    reservation.deleteDateString = sQLiteDataReader["deleteDate"].ToString();
                    reservation.deleteDate = DateTime.Parse(reservation.deleteDateString);
                    reservation.deleteDateString2 = sQLiteDataReader["deleteDate2"].ToString();
                    reservation.deleteDate2 = DateTime.Parse(reservation.deleteDateString2);
                    reservation.item_id = sQLiteDataReader["item_id"].ToString();
                    reservation.consider_comment_str = sQLiteDataReader["check_comment"].ToString();
                    reservation.consider_favorite_str = sQLiteDataReader["check_favorite"].ToString();
                    reservation.consider_comment_str2 = sQLiteDataReader["check_comment2"].ToString();
                    reservation.consider_favorite_str2 = sQLiteDataReader["check_favorite2"].ToString();
                    reservation.reexhibit_flag_str = sQLiteDataReader["reexhibit_flag"].ToString();
                    if (reservation.consider_comment_str == "見る") reservation.consider_comment = true;
                    else reservation.consider_comment = false;
                    if (reservation.consider_favorite_str == "見る") reservation.consider_favorite = true;
                    else reservation.consider_favorite = false;
                    if (reservation.consider_comment_str2 == "見る") reservation.consider_comment2 = true;
                    else reservation.consider_comment2 = false;
                    if (reservation.consider_favorite_str2 == "見る") reservation.consider_favorite2 = true;
                    else reservation.consider_favorite2 = false;
                    if (reservation.reexhibit_flag_str == "する") reservation.reexhibit_flag = true;
                    else reservation.reexhibit_flag = false;
                    reservation.accountNickName = sQLiteDataReader["nickname"].ToString();
                    reservation.item_name = sQLiteDataReader["ItemName"].ToString();
                    reservation.imagepath = sQLiteDataReader["Pic1"].ToString();
                    rst.Add(reservation);
                } catch (Exception ex) {
                    Log.Logger.Error("予約読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        public List<ReservationSettingForm.ReservationSetting> selectReservationFromName(string text) {
            List<ReservationSettingForm.ReservationSetting> rst = new List<ReservationSettingForm.ReservationSetting>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("SELECT r.Id, r.itemDBId, r.accountDBId, r.status, r.exhibitDate, r.deleteDate, r.deleteDate2, r.item_id, r.check_favorite, r.check_favorite2, r.check_comment, r.check_comment2, i.ItemName, i.Pic1, a.nickname from ((reservations r JOIN items i ON r.itemDBId = i.Id )  INNER JOIN accounts a ON a.Id = r.accountDBId) where i.ItemName like '%" + text + "%';", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    ReservationSettingForm.ReservationSetting reservation = new ReservationSettingForm.ReservationSetting();
                    reservation.DBId = int.Parse(sQLiteDataReader["Id"].ToString());
                    reservation.itemDBId = int.Parse(sQLiteDataReader["itemDBId"].ToString());
                    reservation.accountDBId = int.Parse(sQLiteDataReader["accountDBId"].ToString());
                    reservation.exhibit_status = int.Parse(sQLiteDataReader["status"].ToString());
                    reservation.exhibit_status_str = ReservationSettingForm.get_exhibit_status_str(reservation.exhibit_status);
                    reservation.exhibitDateString = sQLiteDataReader["exhibitDate"].ToString();
                    reservation.exhibitDate = DateTime.Parse(reservation.exhibitDateString);
                    reservation.deleteDateString = sQLiteDataReader["deleteDate"].ToString();
                    reservation.deleteDate = DateTime.Parse(reservation.deleteDateString);
                    reservation.deleteDateString2 = sQLiteDataReader["deleteDate2"].ToString();
                    reservation.deleteDate2 = DateTime.Parse(reservation.deleteDateString2);
                    reservation.item_id = sQLiteDataReader["item_id"].ToString();
                    reservation.consider_comment_str = sQLiteDataReader["check_comment"].ToString();
                    reservation.consider_favorite_str = sQLiteDataReader["check_favorite"].ToString();
                    reservation.consider_comment_str2 = sQLiteDataReader["check_comment2"].ToString();
                    reservation.consider_favorite_str2 = sQLiteDataReader["check_favorite2"].ToString();
                    reservation.reexhibit_flag_str = sQLiteDataReader["reexhibit_flag"].ToString();
                    if (reservation.consider_comment_str == "見る") reservation.consider_comment = true;
                    else reservation.consider_comment = false;
                    if (reservation.consider_favorite_str == "見る") reservation.consider_favorite = true;
                    else reservation.consider_favorite = false;
                    if (reservation.consider_comment_str2 == "見る") reservation.consider_comment2 = true;
                    else reservation.consider_comment2 = false;
                    if (reservation.consider_favorite_str2 == "見る") reservation.consider_favorite2 = true;
                    else reservation.consider_favorite2 = false;
                    if (reservation.reexhibit_flag_str == "する") reservation.reexhibit_flag = true;
                    else reservation.reexhibit_flag = false;
                    reservation.accountNickName = sQLiteDataReader["nickname"].ToString();
                    reservation.item_name = sQLiteDataReader["ItemName"].ToString();
                    reservation.imagepath = sQLiteDataReader["Pic1"].ToString();
                    rst.Add(reservation);
                } catch (Exception ex) {
                    Log.Logger.Error("予約読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        public bool deleteReservation(List<int> DBIdList) {
            if (DBIdList.Count == 0) return true;
            try {
                string text = string.Join(",", DBIdList.ToArray());
                this.conn.Open();
                string commandText = "DELETE FROM reservations Where Id in (" + text + ");";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("予約DBからの予約削除成功");
                return true;
            } catch (Exception ex) {
                conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("予約DBからの予約削除失敗");
                return false;
            }
        }

        public bool updateItemID(int DBId, string item_id) {
            try {
                conn.Open();
                string commandText = "UPDATE reservations SET "
                                    + "item_id = '" + item_id + "'"
                                    + " WHERE Id = " + DBId.ToString() + ";";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("予約DBの商品ID更新に成功");
                return true;
            } catch (Exception ex) {
                conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("予約DBの商品ID更新に失敗");
                return false;
            }
        }

        public bool updateExhibitStatus(int DBId, int status) {
            try {
                conn.Open();
                string commandText = "UPDATE reservations SET "
                                    + "status = " + status.ToString()
                                    + " WHERE Id = " + DBId.ToString() + ";";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("予約DBの実行結果状態更新に成功");
                return true;
            } catch (Exception ex) {
                conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("予約DBの実行結果状態更新に失敗");
                return false;
            }
        }
    }
}
