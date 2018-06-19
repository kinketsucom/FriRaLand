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

namespace FriRaLand {
    public partial class ItemRegisterForm : Form {
        public ItemRegisterForm() {
            InitializeComponent();
        }

        private void ItemRegisterForm_Load(object sender, EventArgs e) {
            //フリル側

            //ブランド
            foreach (FriRaCommon.FrilBrand p in FriRaCommon.fril_brands){
                this.Fril_BrandComboBox.Items.Add(p);
            }
            //カテゴリレベル1
            foreach (var c in FriRaCommon.fril_categoryDictionary[0]) {
                this.Fril_CategoryComboBoxLevel1.Items.Add(c);
            }
            //商品の状態
            foreach (KeyValuePair<string, string> p in FriRaCommon.conditionTypeFril) {
                this.Fril_ItemConditionComboBox.Items.Add(p);
            }
            //配送料の負担
            foreach (KeyValuePair<string, string> p in FriRaCommon.shippingPayersFril) {
                this.Fril_ShippingPayerComboBox.Items.Add(p);
            }
           
            //共通
            //発送までの日数
            foreach (KeyValuePair<string, string> p in FriRaCommon.shippingDurations) {
                this.ShippingDurationComboBox.Items.Add(p);
            }
            //都道府県
            foreach (KeyValuePair<string, string> p in FriRaCommon.shippingFromAreas) {
                this.ShippingAreaComboBox.Items.Add(p);
            }
        }
        FriRaCommon.FrilCategory nowfril_selectedCategory; //フリルの最下層選択中カテゴリ
        FriRaCommon.RakumaCategory nowrakuma_selectedCategory; //ラクマの最下層選択中カテゴリ
        #region GUIFormat
        private void Fril_CategoryComboBoxLevel1_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FriRaCommon.FrilCategory)e.ListItem).name;
        }

        private void Fril_CategoryComboBoxLevel2_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FriRaCommon.FrilCategory)e.ListItem).name;
        }

        private void Fril_CategoryComboBoxLevel3_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FriRaCommon.FrilCategory)e.ListItem).name;
        }

        private void Fril_CategoryComboBoxLevel4_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FriRaCommon.FrilCategory)e.ListItem).name;
        }

        private void Fril_SizeComboBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FriRaCommon.FrilSizeInfo)e.ListItem).name;
        }

        private void Fril_BrandComboBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FriRaCommon.FrilBrand)e.ListItem).name;
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
            nowfril_selectedCategory = (FriRaCommon.FrilCategory)Fril_CategoryComboBoxLevel1.SelectedItem;
            this.Fril_CategoryComboBoxLevel2.Items.Clear();
            this.Fril_CategoryComboBoxLevel3.Items.Clear();
            this.Fril_CategoryComboBoxLevel4.Items.Clear();
            if (FriRaCommon.fril_categoryDictionary.ContainsKey(nowfril_selectedCategory.id)) {
                this.Fril_CategoryComboBoxLevel2.Visible = true;
                this.Fril_CategoryComboBoxLevel3.Visible = false;
                this.Fril_CategoryComboBoxLevel4.Visible = false;
                this.Fril_CategoryComboBoxLevel2.SelectedIndex = -1;
                this.Fril_CategoryComboBoxLevel3.SelectedIndex = -1;
                this.Fril_CategoryComboBoxLevel4.SelectedIndex = -1;
                this.Fril_CategoryComboBoxLevel2.Text = "選択してください";
                foreach (var c in FriRaCommon.fril_categoryDictionary[nowfril_selectedCategory.id]) {
                    //Console.WriteLine(c.id.ToString() + ":" + c.name.ToString());
                    this.Fril_CategoryComboBoxLevel2.Items.Add(c);
                }
            }
            SetFrilSizeComboBox();
        }

        private void Fril_CategoryComboBoxLevel2_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.Fril_CategoryComboBoxLevel2.SelectedIndex < 0) return;
            nowfril_selectedCategory = (FriRaCommon.FrilCategory)Fril_CategoryComboBoxLevel2.SelectedItem;
            this.Fril_CategoryComboBoxLevel3.Items.Clear();
            this.Fril_CategoryComboBoxLevel4.Items.Clear();
            if (FriRaCommon.fril_categoryDictionary.ContainsKey(nowfril_selectedCategory.id)) {
                foreach (var c in FriRaCommon.fril_categoryDictionary[nowfril_selectedCategory.id]) {
                    this.Fril_CategoryComboBoxLevel3.Visible = true;
                    this.Fril_CategoryComboBoxLevel4.Visible = false;
                    this.Fril_CategoryComboBoxLevel3.SelectedIndex = -1;
                    this.Fril_CategoryComboBoxLevel4.SelectedIndex = -1;
                    this.Fril_CategoryComboBoxLevel3.Items.Add(c);
                    this.Fril_CategoryComboBoxLevel3.Text = "選択してください";
                }
            }
            SetFrilSizeComboBox();
        }

        private void Fril_CategoryComboBoxLevel3_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.Fril_CategoryComboBoxLevel3.SelectedIndex < 0) return;
            nowfril_selectedCategory = (FriRaCommon.FrilCategory)Fril_CategoryComboBoxLevel3.SelectedItem;
            this.Fril_CategoryComboBoxLevel4.Items.Clear();
            if (FriRaCommon.fril_categoryDictionary.ContainsKey(nowfril_selectedCategory.id)) {
                foreach (var c in FriRaCommon.fril_categoryDictionary[nowfril_selectedCategory.id]) {
                    this.Fril_CategoryComboBoxLevel4.Items.Add(c);
                    this.Fril_CategoryComboBoxLevel4.Visible = true;
                    this.Fril_CategoryComboBoxLevel4.SelectedIndex = -1;
                    this.Fril_CategoryComboBoxLevel4.Text = "選択してください";
                }
            }
            SetFrilSizeComboBox();
        }

        private void Fril_CategoryComboBoxLevel4_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.Fril_CategoryComboBoxLevel4.SelectedIndex < 0) return;
            nowfril_selectedCategory = (FriRaCommon.FrilCategory)Fril_CategoryComboBoxLevel4.SelectedItem;
            if (FriRaCommon.fril_categoryDictionary.ContainsKey(nowfril_selectedCategory.id)) {
                //Level5がないのでエラー　ないと思う
                MessageBox.Show("このカテゴリは指定できません");
                Log.Logger.Error("フリルカテゴリがレベル4以上");
            }
            SetFrilSizeComboBox();
        }

        
        private void SetFrilSizeComboBox() {
            //現在選択されたカテゴリに応じてサイズのComboBoxを修正する
            if (FriRaCommon.fril_default_sizeInfoDictionary.ContainsKey(nowfril_selectedCategory.size_group_id)) {
                this.Fril_SizeComboBox.Items.Clear();
                foreach (var sizeinfo in FriRaCommon.fril_default_sizeInfoDictionary[nowfril_selectedCategory.size_group_id]) {
                    this.Fril_SizeComboBox.Items.Add(sizeinfo);
                }
                this.Fril_SizeComboBox.Enabled = true;
            }
            else {
                this.Fril_SizeComboBox.Items.Clear();
                this.Fril_SizeComboBox.Enabled = false;
            }
        }
        

        private void Fril_ShippingPayerComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            this.Fril_ShippingMethodComboBox.Items.Clear();
            if (this.Fril_ShippingPayerComboBox.SelectedItem == null) return;
            if (this.Fril_ShippingPayerComboBox.SelectedIndex == 0) {
                //送料込み
                foreach (KeyValuePair<string, string> p in FriRaCommon.shippingMethodsSellerFril) {
                    this.Fril_ShippingMethodComboBox.Items.Add(p);
                }
            }
            else if (this.Fril_ShippingPayerComboBox.SelectedIndex == 1) {
                //着払い
                foreach (KeyValuePair<string, string> p in FriRaCommon.shippingMethodsBuyerFril) {
                    this.Fril_ShippingMethodComboBox.Items.Add(p);
                }
            }
        }

     
        #endregion

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


        //出品ボタン
        private void ExhibitNowButton_Click(object sender, EventArgs e)
        {
            FrilAPI api = new FrilAPI(TestForm.mail, TestForm.pass);
            CollectSellSettingsFromGUI();
        }

        //保存ボタン
        private void SaveExhibitItemButton_Click(object sender, EventArgs e) {
            CookieContainer cc = new CookieContainer();
            FrilAPI api = new FrilAPI(TestForm.mail, TestForm.pass);
            try{
                if(!api.tryFrilLogin(cc)) throw new Exception("ログイン失敗(mailかpassが間違っています)");
                FrilItem item = CollectSellSettingsFromGUI();
                api.Sell(item, cc);
            } catch(Exception ex) {
                Console.WriteLine(ex);
            }
        }

        //
        private FrilItem CollectSellSettingsFromGUI() {
            FrilItem item_data = new FrilItem();
            //送料負担者
            if (this.Fril_ShippingPayerComboBox.SelectedItem != null) {
                KeyValuePair<string, string> carriage = (KeyValuePair<string, string>)this.Fril_ShippingPayerComboBox.SelectedItem;
                //Console.WriteLine("carriage:" + carriage.Value);
                item_data.carriage = int.Parse(carriage.Value);
            }

            //カテゴリレベル２
            if (this.Fril_CategoryComboBoxLevel2.SelectedItem != null) {
                FriRaCommon.FrilCategory category2 = (FriRaCommon.FrilCategory)this.Fril_CategoryComboBoxLevel2.SelectedItem;
                //Console.WriteLine("p_category：" + category2.id);
                item_data.category_p_id = category2.id;//p_categoryはおそらくこれ
            }
            //カテゴリレベル３
            if (this.Fril_CategoryComboBoxLevel3.SelectedItem != null) {
                FriRaCommon.FrilCategory category3 = (FriRaCommon.FrilCategory)this.Fril_CategoryComboBoxLevel3.SelectedItem;
                //Console.WriteLine("カテゴリレベル３：" + category3.id);
                item_data.category_id = category3.id;
            }
            //カテゴリレベル４
            if (this.Fril_CategoryComboBoxLevel4.SelectedItem != null) {
                FriRaCommon.FrilCategory category4 = (FriRaCommon.FrilCategory)this.Fril_CategoryComboBoxLevel4.SelectedItem;
                //Console.WriteLine("カテゴリレベル４：" + category4.id);
                item_data.category_id = category4.id;
            }
            //配送元地域
            if (this.ShippingAreaComboBox.SelectedItem != null) {
                KeyValuePair<string, string> area = (KeyValuePair<string, string>)this.ShippingAreaComboBox.SelectedItem;
                // Console.WriteLine("category_area:" + area.Value);
                item_data.d_area = int.Parse(area.Value);
            }
            //配送日数
            if (this.ShippingDurationComboBox.SelectedItem != null) {
                KeyValuePair<string, string> duration = (KeyValuePair<string, string>)this.ShippingDurationComboBox.SelectedItem;
                // Console.WriteLine("delivery_date:" + duration.Value);
                item_data.d_date = int.Parse(duration.Value);
            }
            //配送方法
            if (this.Fril_ShippingMethodComboBox.SelectedItem != null) {
                KeyValuePair<string, string> method = (KeyValuePair<string, string>)this.Fril_ShippingMethodComboBox.SelectedItem;
                //Console.WriteLine("delivery_method:" + method.Value);
                item_data.d_method = int.Parse(method.Value);
            }
            //商品詳細
            if (!string.IsNullOrEmpty(this.DescriptionTextBox.Text)) {
                string detail = this.DescriptionTextBox.Text;
                //Console.WriteLine("detail:"+detail);
                item_data.detail = detail;
            } else {
                item_data.detail = "detail";
            }
            //item_id
            //Console.WriteLine("item_id:0");
            //カテゴリレベル１いらんきがする
            //if (this.Fril_CategoryComboBoxLevel1.SelectedItem != null) {
            //    FriRaCommon.FrilCategory category1 = (FriRaCommon.FrilCategory)this.Fril_CategoryComboBoxLevel1.SelectedItem;
            //    Console.WriteLine("p_category：" + category1.id);
            //    //item_data.category_p_id = category1.id;
            //}
            //request_required
            //Console.WriteLine("request_required:0");
            //商品料金
            if (!string.IsNullOrEmpty(this.PriceTextBox.Text)) {
                int s_price = int.Parse(this.PriceTextBox.Text);
                //Console.WriteLine("sell_price:"+this.PriceTextBox.Text);
                item_data.s_price = s_price;
            }
            //サイズID,サイズ名
            if (this.Fril_SizeComboBox.SelectedItem != null) {
                FriRaCommon.FrilSizeInfo size = (FriRaCommon.FrilSizeInfo)this.Fril_SizeComboBox.SelectedItem;
                //Console.WriteLine("size:"+size.id+"\nsize_name:"+size.name);
                item_data.size_id = size.id;
                item_data.size_name = size.name;
            } else {
                item_data.size_id = 19999;
                item_data.size_name = "なし";
                //Console.WriteLine("size:" + item_data.size_id);
            }
            //商品状態
            if (this.Fril_ItemConditionComboBox.SelectedItem != null) {
                KeyValuePair<string, string> status = (KeyValuePair<string, string>)this.Fril_ItemConditionComboBox.SelectedItem;
                //Console.WriteLine("status:" + status.Value);
                item_data.status = int.Parse(status.Value);
            }
            //商品名前
            if (!string.IsNullOrEmpty(this.ItemNameTextBox.Text)) {
                string name = this.ItemNameTextBox.Text;
                //Console.WriteLine("title:"+name);
                item_data.item_name = name;
            } else {
                item_data.item_name = "title";
            }
            ;//画像パス
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

            //任意部分
            //ブランドid
            if (this.Fril_BrandComboBox.SelectedItem != null) {
                FriRaCommon.FrilBrand brand = (FriRaCommon.FrilBrand)this.Fril_BrandComboBox.SelectedItem;
                item_data.brand_id = brand.id;
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
    }

}
