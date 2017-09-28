using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            //ラクマ側
            //カテゴリレベル1
            foreach (var c in FriRaCommon.rakuma_categoryDictionary[0]) {
                this.Rakuma_CategoryComboBoxLevel1.Items.Add(c);
            }
            //商品の状態
            foreach (KeyValuePair<string, string> p in FriRaCommon.conditionTypeRakuma) {
                this.Rakuma_ItemConditionComboBox.Items.Add(p);
            }
            //配送料の負担
            foreach (KeyValuePair<string, string> p in FriRaCommon.shippingPayersRakuma) {
                this.Rakuma_ShippingPayerComboBox.Items.Add(p);
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
        private void Rakuma_CategoryComboBoxLevel1_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FriRaCommon.RakumaCategory)e.ListItem).name;
        }

        private void Rakuma_CategoryComboBoxLevel2_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FriRaCommon.RakumaCategory)e.ListItem).name;
        }

        private void Rakuma_CategoryComboBoxLevel3_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FriRaCommon.RakumaCategory)e.ListItem).name;
        }

        private void Rakuma_CategoryComboBoxLevel4_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FriRaCommon.RakumaCategory)e.ListItem).name;
        }

        private void Rakuma_SizeComboBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FriRaCommon.RakumaSize)e.ListItem).title;
        }

        private void Rakuma_BrandComboBox_Format(object sender, ListControlConvertEventArgs e) {

        }

        private void Rakuma_ItemConditionComboBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((KeyValuePair<string, string>)e.ListItem).Key;
        }

        private void Rakuma_ShippingPayerComboBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((KeyValuePair<string, string>)e.ListItem).Key;
        }

        private void Rakuma_ShippingMethodComboBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((KeyValuePair<string, string>)e.ListItem).Key;
        }

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

        private void Rakuma_CategoryComboBoxLevel1_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.Rakuma_CategoryComboBoxLevel1.SelectedIndex < 0) return;
            nowrakuma_selectedCategory = (FriRaCommon.RakumaCategory)Rakuma_CategoryComboBoxLevel1.SelectedItem;
            this.Rakuma_CategoryComboBoxLevel2.Items.Clear();
            this.Rakuma_CategoryComboBoxLevel3.Items.Clear();
            this.Rakuma_CategoryComboBoxLevel4.Items.Clear();
            if (FriRaCommon.rakuma_categoryDictionary.ContainsKey(nowrakuma_selectedCategory.id)) {
                this.Rakuma_CategoryComboBoxLevel2.Visible = true;
                this.Rakuma_CategoryComboBoxLevel3.Visible = false;
                this.Rakuma_CategoryComboBoxLevel4.Visible = false;
                this.Rakuma_CategoryComboBoxLevel2.SelectedIndex = -1;
                this.Rakuma_CategoryComboBoxLevel3.SelectedIndex = -1;
                this.Rakuma_CategoryComboBoxLevel4.SelectedIndex = -1;
                this.Rakuma_CategoryComboBoxLevel2.Text = "選択してください";
                foreach (var c in FriRaCommon.rakuma_categoryDictionary[nowrakuma_selectedCategory.id]) {
                    this.Rakuma_CategoryComboBoxLevel2.Items.Add(c);
                }
            }
            SetRakumaSizeComboBox();
        }

        private void Rakuma_CategoryComboBoxLevel2_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.Rakuma_CategoryComboBoxLevel2.SelectedIndex < 0) return;
            nowrakuma_selectedCategory = (FriRaCommon.RakumaCategory)Rakuma_CategoryComboBoxLevel2.SelectedItem;
            this.Rakuma_CategoryComboBoxLevel3.Items.Clear();
            this.Rakuma_CategoryComboBoxLevel4.Items.Clear();
            if (FriRaCommon.rakuma_categoryDictionary.ContainsKey(nowrakuma_selectedCategory.id)) {
                foreach (var c in FriRaCommon.rakuma_categoryDictionary[nowrakuma_selectedCategory.id]) {
                    this.Rakuma_CategoryComboBoxLevel3.Visible = true;
                    this.Rakuma_CategoryComboBoxLevel4.Visible = false;
                    this.Rakuma_CategoryComboBoxLevel3.SelectedIndex = -1;
                    this.Rakuma_CategoryComboBoxLevel4.SelectedIndex = -1;
                    this.Rakuma_CategoryComboBoxLevel3.Text = "選択してください";
                    this.Rakuma_CategoryComboBoxLevel3.Items.Add(c);
                }
            }
            SetRakumaSizeComboBox();
        }

        private void Rakuma_CategoryComboBoxLevel3_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.Rakuma_CategoryComboBoxLevel3.SelectedIndex < 0) return;
            nowrakuma_selectedCategory = (FriRaCommon.RakumaCategory)Rakuma_CategoryComboBoxLevel3.SelectedItem;
            this.Rakuma_CategoryComboBoxLevel4.Items.Clear();
            if (FriRaCommon.rakuma_categoryDictionary.ContainsKey(nowrakuma_selectedCategory.id)) {
                foreach (var c in FriRaCommon.rakuma_categoryDictionary[nowrakuma_selectedCategory.id]) {
                    this.Rakuma_CategoryComboBoxLevel4.Items.Add(c);
                    this.Rakuma_CategoryComboBoxLevel4.Visible = true;
                    this.Rakuma_CategoryComboBoxLevel4.SelectedIndex = -1;
                    this.Rakuma_CategoryComboBoxLevel4.Text = "選択してください";
                }
            }
            SetRakumaSizeComboBox();
        }

        private void Rakuma_CategoryComboBoxLevel4_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.Rakuma_CategoryComboBoxLevel4.SelectedIndex < 0) return;
            nowrakuma_selectedCategory = (FriRaCommon.RakumaCategory)Rakuma_CategoryComboBoxLevel4.SelectedItem;
            if (FriRaCommon.rakuma_categoryDictionary.ContainsKey(nowrakuma_selectedCategory.id)) {
                //Level5がないのでエラー　ないと思う
                MessageBox.Show("このカテゴリは指定できません");
                Log.Logger.Error("ラクマカテゴリがレベル4以上");
            }
            SetRakumaSizeComboBox();
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
        private void SetRakumaSizeComboBox() {
            //現在選択されたカテゴリに応じてサイズのComboBoxを修正する
            if (FriRaCommon.rakuma_sizeDictionary.ContainsKey(nowrakuma_selectedCategory.sizeType)) {
                this.Rakuma_SizeComboBox.Items.Clear();
                foreach (var sizeinfo in FriRaCommon.rakuma_sizeDictionary[nowrakuma_selectedCategory.sizeType]) {
                    this.Rakuma_SizeComboBox.Items.Add(sizeinfo);
                }
                this.Rakuma_SizeComboBox.Enabled = true;
            }
            else {
                this.Rakuma_SizeComboBox.Items.Clear();
                this.Rakuma_SizeComboBox.Enabled = false;
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

        private void Rakuma_ShippingPayerComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            this.Rakuma_ShippingMethodComboBox.Items.Clear();
            if (this.Rakuma_ShippingPayerComboBox.SelectedItem == null) return;
            if (this.Rakuma_ShippingPayerComboBox.SelectedIndex == 0) {
                //送料込み
                foreach (KeyValuePair<string, string> p in FriRaCommon.shippingMethodsSellerRakuma) {
                    this.Rakuma_ShippingMethodComboBox.Items.Add(p);
                }
            }
            else if (this.Rakuma_ShippingPayerComboBox.SelectedIndex == 1) {
                //着払い
                foreach (KeyValuePair<string, string> p in FriRaCommon.shippingMethodsBuyerRakuma) {
                    this.Rakuma_ShippingMethodComboBox.Items.Add(p);
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
    }
}
