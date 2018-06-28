using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace RakuLand.DBHelper {
    class FrilItemDBHelper {
        public const string DBname = "database.db";
        private SQLiteConnection conn;
        public FrilItemDBHelper() {
            this.conn = new SQLiteConnection("Data Source=" + DBname + ";foreign keys = true;");
        }
     

        public void onCreate() {
            conn.Open();
            string commandText = "CREATE TABLE IF NOT EXISTS items ( Id INTEGER PRIMARY KEY AUTOINCREMENT,"
                                + "item_id TEXT,item_name TEXT, detail TEXT,"
                                + "s_price INTEGER,status INTEGER, t_status INTEGER,"
                                + "carriage INTEGER,d_method INTEGER, d_date INTEGER,d_area INTEGER,"
                                + "user_id TEXT, created_at TEXT,"//created_atはDateTime型
                                + "screen_name TEXT, category_id INTEGER, category_p_id INTEGER,category_level1_id INTEGER,category_level2_id INTEGER,category_level3_id INTEGER,category_level4_id INTEGER,"
                                + "size_id INTEGER, size_name TEXT,brand_id INTEGER,"
                                + "i_brand_id INTEGER, comments_count INTEGER,"
                                + "likes_count INTEGER,"
                                + "Pic1 TEXT,Pic2 TEXT,Pic3 TEXT,Pic4 TEXT,number INTEGER);";//imagepathsのほう
                   
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
        private int getMaxNumber() {
            //numberの最大値を取得する
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select max(number) from items;", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            int max_number = -1;
            while (sQLiteDataReader.Read()) {
                try {
                    max_number = int.Parse(sQLiteDataReader["max(number)"].ToString());
                } catch (Exception ex) {
                    Log.Logger.Error("DBのnumberの最大値取得失敗");
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            Console.WriteLine(max_number);
            return max_number;
        }
        public void addNumberColumn() {
            int user_version = LoadUserVersion();
            //更新
            if (user_version < 14) {
                conn.Open();
                string commandText = "pragma user_version = 14;";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();

                //conn.Open();
                //commandText = "alter table items add number INTEGER;";
                //command = conn.CreateCommand();
                //command.CommandText = commandText;
                //command.ExecuteNonQuery();
                //command.Dispose();
                //conn.Close();

                conn.Open();
                commandText = "update items set number = rowid;";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
        }
        public void addItem(FrilItem item) {
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
            string commandText = "INSERT INTO items ("
                                    + "item_id, item_name, detail, s_price, status, t_status,"
                                    + "carriage, d_method, d_date, d_area, "
                                    + "user_id, created_at,screen_name,category_id, "
                                    + "category_p_id,category_level1_id,category_level2_id,category_level3_id,category_level4_id,size_id,size_name,brand_id,i_brand_id,"
                                    + "comments_count,likes_count,Pic1,Pic2,Pic3,Pic4,number) "
                                    + "VALUES ('" + item.item_id.Replace("'", "''") + "','" + item.item_name.Replace("'", "''")
                                    + "','" + item.detail + "'," + item.s_price.ToString() + "," + item.status.ToString()
                                    + "," + item.t_status.ToString() + ","
                                    + item.carriage.ToString() + "," + item.d_method.ToString() + ","
                                    + item.d_date.ToString() + "," + item.d_area.ToString() + ",'"
                                    + item.user_id + "','" + item.created_at.ToString() + "','" + item.screen_name.ToString() + "',"
                                    + item.category_id.ToString() + "," + item.category_p_id.ToString() + "," + item.category_level1_id.ToString() + "," + item.category_level2_id.ToString() + "," + item.category_level3_id.ToString() + "," + item.category_level4_id.ToString() + ","
                                    + item.size_id.ToString() + ",'" + item.size_name + "',"
                                    + item.brand_id.ToString() + "," + item.i_brand_id + ","
                                    + item.comments_count.ToString() + "," + item.likes_count + ",'"
                                    + item.imagepaths[0].ToString() + "','" + item.imagepaths[1] + "','"
                                    + item.imagepaths[2].ToString() + "','" + item.imagepaths[3] + "'," + new_number.ToString() + ");";

            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
    }

        public List<FrilItem> loadItems() {
            List<FrilItem> rst = new List<FrilItem>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select * from ((items it LEFT OUTER JOIN item_family if ON if.itemDBId = it.Id )  LEFT OUTER JOIN zaiko z ON z.parent_id = if.parent_id) order by number;", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
           while (sQLiteDataReader.Read()) {
                try {
                    FrilItem item = new FrilItem();
                    item.item_id = sQLiteDataReader["item_id"].ToString();
                    item.item_name = sQLiteDataReader["item_name"].ToString();
                    item.detail = sQLiteDataReader["detail"].ToString();
                    item.s_price = int.Parse(sQLiteDataReader["s_price"].ToString());
                    item.status = int.Parse(sQLiteDataReader["status"].ToString());
                    item.t_status = int.Parse(sQLiteDataReader["t_status"].ToString());
                    item.carriage = int.Parse(sQLiteDataReader["carriage"].ToString());
                    item.d_method = int.Parse(sQLiteDataReader["d_method"].ToString());
                    item.d_date = int.Parse(sQLiteDataReader["d_date"].ToString());
                    item.d_area = int.Parse(sQLiteDataReader["d_area"].ToString());
                    item.user_id = sQLiteDataReader["user_id"].ToString();
                    item.created_at = DateTime.Parse( sQLiteDataReader["created_at"].ToString());
                    item.screen_name = sQLiteDataReader["screen_name"].ToString();
                    item.category_id = int.Parse(sQLiteDataReader["category_id"].ToString());
                    item.category_p_id = int.Parse(sQLiteDataReader["category_p_id"].ToString());
                    item.size_id = int.Parse(sQLiteDataReader["size_id"].ToString());
                    item.size_name = sQLiteDataReader["size_name"].ToString();
                    item.brand_id = int.Parse(sQLiteDataReader["brand_id"].ToString());
                    item.i_brand_id = int.Parse(sQLiteDataReader["i_brand_id"].ToString());
                    item.comments_count = int.Parse(sQLiteDataReader["comments_count"].ToString());
                    item.likes_count = int.Parse(sQLiteDataReader["likes_count"].ToString());
                    item.imagepaths[0] = sQLiteDataReader["Pic1"].ToString();
                    item.imagepaths[1] = sQLiteDataReader["Pic2"].ToString();
                    item.imagepaths[2] = sQLiteDataReader["Pic3"].ToString();
                    item.imagepaths[3] = sQLiteDataReader["Pic4"].ToString();
                    item.parent_id = sQLiteDataReader.IsDBNull(sQLiteDataReader.GetOrdinal("parent_id")) ? "" : sQLiteDataReader["parent_id"].ToString();
                    item.child_id = sQLiteDataReader.IsDBNull(sQLiteDataReader.GetOrdinal("child_id")) ? "" : sQLiteDataReader["child_id"].ToString();
                    item.zaikonum = sQLiteDataReader.IsDBNull(sQLiteDataReader.GetOrdinal("zaikonum")) ? -1 : int.Parse(sQLiteDataReader["zaikonum"].ToString());
                    //mitem.loadImageFromFile();
                    item.Image = null;
                    item.DBId = int.Parse(sQLiteDataReader["Id"].ToString());
                    rst.Add(item);
                } catch (Exception ex) {
                    Log.Logger.Error("商品読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        public List<FrilItem> selectItem(List<int> DBIdList) {
            List<FrilItem> rst = new List<FrilItem>();
            if (DBIdList.Count == 0) return rst;
            string text = string.Join(",", DBIdList.ToArray());
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select * from ((items it LEFT OUTER JOIN item_family if ON if.itemDBId = it.Id )  LEFT OUTER JOIN zaiko z ON z.parent_id = if.parent_id) where it.id in (" + text + ") order by number;", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    FrilItem item = new FrilItem();
                    item.item_id = sQLiteDataReader["item_id"].ToString();
                    item.item_name = sQLiteDataReader["item_name"].ToString();
                    item.detail = sQLiteDataReader["detail"].ToString();
                    item.s_price = int.Parse(sQLiteDataReader["s_price"].ToString());
                    item.status = int.Parse(sQLiteDataReader["status"].ToString());
                    item.t_status = int.Parse(sQLiteDataReader["t_status"].ToString());
                    item.carriage = int.Parse(sQLiteDataReader["carriage"].ToString());
                    item.d_method = int.Parse(sQLiteDataReader["d_method"].ToString());
                    item.d_date = int.Parse(sQLiteDataReader["d_date"].ToString());
                    item.d_area = int.Parse(sQLiteDataReader["d_area"].ToString());
                    item.user_id = sQLiteDataReader["user_id"].ToString();
                    item.created_at = DateTime.Parse(sQLiteDataReader["created_at"].ToString());
                    item.screen_name = sQLiteDataReader["screen_name"].ToString();
                    item.category_id = int.Parse(sQLiteDataReader["category_id"].ToString());
                    item.category_p_id = int.Parse(sQLiteDataReader["category_p_id"].ToString());
                    item.size_id = int.Parse(sQLiteDataReader["size_id"].ToString());
                    item.size_name = sQLiteDataReader["size_name"].ToString();
                    item.brand_id = int.Parse(sQLiteDataReader["brand_id"].ToString());
                    item.i_brand_id = int.Parse(sQLiteDataReader["i_brand_id"].ToString());
                    item.comments_count = int.Parse(sQLiteDataReader["comments_count"].ToString());
                    item.likes_count = int.Parse(sQLiteDataReader["likes_count"].ToString());
                    item.imagepaths[0] = sQLiteDataReader["Pic1"].ToString();
                    item.imagepaths[1] = sQLiteDataReader["Pic2"].ToString();
                    item.imagepaths[2] = sQLiteDataReader["Pic3"].ToString();
                    item.imagepaths[3] = sQLiteDataReader["Pic4"].ToString();
                    item.parent_id = sQLiteDataReader.IsDBNull(sQLiteDataReader.GetOrdinal("parent_id")) ? "" : sQLiteDataReader["parent_id"].ToString();
                    item.child_id = sQLiteDataReader.IsDBNull(sQLiteDataReader.GetOrdinal("child_id")) ? "" : sQLiteDataReader["child_id"].ToString();
                    item.zaikonum = sQLiteDataReader.IsDBNull(sQLiteDataReader.GetOrdinal("zaikonum")) ? -1 : int.Parse(sQLiteDataReader["zaikonum"].ToString());
                    item.Image = null;//mitem.loadImageFromFile();
                    item.DBId = int.Parse(sQLiteDataReader["Id"].ToString());
                    rst.Add(item);
                } catch (Exception ex) {
                    Log.Logger.Error("商品読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        public List<FrilItem> selectRegisterItem(List<int> DBId) {
            List<FrilItem> rst = new List<FrilItem>();
            if (DBId.Count == 0) return rst;
            string text = string.Join(",", DBId.ToArray());
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select * from items where Id in (" + text + ") order by number;", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    FrilItem item = new FrilItem();
                    item.item_name = sQLiteDataReader["item_name"].ToString();
                    item.detail = sQLiteDataReader["detail"].ToString();
                    item.imagepaths[0] = sQLiteDataReader["Pic1"].ToString();
                    item.imagepaths[1] = sQLiteDataReader["Pic2"].ToString();
                    item.imagepaths[2] = sQLiteDataReader["Pic3"].ToString();
                    item.imagepaths[3] = sQLiteDataReader["Pic4"].ToString();
                    item.category_level1_id = int.Parse(sQLiteDataReader["category_level1_id"].ToString());
                    item.category_level2_id = int.Parse(sQLiteDataReader["category_level2_id"].ToString());
                    item.category_level3_id = int.Parse(sQLiteDataReader["category_level3_id"].ToString());
                    item.category_level4_id = int.Parse(sQLiteDataReader["category_level4_id"].ToString());
                    item.size_id = int.Parse(sQLiteDataReader["size_id"].ToString());
                    item.brand_id = int.Parse(sQLiteDataReader["brand_id"].ToString());
                    item.status = int.Parse(sQLiteDataReader["status"].ToString());
                    item.carriage = int.Parse(sQLiteDataReader["carriage"].ToString());
                    item.d_date = int.Parse(sQLiteDataReader["d_date"].ToString());
                    item.d_method = int.Parse(sQLiteDataReader["d_method"].ToString());
                    item.d_area = int.Parse(sQLiteDataReader["d_area"].ToString());
                    item.s_price = int.Parse(sQLiteDataReader["s_price"].ToString());
                    rst.Add(item);
                } catch (Exception ex) {
                    Log.Logger.Error("商品読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        public List<FrilItem> selectItemFromName(string text) {
            List<FrilItem> rst = new List<FrilItem>();
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select * from ((items it LEFT OUTER JOIN item_family if ON if.itemDBId = it.Id )  LEFT OUTER JOIN zaiko z ON z.parent_id = if.parent_id) where ItemDescription like '%" + text + "%' order by number;", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    FrilItem item = new FrilItem();
                    item.item_id = sQLiteDataReader["item_id"].ToString();
                    item.item_name = sQLiteDataReader["item_name"].ToString();
                    item.detail = sQLiteDataReader["detail"].ToString();
                    item.s_price = int.Parse(sQLiteDataReader["s_price"].ToString());
                    item.status = int.Parse(sQLiteDataReader["status"].ToString());
                    item.t_status = int.Parse(sQLiteDataReader["t_status"].ToString());
                    item.carriage = int.Parse(sQLiteDataReader["carriage"].ToString());
                    item.d_method = int.Parse(sQLiteDataReader["d_method"].ToString());
                    item.d_date = int.Parse(sQLiteDataReader["d_date"].ToString());
                    item.d_area = int.Parse(sQLiteDataReader["d_area"].ToString());
                    item.user_id = sQLiteDataReader["user_id"].ToString();
                    item.created_at = DateTime.Parse(sQLiteDataReader["created_at"].ToString());
                    item.screen_name = sQLiteDataReader["screen_name"].ToString();
                    item.category_id = int.Parse(sQLiteDataReader["category_id"].ToString());
                    item.category_p_id = int.Parse(sQLiteDataReader["category_p_id"].ToString());
                    item.size_id = int.Parse(sQLiteDataReader["size_id"].ToString());
                    item.size_name = sQLiteDataReader["size_name"].ToString();
                    item.brand_id = int.Parse(sQLiteDataReader["brand_id"].ToString());
                    item.i_brand_id = int.Parse(sQLiteDataReader["i_brand_id"].ToString());
                    item.comments_count = int.Parse(sQLiteDataReader["comments_count"].ToString());
                    item.likes_count = int.Parse(sQLiteDataReader["likes_count"].ToString());
                    item.imagepaths[0] = sQLiteDataReader["Pic1"].ToString();
                    item.imagepaths[1] = sQLiteDataReader["Pic2"].ToString();
                    item.imagepaths[2] = sQLiteDataReader["Pic3"].ToString();
                    item.imagepaths[3] = sQLiteDataReader["Pic4"].ToString();
                    //FrilItem mitem = ItemRegisterForm.getFrilItemFromExhibitItem(item);
                    //mitem.parent_id = sQLiteDataReader.IsDBNull(sQLiteDataReader.GetOrdinal("parent_id")) ? "" : sQLiteDataReader["parent_id"].ToString();
                    //mitem.child_id = sQLiteDataReader.IsDBNull(sQLiteDataReader.GetOrdinal("child_id")) ? "" : sQLiteDataReader["child_id"].ToString();
                    //mitem.zaikonum = sQLiteDataReader.IsDBNull(sQLiteDataReader.GetOrdinal("zaikonum")) ? -1 : int.Parse(sQLiteDataReader["zaikonum"].ToString());
                    //mitem.Image = null;//mitem.loadImageFromFile();
                    //mitem.DBId = int.Parse(sQLiteDataReader["Id"].ToString());
                    //rst.Add(mitem);
                } catch (Exception ex) {
                    Log.Logger.Error("商品読み込み中エラー : " + ex.Message);
                }
            }
            sQLiteDataReader.Close();
            this.conn.Close();
            sQLiteCommand.Dispose();
            return rst;
        }

        public bool updateItem(int DBId, FrilItem item) {
            try {
                conn.Open();
                string commandText = "UPDATE items SET "
                                    + "item_id = '" + item.item_id.Replace("'", "''") + "',"
                                    + "item_name = '" + item.item_name.Replace("'", "''") + "',"
                                    + "detail = '" + item.detail.Replace("'", "''") + "',"
                                    + "s_price = " + item.s_price.ToString() + ","
                                    + "status = " + item.status.ToString() + ","
                                    + "t_status = " + item.t_status.ToString() + ","
                                    + "carriage = " + item.carriage.ToString() + ","
                                    + "d_method = " + item.d_method.ToString() + ","
                                    + "d_date = " + item.d_date.ToString() + ","
                                    + "d_area = " + item.d_area.ToString() + ","
                                    + "user_id = '" + item.user_id.Replace("'", "''") + "',"
                                    + "created_at = '" + item.created_at.ToString().Replace("'", "''") + "',"
                                    + "screen_name = '" + item.screen_name.Replace("'", "''") + "',"
                                    + "category_id = " + item.category_id.ToString() + ","
                                    + "category_p_id = " + item.category_p_id.ToString() + ","
                                    + "category_level1_id = " + item.category_level1_id.ToString() + ","
                                    + "category_level2_id = " + item.category_level2_id.ToString() + ","
                                    + "category_level3_id = " + item.category_level3_id.ToString() + ","
                                    + "category_level4_id = " + item.category_level4_id.ToString() + ","
                                    + "size_id = " + item.size_id.ToString() + ","
                                    + "size_name = '" + item.size_name.Replace("'", "''") + "',"
                                    + "brand_id = " + item.brand_id.ToString() + ","
                                    + "i_brand_id = " + item.i_brand_id.ToString() + ","
                                    + "comments_count = " + item.comments_count.ToString() + ","
                                    + "brand_id = " + item.brand_id.ToString() + ","
                                    + "likes_count = " + item.likes_count.ToString() + ","
                                    + "Pic1 = '" + item.imagepaths[0] + "',"
                                    + "Pic2 = '" + item.imagepaths[1] + "',"
                                    + "Pic3 = '" + item.imagepaths[2] + "',"
                                    + "Pic4 = '" + item.imagepaths[3] + "',"
                                    + " WHERE Id = " + DBId.ToString() + ";";

                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("商品DBの更新に成功");
                return true;
            } catch (Exception ex) {
                conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("商品DBの更新に失敗");
                return false;
            }
        }


        //同一商品がDBに存在すればID返す なければ0
        public int getItemDBId(FrilItem item) {
            int rst = 0;
            this.conn.Open();
            try {
                SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.Id from items i where"
                                    + "i.item_id = '" + item.item_id.Replace("'", "''") + "'"
                                    + " AND i.item_name = '" + item.item_name.Replace("'", "''") + "'"
                                    + " AND i.detail = '" + item.detail.Replace("'", "''") + "'"
                                    + " AND i.s_price = " + item.s_price.ToString() + ""
                                    + " AND i.status = " + item.status.ToString() + ""
                                    + " AND i.t_status = " + item.t_status.ToString() + ""
                                    + " AND i.carriage = " + item.carriage.ToString() + ""
                                    + " AND i.d_method = " + item.d_method.ToString() + ""
                                    + " AND i.d_date = " + item.d_date.ToString() + ""
                                    + " AND i.d_area = " + item.d_area.ToString() + ""
                                    + " AND i.user_id = '" + item.user_id.Replace("'", "''") + "'"
                                    + " AND i.created_at = '" + item.created_at.ToString().Replace("'", "''") + "'"
                                    + " AND i.screen_name = '" + item.screen_name.Replace("'", "''") + "'"
                                    + " AND i.category_id = " + item.category_id.ToString() + ""
                                    + " AND i.category_p_id = " + item.category_p_id.ToString() + ""
                                    + " AND i.size_id = " + item.size_id.ToString() + ""
                                    + " AND i.size_name = '" + item.size_name.Replace("'", "''") + "'"
                                    + " AND i.brand_id = " + item.brand_id.ToString() + ""
                                    + " AND i.i_brand_id = " + item.i_brand_id.ToString() + ""
                                    + " AND i.comments_count = " + item.comments_count.ToString() + ""
                                    + " AND i.brand_id = " + item.brand_id.ToString() + ""
                                    + " AND i.likes_count = " + item.likes_count.ToString() + ""
                                    + " AND i.Pic1 = '" + item.imagepaths[0] + "'"
                                    + " AND i.Pic2 = '" + item.imagepaths[1] + "'"
                                    + " AND i.Pic3 = '" + item.imagepaths[2] + "'"
                                    + " AND i.Pic4 = '" + item.imagepaths[3] + "';", this.conn);

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
        public void swapNumber(int DBId1, int DBId2) {
            //それぞれのnumberを取得する
            int number1, number2;
            number1 = number2 = -1;

            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.number from items i where i.id = " + DBId1.ToString() + ";", this.conn);
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
            sQLiteCommand = new SQLiteCommand("select i.number from items i where i.id = " + DBId2.ToString() + ";", this.conn);
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
                string commandText = "update items set number = " + number2.ToString() + " where id = " + DBId1.ToString() + ";";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();

                conn.Open();
                commandText = "update items set number = " + number1.ToString() + " where id = " + DBId2.ToString() + ";";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
        }

        public bool deleteItem(List<int> DBIdList) {
            if (DBIdList.Count == 0) return true;
            try {
                string text = string.Join(",", DBIdList.ToArray());
                this.conn.Open();
                string commandText = "DELETE FROM items Where Id in (" + text + ");";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                Log.Logger.Info("商品DBからの商品削除成功");
                return true;
            } catch (Exception ex) {
                conn.Close();
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("商品DBからの商品削除失敗");
                return false;
            }
        }











    }
}





