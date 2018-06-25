using FriRaLand.DBHelper;
using FriRaLand.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using static FriRaLand.Common;

namespace FriRaLand {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private List<FrilItem> LocalItemDataBindList = new List<FrilItem>();
        public const string ProductName = "Friland";
        private List<ReservationSettingForm.ReservationSetting> ReservationDataBindList = new List<ReservationSettingForm.ReservationSetting>();

        //public class Account {
        //    public int DBId;
        //    public string email;
        //    public string password;
        //    public string auth_token;
        //    public string sellerid;
        //    public int kengai_num;
        //    public int hanbai_num { get; set; }
        //    public int exhibit_cnt { get; set; }
        //    public string last_exhibitTime_str; 
        //    public string nickname { get; set; }
        //    public bool addSpecialTextToItemName;
        //    public bool insertEmptyStrToItemName;
        //    public int defaultbankaddressId = -1;
        //    public DateTime token_update_date { get; set; }
        //    public DateTime expiration_date { get; set; }
        //}


        System.Random random = new System.Random();
        private List<FrilAPI> FrilAPIList = new List<FrilAPI>();
        private Dictionary<string, FrilAPI> sellerIDtoAPIDictionary = new Dictionary<string, FrilAPI>(); //sellerid -> API

        private void MainForm_Load(object sender, EventArgs e) {
            FrilCommon.init();
            //new ItemRegisterForm().Show();
            FrilItemDBHelper DBhelper = new FrilItemDBHelper();
            DBhelper.onCreate();
            AccountDBHelper accountDBHelper = new AccountDBHelper();
            accountDBHelper.onCreate();
 
            ReservationDBHelper reservationDBhelper = new ReservationDBHelper();
            reservationDBhelper.onCreate();
            //reservationDBhelper.addReexhibitFlagColumn();//2->3
            accountDBHelper.addKengai_ExhibitCnt_LastExhibitTime_Column();//3->4
            //reservationDBhelper.addDelete2Column();//4->5//追加したので不要
            //GroupBelongDBHelper groupbelongDBHelper = new GroupBelongDBHelper();
            //groupbelongDBHelper.onCreate();
            //GroupKindDBHelper groupkindDBHelper = new GroupKindDBHelper();
            //groupkindDBHelper.onCreate();
            new SettingsDBHelper().onCreate();
            Settings.updateTemplateSettingDBForAddTitle();
            new ShuppinRirekiDBHelper().onCreate();
            new ShuppinRirekiDBHelper().addCreatedDateColumn();//5->6
            new ShuppinRirekiDBHelper().addUretaColumn();//6->7
            accountDBHelper.addHanbaiNumColumn();//7->8
            accountDBHelper.addItemNameSpeccialSettingsColumn();//8->9
            new ShuppinRirekiDBHelper().addReexhibitFlag();//9->10
            //new GroupKindDBHelper().addNumberColumn();//10->11
            accountDBHelper.addDefaultBankAddressColumn();//11->12
            DBhelper.addNumberColumn();//13->14
            new ItemNoteDBHelper().onCreate();
            new ItemFamilyDBHelper().onCreate();
            new ZaikoDBHelper().onCreate();
            //new DailyExhibitDBHelper().onCreate();
            new ExhibitLogDBHelper().onCreate();
            new ExhibitLogDBHelper().addItemIDColumn();//14->15
            //new DefaultBankAddressBankDBHelper().onCreate();
            //new DefaultBankAddressBankDBHelper().updateDefaultBankAddressToList();
            //new NesageCntDBHelper().onCreate();
            //new NesageLogDBHelper().onCreate();
            LocalItemDataGridView.AutoGenerateColumns = false;
            ReservationDataGridView.AutoGenerateColumns = false;
            ExhibittedDataGridView.AutoGenerateColumns = false;
            ReloadLocalItem("");
            ReloadReservationItem("");
            //ReloadDailyExhibit();
            InitializeAccountData();
            InitializeMainForm();//データ表示グリッドの初期化
            //SetAutoKengaiTimer();
            //SetNotificationTimer();
            //ライセンスチェックを行う

            //new TestForm().Show();

        }


        private void InitializeMainForm() {
            LocalItemDataGridView.VirtualMode = true;
            //データソースの設定
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
        private async void ReloadReservationItem(string text) {
            ReservationDBHelper reservationDBHelper = new ReservationDBHelper();
            List<ReservationSettingForm.ReservationSetting> loadresult;
            if (string.IsNullOrEmpty(text)) loadresult = await Task.Run(() => reservationDBHelper.loadReservations());
            else loadresult = await Task.Run(() => reservationDBHelper.selectReservationFromName(text));
            ReservationDataBindList.Clear();
            int row_num = 0;
            foreach (var reservation in loadresult) {
                //取消日時がデフォルト（取消しない）ものは取消日時文字列を空に
                if (reservation.deleteDate.ToString() == ReservationSettingForm.ReservationSetting.dafaultDate) reservation.deleteDateString = "";
                if (reservation.deleteDate2.ToString() == ReservationSettingForm.ReservationSetting.dafaultDate) reservation.deleteDateString2 = "";
                ReservationDataBindList.Add(reservation);
            }

            ReservationDataGridView.RowCount = ReservationDataBindList.Count;
            ReservationDataGridView.Refresh();
            ReservationDataGridView.ClearSelection();
        }

        public void OnBackFromItemExhibitForm() {
            ReloadLocalItem("");
            ReloadReservationItem("");
        }
        public void OnBackFromReservationSettingForm() {
            ReloadReservationItem("");
        }

        private void register_button_Click(object sender, EventArgs e) {
                //if (!LicenseForm.checkCanUseWithErrorWindow()) return;
                //if (checkNowAutoMode()) return;
                ItemRegisterForm f = new ItemRegisterForm();
                //f.apilist = FrilAPIList;
                f.mainform = this;
                f.Show();
        }

        private void deleteItemButton_Click(object sender, EventArgs e) {
            //if (checkNowAutoMode()) return;
            //if (!LicenseForm.checkCanUseWithErrorWindow()) return;
            List<int> deleteIdList = new List<int>();
            foreach (DataGridViewRow row in LocalItemDataGridView.SelectedRows) deleteIdList.Add(LocalItemDataBindList[row.Index].DBId);
            if (deleteIdList.Count == 0) {
                MessageBox.Show("商品が選択されていません");
                return;
            }
            DialogResult result = MessageBox.Show("選択した" + deleteIdList.Count.ToString() + "件の商品を削除しますか?", "質問", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result != DialogResult.Yes) return;
            new FrilItemDBHelper().deleteItem(deleteIdList);
            //商品を削除した場合はその商品の出品予約も削除する
            var reservationDBHelper = new ReservationDBHelper();
            List<ReservationSettingForm.ReservationSetting> reservationList = reservationDBHelper.loadReservations();
            var deleteItemIdArray = deleteIdList.ToArray();
            List<int> deleteReservationIdList = new List<int>();
            foreach (var reservation in reservationList) if (Array.IndexOf(deleteItemIdArray, reservation.itemDBId) >= 0) deleteReservationIdList.Add(reservation.DBId);
            reservationDBHelper.deleteReservation(deleteReservationIdList);
            ReloadLocalItem("");
            ReloadReservationItem("");
        }
        private int exhibit_success_num = 0;
        private int exhibit_failed_num = 0;
        private int delete_success_num = 0;
        private int delete_failed_num = 0;
        private int reexhibit_success_num = 0;
        private int reexhibit_failed_num = 0;
        private int zaikogire_num = 0;
        private List<string> reservation_fail_logs = new List<string>();

        private List<ReservationSettingForm.ReservationSetting> ReservationObjects = new List<ReservationSettingForm.ReservationSetting>(); //予約されるべき商品
        private void ReservationToggleButton_Click(object sender, EventArgs e) {
            //if (!LicenseForm.checkCanUseWithErrorWindow()) return;
            if (ReservationTimer.Enabled) {
                //ON->OFF
                ReservationTimer.Enabled = false;
                ReExhibitTimer.Enabled = false;
                //キャンセルリクエストを送る
                ReservationBackgroundWorker.CancelAsync();
                ReExhibitBackgroundWorker3.CancelAsync();
                ReservationToggleButton.Text = "予約実行";
                ReservationToggleButton.BackColor = Color.Transparent;
                ReloadReservationItem("");
            } else {
                //OFF->ON
                ReExhibitTimer.Interval = Settings.getReexhibitCheckInterval() * 60 * 1000;
                if (ReservationBackgroundWorker.IsBusy || ReExhibitBackgroundWorker3.IsBusy) {
                    //前にキャンセルしたbackgroundWorkerがまだ動いているので無理
                    MessageBox.Show("しばらく待ってから実行してください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                } else {
                    ReservationToggleButton.Text = "予約停止";
                    ReservationToggleButton.BackColor = Color.Red;
                    //現在時刻より後の商品だけを対象に追加する
                    this.ReservationObjects.Clear();
                    var allReservationList = new ReservationDBHelper().loadReservations();
                    foreach (var reservation in allReservationList) {
                        if (reservation.exhibitDate > DateTime.Now) {
                            //出品する必要がある
                            reservation.doexhibit_flag = true;
                            //削除機能なしかチェック
                            if (reservation.deleteDate == DateTime.Parse(ReservationSettingForm.ReservationSetting.dafaultDate)) reservation.docancel_flag = false;
                            else reservation.docancel_flag = true;
                            //削除機能2なしかチェック
                            if (reservation.deleteDate2 == DateTime.Parse(ReservationSettingForm.ReservationSetting.dafaultDate)) reservation.docancel_flag2 = false;
                            else reservation.docancel_flag2 = true;
                            ReservationObjects.Add(reservation);
                        } else {
                            //出品はしない
                            //削除する必要がなければcontinue
                            if (reservation.deleteDate == DateTime.Parse(ReservationSettingForm.ReservationSetting.dafaultDate) && reservation.deleteDate2 == DateTime.Parse(ReservationSettingForm.ReservationSetting.dafaultDate)) continue;
                            //1か2のどちらかを削除する
                            reservation.doexhibit_flag = false;
                            if (reservation.deleteDate != DateTime.Parse(ReservationSettingForm.ReservationSetting.dafaultDate)) {
                                if (reservation.deleteDate > DateTime.Now) {
                                    //削除1をする必要がある
                                    reservation.docancel_flag = true;
                                }
                            }
                            if (reservation.deleteDate2 != DateTime.Parse(ReservationSettingForm.ReservationSetting.dafaultDate)) {
                                if (reservation.deleteDate2 > DateTime.Now) {
                                    //削除2をする必要がある
                                    reservation.docancel_flag2 = true;
                                }
                            }
                            ReservationObjects.Add(reservation);
                        }
                    }
                    exhibit_success_num = exhibit_failed_num = delete_success_num = delete_failed_num = reexhibit_success_num = reexhibit_failed_num = zaikogire_num = 0;
                    exhibit_success_num_label.Text = delete_success_num_label.Text = reexhibit_success_num_label.Text = "0";
                    exhibit_failed_num_label.Text = delete_failed_num_label.Text = reexhibit_failed_num_label.Text = "0";
                    zaikogire_num_label.Text = "0";
                    reservation_fail_logs.Clear();
                    //（準備がちゃんとおわってからタイマースタートするように）
                    //タイマースタート
                    ReservationTimer.Enabled = true;
                    ReExhibitTimer.Enabled = true;
                }
            }


        }
        public void OnBackFromAccountManageForm() {
            //アカウント管理画面から戻って来たときに行う処理
            //アカウント情報をもう一度読み込む
            //アカウント情報コンテナの再設定
            InitializeAccountData();
        }
        Dictionary<int, FrilAPI> FrilAPIDictionary = new Dictionary<int, FrilAPI>(); //accountDBId, FrilAPI
        Dictionary<int, Common.Account> FrilAccountDictionary = new Dictionary<int, Common.Account>(); //accountDBId, Account
        private void InitializeAccountData() {
            //アカウントリストの読み込み,APIリストの作成
            this.sellerIDtoAPIDictionary.Clear();
            this.accountListComboBox.Items.Clear();
            FrilAPIList.Clear();
            var accountList = AccountManageForm.accountLoader();
            FrilAPIDictionary = new Dictionary<int, FrilAPI>(); //accountDBId, FrilAPI
            FrilAccountDictionary = new Dictionary<int, Common.Account>(); //accountDBId, Account
            foreach (var a in accountList) {
                var api = new FrilAPI(a);
                this.accountListComboBox.Items.Add(api);
                FrilAPIList.Add(api);
                FrilAPIDictionary[a.DBId] = api;
                FrilAccountDictionary[a.DBId] = a;
                this.sellerIDtoAPIDictionary[a.sellerid] = api;
            }
            if (accountList != null && accountList.Count > 0) accountListComboBox.SelectedIndex = 0;

            ////グループリストの読み込み
            //this.groupListComboBox.Items.Clear();
            //groupList.Clear();
            //var groupBelongDictionary = new GroupBelongDBHelper().loadGroupBelongDictionary();
            //var groupKindDictionary = new GroupKindDBHelper().loadGroupKindDictionary();
            //foreach (KeyValuePair<int, string> p in groupKindDictionary) {
            //    try {
            //        List<int> belongs = groupBelongDictionary[p.Key];
            //        Group tmpGroup = new Group();
            //        tmpGroup.groupname = p.Value;
            //        tmpGroup.accountList = new List<Account>();
            //        tmpGroup.apiList = new List<MercariAPI>();
            //        foreach (int belong in belongs) {
            //            tmpGroup.accountList.Add(mercariAccountDictionary[belong]);
            //            tmpGroup.apiList.Add(mercariAPIDictionary[belong]);
            //        }
            //        groupList.Add(tmpGroup);
            //        this.groupListComboBox.Items.Add(tmpGroup);
            //    } catch (Exception ex) {
            //        Log.Logger.Error(ex.Message);
            //        Log.Logger.Error("グループリストの作成に失敗");
            //        MessageBox.Show("グループリストの作成に失敗しました", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //if (groupList != null && groupList.Count > 0) groupListComboBox.SelectedIndex = 0;
        }

        private void exhibitRegisterButton_Click(object sender, EventArgs e) {
            //if (!LicenseForm.checkCanUseWithErrorWindow()) return;
            //if (checkNowAutoMode()) return;
            List<int> selectIdList = new List<int>();
            foreach (DataGridViewRow row in LocalItemDataGridView.SelectedRows) selectIdList.Add(LocalItemDataBindList[row.Index].DBId);
            if (selectIdList.Count != 1) {
                MessageBox.Show("出品登録する商品を1つだけ選択してください。");
                return;
            }
            FrilItem item = new FrilItemDBHelper().selectItem(selectIdList)[0];
            ReservationSettingForm f = new ReservationSettingForm(item, this);
            f.Show();
        }
        private bool checkNowAutoMode() {
            if (this.ReservationToggleButton.BackColor == Color.Transparent) return false;
            MessageBox.Show("現在予約実行中です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return true;
        }

        private void アカウント管理ToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void 通知一覧ToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void 出金管理ToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void 圏外チェックToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void 条件指定による購入ToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void dBメンテナンスToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void ライセンスToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void オプションToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void アカウント管理ToolStripMenuItem_Click_1(object sender, EventArgs e) {
            //if (!LicenseForm.checkCanUseWithErrorWindow()) return;
            if (checkNowAutoMode()) return;
            AccountManageForm f = new AccountManageForm();
            f.mainform = this;
            this.Enabled = false;
            f.Show();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e) {
            //タブが変更されたとき
            //出品済みタブ以外では、グリッド右上の表の参照のボタンなどを表示する
            if (tabControl1.SelectedIndex == 0) {
                LocalItemDataGridView.Visible = true;
                ExhibittedDataGridView.Visible = false;
                ReservationDataGridView.Visible = false;
                DailyExhibitDataGridView.Visible = false;
            }
            if (tabControl1.SelectedIndex == 1) {
                LocalItemDataGridView.Visible = false;
                ExhibittedDataGridView.Visible = false;
                ReservationDataGridView.Visible = true;
                DailyExhibitDataGridView.Visible = false;
            }
            if (tabControl1.SelectedIndex == 2) {
                //出品済みタブ
                ExhibittedDataGridView.Visible = true;
                LocalItemDataGridView.Visible = false;
                ReservationDataGridView.Visible = false;
                DailyExhibitDataGridView.Visible = false;
            }
            if (tabControl1.SelectedIndex == 3) {
                ExhibittedDataGridView.Visible = false;
                LocalItemDataGridView.Visible = false;
                ReservationDataGridView.Visible = false;
                DailyExhibitDataGridView.Visible = true;
            }
        }

        private void ReservationDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            try {
                if (ReservationDataBindList.Count <= e.RowIndex) return;
                ReservationSettingForm.ReservationSetting reservation = ReservationDataBindList[e.RowIndex];
                if (reservation.itemImage == null) reservation.loadImageFromFile();
                switch (e.ColumnIndex) {
                    case 0:
                        e.Value = reservation.exhibit_status_str;
                        break;
                    case 1:
                        e.Value = reservation.itemImage;
                        break;
                    case 2:
                        e.Value = reservation.item_name;
                        break;
                    case 3:
                        e.Value = reservation.accountNickName;
                        break;
                    case 4:
                        e.Value = reservation.exhibitDateString;
                        break;
                    case 5:
                        e.Value = reservation.deleteDateString;
                        break;
                    case 6:
                        e.Value = reservation.consider_favorite_str;
                        break;
                    case 7:
                        e.Value = reservation.consider_comment_str;
                        break;
                    case 8:
                        e.Value = reservation.deleteDateString2;
                        break;
                    case 9:
                        e.Value = reservation.consider_favorite_str2;
                        break;
                    case 10:
                        e.Value = reservation.consider_comment_str2;
                        break;
                    case 11:
                        e.Value = reservation.reexhibit_flag_str;
                        break;
                }
            } catch {

            }
        }

        private void deleteReservationItem_Click(object sender, EventArgs e) {
            //if (!LicenseForm.checkCanUseWithErrorWindow()) return;
            if (checkNowAutoMode()) return;
            List<int> deleteIdList = new List<int>();
            foreach (DataGridViewRow row in ReservationDataGridView.SelectedRows) deleteIdList.Add(ReservationDataBindList[row.Index].DBId);
            if (deleteIdList.Count == 0) {
                MessageBox.Show("商品が選択されていません");
                return;
            }
            DialogResult result = MessageBox.Show("選択した" + deleteIdList.Count.ToString() + "件の予約を削除しますか?", "質問", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result != DialogResult.Yes) return;
            new ReservationDBHelper().deleteReservation(deleteIdList);
            ReloadReservationItem("");
        }

        private void ReservationTimer_Tick(object sender, EventArgs e) {
            //既に予約処理が実行中ならば実行しない
            if (ReservationBackgroundWorker.IsBusy) return;
            ReservationBackgroundWorker.RunWorkerAsync();
        }

        private BackgroundWorker bgWorker;
        private void ReservationbackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
            bgWorker = (BackgroundWorker)sender;
            var accountDBHelper = new AccountDBHelper();
            var reservationDBHelper = new ReservationDBHelper();
            //var shuppinrirekiDBHelper = new ShuppinRirekiDBHelper();
            var itemDBHelper = new FrilItemDBHelper();
            var zaikoDBHelper = new ZaikoDBHelper();
            var itemfamilyDBHelper = new ItemFamilyDBHelper();
            var exhibitLogDBHelper = new ExhibitLogDBHelper();
            var reservation = new ReservationSettingForm.ReservationSetting();
            for (int i = 0; i < this.ReservationObjects.Count; i++) {
                //キャンセルリクエストがあったら即キャンセル
                if (bgWorker.CancellationPending) {
                    e.Cancel = true;
                    return;
                }
                try {
                    reservation = this.ReservationObjects[i];
                    if (reservation.doexhibit_flag && DateTime.Now > reservation.exhibitDate) {
                        //出品成功しようが失敗しようがフラグを折る（二度以上リトライしないように）
                        ReservationObjects[i].doexhibit_flag = false;
                        //自動出品ルーチン
                        Account a = accountDBHelper.selectItem(new List<int> { reservation.accountDBId })[0];
                        Log.Logger.Info("自動出品用のアカウント取得成功");

                        FrilAPI api = new FrilAPI(a);
                        //api = Common.checkFrilAPI(api);//FIXIT:これは自動更新用なので
                        FrilItem item = itemDBHelper.selectItem(new List<int> { reservation.itemDBId })[0];
                        Log.Logger.Info("自動出品対象の商品をDBから取得成功");
                        var family = itemfamilyDBHelper.getItemFamilyFromItemDBId(item.DBId);
                        string parent_id = (family == null ? "" : family.parent_id);
                        string child_id = (family == null ? "" : family.child_id);
                        //出品できるか調べる
                        if (ExhibitService.canExhibitItem(a, item) < 0) {
                            if (ExhibitService.canExhibitItem(a, item) == -1) {
                                reservation_fail_logs.Add(string.Format("{0}:{1}:親ID:{2}子ID:{3}:出品失敗:圏外数オーバー", DateTime.Now.ToString(), api.account.nickname, parent_id, child_id));
                                exhibit_failed_num++;
                            } else if (ExhibitService.canExhibitItem(a, item) == -2) {
                                reservation_fail_logs.Add(string.Format("{0}:{1}:親ID:{2}子ID:{3}:出品失敗:在庫切れ", DateTime.Now.ToString(), api.account.nickname, parent_id, child_id));
                                zaikogire_num++;
                            }
                            continue;
                        }
                        //出品実行
                        //FrilItem result = SellWithOption(a, item);
                        CookieContainer cc = new CookieContainer();
                        string result_item_id = api.Sell(item, cc);
                        if (result_item_id != null) {
                            //出品した商品に関するIDを更新
                            Log.Logger.Info("自動出品に成功 : " + result_item_id);
                            reservationDBHelper.updateItemID(reservation.DBId, result_item_id);
                            reservationDBHelper.updateExhibitStatus(reservation.DBId, ReservationSettingForm.Status.Success);
                            //出品後の共通DB操作実行
                            ExhibitService.updateDBOnExhibitCommon(a, api, item, item.DBId, reservation.reexhibit_flag);
                            //バインドリストのstatus_str更新～
                            for (int k = 0; k < ReservationDataBindList.Count; k++) {
                                if (ReservationDataBindList[k].DBId == reservation.DBId) {
                                    ReservationDataBindList[k].exhibit_status_str =
                                        ReservationSettingForm.get_exhibit_status_str(ReservationSettingForm.Status.Success);
                                    bgWorker.ReportProgress(i * 100 / ReservationObjects.Count); //GUI更新リクエスト
                                    break;
                                }
                            }
                            exhibit_success_num++;
                        } else {
                            //出品失敗
                            Log.Logger.Error("自動出品失敗");
                            reservationDBHelper.updateExhibitStatus(reservation.DBId, ReservationSettingForm.Status.Failed);
                            for (int k = 0; k < ReservationDataBindList.Count; k++) {
                                if (ReservationDataBindList[k].DBId == reservation.DBId) {
                                    ReservationDataBindList[k].exhibit_status_str =
                                        ReservationSettingForm.get_exhibit_status_str(ReservationSettingForm.Status.Failed);
                                    bgWorker.ReportProgress(i * 100 / ReservationObjects.Count); //GUI更新リクエスト
                                    break;
                                }
                            }
                            exhibit_failed_num++;
                            if (a.expiration_date < DateTime.Now) reservation_fail_logs.Add(string.Format("{0}:{1}:親ID:{2}子ID:{3}:出品失敗:トークン有効期限切れ", DateTime.Now.ToString(), api.account.nickname, parent_id, child_id));
                            else reservation_fail_logs.Add(string.Format("{0}:{1}:親ID:{2}子ID:{3}:出品失敗:リクエスト失敗", DateTime.Now.ToString(), api.account.nickname, parent_id, child_id));
                        }
                    } else if (reservation.docancel_flag && DateTime.Now > reservation.deleteDate) {
                        //削除（停止）成功しようが失敗しようがフラグを折る（二度以上リトライしないように）
                        ReservationObjects[i].docancel_flag = false;
                        Account a = accountDBHelper.selectItem(new List<int> { reservation.accountDBId })[0];
                        Log.Logger.Info("自動削除用のアカウント取得成功");
                        FrilAPI api = new FrilAPI(a.email,a.password);
                        api = Common.checkFrilAPI(api);
                        //削除を実行する
                        string deleteitemid = reservationDBHelper.selectReservation(new List<int> { reservation.DBId })[0].item_id;
                        Log.Logger.Info("自動削除対象の商品をDBから取得成功");
                        int deleteitemDBId = reservationDBHelper.selectReservation(new List<int> { reservation.DBId })[0].DBId;
                        var family = itemfamilyDBHelper.getItemFamilyFromItemDBId(deleteitemDBId);
                        string parent_id = (family == null ? "" : family.parent_id);
                        string child_id = (family == null ? "" : family.child_id);
                        if (deleteitemid != "") {
                            //現在の商品の状態を取得する
                            FrilItem item = api.GetItemInfobyItemIDWithDetail(deleteitemid);
                            if (item == null) {
                                Log.Logger.Error("商品情報取得失敗により削除(停止)失敗");
                                reservation_fail_logs.Add(string.Format("{0}:{1}:親ID:{2}子ID:{3}:削除失敗:削除対象の商品情報取得失敗", DateTime.Now.ToString(), api.account.nickname, parent_id, child_id));
                                delete_failed_num++;
                            } else {
                                if (reservation.consider_comment && item.num_comments > 0) {
                                    Log.Logger.Info("コメントが付いているので削除(停止)しない");
                                    //continue; なんども実行されることになるのでコメントがついてるので削除(停止)しないならそれでもうおわり
                                } else if (reservation.consider_favorite && item.num_likes > 0) {
                                    Log.Logger.Info("いいねが付いているので削除(停止)しない");
                                    //continue; なんども実行されることになるのでいいねがついてるので削除(停止)しないならそれでもうおわり
                                } else {
                                    if (this.isStopCheckBox.Checked == false) {
                                        bool result = api.Cancel(deleteitemid);
                                        if (result) {
                                            Log.Logger.Info("削除成功 : " + deleteitemid);
                                            //削除・停止後の共通DB操作を行う
                                            ExhibitService.updateDBOnCancelOrStop(item);
                                            delete_success_num++;
                                        } else {
                                            Log.Logger.Error("削除失敗 : " + deleteitemid);
                                            delete_failed_num++;
                                            if (a.expiration_date < DateTime.Now) reservation_fail_logs.Add(string.Format("{0}:{1}:親ID:{2}子ID:{3}:削除失敗:トークン有効期限切れ", DateTime.Now.ToString(), api.account.nickname, parent_id, child_id));
                                            else reservation_fail_logs.Add(string.Format("{0}:{1}:親ID:{2}子ID:{3}:削除失敗:リクエスト失敗", DateTime.Now.ToString(), api.account.nickname, parent_id, child_id));
                                        }
                                    } else {
                                        bool result = api.Stop(deleteitemid);
                                        if (result) {
                                            Log.Logger.Info("停止成功 : " + deleteitemid);
                                            //削除・停止後の共通DB操作を行う
                                            ExhibitService.updateDBOnCancelOrStop(item);
                                            delete_success_num++;
                                        } else {
                                            Log.Logger.Error("停止失敗 : " + deleteitemid);
                                            delete_failed_num++;
                                            if (a.expiration_date < DateTime.Now) reservation_fail_logs.Add(string.Format("{0}:{1}:親ID:{2}子ID:{3}:停止失敗:トークン有効期限切れ", DateTime.Now.ToString(), api.account.nickname, parent_id, child_id));
                                            else reservation_fail_logs.Add(string.Format("{0}:{1}:親ID:{2}子ID:{3}:停止失敗:リクエスト失敗", DateTime.Now.ToString(), api.account.nickname, parent_id, child_id));
                                        }
                                    }
                                }
                            }
                        } else {
                            //商品ID不明
                            Log.Logger.Error("商品ID不明により削除失敗");
                            reservation_fail_logs.Add(string.Format("{0}:{1}:親ID:{2}子ID:{3}:削除(停止)失敗:商品ID不明", DateTime.Now.ToString(), api.account.nickname, parent_id, child_id));
                            delete_failed_num++;
                        }
                        bgWorker.ReportProgress(i * 100 / ReservationObjects.Count); //GUI更新リクエスト
                        //いいね考慮とか
                    } else if (reservation.docancel_flag2 && DateTime.Now > reservation.deleteDate2) {
                        //削除（停止）成功しようが失敗しようがフラグを折る（二度以上リトライしないように）
                        ReservationObjects[i].docancel_flag2 = false;
                        Account a = accountDBHelper.selectItem(new List<int> { reservation.accountDBId })[0];
                        Log.Logger.Info("自動削除2用のアカウント取得成功");
                        FrilAPI api = new FrilAPI(a.email,a.password);
                        api = Common.checkFrilAPI(api);
                        //削除を実行する
                        string deleteitemid = reservationDBHelper.selectReservation(new List<int> { reservation.DBId })[0].item_id;
                        Log.Logger.Info("自動削除2対象の商品をDBから取得成功");
                        int deleteitemDBId = reservationDBHelper.selectReservation(new List<int> { reservation.DBId })[0].DBId;
                        var family = itemfamilyDBHelper.getItemFamilyFromItemDBId(deleteitemDBId);
                        string parent_id = (family == null ? "" : family.parent_id);
                        string child_id = (family == null ? "" : family.child_id);
                        if (deleteitemid != "") {
                            //現在の商品の状態を取得する
                            FrilItem item = api.GetItemInfobyItemIDWithDetail(deleteitemid);
                            if (item == null) {
                                Log.Logger.Error("商品情報取得失敗により削除2(停止)失敗");
                                delete_failed_num++;
                            } else {
                                if (reservation.consider_comment2 && item.num_comments > 0) {
                                    Log.Logger.Info("コメントが付いているので削除2(停止)しない");
                                    //continue; なんども実行されることになるのでコメントがついてるので削除(停止)しないならそれでもうおわり
                                } else if (reservation.consider_favorite2 && item.num_likes > 0) {
                                    Log.Logger.Info("いいねが付いているので削除2(停止)しない");
                                    //continue; なんども実行されることになるのでいいねがついてるので削除(停止)しないならそれでもうおわり
                                } else {
                                    if (this.isStopCheckBox.Checked == false) {
                                        bool result = api.Cancel(deleteitemid);
                                        if (result) {
                                            Log.Logger.Info("削除2成功 : " + deleteitemid);
                                            //削除・停止後の共通DB操作を行う
                                            ExhibitService.updateDBOnCancelOrStop(item);
                                            delete_success_num++;
                                        } else {
                                            Log.Logger.Error("削除2失敗 : " + deleteitemid);
                                            delete_failed_num++;
                                            if (a.expiration_date < DateTime.Now) reservation_fail_logs.Add(string.Format("{0}:{1}:親ID:{2}子ID:{3}:削除2失敗:トークン有効期限切れ", DateTime.Now.ToString(), api.account.nickname, parent_id, child_id));
                                            else reservation_fail_logs.Add(string.Format("{0}:{1}:親ID:{2}子ID:{3}:削除2失敗:リクエスト失敗", DateTime.Now.ToString(), api.account.nickname, parent_id, child_id));
                                        }
                                    } else {
                                        bool result = api.Stop(deleteitemid);
                                        if (result) {
                                            Log.Logger.Info("停止2成功 : " + deleteitemid);
                                            //削除・停止後の共通DB操作を行う
                                            ExhibitService.updateDBOnCancelOrStop(item);
                                            delete_success_num++;
                                        } else {
                                            Log.Logger.Error("停止2失敗 : " + deleteitemid);
                                            delete_failed_num++;
                                            if (a.expiration_date < DateTime.Now) reservation_fail_logs.Add(string.Format("{0}:{1}:親ID:{2}子ID:{3}:停止2失敗:トークン有効期限切れ", DateTime.Now.ToString(), api.account.nickname, parent_id, child_id));
                                            else reservation_fail_logs.Add(string.Format("{0}:{1}:親ID:{2}子ID:{3}:停止2失敗:リクエスト失敗", DateTime.Now.ToString(), api.account.nickname, parent_id, child_id));
                                        }
                                    }
                                }
                            }
                        } else {
                            //商品ID不明
                            Log.Logger.Error("商品ID不明により削除2失敗");
                            delete_failed_num++;
                        }
                        bgWorker.ReportProgress(i * 100 / ReservationObjects.Count); //GUI更新リクエスト
                    }
                } catch (Exception ex) {
                    if (reservation != null && reservation.doexhibit_flag) {
                        exhibit_failed_num++;
                        reservation_fail_logs.Add(string.Format("{0}:出品失敗:予期せぬエラー", DateTime.Now.ToString()));
                    }
                    if (reservation != null && (reservation.docancel_flag || reservation.docancel_flag2)) {
                        delete_failed_num++;
                        reservation_fail_logs.Add(string.Format("{0}:削除(停止)失敗:予期せぬエラー", DateTime.Now.ToString()));
                    }
                    Log.Logger.Error("自動出品ループ内で想定外のエラー発生" + ex.Message);
                }
            }
        }
        //商品名に文字列を付加したり、空白文字列をいれて出品する
        private FrilItem SellWithOption(Account a, FrilItem item) {
            FrilItem cloneItem = item.Clone();
            FrilAPI api = new FrilAPI(a.email,a.password);
            api = Common.checkFrilAPI(api);
            if (a.addSpecialTextToItemName) {
                //商品名に文字列を付加する
                string[] cloneItemNameHeaders = Settings.getItemNameHeaderList().ToArray();
                if (cloneItemNameHeaders.Length > 0) {
                    string header_str = cloneItemNameHeaders[Common.random.Next(cloneItemNameHeaders.Length)];
                    cloneItem.item_name = header_str + cloneItem.item_name;
                }
            }
            if (a.insertEmptyStrToItemName) {
                //商品名のランダムな位置に空白を挿入する
                int len = cloneItem.item_name.Length;
                int insertIndex = Common.random.Next(len);
                cloneItem.item_name = cloneItem.item_name.Insert(insertIndex, " ");
            }

            return api.Sell(cloneItem, cloneItem.imagepaths);
        }

        private void ReservationbackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            //GUI更新
            this.ReservationDataGridView.Refresh();
            this.exhibit_success_num_label.Text = this.exhibit_success_num.ToString();
            this.exhibit_failed_num_label.Text = this.exhibit_failed_num.ToString();
            this.delete_success_num_label.Text = this.delete_success_num.ToString();
            this.delete_failed_num_label.Text = this.delete_failed_num.ToString();
            this.zaikogire_num_label.Text = this.zaikogire_num.ToString();
            this.reexhibit_success_num_label.Text = this.reexhibit_success_num.ToString();
            this.reexhibit_failed_num_label.Text = this.reexhibit_failed_num.ToString();
        }

        private void ReservationbackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {

        }

        private void accountListComboBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((FrilAPI)e.ListItem).account.nickname;
        }
    }
}
