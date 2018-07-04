﻿using RakuLand.DBHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RakuLand.FrilCommon;

namespace RakuLand {
    public partial class ItemRegisterForm : Form {
        public bool is_editmode = false; //編集から開いた場合はis_editmode = true. 商品登録（新規）から開いた場合はfalse;
        public bool setting_now = true;//GUIはめこんでますよフラグ
        public MainForm mainform;
        private int openItemDBId;
        public ItemRegisterForm() {
            InitializeComponent();
        }
        public List<FrilAPI> apilist;
        private bool category_not_exist = false;

        #region いらないとおもいました
        //private int openItemDBId;
        //public class FrilExhibitItem {
        //    public string itemid = "";//商品編集で使用する
        //    public bool can_exhibit = false; //出品可能かどうか
        //    public string name;
        //    public string description;
        //    public string[] picturelocation = new string[4];
        //    public int category_level1_id;
        //    public int category_level2_id;
        //    public int category_level3_id;
        //    public int category_level4_id;
        //    public int category_level1_selected_index;
        //    public int category_level2_selected_index;
        //    public int category_level3_selected_index;
        //    public int category_level4_selected_index;
        //    public int size_id;
        //    public int size_selected_index;
        //    public int brand_id;
        //    public int brand_selected_index;
        //    public int item_condition_id;
        //    public int item_condition_selected_index;
        //    public int shipping_payer_id;
        //    public int shipping_payer_selected_index;
        //    public int shipping_duration_id;
        //    public int shipping_duration_selected_index;
        //    public int shipping_method_id;
        //    public int shipping_method_selected_index;
        //    public int shipping_area_id;
        //    public int shipping_area_selected_index;
        //    public int price;
        //}
        #endregion
        #region いらないとおもいました
        //FrilExhibitItemからFrilItemを作成 画像の情報以外を渡す
        //static public FrilItem getFrilItemFromExhibitItem(FrilExhibitItem exhibititem) {
        //    if (exhibititem == null) return null;
        //    FrilItem res = new FrilItem();
        //    res.item_name = exhibititem.name;
        //    res.detail = exhibititem.description;
        //    res.category_root_category_id = exhibititem.category_level1_id;
        //    res.category_parent_category_id = exhibititem.category_level2_id;
        //    res.category_id = exhibititem.category_level3_id;
        //    res.item_condition = exhibititem.item_condition_id;
        //    res.size = exhibititem.size_id;
        //    res.brand_name = exhibititem.brand_id;
        //    res.price = exhibititem.price;
        //    res.shipping_duration = exhibititem.shipping_duration_id;
        //    res.shipping_from_area = exhibititem.shipping_area_id;
        //    res.shipping_method = exhibititem.shipping_method_id;
        //    res.shipping_payer = exhibititem.shipping_payer_id;
        //    res.imagepaths = exhibititem.picturelocation;
        //    //res.status_message = exhibititem.can_exhibit ? "出品可" : "出品不可";
        //    return res;
        //}
        #endregion

        private void ItemRegisterForm_Load(object sender, EventArgs e) {
            //ブランド
            foreach (FrilCommon.FrilBrand p in FrilCommon.fril_brands) {
                this.Fril_BrandComboBox.Items.Add(p);
            }
            //カテゴリレベル1
            foreach (var c in FrilCommon.fril_categoryDictionary[0]) {
                this.Fril_CategoryComboBoxLevel1.Items.Add(c);
            }
            //商品の状態
            foreach (KeyValuePair<string, string> p in FrilCommon.conditionTypeFril) {
                this.Fril_ItemConditionComboBox.Items.Add(p);
            }
            //配送料の負担
            foreach (KeyValuePair<string, string> p in FrilCommon.shippingPayersFril) {
                this.Fril_ShippingPayerComboBox.Items.Add(p);
            }

            //共通
            //発送までの日数
            foreach (KeyValuePair<string, string> p in FrilCommon.shippingDurations) {
                this.ShippingDurationComboBox.Items.Add(p);
            }
            //都道府県
            foreach (KeyValuePair<string, string> p in FrilCommon.shippingFromAreas) {
                this.ShippingAreaComboBox.Items.Add(p);
            }
            //即時出品用アカウント選択ComboBox
            foreach (var api in apilist) {
                ApiListComboBox.Items.Add(api);
            }
        }
        private void ItemRegisterForm_Shown(object sender, EventArgs e) {
            if (is_editmode) {
                SetGUIFromItem(this.openItem);
            }
        }



        FrilCommon.FrilCategory nowfril_selectedCategory; //フリルの最下層選択中カテゴリ
        #region GUIFormat
        private void Fril_CategoryComboBoxLevel1_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FrilCommon.FrilCategory)e.ListItem).name;
        }

        private void Fril_CategoryComboBoxLevel2_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FrilCommon.FrilCategory)e.ListItem).name;
        }

        private void Fril_CategoryComboBoxLevel3_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FrilCommon.FrilCategory)e.ListItem).name;
        }

        private void Fril_CategoryComboBoxLevel4_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FrilCommon.FrilCategory)e.ListItem).name;
        }

        private void Fril_SizeComboBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FrilCommon.FrilSizeInfo)e.ListItem).name;
        }

        private void Fril_BrandComboBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FrilCommon.FrilBrand)e.ListItem).name;
        }

        private void Fril_ItemConditionComboBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((KeyValuePair<string, string>)e.ListItem).Key;
        }

        private void Fril_ShippingPayerComboBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((KeyValuePair<string, string>)e.ListItem).Key;
        }

        private void Fril_ShippingMethodComboBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((KeyValuePair<string, string>)e.ListItem).Key;
        }
        //private void Rakuma_CategoryComboBoxLevel1_Format(object sender, ListControlConvertEventArgs e) {
        //    e.Value = ((FriRaCommon.RakumaCategory)e.ListItem).name;
        //}

        //private void Rakuma_CategoryComboBoxLevel2_Format(object sender, ListControlConvertEventArgs e) {
        //    e.Value = ((FriRaCommon.RakumaCategory)e.ListItem).name;
        //}

        //private void Rakuma_CategoryComboBoxLevel3_Format(object sender, ListControlConvertEventArgs e) {
        //    e.Value = ((FriRaCommon.RakumaCategory)e.ListItem).name;
        //}

        //private void Rakuma_CategoryComboBoxLevel4_Format(object sender, ListControlConvertEventArgs e) {
        //    e.Value = ((FriRaCommon.RakumaCategory)e.ListItem).name;
        //}

        //private void Rakuma_SizeComboBox_Format(object sender, ListControlConvertEventArgs e) {
        //    e.Value = ((FriRaCommon.RakumaSize)e.ListItem).title;
        //}

        //private void Rakuma_BrandComboBox_Format(object sender, ListControlConvertEventArgs e) {

        //}

        //private void Rakuma_ItemConditionComboBox_Format(object sender, ListControlConvertEventArgs e) {
        //    e.Value = ((KeyValuePair<string, string>)e.ListItem).Key;
        //}

        //private void Rakuma_ShippingPayerComboBox_Format(object sender, ListControlConvertEventArgs e) {
        //    e.Value = ((KeyValuePair<string, string>)e.ListItem).Key;
        //}

        //private void Rakuma_ShippingMethodComboBox_Format(object sender, ListControlConvertEventArgs e) {
        //    e.Value = ((KeyValuePair<string, string>)e.ListItem).Key;
        //}

        private void ShippingAreaComboBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((KeyValuePair<string, string>)e.ListItem).Key;
        }

        private void ShippingDurationComboBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((KeyValuePair<string, string>)e.ListItem).Key;
        }

        #endregion
        #region ComboBoxSelectedIndexChanged
        private void Fril_CategoryComboBoxLevel1_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.Fril_CategoryComboBoxLevel1.SelectedIndex < 0) return;
            nowfril_selectedCategory = (FrilCommon.FrilCategory)Fril_CategoryComboBoxLevel1.SelectedItem;
            this.Fril_CategoryComboBoxLevel2.Items.Clear();
            this.Fril_CategoryComboBoxLevel3.Items.Clear();
            this.Fril_CategoryComboBoxLevel4.Items.Clear();
            if (FrilCommon.fril_categoryDictionary.ContainsKey(nowfril_selectedCategory.id)) {
                this.Fril_CategoryComboBoxLevel2.Visible = true;
                this.Fril_CategoryComboBoxLevel3.Visible = false;
                this.Fril_CategoryComboBoxLevel4.Visible = false;
                this.Fril_CategoryComboBoxLevel2.SelectedIndex = -1;
                this.Fril_CategoryComboBoxLevel3.SelectedIndex = -1;
                this.Fril_CategoryComboBoxLevel4.SelectedIndex = -1;
                this.Fril_CategoryComboBoxLevel2.Text = "選択してください";
                foreach (var c in FrilCommon.fril_categoryDictionary[nowfril_selectedCategory.id]) {
                    //Console.WriteLine(c.id.ToString() + ":" + c.name.ToString());
                    this.Fril_CategoryComboBoxLevel2.Items.Add(c);
                }
            }

            if (is_editmode && setting_now) {
                if (this.openItem.category_level2_id >= 0) {
                    this.Fril_CategoryComboBoxLevel2.SelectedIndex = TabIndexFromList(FrilCommon.fril_categoryDictionary[openItem.category_level1_id] , openItem.category_level2_id);
                }
            }
        }
  


        private void Fril_CategoryComboBoxLevel2_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.Fril_CategoryComboBoxLevel2.SelectedIndex < 0) return;
            nowfril_selectedCategory = (FrilCommon.FrilCategory)Fril_CategoryComboBoxLevel2.SelectedItem;
            this.Fril_CategoryComboBoxLevel3.Items.Clear();
            this.Fril_CategoryComboBoxLevel4.Items.Clear();
            if (FrilCommon.fril_categoryDictionary.ContainsKey(nowfril_selectedCategory.id)) {
                foreach (var c in FrilCommon.fril_categoryDictionary[nowfril_selectedCategory.id]) {
                    this.Fril_CategoryComboBoxLevel3.Visible = true;
                    this.Fril_CategoryComboBoxLevel4.Visible = false;
                    this.Fril_CategoryComboBoxLevel3.SelectedIndex = -1;
                    this.Fril_CategoryComboBoxLevel4.SelectedIndex = -1;
                    this.Fril_CategoryComboBoxLevel3.Items.Add(c);
                    this.Fril_CategoryComboBoxLevel3.Text = "選択してください";
                }
            }
            SetFrilSizeComboBox();

            if (is_editmode && setting_now) {
                if (this.openItem.category_level3_id >= 0) {
                    this.Fril_CategoryComboBoxLevel3.SelectedIndex = TabIndexFromList(FrilCommon.fril_categoryDictionary[openItem.category_level2_id], openItem.category_level3_id);
                }
            }
        }

        private void Fril_CategoryComboBoxLevel3_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.Fril_CategoryComboBoxLevel3.SelectedIndex < 0) return;
            nowfril_selectedCategory = (FrilCommon.FrilCategory)Fril_CategoryComboBoxLevel3.SelectedItem;
            this.Fril_CategoryComboBoxLevel4.Items.Clear();
            if (FrilCommon.fril_categoryDictionary.ContainsKey(nowfril_selectedCategory.id)) {
                foreach (var c in FrilCommon.fril_categoryDictionary[nowfril_selectedCategory.id]) {
                    this.Fril_CategoryComboBoxLevel4.Items.Add(c);
                    this.Fril_CategoryComboBoxLevel4.Visible = true;
                    this.Fril_CategoryComboBoxLevel4.SelectedIndex = -1;
                    this.Fril_CategoryComboBoxLevel4.Text = "選択してください";
                }
            }
            SetFrilSizeComboBox();
            if (is_editmode && setting_now) {
                try {
                    if (this.openItem.category_level4_id >= 0) {
                        if (FrilCommon.fril_categoryDictionary.ContainsKey(openItem.category_level3_id)) {
                            this.Fril_CategoryComboBoxLevel4.SelectedIndex = TabIndexFromList(FrilCommon.fril_categoryDictionary[openItem.category_level3_id], openItem.category_level4_id);
                        }
                    }
                } catch (Exception ex) {
                    Log.Logger.Error(ex);
                    Console.WriteLine(ex);
                    category_not_exist = true;
                }
            }
        }

        private void Fril_CategoryComboBoxLevel4_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.Fril_CategoryComboBoxLevel4.SelectedIndex < 0) return;
            nowfril_selectedCategory = (FrilCommon.FrilCategory)Fril_CategoryComboBoxLevel4.SelectedItem;
            if (FrilCommon.fril_categoryDictionary.ContainsKey(nowfril_selectedCategory.id)) {
                //Level5がないのでエラー　ないと思う
                MessageBox.Show("このカテゴリは指定できません");
                Log.Logger.Error("フリルカテゴリがレベル4以上");
            }
            SetFrilSizeComboBox();
        }


        private void SetFrilSizeComboBox() {
            //現在選択されたカテゴリに応じてサイズのComboBoxを修正する
            if (FrilCommon.fril_default_sizeInfoDictionary.ContainsKey(nowfril_selectedCategory.size_group_id)) {
                this.Fril_SizeComboBox.Items.Clear();
                foreach (var sizeinfo in FrilCommon.fril_default_sizeInfoDictionary[nowfril_selectedCategory.size_group_id]) {
                    this.Fril_SizeComboBox.Items.Add(sizeinfo);
                }
                this.Fril_SizeComboBox.Enabled = true;


                if (is_editmode && setting_now) {
                    if (this.openItem.size_id==19999) {
                        Fril_SizeComboBox.Text = "なし";
                         
                        return;
                    }
                    //カテゴリがきまればサイズとブランドの候補はきまるので候補からIDが一致するSelectedIndexを見つければいい
                    if (openItem.size_id >= 0 && Fril_SizeComboBox.Enabled) {
                        Console.WriteLine(openItem.size_id);
                        Fril_SizeComboBox.SelectedIndex = TabIndexFromDictionary(FrilCommon.fril_default_sizeInfoDictionary, 10004);
                        this.setting_now = false;
                    }
                }



            } else {
                this.Fril_SizeComboBox.Items.Clear();
                this.Fril_SizeComboBox.Enabled = false;
            }
        }


        private void Fril_ShippingPayerComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            this.Fril_ShippingMethodComboBox.Items.Clear();
            if (this.Fril_ShippingPayerComboBox.SelectedItem == null) return;
            if (this.Fril_ShippingPayerComboBox.SelectedIndex == 0) {
                //送料込み
                foreach (KeyValuePair<string, string> p in FrilCommon.shippingMethodsSellerFril) {
                    this.Fril_ShippingMethodComboBox.Items.Add(p);
                }
            } else if (this.Fril_ShippingPayerComboBox.SelectedIndex == 1) {
                //着払い
                foreach (KeyValuePair<string, string> p in FrilCommon.shippingMethodsBuyerFril) {
                    this.Fril_ShippingMethodComboBox.Items.Add(p);
                }
            }
        }


        #endregion
        #region pictureBox
        private void pictureBox1_Click(object sender, EventArgs e) {
            //画像選択画面を表示する
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JPGファイル(*.jpg;*.jpeg)|*.jpg;*.jpeg|すべてのファイル(*.*)|*.*";
            ofd.Title = "開くファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK) {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する
                string filename = ofd.FileName;
                this.pictureBox1.ImageLocation = Common.getExhibitionImageFromPath(filename);
            }
        }
        private void pictureBox2_Click(object sender, EventArgs e) {
            //画像選択画面を表示する
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JPGファイル(*.jpg;*.jpeg)|*.jpg;*.jpeg|すべてのファイル(*.*)|*.*";
            ofd.Title = "開くファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK) {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する
                string filename = ofd.FileName;
                this.pictureBox2.ImageLocation = Common.getExhibitionImageFromPath(filename);
            }
        }
        private void pictureBox3_Click(object sender, EventArgs e) {
            //画像選択画面を表示する
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JPGファイル(*.jpg;*.jpeg)|*.jpg;*.jpeg|すべてのファイル(*.*)|*.*";
            ofd.Title = "開くファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK) {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する
                string filename = ofd.FileName;
                this.pictureBox3.ImageLocation = Common.getExhibitionImageFromPath(filename);
            }
        }
        private void pictureBox4_Click(object sender, EventArgs e) {
            //画像選択画面を表示する
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JPGファイル(*.jpg;*.jpeg)|*.jpg;*.jpeg|すべてのファイル(*.*)|*.*";
            ofd.Title = "開くファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK) {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する
                string filename = ofd.FileName;
                this.pictureBox4.ImageLocation = Common.getExhibitionImageFromPath(filename);
            }
        }
        private void pic1Reset_Click(object sender, EventArgs e) {
            this.pictureBox1.ImageLocation = "";
        }
        private void pic2Reset_Click(object sender, EventArgs e) {
            this.pictureBox2.ImageLocation = "";
        }
        private void pic3Reset_Click(object sender, EventArgs e) {
            this.pictureBox3.ImageLocation = "";
        }
        private void pic4Reset_Click(object sender, EventArgs e) {
            this.pictureBox4.ImageLocation = "";
        }
        #endregion

        //出品ボタンTODO:まだです
        private void ExhibitNowButton_Click(object sender, EventArgs e) {
            //今すぐ出品
            if (ApiListComboBox.Items.Count <= 0 || ApiListComboBox.SelectedIndex < 0) {

                return;
            }
            //FrilAPI api = new FrilAPI(TestForm.mail, TestForm.pass);
            //try {
            //    if (!api.tryFrilLogin(api.account.cc)) throw new Exception("ログイン失敗(mailかpassが間違っています)");
            //    FrilItem item = CollectSellSettingsFromGUI();
            //    api.Sell(item, api.account.cc);
            //} catch (Exception ex) {
            //    Console.WriteLine(ex);
            //}
        }

        //保存ボタン
        private void SaveExhibitItemButton_Click(object sender, EventArgs e) {

            //画像制限
            if (string.IsNullOrEmpty(pictureBox1.ImageLocation)){
                MessageBox.Show("Main写真は必須です。\n写真を設定してください。", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                pictureBox1.Focus();
                return;
            }
            
            //商品タイトル制限
            if (string.IsNullOrEmpty(ItemNameTextBox.Text)) {
                MessageBox.Show("タイトルは必須です。\n１～４０文字で商品名を設定してください。", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ItemNameTextBox.Focus();
                return;
            }
            //商品説明制限
            if (string.IsNullOrEmpty(DescriptionTextBox.Text)) {
                MessageBox.Show("商品説明は必須です。\n１～１０００文字で商品名を設定してください。", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                DescriptionTextBox.Focus();
                return;
            }
            //価格制限
            if (String.IsNullOrEmpty(PriceTextBox.Text)) {
                MessageBox.Show("価格は必須です。\n価格は\\300～\\500,000円にしてください。", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                PriceTextBox.Focus();
            }
            if (int.Parse(PriceTextBox.Text) < 300 || 500000 < int.Parse(PriceTextBox.Text)) {
                MessageBox.Show("ラクマの設定可能価格を超えています。\n価格は\\300～\\500,000円にしてください。", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                PriceTextBox.Focus();
                return;
            }


            //GUIからItem情報を保存する
            FrilItem item = CollectSellSettingsFromGUI();
            //商品名が設定で制限した長さ以内か調べる
            //if (this.ItemNameTextBox.Text.Length > Settings.getItemNameMaxLength()) {
            //    MessageBox.Show("商品名の長さがオプションで設定した長さより長いです", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}



            //string new_parent_id = this.parentIDTextBox.Text.Trim();
            //string new_child_id = this.childIDTextBox.Text.Trim();
            #region ItemFamilyValidate
            //parent_idとchild_idを保存できるかをしらべる
            //if (new_parent_id != "" && new_child_id == "") {
            //    MessageBox.Show("親IDと子IDの両方を入力するか、両方を空にしてください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //} else if (new_parent_id == "" && new_child_id != "") {
            //    MessageBox.Show("親IDと子IDの両方を入力するか、両方を空にしてください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //} else if (new_parent_id != "" && new_child_id != "") {
            //    //既に親IDと子IDが存在するか調べる
            //    int existItemDBID = new ItemFamilyDBHelper().getItemDBIdFromParentChild(new_parent_id, new_child_id);
            //    if (existItemDBID >= 0) {
            //        //既に同じ親IDと子IDの商品がItemFamilyDBに存在する
            //        //編集モードでない場合はNG
            //        //編集モードでかつ、既にItemFamilyDBに存在するレコードの商品IDと同じでない場合はNG
            //        if ((this.is_editmode == false) || (this.is_editmode && existItemDBID != this.openItemDBId)) {
            //            MessageBox.Show("既に同じ親IDと子IDの商品が存在します", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            return;
            //        }
            //    }
            //}
            #endregion
            //ItemFamilyDBHelper itemFamilyDBHelper = new ItemFamilyDBHelper();
            //ZaikoDBHelper zaikoDBHelper = new ZaikoDBHelper();
            FrilItemDBHelper Dbhelper = new FrilItemDBHelper();
            //商品情報の更新,新規追加をまず行う
            if (this.is_editmode) {
                Dbhelper.updateItem(this.openItemDBId, item);
            } else {
                Dbhelper.addItem(item);
                this.openItemDBId = Dbhelper.getItemDBId(item);
            }
            //在庫情報および親子情報の更新,追加を行う
            //if (new_parent_id != "" && new_child_id != "") {
            //    if (this.parent_id == "" && this.child_id == "") {
            //        //親IDと子IDを新規追加
            //        if (zaikoDBHelper.getZaikoNum(new_parent_id) < 0) zaikoDBHelper.updateZaikoInfo(new_parent_id, 0);
            //        itemFamilyDBHelper.addItemFamily(new_parent_id, new_child_id, this.openItemDBId);
            //    } else {
            //        //親IDと子IDを更新
            //        itemFamilyDBHelper.updateItemFamily(this.parent_id, this.child_id, new_parent_id, new_child_id);
            //    }
            //}
            ////編集前は親IDと子ID存在したが今回は空になっている場合、ItemFamilyから該当データを消去
            //if (this.is_editmode && this.parent_id != "" && this.child_id != "" && new_parent_id == "" && new_child_id == "") {
            //    itemFamilyDBHelper.deleteItemFamily(this.parent_id, this.child_id);
            //}
            this.Close();

        }
        private FrilItem openItem;
        //編集モード
        public ItemRegisterForm(FrilItem loaditem, int DBId) {
            InitializeComponent();
            this.openItem = loaditem;
            this.is_editmode = true;
            this.openItemDBId = DBId;
        }

        private int TabIndexFromDictionary(Dictionary<string,string> dic,int target) {
            int index = 0;
            foreach (var val in dic) {
                if (int.Parse(val.Value) == target) {
                    break;
                }
                index = index + 1;
            }
            return index;
        }
        private int TabIndexFromList(List<FrilCommon.FrilBrand> brand_list, int target) {//ブランドリスト
            int index = 0;
            foreach (var val in brand_list) {
                if (val.id == target) {
                    break;
                }
                index = index + 1;
            }
            return index;
        }
        private int TabIndexFromList(List<FrilCommon.FrilCategory> category_list, int target) {//カテゴリリスト
            int index = 0;
            foreach (var val in category_list) {
                if (val.id == target) {
                    break;
                }
                index = index + 1;
            }
            return index;
        }


        //編集モードのときにGUIへ情報をセットする
        private void SetGUIFromItem(FrilItem loaditem) {
            try { 
                //GUIにセット（ComboBoxのSelectedIndexをプログラムから書き換えた場合もイベントは呼ばれるのでindexを変えるだけでいい）
                this.ItemNameTextBox.Text = loaditem.item_name;
                this.DescriptionTextBox.Text = loaditem.detail;
                this.pictureBox1.ImageLocation = loaditem.imagepaths[0];
                this.pictureBox2.ImageLocation = loaditem.imagepaths[1];
                this.pictureBox3.ImageLocation = loaditem.imagepaths[2];
                this.pictureBox4.ImageLocation = loaditem.imagepaths[3];

                if (loaditem.status >= 0) this.Fril_ItemConditionComboBox.SelectedIndex = TabIndexFromDictionary(FrilCommon.conditionTypeFril, loaditem.status);
                if (loaditem.carriage >= 0) this.Fril_ShippingPayerComboBox.SelectedIndex = TabIndexFromDictionary(FrilCommon.shippingPayersFril, loaditem.carriage);
                if (loaditem.d_method >= 0) this.Fril_ShippingMethodComboBox.SelectedIndex = TabIndexFromDictionary(FrilCommon.shippingMethodsBuyerFril,loaditem.d_method);
                if (loaditem.d_area >= 0) this.ShippingAreaComboBox.SelectedIndex = TabIndexFromDictionary(FrilCommon.shippingFromAreas,loaditem.d_area);
                if (loaditem.d_date >= 0) this.ShippingDurationComboBox.SelectedIndex = TabIndexFromDictionary(FrilCommon.shippingFromAreas, loaditem.d_date);
                if (loaditem.s_price <= 0) this.PriceTextBox.Text = "";
                else this.PriceTextBox.Text = loaditem.s_price.ToString();
                if (loaditem.brand_id > 0) this.Fril_BrandComboBox.SelectedIndex = TabIndexFromList(FrilCommon.fril_brands, loaditem.brand_id);//TODO:ブランドが選択されてないときは0なので>0にした
                if (loaditem.category_level1_id >= 0) {
                    int index = TabIndexFromList(fril_categoryDictionary[0], loaditem.category_level1_id);
                    this.Fril_CategoryComboBoxLevel1.SelectedIndex = index;
                }
                //TODO:販売アカウントをセットしてない？


            } catch (Exception ex) {
                Log.Logger.Error(ex.Message);
                Console.WriteLine(ex);

                if (!category_not_exist) {
                    Log.Logger.Error("フリル商品からGUIセットに失敗");
                    MessageBox.Show("商品の読み込みに失敗しました.", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }
        private int TabIndexFromDictionary(Dictionary<int, List<FrilCategory>> dic, int target) {
            int index = 0;
            foreach (var val in dic) {
                if (val.Key == target) {
                    break;
                }
                index = index + 1;
            }
            return index;
        }


        private int TabIndexFromDictionary(Dictionary<int, List<FrilSizeInfo>> dic, int target) {
            int index = 0;
            foreach (var val in dic) {
                index = 0;
                foreach (var size in val.Value) {
                    if (size.id == target) {
                        return index;
                    }
                    index = index + 1;
                }
            }
            return index;
        }




        //GUIからitem情報を得る
        private FrilItem CollectSellSettingsFromGUI() {

            FrilItem item_data = new FrilItem();
            item_data.item_id = "0";
            //商品名前
            if (!string.IsNullOrEmpty(this.ItemNameTextBox.Text)) {
                string name = this.ItemNameTextBox.Text;
                item_data.item_name = name;
            } else {
                item_data.item_name = "title";
            }
            //商品詳細
            if (!string.IsNullOrEmpty(this.DescriptionTextBox.Text)) {
                string detail = this.DescriptionTextBox.Text;
                item_data.detail = detail;
            } else {
                item_data.detail = "detail";
            }
            //商品料金
            if (!string.IsNullOrEmpty(this.PriceTextBox.Text)) {
                int s_price = int.Parse(this.PriceTextBox.Text);
                item_data.s_price = s_price;
            }
            //商品状態
            if (this.Fril_ItemConditionComboBox.SelectedItem != null) {
                KeyValuePair<string, string> status = (KeyValuePair<string, string>)this.Fril_ItemConditionComboBox.SelectedItem;
                //Console.WriteLine("status:" + status.Value);
                item_data.status = int.Parse(status.Value);
            }
            //FIXIT:t_status
            //送料負担者
            if (this.Fril_ShippingPayerComboBox.SelectedItem != null) {
                KeyValuePair<string, string> carriage = (KeyValuePair<string, string>)this.Fril_ShippingPayerComboBox.SelectedItem;
                item_data.carriage = int.Parse(carriage.Value);
            }
            //配送方法
            if (this.Fril_ShippingMethodComboBox.SelectedItem != null) {
                KeyValuePair<string, string> method = (KeyValuePair<string, string>)this.Fril_ShippingMethodComboBox.SelectedItem;
                item_data.d_method = int.Parse(method.Value);
            }
            //配送日数
            if (this.ShippingDurationComboBox.SelectedItem != null) {
                KeyValuePair<string, string> duration = (KeyValuePair<string, string>)this.ShippingDurationComboBox.SelectedItem;
                item_data.d_date = int.Parse(duration.Value);
            }
            //配送元地域
            if (this.ShippingAreaComboBox.SelectedItem != null) {
                KeyValuePair<string, string> area = (KeyValuePair<string, string>)this.ShippingAreaComboBox.SelectedItem;
                item_data.d_area = int.Parse(area.Value);
            }
            //FIXIT:user_id;//出品者ID
            //FIXIT:created_at;//ex)2017-09-27T09:12:57+09:00
            //FIXIT:screen_name; //出品者アカウント名
            //カテゴリレベル１
            if (this.Fril_CategoryComboBoxLevel1.SelectedItem != null) {
                FrilCommon.FrilCategory category1 = (FrilCommon.FrilCategory)this.Fril_CategoryComboBoxLevel1.SelectedItem;
                item_data.category_level1_id = category1.id;
            }
            //カテゴリレベル２ category_p_idになる
            if (this.Fril_CategoryComboBoxLevel2.SelectedItem != null) {
                FrilCommon.FrilCategory category2 = (FrilCommon.FrilCategory)this.Fril_CategoryComboBoxLevel2.SelectedItem;
                item_data.category_p_id = category2.id;//p_categoryはおそらくこれ
                item_data.category_level2_id = category2.id;
            }
            //カテゴリレベル３
            if (this.Fril_CategoryComboBoxLevel3.SelectedItem != null) {
                FrilCommon.FrilCategory category3 = (FrilCommon.FrilCategory)this.Fril_CategoryComboBoxLevel3.SelectedItem;
                item_data.category_id = category3.id;
                item_data.category_level3_id = category3.id;
            }
            //カテゴリレベル４
            if (this.Fril_CategoryComboBoxLevel4.SelectedItem != null) {
                FrilCommon.FrilCategory category4 = (FrilCommon.FrilCategory)this.Fril_CategoryComboBoxLevel4.SelectedItem;
                item_data.category_id = category4.id;
                item_data.category_level4_id = category4.id;
            }
            //サイズID,サイズ名
            if (this.Fril_SizeComboBox.SelectedItem != null) {
                FrilCommon.FrilSizeInfo size = (FrilCommon.FrilSizeInfo)this.Fril_SizeComboBox.SelectedItem;
                item_data.size_id = size.id;
                item_data.size_name = size.name;
            } else {
                item_data.size_id = 19999;
                item_data.size_name = "なし";
            }
            //任意部分
            //ブランドid
            if (this.Fril_BrandComboBox.SelectedItem != null) {
                FrilCommon.FrilBrand brand = (FrilCommon.FrilBrand)this.Fril_BrandComboBox.SelectedItem;
                item_data.brand_id = brand.id;
            }
            //FIXIT:i_brand_id
            //FIXIT:commens_count;
            //FIXIT:likes_count;
            //FIXIT:imageurls[4]//画像URL
            //ローカル画像パス
            if (!string.IsNullOrEmpty(pictureBox1.ImageLocation)){
                item_data.imagepaths[0] = pictureBox1.ImageLocation;
            }
            if (!string.IsNullOrEmpty(pictureBox2.ImageLocation)) {
                item_data.imagepaths[1] = pictureBox2.ImageLocation;
            }
            if (!string.IsNullOrEmpty(pictureBox3.ImageLocation)) {
                item_data.imagepaths[2] = pictureBox3.ImageLocation;
            }
            if (!string.IsNullOrEmpty(pictureBox4.ImageLocation)) {
                item_data.imagepaths[3] = pictureBox4.ImageLocation;
            }
            if (ApiListComboBox.SelectedIndex != -1) {
                item_data.user_id = ((FrilAPI)ApiListComboBox.SelectedItem).account.userId;
            }
            return item_data;
        }

        private void PriceTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            // 制御文字は入力可
            if (char.IsControl(e.KeyChar)) {
                e.Handled = false;
                return;
            }

            // 数字(0-9)は入力可
            if (char.IsDigit(e.KeyChar)) {
                e.Handled = false;
                return;
            }
            // 上記以外は入力不可
            e.Handled = true;
        }

        private void ItemRegisterForm_FormClosing(object sender, FormClosingEventArgs e) {
            this.mainform.OnBackFromItemExhibitForm();
        }



        private void PriceTextBox_Leave(object sender, EventArgs e) {
            PriceTextBox.Text = Microsoft.VisualBasic.Strings.StrConv(PriceTextBox.Text, Microsoft.VisualBasic.VbStrConv.Narrow);
        }


        #region 細かいデザイン部分
        private void ItemNameTextBox_TextChanged(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(ItemNameTextBox.Text)){
                int text_num = ItemNameTextBox.Text.Length;
                ItemNameCountLabel.Text = string.Format("{0}/40", text_num);
            } else {
                ItemNameCountLabel.Text = "0/40";
            }
        }
        private void DescriptionTextBox_TextChanged(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(DescriptionTextBox.Text)) {
                int text_num = DescriptionTextBox.Text.Length;
                ItemDetailCountLabel.Text = string.Format("{0}/1000", text_num);
            } else {
                ItemDetailCountLabel.Text = "0/1000";
            }
        }




        #endregion

        private void ApiListComboBox_Format(object sender, ListControlConvertEventArgs e) {
            FrilAPI api = (FrilAPI)e.ListItem;
            e.Value = api.account.nickname;
        }
    }

}
