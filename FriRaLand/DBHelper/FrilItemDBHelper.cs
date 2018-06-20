using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace FriRaLand.DBHelper {
    class FrilItemDBHelper {
        public const string DBname = "database.db";
        private SQLiteConnection conn;
        public FrilItemDBHelper() {
            this.conn = new SQLiteConnection("Data Source=" + DBname + ";foreign keys = true;");
        }

        public void onCreate() {
            conn.Open();
            string commandText = "CREATE TABLE IF NOT EXISTS items ( Id INTEGER PRIMARY KEY AUTOINCREMENT,"
                                + "ItemName TEXT,ItemDescription TEXT,"
                                + "Pic1 TEXT,Pic2 TEXT,Pic3 TEXT,Pic4 TEXT,"
                                + "CategoryLevel1 INTEGER, CategoryLevel2 INTEGER, CategoryLevel3 INTEGER, CategoryLevel4 INTEGER, "
                                + "Size INTEGER, Brand INTEGER, ItemCondition INTEGER, "
                                + "ShippingPayer INTEGER, ShippingDuration INTEGER, ShippingMethod INTEGER,ShippingArea INTEGER,"
                                + "Price INTEGER);";
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

                conn.Open();
                commandText = "alter table items add number INTEGER;";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();

                conn.Open();
                commandText = "update items set number = rowid;";
                command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
        }
        public void addItem(ItemRegisterForm.FrilExhibitItem item) {
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
                                + "ItemName, ItemDescription, Pic1, Pic2, Pic3, Pic4,"
                                + "CategoryLevel1, CategoryLevel2, CategoryLevel3, CategoryLevel4, "
                                + "Size, Brand, ItemCondition, "
                                + "ShippingPayer, ShippingDuration, ShippingMethod, ShippingArea,"
                                + "Price, number) "
                                + "VALUES ('" + item.name.Replace("'", "''") + "','" + item.description.Replace("'", "''") + "','" + item.picturelocation[0] + "','" + item.picturelocation[1] + "','" + item.picturelocation[2] + "','" + item.picturelocation[3] + "',"
                                + item.category_level1_id.ToString() + "," + item.category_level2_id.ToString() + "," + item.category_level3_id.ToString() + "," + item.category_level4_id.ToString() + ","
                                + item.size_id.ToString() + "," + item.brand_id.ToString() + "," + item.item_condition_id.ToString() + ","
                                + item.shipping_payer_id.ToString() + "," + item.shipping_duration_id.ToString() + "," + item.shipping_method_id.ToString() + "," + item.shipping_area_id.ToString() + ","
                                + item.price.ToString() + ", " + new_number.ToString() + ");";
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
                    ItemRegisterForm.FrilExhibitItem item = new ItemRegisterForm.FrilExhibitItem();
                    item.name = sQLiteDataReader["ItemName"].ToString();
                    item.description = sQLiteDataReader["ItemDescription"].ToString();
                    item.picturelocation[0] = sQLiteDataReader["Pic1"].ToString();
                    item.picturelocation[1] = sQLiteDataReader["Pic2"].ToString();
                    item.picturelocation[2] = sQLiteDataReader["Pic3"].ToString();
                    item.picturelocation[3] = sQLiteDataReader["Pic4"].ToString();
                    item.category_level1_id = int.Parse(sQLiteDataReader["CategoryLevel1"].ToString());
                    item.category_level2_id = int.Parse(sQLiteDataReader["CategoryLevel2"].ToString());
                    item.category_level3_id = int.Parse(sQLiteDataReader["CategoryLevel3"].ToString());
                    item.category_level4_id = int.Parse(sQLiteDataReader["CategoryLevel4"].ToString());
                    item.size_id = int.Parse(sQLiteDataReader["Size"].ToString());
                    item.brand_id = int.Parse(sQLiteDataReader["Brand"].ToString());
                    item.item_condition_id = int.Parse(sQLiteDataReader["ItemCondition"].ToString());
                    item.shipping_payer_id = int.Parse(sQLiteDataReader["ShippingPayer"].ToString());
                    item.shipping_duration_id = int.Parse(sQLiteDataReader["ShippingDuration"].ToString());
                    item.shipping_method_id = int.Parse(sQLiteDataReader["ShippingMethod"].ToString());
                    item.shipping_area_id = int.Parse(sQLiteDataReader["ShippingArea"].ToString());
                    item.price = int.Parse(sQLiteDataReader["Price"].ToString());
                    //FrilItem mitem = ItemRegisterForm.getFrilItemFromExhibitItem(item);
                    //mitem.parent_id = sQLiteDataReader.IsDBNull(sQLiteDataReader.GetOrdinal("parent_id")) ? "" : sQLiteDataReader["parent_id"].ToString();
                    //mitem.child_id = sQLiteDataReader.IsDBNull(sQLiteDataReader.GetOrdinal("child_id")) ? "" : sQLiteDataReader["child_id"].ToString();
                    //mitem.zaikonum = sQLiteDataReader.IsDBNull(sQLiteDataReader.GetOrdinal("zaikonum")) ? -1 : int.Parse(sQLiteDataReader["zaikonum"].ToString());
                    //// mitem.loadImageFromFile();
                    //mitem.Image = null;
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

        public List<FrilItem> selectItem(List<int> DBIdList) {
            List<FrilItem> rst = new List<FrilItem>();
            if (DBIdList.Count == 0) return rst;
            string text = string.Join(",", DBIdList.ToArray());
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select * from ((items it LEFT OUTER JOIN item_family if ON if.itemDBId = it.Id )  LEFT OUTER JOIN zaiko z ON z.parent_id = if.parent_id) where it.id in (" + text + ") order by number;", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    ItemRegisterForm.FrilExhibitItem item = new ItemRegisterForm.FrilExhibitItem();
                    item.name = sQLiteDataReader["ItemName"].ToString();
                    item.description = sQLiteDataReader["ItemDescription"].ToString();
                    item.picturelocation[0] = sQLiteDataReader["Pic1"].ToString();
                    item.picturelocation[1] = sQLiteDataReader["Pic2"].ToString();
                    item.picturelocation[2] = sQLiteDataReader["Pic3"].ToString();
                    item.picturelocation[3] = sQLiteDataReader["Pic4"].ToString();
                    item.category_level1_id = int.Parse(sQLiteDataReader["CategoryLevel1"].ToString());
                    item.category_level2_id = int.Parse(sQLiteDataReader["CategoryLevel2"].ToString());
                    item.category_level3_id = int.Parse(sQLiteDataReader["CategoryLevel3"].ToString());
                    item.category_level4_id = int.Parse(sQLiteDataReader["CategoryLevel4"].ToString());
                    item.size_id = int.Parse(sQLiteDataReader["Size"].ToString());
                    item.brand_id = int.Parse(sQLiteDataReader["Brand"].ToString());
                    item.item_condition_id = int.Parse(sQLiteDataReader["ItemCondition"].ToString());
                    item.shipping_payer_id = int.Parse(sQLiteDataReader["ShippingPayer"].ToString());
                    item.shipping_duration_id = int.Parse(sQLiteDataReader["ShippingDuration"].ToString());
                    item.shipping_method_id = int.Parse(sQLiteDataReader["ShippingMethod"].ToString());
                    item.shipping_area_id = int.Parse(sQLiteDataReader["ShippingArea"].ToString());
                    item.price = int.Parse(sQLiteDataReader["Price"].ToString());
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

        public List<ItemRegisterForm.FrilExhibitItem> selectExhibitItem(List<int> DBId) {
            List<ItemRegisterForm.FrilExhibitItem> rst = new List<ItemRegisterForm.FrilExhibitItem>();
            if (DBId.Count == 0) return rst;
            string text = string.Join(",", DBId.ToArray());
            this.conn.Open();
            SQLiteCommand sQLiteCommand = new SQLiteCommand("select * from items where Id in (" + text + ") order by number;", this.conn);
            SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            while (sQLiteDataReader.Read()) {
                try {
                    ItemRegisterForm.FrilExhibitItem item = new ItemRegisterForm.FrilExhibitItem();
                    item.name = sQLiteDataReader["ItemName"].ToString();
                    item.description = sQLiteDataReader["ItemDescription"].ToString();
                    item.picturelocation[0] = sQLiteDataReader["Pic1"].ToString();
                    item.picturelocation[1] = sQLiteDataReader["Pic2"].ToString();
                    item.picturelocation[2] = sQLiteDataReader["Pic3"].ToString();
                    item.picturelocation[3] = sQLiteDataReader["Pic4"].ToString();
                    item.category_level1_id = int.Parse(sQLiteDataReader["CategoryLevel1"].ToString());
                    item.category_level2_id = int.Parse(sQLiteDataReader["CategoryLevel2"].ToString());
                    item.category_level3_id = int.Parse(sQLiteDataReader["CategoryLevel3"].ToString());
                    item.category_level4_id = int.Parse(sQLiteDataReader["CategoryLevel4"].ToString());
                    item.size_id = int.Parse(sQLiteDataReader["Size"].ToString());
                    item.brand_id = int.Parse(sQLiteDataReader["Brand"].ToString());
                    item.item_condition_id = int.Parse(sQLiteDataReader["ItemCondition"].ToString());
                    item.shipping_payer_id = int.Parse(sQLiteDataReader["ShippingPayer"].ToString());
                    item.shipping_duration_id = int.Parse(sQLiteDataReader["ShippingDuration"].ToString());
                    item.shipping_method_id = int.Parse(sQLiteDataReader["ShippingMethod"].ToString());
                    item.shipping_area_id = int.Parse(sQLiteDataReader["ShippingArea"].ToString());
                    item.price = int.Parse(sQLiteDataReader["Price"].ToString());
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
                    ItemRegisterForm.FrilExhibitItem item = new ItemRegisterForm.FrilExhibitItem();
                    item.name = sQLiteDataReader["ItemName"].ToString();
                    item.description = sQLiteDataReader["ItemDescription"].ToString();
                    item.picturelocation[0] = sQLiteDataReader["Pic1"].ToString();
                    item.picturelocation[1] = sQLiteDataReader["Pic2"].ToString();
                    item.picturelocation[2] = sQLiteDataReader["Pic3"].ToString();
                    item.picturelocation[3] = sQLiteDataReader["Pic4"].ToString();
                    item.category_level1_id = int.Parse(sQLiteDataReader["CategoryLevel1"].ToString());
                    item.category_level2_id = int.Parse(sQLiteDataReader["CategoryLevel2"].ToString());
                    item.category_level3_id = int.Parse(sQLiteDataReader["CategoryLevel3"].ToString());
                    item.category_level4_id = int.Parse(sQLiteDataReader["CategoryLevel4"].ToString());
                    item.size_id = int.Parse(sQLiteDataReader["Size"].ToString());
                    item.brand_id = int.Parse(sQLiteDataReader["Brand"].ToString());
                    item.item_condition_id = int.Parse(sQLiteDataReader["ItemCondition"].ToString());
                    item.shipping_payer_id = int.Parse(sQLiteDataReader["ShippingPayer"].ToString());
                    item.shipping_duration_id = int.Parse(sQLiteDataReader["ShippingDuration"].ToString());
                    item.shipping_method_id = int.Parse(sQLiteDataReader["ShippingMethod"].ToString());
                    item.shipping_area_id = int.Parse(sQLiteDataReader["ShippingArea"].ToString());
                    item.price = int.Parse(sQLiteDataReader["Price"].ToString());
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

        public bool updateItem(int DBId, ItemRegisterForm.FrilExhibitItem item) {
            try {
                conn.Open();
                string commandText = "UPDATE items SET "
                                    + "ItemName = '" + item.name.Replace("'", "''") + "',"
                                    + "ItemDescription = '" + item.description.Replace("'", "''") + "',"
                                    + "Pic1 = '" + item.picturelocation[0] + "',"
                                    + "Pic2 = '" + item.picturelocation[1] + "',"
                                    + "Pic3 = '" + item.picturelocation[2] + "',"
                                    + "Pic4 = '" + item.picturelocation[3] + "',"
                                    + "CategoryLevel1 = " + item.category_level1_id.ToString() + ","
                                    + "CategoryLevel2 = " + item.category_level2_id.ToString() + ","
                                    + "CategoryLevel3 = " + item.category_level3_id.ToString() + ","
                                    + "CategoryLevel4 = " + item.category_level4_id.ToString() + ","
                                    + "Size = " + item.size_id.ToString() + ", Brand = " + item.brand_id.ToString() + ", ItemCondition =  " + item.item_condition_id.ToString() + ","
                                    + "ShippingPayer = " + item.shipping_payer_id.ToString() + ", ShippingDuration = " + item.shipping_duration_id.ToString() + ","
                                    + "ShippingMethod = " + item.shipping_method_id.ToString() + ", ShippingArea = " + item.shipping_area_id.ToString() + ","
                                    + "Price = " + item.price.ToString()
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

        //同一商品がDBに存在すればID返す なければ0
        public int getItemDBId(ItemRegisterForm.FrilExhibitItem item) {
            int rst = 0;
            this.conn.Open();
            try {
                SQLiteCommand sQLiteCommand = new SQLiteCommand("select i.Id from items i where"
                                    + " i.ItemName = '" + item.name.Replace("'", "''") + "'"
                                    + " AND i.ItemDescription = '" + item.description.Replace("'", "''") + "'"
                                    + " AND i.Pic1 = '" + item.picturelocation[0].Replace("'", "''") + "'"
                                    + " AND i.Pic2 = '" + item.picturelocation[1].Replace("'", "''") + "'"
                                    + " AND i.Pic3 = '" + item.picturelocation[2].Replace("'", "''") + "'"
                                    + " AND i.Pic4 = '" + item.picturelocation[3].Replace("'", "''") + "'"
                                    + " AND i.CategoryLevel1 = " + item.category_level1_id.ToString()
                                    + " AND i.CategoryLevel2 = " + item.category_level2_id.ToString()
                                    + " AND i.CategoryLevel3 = " + item.category_level3_id.ToString()
                                    + " AND i.CategoryLevel4 = " + item.category_level4_id.ToString()
                                    + " AND i.Size = " + item.size_id.ToString()
                                    + " AND i.Brand = " + item.brand_id.ToString()
                                    + " AND i.ItemCondition = " + item.item_condition_id.ToString()
                                    + " AND i.ShippingPayer = " + item.shipping_payer_id.ToString()
                                    + " AND i.ShippingDuration = " + item.shipping_duration_id.ToString()
                                    + " AND i.ShippingMethod = " + item.shipping_method_id.ToString()
                                    + " AND i.ShippingArea = " + item.shipping_area_id.ToString()
                                    + " AND i.ShippingPayer = " + item.shipping_payer_id.ToString()
                                    + " AND i.Price = " + item.price.ToString() + ";", this.conn);
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
    }
}





