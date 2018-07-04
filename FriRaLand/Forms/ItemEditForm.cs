using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RakuLand.FrilCommon;

namespace RakuLand.Forms {
    public partial class ItemEditForm : Form {
        FrilItem load_item;
        FrilAPI api;
        bool setting_now = true;
        int p_index = 0;
        private bool category_not_exist = false;
        public ItemEditForm(FrilItem load_item,FrilAPI api) {
            InitializeComponent();
            this.load_item = load_item;
            this.api = api;
        }

        private void ItemEditForm_Load(object sender, EventArgs e) {
            //ブランド
            foreach (FrilCommon.FrilBrand p in FrilCommon.fril_brands) {
                this.BrandComboBox.Items.Add(p);
            }
            //カテゴリレベル1
            foreach (var c in FrilCommon.fril_categoryDictionary[0]) {
                this.CategoryComboBoxLevel1.Items.Add(c);
            }
            //商品の状態
            foreach (KeyValuePair<string, string> p in FrilCommon.conditionTypeFril) {
                this.ItemConditionComboBox.Items.Add(p);
            }
            //配送料の負担
            foreach (KeyValuePair<string, string> p in FrilCommon.shippingPayersFril) {
                this.ShippingPayerComboBox.Items.Add(p);
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
        }
        private void ItemEditForm_Shown(object sender, EventArgs e) {
            SetGUIFromItem(this.load_item);
            setting_now = false;
        }

        //GUIへ情報をセットする
        private void SetGUIFromItem(FrilItem loaditem) {
            try {
                //GUIにセット（ComboBoxのSelectedIndexをプログラムから書き換えた場合もイベントは呼ばれるのでindexを変えるだけでいい）
                this.ItemNameTextBox.Text = loaditem.item_name;
                this.DescriptionTextBox.Text = loaditem.detail;
                this.pictureBox1.ImageLocation = loaditem.imagepaths[0];
                this.pictureBox2.ImageLocation = loaditem.imagepaths[1];
                this.pictureBox3.ImageLocation = loaditem.imagepaths[2];
                this.pictureBox4.ImageLocation = loaditem.imagepaths[3];

                if (loaditem.status >= 0) this.ItemConditionComboBox.SelectedIndex = TabIndexFromDictionary(FrilCommon.conditionTypeFril, loaditem.status);
                if (loaditem.carriage >= 0) this.ShippingPayerComboBox.SelectedIndex = TabIndexFromDictionary(FrilCommon.shippingPayersFril, loaditem.carriage);
                if (loaditem.d_method >= 0) this.ShippingMethodComboBox.SelectedIndex = TabIndexFromDictionary(FrilCommon.shippingMethodsBuyerFril, loaditem.d_method);
                if (loaditem.d_area >= 0) this.ShippingAreaComboBox.SelectedIndex = TabIndexFromDictionary(FrilCommon.shippingFromAreas, loaditem.d_area);
                if (loaditem.d_date >= 0) this.ShippingDurationComboBox.SelectedIndex = TabIndexFromDictionary(FrilCommon.shippingFromAreas, loaditem.d_date);
                if (loaditem.s_price <= 0) this.PriceTextBox.Text = "";
                else this.PriceTextBox.Text = loaditem.s_price.ToString();
                if (loaditem.brand_id > 0) this.BrandComboBox.SelectedIndex = TabIndexFromList(FrilCommon.fril_brands, loaditem.brand_id);//TODO:ブランドが選択されてないときは0なので>0にした
                if (loaditem.category_p_id >= 0) {
                    int index = TabIndexFromCategoryPId(FrilCommon.fril_categoryDictionary[0], loaditem.category_p_id);
                    this.CategoryComboBoxLevel1.SelectedIndex = index;
                }
                //TODO:販売アカウントをセットしてない？


            } catch (Exception ex) {
                Log.Logger.Error(ex.Message);
                Console.WriteLine(ex);

                //FIXME:とりあえず書いておく
                //if (!category_not_exist) {
                //    Log.Logger.Error("フリル商品からGUIセットに失敗");
                //    MessageBox.Show("商品の読み込みに失敗しました.", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}

            }
        }

        #region tab番号検索
        private int TabIndexFromCategoryPId( List<FrilCommon.FrilCategory> category_list, int target) {
            int index = 0;
            foreach (var val    in category_list) {
                p_index = 0;
                foreach(var val_unit in val.child_ids) {
                    if (val_unit == target) {
                        return index;
                    }
                    p_index += 1;
                }
                index += 1;
            }
            return index;
        }
        FrilCommon.FrilCategory nowfril_selectedCategory; //フリルの最下層選択中カテゴリ
        private void CategoryComboBoxLevel1_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.CategoryComboBoxLevel1.SelectedIndex < 0) return;
            nowfril_selectedCategory = (FrilCommon.FrilCategory)CategoryComboBoxLevel1.SelectedItem;
            this.CategoryComboBoxLevel2.Items.Clear();
            this.CategoryComboBoxLevel3.Items.Clear();
            this.CategoryComboBoxLevel4.Items.Clear();
            if (FrilCommon.fril_categoryDictionary.ContainsKey(nowfril_selectedCategory.id)) {
                this.CategoryComboBoxLevel2.Visible = true;
                this.CategoryComboBoxLevel3.Visible = false;
                this.CategoryComboBoxLevel4.Visible = false;
                this.CategoryComboBoxLevel2.SelectedIndex = -1;
                this.CategoryComboBoxLevel3.SelectedIndex = -1;
                this.CategoryComboBoxLevel4.SelectedIndex = -1;
                this.CategoryComboBoxLevel2.Text = "選択してください";
                foreach (var c in FrilCommon.fril_categoryDictionary[nowfril_selectedCategory.id]) {
                    //Console.WriteLine(c.id.ToString() + ":" + c.name.ToString());
                    this.CategoryComboBoxLevel2.Items.Add(c);
                }
            }
            if (this.load_item.category_p_id >= 0 && setting_now) {
                this.CategoryComboBoxLevel2.SelectedIndex = p_index;
            }
        }

        private void Fril_CategoryComboBoxLevel2_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.CategoryComboBoxLevel2.SelectedIndex < 0) return;
            nowfril_selectedCategory = (FrilCommon.FrilCategory)CategoryComboBoxLevel2.SelectedItem;
            this.CategoryComboBoxLevel3.Items.Clear();
            this.CategoryComboBoxLevel4.Items.Clear();
            if (FrilCommon.fril_categoryDictionary.ContainsKey(nowfril_selectedCategory.id)) {
                foreach (var c in FrilCommon.fril_categoryDictionary[nowfril_selectedCategory.id]) {
                    this.CategoryComboBoxLevel3.Visible = true;
                    this.CategoryComboBoxLevel4.Visible = false;
                    this.CategoryComboBoxLevel3.SelectedIndex = -1;
                    this.CategoryComboBoxLevel4.SelectedIndex = -1;
                    this.CategoryComboBoxLevel3.Items.Add(c);
                    this.CategoryComboBoxLevel3.Text = "選択してください";
                }
            }
            SetFrilSizeComboBox();

            if (this.load_item.category_id >= 0 && setting_now) {
                this.CategoryComboBoxLevel3.SelectedIndex = TabIndexFromList(FrilCommon.fril_categoryDictionary[load_item.category_p_id], load_item.category_id);
            }
        }

        private void SetFrilSizeComboBox() {
            //現在選択されたカテゴリに応じてサイズのComboBoxを修正する
            if (FrilCommon.fril_default_sizeInfoDictionary.ContainsKey(nowfril_selectedCategory.size_group_id)) {
                this.SizeComboBox.Items.Clear();
                foreach (var sizeinfo in FrilCommon.fril_default_sizeInfoDictionary[nowfril_selectedCategory.size_group_id]) {
                    this.SizeComboBox.Items.Add(sizeinfo);
                }
                this.SizeComboBox.Enabled = true;
                if (this.load_item.size_id == 19999) {
                    SizeComboBox.Text = "なし";

                    return;
                }
                //カテゴリがきまればサイズとブランドの候補はきまるので候補からIDが一致するSelectedIndexを見つければいい
                if (load_item.size_id >= 0 && SizeComboBox.Enabled) {
                    Console.WriteLine(load_item.size_id);
                    SizeComboBox.SelectedIndex = TabIndexFromDictionary(FrilCommon.fril_default_sizeInfoDictionary, 10004);
                }

            } else {
                this.SizeComboBox.Items.Clear();
                this.SizeComboBox.Enabled = false;
            }

        }

        private int TabIndexFromDictionary(Dictionary<string, string> dic, int target) {
            int index = 0;
            foreach (var val in dic) {
                if (int.Parse(val.Value) == target) {
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




        #endregion




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



        private void ShippingPayerComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            this.ShippingMethodComboBox.Items.Clear();
            if (this.ShippingPayerComboBox.SelectedItem == null) return;
            if (this.ShippingPayerComboBox.SelectedIndex == 0) {
                //送料込み
                foreach (KeyValuePair<string, string> p in FrilCommon.shippingMethodsSellerFril) {
                    this.ShippingMethodComboBox.Items.Add(p);
                }
            } else if (this.ShippingPayerComboBox.SelectedIndex == 1) {
                //着払い
                foreach (KeyValuePair<string, string> p in FrilCommon.shippingMethodsBuyerFril) {
                    this.ShippingMethodComboBox.Items.Add(p);
                }
            }
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

        private void PriceTextBox_Leave(object sender, EventArgs e) {
            PriceTextBox.Text = Microsoft.VisualBasic.Strings.StrConv(PriceTextBox.Text, Microsoft.VisualBasic.VbStrConv.Narrow);
        }

        private void CategoryComboBoxLevel3_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.CategoryComboBoxLevel3.SelectedIndex < 0) return;
            nowfril_selectedCategory = (FrilCommon.FrilCategory)CategoryComboBoxLevel3.SelectedItem;
            this.CategoryComboBoxLevel4.Items.Clear();
            if (FrilCommon.fril_categoryDictionary.ContainsKey(nowfril_selectedCategory.id)) {
                foreach (var c in FrilCommon.fril_categoryDictionary[nowfril_selectedCategory.id]) {
                    this.CategoryComboBoxLevel4.Items.Add(c);
                    this.CategoryComboBoxLevel4.Visible = true;
                    this.CategoryComboBoxLevel4.SelectedIndex = -1;
                    this.CategoryComboBoxLevel4.Text = "選択してください";
                }
            }
            SetFrilSizeComboBox();
            if (setting_now) {
                try {
                    if (this.load_item.category_level4_id > 0) {
                        if (FrilCommon.fril_categoryDictionary.ContainsKey(load_item.category_level3_id)) {
                            this.CategoryComboBoxLevel4.SelectedIndex = TabIndexFromList(FrilCommon.fril_categoryDictionary[load_item.category_level3_id], load_item.category_level4_id);
                        }
                    }
                } catch (Exception ex) {
                    Log.Logger.Error(ex);
                    Console.WriteLine(ex);
                    category_not_exist = true;
                }
            }
        }
    }
}
