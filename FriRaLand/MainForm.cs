using FriRaLand.DBHelper;
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
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private List<FrilItem> LocalItemDataBindList = new List<FrilItem>();
        public const string ProductName = "Friland";

        public class Account {
            public int DBId;
            public string email;
            public string password;
            public string access_token;
            public string global_access_token;
            public string sellerid;
            public int kengai_num;
            public int hanbai_num { get; set; }
            public int exhibit_cnt { get; set; }
            public string last_exhibitTime_str; 
            public string nickname { get; set; }
            public bool addSpecialTextToItemName;
            public bool insertEmptyStrToItemName;
            public int defaultbankaddressId = -1;
            public DateTime token_update_date { get; set; }
            public DateTime expiration_date { get; set; }
        }


        private void MainForm_Load(object sender, EventArgs e) {
            FrilCommon.init();
            new ItemRegisterForm().Show();
            FrilItemDBHelper DBhelper = new FrilItemDBHelper();
            AccountDBHelper accountDBHelper = new AccountDBHelper();
            accountDBHelper.onCreate();
            accountDBHelper.addNumberColumn(); //0->1
            accountDBHelper.addExpirationDateColumn();//1->2
            DBhelper.addNumberColumn();//13->14
            new ItemFamilyDBHelper().onCreate();
            new ZaikoDBHelper().onCreate();
            InitializeMainForm();//データ表示グリッドの初期化
            ReloadLocalItem("");
            
            LocalItemDataGridView.AutoGenerateColumns = false;
            ReservationDataGridView.AutoGenerateColumns = false;
            ExhibittedDataGridView.AutoGenerateColumns = false;
            ReloadLocalItem("");

            //new TestForm().Show();




        }


        private void InitializeMainForm() {
            LocalItemDataGridView.VirtualMode = true;
            //データソースの設定
            //ExhibittedDataGridView.DataSource = ExhibittedItemDataBindList;
            //LocalItemDataGridView.DataSource = LocalItemDataBindList;
            //ReservationDataGridView.DataSource = ReservationDataBindList;
            ExhibittedDataGridView.AutoGenerateColumns = false;
            LocalItemDataGridView.AutoGenerateColumns = false;
            ReservationDataGridView.AutoGenerateColumns = false;
            //列の順番がなんかおかしくなるので指定してあげる
            LocalItemDataGridView.Columns["LocalItemDataGridView_Image"].DisplayIndex = 0;
            LocalItemDataGridView.Columns["LocalItemDataGridView_parent_id"].DisplayIndex = 1;
            LocalItemDataGridView.Columns["LocalItemDataGridView_child_id"].DisplayIndex = 2;
            LocalItemDataGridView.Columns["LocalItemDataGridView_zaikonum"].DisplayIndex = 3;
            LocalItemDataGridView.Columns["LocalItemDataGridView_name"].DisplayIndex = 4;
            LocalItemDataGridView.Columns["LocalItemDataGridView_description"].DisplayIndex = 5;
            LocalItemDataGridView.Columns["LocalItemDataGridView_price"].DisplayIndex = 6;
            LocalItemDataGridView.Columns["LocalItemDataGridView_favorite"].Visible = false;
            LocalItemDataGridView.Columns["LocalItemDataGridView_comment"].Visible = false;
            LocalItemDataGridView.Columns["LocalItemDataGridView_created_str"].Visible = false;
            LocalItemDataGridView.Columns["LocalItemDataGridView_status_message"].Visible = false;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_Image"].DisplayIndex = 0;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_status_message"].DisplayIndex = 1;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_name"].DisplayIndex = 2;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_AccountNickName"].DisplayIndex = 3;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_description"].DisplayIndex = 4;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_price"].DisplayIndex = 5;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_favorite"].DisplayIndex = 6;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_comment"].DisplayIndex = 7;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_item_pv"].DisplayIndex = 8;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_created_str"].DisplayIndex = 9;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_comment_time_buyer"].DisplayIndex = 10;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_comment_time_seller"].DisplayIndex = 11;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_transaction_message_num"].DisplayIndex = 12;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_transaction_message_buyer"].DisplayIndex = 13;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_transaction_message_seller"].DisplayIndex = 14;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_bikou"].DisplayIndex = 15;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_address_copyed"].DisplayIndex = 16;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_buyer_simei"].DisplayIndex = 17;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_buyer_name"].DisplayIndex = 18;
            ExhibittedDataGridView.Columns["ExhibittedDataGridView_itemid"].DisplayIndex = 19;

            ReservationDataGridView.Columns["ReservationDataGridView_exhibit_status_str"].DisplayIndex = 0;
            ReservationDataGridView.Columns["ReservationDataGridView_itemImage"].DisplayIndex = 1;
            ReservationDataGridView.Columns["ReservationDataGridView_itemName"].DisplayIndex = 2;
            ReservationDataGridView.Columns["ReservationDataGridView_nickname"].DisplayIndex = 3;
            ReservationDataGridView.Columns["ReservationDataGridView_exhibitDateString"].DisplayIndex = 4;
            ReservationDataGridView.Columns["ReservationDataGridView_deleteDateString"].DisplayIndex = 5;
            ReservationDataGridView.Columns["ReservationDataGridView_favorite"].DisplayIndex = 6;
            ReservationDataGridView.Columns["ReservationDataGridView_comment"].DisplayIndex = 7;
            ReservationDataGridView.Columns["ReservationDataGridView_deleteDateString2"].DisplayIndex = 8;
            ReservationDataGridView.Columns["ReservationDataGridView_favorite2"].DisplayIndex = 9;
            ReservationDataGridView.Columns["ReservationDataGridView_comment2"].DisplayIndex = 10;
            ReservationDataGridView.Columns["ReservationDataGridView_reexhibit_flag"].DisplayIndex = 11;
            ToggleExhibittedDataGridView(true);
        }

        public void ToggleExhibittedDataGridView(bool SellingOrStopping) {
            if (SellingOrStopping) {
                //商品説明も表示
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_description"].Visible = true;
                //購入者氏名は非表示
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_buyer_simei"].Visible = false;
                //いいねとコメント表示
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_favorite"].Visible = true;
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_comment"].Visible = true;
                //購入者コメント時間,出品者コメント時間を表示
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_comment_time_seller"].Visible = true;
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_comment_time_buyer"].Visible = true;
                //購入者取引メッセージ時間, 出品者取引メッセージ時間を非表示
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_transaction_message_seller"].Visible = false;
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_transaction_message_buyer"].Visible = false;
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_transaction_message_num"].Visible = false;
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_buyer_name"].Visible = false;
            } else {
                //商品説明も非表示
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_description"].Visible = false;
                //購入者氏名は表示
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_buyer_simei"].Visible = true;
                //いいねとコメント非表示
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_favorite"].Visible = false;
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_comment"].Visible = false;
                //購入者コメント時間,出品者コメント時間を非表示
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_comment_time_seller"].Visible = false;
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_comment_time_buyer"].Visible = false;
                //購入者取引メッセージ時間, 出品者取引メッセージ時間を表示
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_transaction_message_seller"].Visible = true;
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_transaction_message_buyer"].Visible = true;
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_transaction_message_num"].Visible = true;
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_buyer_name"].Visible = true;
            }
        }

        private void LocalItemDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            try {
                if (LocalItemDataBindList.Count <= e.RowIndex) return;
                FrilItem item = LocalItemDataBindList[e.RowIndex];
                if (item.Image == null) item.loadImageFromFile();
                switch (e.ColumnIndex) {
                    case 0:
                        e.Value = item.Image;
                        break;
                    case 1:
                        e.Value = item.parent_id;
                        break;
                    case 2:
                        e.Value = item.child_id;
                        break;
                    case 3:
                        e.Value = (item.zaikonum < 0) ? "" : item.zaikonum.ToString();
                        break;
                    case 4:
                        e.Value = item.item_name;
                        break;
                    case 5:
                        e.Value = item.detail;
                        break;
                    case 6:
                        e.Value = item.s_price;
                        break;
                }
            } catch {

            }
        }
        private async void ReloadLocalItem(string text) {
            FrilItemDBHelper DBhelper = new FrilItemDBHelper();
            List<FrilItem> loadresult;
            if (string.IsNullOrEmpty(text)) loadresult = await Task.Run(() => DBhelper.loadItems());
            else loadresult = await Task.Run(() => DBhelper.selectItemFromName(text));
            string message = "";// loadresult.Value;
            LocalItemDataBindList.Clear();
            foreach (var item in loadresult) {
                LocalItemDataBindList.Add(item);
            }
            if (message != "") {
                MessageBox.Show(message, MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LocalItemDataGridView.RowCount = LocalItemDataBindList.Count;
            LocalItemDataGridView.Refresh();
            LocalItemDataGridView.ClearSelection();
        }
        









    }
}
