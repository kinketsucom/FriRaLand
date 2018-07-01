using RakuLand.DBHelper;
using RakuLand.Forms;
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
using static RakuLand.Common;
using System.Diagnostics;

namespace RakuLand {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private List<FrilItem> LocalItemDataBindList = new List<FrilItem>();
        public const string Key_LicenseKey = "LicenseKey";
        public const string Registry_Path = @"HKEY_CURRENT_USER\Software\Rakuland";
        public const string ProductName = "RakuLand";
        private List<ReservationSettingForm.ReservationSetting> ReservationDataBindList = new List<ReservationSettingForm.ReservationSetting>();
        private List<FrilItem> ExhibittedItemDataBindList = new List<FrilItem>(); //表にバインドする商品リスト 絞り込み結果はこっち
        private List<FrilItem> ExhibittedItemDataBackBindList = new List<FrilItem>(); //こっちは絞り込んでも減らない

        public const string ProductKind = "RakuLand-sima";
        System.Random random = new System.Random();
        private List<FrilAPI> FrilAPIList = new List<FrilAPI>();
        private Dictionary<string, FrilAPI> sellerIDtoAPIDictionary = new Dictionary<string, FrilAPI>(); //sellerid -> API

        private async void MainForm_Load(object sender, EventArgs e) {
            AdjustLayout();
            //初回起動(キーがなければ起動時刻+7日をレジストリに書き込み）
            string stringValue = (string)Microsoft.Win32.Registry.GetValue(MainForm.Registry_Path, "Expire", "");
            string datestr = DateTime.Now.AddDays(7).ToString();
            if (string.IsNullOrEmpty(stringValue)) Microsoft.Win32.Registry.SetValue(Registry_Path, "Expire", datestr);
            
            //new ItemRegisterForm().Show();
            FrilItemDBHelper DBhelper = new FrilItemDBHelper();
            DBhelper.onCreate();
            AccountDBHelper accountDBHelper = new AccountDBHelper();
            accountDBHelper.onCreate();
            ReservationDBHelper reservationDBhelper = new ReservationDBHelper();
            reservationDBhelper.onCreate();
            accountDBHelper.addKengai_ExhibitCnt_LastExhibitTime_Column();//3->4
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
            string LicenseKey = (string)Microsoft.Win32.Registry.GetValue(Registry_Path, Key_LicenseKey, "");
            bool showflag = false;
            if (string.IsNullOrEmpty(LicenseKey)) {
                //初回起動は必ずライセンス画面表示
                showflag = true;
            } else {
                //ライセンスチェックしてダメならライセンス画面表示
                if (!LicenseForm.checkCanUseWithErrorWindow()) showflag = true;
            }
            if (showflag) {
                //ライセンス画面をだす コントロール使えないように
                this.tabControl1.Enabled = false;
                LicenseForm lf = new LicenseForm(this);
                lf.Show();
            }
            //カテゴリなどの基本的なマスタデータ読み込み
            if (await Task.Run(() => FrilCommon.init()) == false) {
                MessageBox.Show("ラクマからデータの読み込みに失敗しました.プログラムを終了します.\nインターネット環境を確認してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            //ライセンスが認証できていれば更新情報・お知らせ情報を取得する
            /*if (this.tabControl1.Enabled == true) {
                //新しいバージョンまたは新しいお知らせがあれば表示する
                var form = new startUpWindow();
                if (startUpWindow.notifyFlag) form.Show();
            }*/
        }
        public void unlockLicense() {
            this.tabControl1.Enabled = true;
        }
        private void AdjustLayout() {
            //レイアウトの自動調整をする
            this.ExhibittedDataGridView.Width = this.Width - this.ExhibittedDataGridView.Left - 20;
            this.ExhibittedDataGridView.Height = this.Height - this.ExhibittedDataGridView.Top - this.statusStrip1.Height - 40;
            this.LocalItemDataGridView.Width = this.Width - this.LocalItemDataGridView.Left - 20;
            this.LocalItemDataGridView.Height = this.Height - this.LocalItemDataGridView.Top - this.statusStrip1.Height - 40;
            this.ReservationDataGridView.Width = this.Width - this.ReservationDataGridView.Left - 20;
            this.ReservationDataGridView.Height = this.Height - this.ReservationDataGridView.Top - this.statusStrip1.Height - 40;
            this.DailyExhibitDataGridView.Width = this.Width - this.DailyExhibitDataGridView.Left - 20;
            this.DailyExhibitDataGridView.Height = this.Height - this.DailyExhibitDataGridView.Top - this.statusStrip1.Height - 40;
            this.tabControl1.Height = this.Height - this.tabControl1.Top - this.statusStrip1.Height;
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
                //FIXMEOPTION:商品説明も表示
                //ExhibittedDataGridView.Columns["ExhibittedDataGridView_description"].Visible = true;
                //購入者氏名は非表示
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_buyer_simei"].Visible = false;
                //いいねとコメント表示
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_favorite"].Visible = true;
                ExhibittedDataGridView.Columns["ExhibittedDataGridView_comment"].Visible = true;
                //FIXMEOPTION:購入者コメント時間,出品者コメント時間を表示
                //ExhibittedDataGridView.Columns["ExhibittedDataGridView_comment_time_seller"].Visible = true;
                //ExhibittedDataGridView.Columns["ExhibittedDataGridView_comment_time_buyer"].Visible = true;
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
                f.apilist = FrilAPIList;
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
                this.sellerIDtoAPIDictionary[a.userId] = api;
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
            } catch(Exception ex) {
                StackTrace trace = new StackTrace(ex, true); //第二引数のtrueがファイルや行番号をキャプチャするため必要
                foreach (var frame in trace.GetFrames()) {
                    Console.WriteLine(frame.GetFileName());     //filename
                    Console.WriteLine(frame.GetFileLineNumber());   //line number
                }

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
                      
                        string result_item_id = api.Sell(item, api.account.cc);
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
                                if (reservation.consider_comment && item.comments_count > 0) {
                                    Log.Logger.Info("コメントが付いているので削除(停止)しない");
                                    //continue; なんども実行されることになるのでコメントがついてるので削除(停止)しないならそれでもうおわり
                                } else if (reservation.consider_favorite && item.likes_count > 0) {
                                    Log.Logger.Info("いいねが付いているので削除(停止)しない");
                                    //continue; なんども実行されることになるのでいいねがついてるので削除(停止)しないならそれでもうおわり
                                } else {
                                    if (this.isStopCheckBox.Checked == false) {
                                        bool result = api.Cancel(deleteitemid,api.account);
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
                                if (reservation.consider_comment2 && item.comments_count > 0) {
                                    Log.Logger.Info("コメントが付いているので削除2(停止)しない");
                                    //continue; なんども実行されることになるのでコメントがついてるので削除(停止)しないならそれでもうおわり
                                } else if (reservation.consider_favorite2 && item.likes_count > 0) {
                                    Log.Logger.Info("いいねが付いているので削除2(停止)しない");
                                    //continue; なんども実行されることになるのでいいねがついてるので削除(停止)しないならそれでもうおわり
                                } else {
                                    if (this.isStopCheckBox.Checked == false) {
                                        bool result = api.Cancel(deleteitemid,api.account);
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
            FrilAPI api = new FrilAPI(a);
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
           
            item.item_id = api.Sell(cloneItem, api.account.cc);
            return item;
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

        private void get_selling_button_Click(object sender, EventArgs e) {
            //if (!LicenseForm.checkCanUseWithErrorWindow()) return;
            var selectedAPIs = getNowSelectedAPIs();
            if (selectedAPIs.Count == 0) return;
            string get_item_type = "selling";
            UpdateSelling(selectedAPIs,get_item_type);
        }
        private List<FrilAPI> getNowSelectedAPIs() {
            //アカウントがDB上になくなにも選択されていないときは空リストかえす
            List<FrilAPI> rst = new List<FrilAPI>();
            if (this.radioButton1.Checked) {
                if (this.accountListComboBox.SelectedItem != null) rst.Add((FrilAPI)this.accountListComboBox.SelectedItem);

            } 
            //こちらグループアカウント処理なのでいまはコメントアウト
            //else if (this.radioButton2.Checked) {
            //    if (this.groupListComboBox.SelectedItem != null) {
            //        Group g = (Group)this.groupListComboBox.SelectedItem;
            //        foreach (FrilAPI api in g.apiList) rst.Add(api);
            //    }
            //}
            return rst;
        }
        private async void UpdateSelling(List<FrilAPI> apis,string get_item_type) {
            this.toolStripStatusLabel1.Text = "商品情報取得開始";
            DisableAllButton();
            var items = await Task.Run(() => DoGetSelling(apis, get_item_type, this.detailInfoGetCheckBox.Checked));
            if (items == null) {
                EnableAllButton();
                return;
            }
            ExhibittedItemDataBindList.Clear();
            ExhibittedItemDataBackBindList.Clear();
            foreach (var item in items) {
                item.is_sellitem = true;
                ExhibittedItemDataBindList.Add(item);
                ExhibittedItemDataBackBindList.Add(item);
            }
            this.toolStripStatusLabel1.Text = "商品情報取得完了 : " + items.Count.ToString() + "件";
            ExhibittedDataGridView.RowCount = ExhibittedItemDataBindList.Count;
            ExhibittedDataGridView.ClearSelection();
            ExhibittedDataGridView.Refresh();
            ToggleExhibittedDataGridView(true);
            EnableAllButton();
        }
        private void DisableAllButton() {
            //this.ShiboriButton.Enabled = false;
            panel1.Enabled = panel2.Enabled = panel3.Enabled = false;
            this.LocalItemDataGridView.Enabled = this.ReservationDataGridView.Enabled = this.ExhibittedDataGridView.Enabled = false;
            accountListComboBox.Enabled = false;
        }
        private void EnableAllButton() {
            //this.ShiboriButton.Enabled = true;
            panel1.Enabled = panel2.Enabled = panel3.Enabled = true;
            this.LocalItemDataGridView.Enabled = this.ReservationDataGridView.Enabled = this.ExhibittedDataGridView.Enabled = true;
            accountListComboBox.Enabled = true;
        }
        private List<FrilItem> DoGetSelling(List<FrilAPI> apis,string get_item_type, bool detailflag = false) {
            //出品中の商品を取得する
            List<FrilItem> items = new List<FrilItem>();
            var itemNoteDBHelper = new ItemNoteDBHelper();
         
            foreach (var api in apis) {
                var api2 = Common.checkFrilAPI(api);
                var user_items = api2.getSellingItem(api2.account.sellerid,get_item_type,api2.account.cc);
                //var user_items = api2.GetAllItemsWithSellers(api2.account.sellerid, new List<int> { 1 });
                if (detailflag) {
                    //購入者コメント時間と出品者コメント時間を取得
                    for (int i = 0; i < user_items.Count; i++) {
                        var comments = api2.GetComments(user_items[i].item_id);
                        //一番新しいものを取得する
                        foreach (var comment in comments) {
                            if (comment.user_id == api2.account.userId) {
                                if (user_items[i].seller_comment_time < comment.created_at) user_items[i].seller_comment_time = comment.created_at;
                            } else {
                                if (user_items[i].buyer_comment_time < comment.created_at) user_items[i].buyer_comment_time = comment.created_at;
                            }
                        }
                    }
                }
                //商品備考をセットする
                var itemnoteDictionary = itemNoteDBHelper.loadItemNotesDictionary();
                for (int i = 0; i < user_items.Count; i++) {
                    if (itemnoteDictionary.ContainsKey(user_items[i].item_id)) {
                        var note = itemnoteDictionary[user_items[i].item_id];
                        user_items[i].bikou = note.bikou;
                        user_items[i].address_copyed = note.address_copyed;
                    } else {
                        user_items[i].bikou = "";
                        user_items[i].address_copyed = false;
                    }
                }
                
                items.AddRange(user_items);
            }
            return items;
        }

        private void ExhibittedDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            try {
                if (ExhibittedItemDataBindList.Count <= e.RowIndex) return;
                FrilItem item = ExhibittedItemDataBindList[e.RowIndex];
                if (item.Image == null) item.loadImageFromThumbnail();
                //出品中,停止中
                switch (e.ColumnIndex) {
                    case 0:
                        e.Value = item.Image;
                        break;
                    case 1:
                        e.Value = item.status_message;
                        //switch (item.t_status) {
                        //    case 3:
                        //        e.Value += "(決済済)";
                        //        break;
                        //    case 4:
                        //        e.Value += "(発送通知済)";
                        //        break;
                        //    case 5:
                        //        e.Value += "(評価受理済)";
                        //        break;
                        //}
                        break;
                    case 2:
                        e.Value = item.screen_name;
                        break;
                    case 3:
                        e.Value = item.item_name;
                        break;
                    case 4:
                        e.Value = item.detail;
                        break;
                    case 5:
                        e.Value = item.s_price;
                        break;
                    case 6:
                        e.Value = item.likes_count;
                        break;
                    case 7:
                        e.Value = item.comments_count;
                        break;
                    case 8:
                        e.Value = (item.item_pv >= 0) ? item.item_pv.ToString() : "";
                        break;
                    case 9:
                        e.Value = item.created_str;
                        break;
                    case 10:
                        e.Value = (item.buyer_comment_time != Common.UNIX_EPOCH) ? item.buyer_comment_time.ToString() : "";
                        break;
                    case 11:
                        e.Value = (item.seller_comment_time != Common.UNIX_EPOCH) ? item.seller_comment_time.ToString() : "";
                        break;
                    case 12:
                        e.Value = (item.transaction_message_num >= 0) ? item.transaction_message_num.ToString() : "";
                        break;
                    case 13:
                        e.Value = (item.buyer_transaction_message_time != Common.UNIX_EPOCH) ? item.buyer_transaction_message_time.ToString() : "";
                        break;
                    case 14:
                        e.Value = (item.seller_transaction_message_time != Common.UNIX_EPOCH) ? item.seller_transaction_message_time.ToString() : "";
                        break;
                    case 15:
                        e.Value = item.bikou;
                        break;
                    case 16:
                        e.Value = item.address_copyed;
                        break;
                    case 17:
                        e.Value = item.buyer_simei;
                        break;
                    case 18:
                        e.Value = item.buyer_name;
                        break;
                    case 19:
                        e.Value = item.item_id;
                        break;
                }
            } catch(Exception ex) {
               Dev.printE(ex);
            }
        }

        private async void CheckItemAllCancel_Click(object sender, EventArgs e) {
           //if (!LicenseForm.checkCanUseWithErrorWindow()) return;
            //選択商品の一括取り消し
            DialogResult dr = MessageBox.Show("選択商品の一括取り消しを行いますか？", MainForm.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No) {
                return;
            }
            this.toolStripStatusLabel1.Text = "一括取り消し開始";
            DisableAllButton();
            var res = await Task.Run(() => DoAllCancel());
            //表の更新(成功0個のときは更新不要）
            int successnum = res.Value; string message = res.Key;
            //結果を表示
            MessageBox.Show(message, "一括取り消し", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //出品一覧更新
            var selectedAPIs = getNowSelectedAPIs();
            string get_item_type = "selling";
            if (selectedAPIs.Count != 0) UpdateSelling(selectedAPIs,get_item_type);
            this.toolStripStatusLabel1.Text = "一括取り消し完了(" + message + ")";
            ExhibittedDataGridView.RowCount = ExhibittedItemDataBindList.Count;
            ExhibittedDataGridView.ClearSelection();
            ExhibittedDataGridView.Refresh();
            EnableAllButton();
        }
        private KeyValuePair<string, int> DoAllCancel() {
            int trynum = 0;
            int successnum = 0;
            var itemNoteDBHelper = new ItemNoteDBHelper();
            var shuppinrirekiDBHelper = new ShuppinRirekiDBHelper();
            foreach (DataGridViewRow row in ExhibittedDataGridView.SelectedRows) {
                FrilItem item = ExhibittedItemDataBindList[row.Index];
                trynum++;
                //取引中の商品は取り消した場合エラーになる
                //商品に該当するapiを使用して商品を取り消す
                if (!item.is_sellitem) continue;//購入した商品の場合は出品取消できない
                bool res = false;
                if (sellerIDtoAPIDictionary.ContainsKey(item.user_id)) {
                    var api = sellerIDtoAPIDictionary[item.user_id];
                    var api2 = Common.checkFrilAPI(api);
                    res = api2.Cancel(item.item_id,api.account);
                    //出品履歴,商品備考データが存在する場合は該当レコードを削除
                    itemNoteDBHelper.deleteItemNote(item.item_id);
                    shuppinrirekiDBHelper.deleteRireki(item.item_id);
                }
                if (res) successnum++;
                this.toolStripStatusLabel1.Text = "取り消し完了:" +item.item_name;
                System.Threading.Thread.Sleep(Settings.getIkkatuTorikesiInterval() * 1000);
            }
            return new KeyValuePair<string, int>(string.Format("成功: {0} 失敗:{1}", successnum, trynum - successnum), successnum);
        }

        private void SelectItemAllExhibitButtonFromReservationTab_Click(object sender, EventArgs e) {
            //if (!LicenseForm.checkCanUseWithErrorWindow()) return;
            if (checkNowAutoMode()) return;
            //選択商品の一括出品を行う
            //確認画面を表示
            DialogResult dr = MessageBox.Show("選択商品の一括出品を行いますか？\n注:画面左上で選択したアカウントではなく予約で設定されているアカウントから出品します", MainForm.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No) {
                return;
            }
            this.toolStripStatusLabel1.Text = "一括出品開始";
            exhibit_success_num = exhibit_failed_num = 0;
            this.exhibit_success_num_label.Text = "0";
            this.exhibit_failed_num_label.Text = "0";
            DisableAllButton();
            this.backgroundWorker2.RunWorkerAsync();
        }

        BackgroundWorker bgWorker2;
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e) {
            bgWorker2 = (BackgroundWorker)sender;
            exhibit_success_num = exhibit_failed_num = 0;
            FrilItemDBHelper itemDBHelper = new FrilItemDBHelper();
            AccountDBHelper accountDBHelper = new AccountDBHelper();
            ShuppinRirekiDBHelper shuppinRirekiDBHelper = new ShuppinRirekiDBHelper();
            ZaikoDBHelper zaikoDBHelper = new ZaikoDBHelper();
            ItemFamilyDBHelper itemfamilyDBHelper = new ItemFamilyDBHelper();
            ExhibitLogDBHelper exhibitLogDBHelper = new ExhibitLogDBHelper();
            int num = 0;
            foreach (DataGridViewRow row in ReservationDataGridView.SelectedRows) {
                var reservation = ReservationDataBindList[row.Index];
                FrilItem item = itemDBHelper.selectItem(new List<int> { reservation.itemDBId })[0];
                Account a = accountDBHelper.selectItem(new List<int> { reservation.accountDBId })[0];
                FrilAPI api = new FrilAPI(a);
                int zaikonum = zaikoDBHelper.getZaikoNum(item.parent_id);
                bool parent_exist = zaikoDBHelper.isexistParentid(item.parent_id);
                //親IDが存在し、在庫が0なら出品しない
                if (parent_exist && zaikonum <= 0) {
                    exhibit_failed_num++;
                    continue;
                }
                api = Common.checkFrilAPI(api);
                //this.toolStripStatusLabel1.Text = ("出品中: " + item.name);
                FrilItem res = SellWithOption(a, item);
                if (res != null) {
                    var sr = new ShuppinRirekiDBHelper.ShuppinRireki();
                    sr.itemDBId = item.DBId;
                    sr.item_id = res.item_id;
                    sr.accountDBId = a.DBId;
                    shuppinRirekiDBHelper.addShuppinRireki(sr);
                    exhibit_success_num++;
                    //出品ログを追加
                    var itemfamily = itemfamilyDBHelper.getItemFamilyFromItemDBId(item.DBId);
                    exhibitLogDBHelper.addExhibitLog(api.account.nickname, item.created_date, itemfamily, res.item_id);
                    //GUIだけ更新してDBには入れない
                    ReservationDataBindList[row.Index].exhibit_status_str = ReservationSettingForm.get_exhibit_status_str(ReservationSettingForm.Status.Success);
                } else {
                    exhibit_failed_num++;
                }
                num++;
                bgWorker2.ReportProgress(num * 100 / ReservationDataGridView.SelectedRows.Count); //GUI更新リクエスト
                System.Threading.Thread.Sleep(Settings.getIkkatuShuppinInterval() * 1000);
            }
        }
        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            this.toolStripStatusLabel1.Text = string.Format("一括出品完了(成功{0}, 失敗{1})", exhibit_success_num, exhibit_failed_num);
            EnableAllButton();
        }
        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            this.ReservationDataGridView.Refresh();
            this.exhibit_success_num_label.Text = exhibit_success_num.ToString();
            this.exhibit_failed_num_label.Text = exhibit_failed_num.ToString();
        }

        private void get_trading_button_Click(object sender, EventArgs e) {
            //if (!LicenseForm.checkCanUseWithErrorWindow()) return;
            var selectedAPIs = getNowSelectedAPIs();
            if (selectedAPIs.Count == 0) return;
            string get_item_type = "trading";
            UpdateSelling(selectedAPIs, get_item_type);
            //UpdateTrading(selectedAPIs,get_item_type);
        }
        private async void UpdateTrading(List<FrilAPI> apis) {
            this.toolStripStatusLabel1.Text = "取引中商品取得開始";
            DisableAllButton();
            var items = await Task.Run(() => DoGetTrading(apis, this.detailInfoGetCheckBox.Checked));
            if (items == null) {
                EnableAllButton();
                return;
            }
            ExhibittedItemDataBindList.Clear();
            ExhibittedItemDataBackBindList.Clear();
            foreach (var item in items) {
                item.is_sellitem = true;
                ExhibittedItemDataBindList.Add(item);
                ExhibittedItemDataBackBindList.Add(item);
            }
            this.toolStripStatusLabel1.Text = "取引中商品取得完了 : " + items.Count.ToString() + "件";
            ExhibittedDataGridView.RowCount = ExhibittedItemDataBindList.Count;
            ExhibittedDataGridView.ClearSelection();
            ExhibittedDataGridView.Refresh();
            EnableAllButton();
            ToggleExhibittedDataGridView(false);
            ReloadLocalItem("");
        }
        private List<FrilItem> DoGetTrading(List<FrilAPI> apis, bool detailflag = false) {
            //出品履歴を取得
            var shuppinrirekiDBHelper = new ShuppinRirekiDBHelper();
            var zaikoDBHelper = new ZaikoDBHelper();
            var shuppinrirekiDic = shuppinrirekiDBHelper.loadRirekiDictionary();
            var itemParentDic = new ItemFamilyDBHelper().getItemIdToParentIdDictionary();
            var itemNoteDBHelper = new ItemNoteDBHelper();
            //取引中の商品を取得する
            List<FrilItem> items = new List<FrilItem>();
            foreach (var api in apis) {
                var api2 = Common.checkFrilAPI(api);
                var user_items = api2.GetAllItemsWithSellers(api2.account.sellerid, new List<int> { 2 });
                if (detailflag) {
                    //購入者取引メッセージ時間と出品者取引メッセージ時間を取得
                    for (int i = 0; i < user_items.Count; i++) {
                        var comments = api2.GetTransactionMessages(user_items[i].item_id);
                        //一番新しいものを取得する
                        foreach (var comment in comments) {
                            if (comment.user_id == api2.account.sellerid) {
                                if (user_items[i].seller_transaction_message_time < comment.created_at) user_items[i].seller_transaction_message_time = comment.created_at;
                            } else {
                                if (user_items[i].buyer_transaction_message_time < comment.created_at) user_items[i].buyer_transaction_message_time = comment.created_at;
                            }
                        }
                        user_items[i].transaction_message_num = comments.Count;
                        //購入者氏名を調べる
                        var info = api2.GetTransactionInfo(user_items[i].item_id);
                        if (string.IsNullOrEmpty(info.buyername) == false) user_items[i].buyer_simei = info.buyername;
                    }

                }
                //商品備考をセットする
                var itemnoteDictionary = itemNoteDBHelper.loadItemNotesDictionary();
                for (int i = 0; i < user_items.Count; i++) {
                    if (itemnoteDictionary.ContainsKey(user_items[i].item_id)) {
                        var note = itemnoteDictionary[user_items[i].item_id];
                        user_items[i].bikou = note.bikou;
                        user_items[i].address_copyed = note.address_copyed;
                    } else {
                        user_items[i].bikou = "";
                        user_items[i].address_copyed = false;
                    }
                }
                items.AddRange(user_items);
                var accountDBHelper = new AccountDBHelper();
                var exhibitLogDBHelper = new ExhibitLogDBHelper();
                var itemfamilyDBHelper = new ItemFamilyDBHelper();
                //各商品について売れていた場合の処理を実行
                foreach (var item in user_items) {
                    ExhibitService.updateDBOnSold(api2, item);
                }
                //shuppinrirekiDBHelper.updateShuppinRireki(uretaItemIDList, true);
            }
            return items;
        }

        private void ExhibittedDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            //出品中商品の場合はダブルクリックでコメントを開く
            if (this.ExhibittedDataGridView.SelectedRows.Count == 1) {
                int index = this.ExhibittedDataGridView.SelectedRows[0].Index;
                FrilItem item = this.ExhibittedItemDataBindList[index];
                if (item.item_status_in_fril == "selling") {//TODO:t_statusに変えるほうが安心
                    if (sellerIDtoAPIDictionary.ContainsKey(item.user_id.ToString())) {//自分なのでseller.id = userid
                        var api = sellerIDtoAPIDictionary[item.user_id.ToString()];
                        var api2 = Common.checkFrilAPI(api);
                        CommentForm f = new CommentForm(api2, item.item_id, this);
                        f.Show();
                    }
                } else if (item.item_status_in_fril == "trading") {
                    var api = sellerIDtoAPIDictionary[item.user_id.ToString()];
                    var api2 = Common.checkFrilAPI(api);
                    TransactionMessageForm f = new TransactionMessageForm(api2, item.item_id);
                    f.Show();
                }
            }
        }

        private void editItemButton_Click(object sender, EventArgs e) {
            if (checkNowAutoMode()) return;
            //if (!LicenseForm.checkCanUseWithErrorWindow()) return;
            List<int> selectIdList = new List<int>();
            foreach (DataGridViewRow row in LocalItemDataGridView.SelectedRows) selectIdList.Add(LocalItemDataBindList[row.Index].DBId);
            if (selectIdList.Count != 1) {
                MessageBox.Show("編集する商品を1つだけ選択してください。");
                return;
            }
            FrilItem item = new FrilItemDBHelper().selectRegisterItem(selectIdList)[0];
            ItemRegisterForm f = new ItemRegisterForm(item, selectIdList[0]);
            f.mainform = this;
            f.apilist = this.FrilAPIList;
            f.Show();
        }

        private async void SelectItemAllExhibitButton_Click(object sender, EventArgs e) {
            //if (!LicenseForm.checkCanUseWithErrorWindow()) return;
            var selectedAPIs = getNowSelectedAPIs();
            if (selectedAPIs.Count != 1) {
                MessageBox.Show("商品を出品するアカウントを選択してください。\nグループは使用できません。", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (checkNowAutoMode()) return;
            //選択商品の一括出品を行う
            //確認画面を表示
            DialogResult dr = MessageBox.Show("選択商品の一括出品を行いますか？", MainForm.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No) {
                return;
            }
            this.toolStripStatusLabel1.Text = "一括出品開始";
            DisableAllButton();
            string result = await Task.Run(() => DoAllExhibitFromLocalItemTab(selectedAPIs));
            //結果を表示
            MessageBox.Show(result, "一括出品", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //一括出品終了後、出品一覧を更新→なし
            //UpdateSelling();
            this.toolStripStatusLabel1.Text = "一括出品完了(" + result + ")";
            EnableAllButton();

        }

        private string DoAllExhibitFromLocalItemTab(List<FrilAPI> apis) {
            int trynum = 0;
            int successnum = 0;
            var shuppinRirekiDBHelper = new ShuppinRirekiDBHelper();
            var zaikoDBHelper = new ZaikoDBHelper();
            ItemFamilyDBHelper itemfamilyDBHelper = new ItemFamilyDBHelper();
            ExhibitLogDBHelper exhibitLogDBHelper = new ExhibitLogDBHelper();
            foreach (DataGridViewRow row in LocalItemDataGridView.SelectedRows) {
                FrilItem item = LocalItemDataBindList[row.Index];
                trynum++;
                int zaikonum = zaikoDBHelper.getZaikoNum(item.parent_id);
                bool parent_exist = zaikoDBHelper.isexistParentid(item.parent_id);
                //親IDが存在し、在庫が0なら出品しない
                //if (parent_exist && zaikonum <= 0) continue;FIFXME:親子IDはまだつくらない
                this.toolStripStatusLabel1.Text = ("出品中: " + item.item_name);
                apis[0] = Common.checkFrilAPI(apis[0]);
                FrilItem res = SellWithOption(apis[0].account, item);
                AccountDBHelper accountDBHelper = new AccountDBHelper();
                if (res != null) {
                    Account a = accountDBHelper.getAccountFromSellerid(apis[0].account.userId);
                    a.exhibit_cnt++;
                    a.last_exhibitTime_str = DateTime.Now.ToString();
                    accountDBHelper.updateAccount(a.DBId, a);
                    successnum++;
                    //出品履歴を追加
                    var sr = new ShuppinRirekiDBHelper.ShuppinRireki();
                    sr.itemDBId = item.DBId;
                    sr.item_id = res.item_id;
                    sr.accountDBId = a.DBId;
                    shuppinRirekiDBHelper.addShuppinRireki(sr);
                    //出品ログを追加
                    var itemfamily = itemfamilyDBHelper.getItemFamilyFromItemDBId(item.DBId);
                    exhibitLogDBHelper.addExhibitLog(apis[0].account.nickname, res.created_date, itemfamily, res.item_id);
                }
                System.Threading.Thread.Sleep(Settings.getIkkatuShuppinInterval() * 1000);
            }
            return string.Format("成功: {0} 失敗:{1}", successnum, trynum - successnum);
        }

        private void バージョン情報ToolStripMenuItem_Click(object sender, EventArgs e) {
            new StartUpForm().Show();
        }



        #region 住所をエクセルに出力
        private async void saveAllDeliveryAddress_Click(object sender, EventArgs e) {
            //if (!LicenseForm.checkCanUseWithErrorWindow()) return;
            //選択商品の一括住所取得
            DialogResult dr = MessageBox.Show("選択商品の一括住所取得をおこないますか？\n※「支払い待ち」状態の商品の住所は取得されません。", MainForm.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No) {
                return;
            }
            bool includeUketoriMachi = true;
            DialogResult dr2 = MessageBox.Show("受け取り待ちの商品も含めて住所取得を行いますか？「はい」を押すと含めます。", MainForm.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr2 == DialogResult.No) {
                includeUketoriMachi = false;
            }
            bool addressCombine = false;
            //FIXME:別の列はまたあとで考える
            //DialogResult dr3 = MessageBox.Show("住所1と住所2を別にしますか？「はい」を押すと別々の列にします。", MainForm.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //if (dr3 == DialogResult.No) {
            //    addressCombine = true;
            //}
            //SaveFileDialogクラスのインスタンスを作成
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "発送先住所.csv";
            sfd.Filter = "CSVファイル(*.csv)|*.csv";
            sfd.Title = "保存先のファイルを選択してください";
            //ダイアログを表示する
            if (sfd.ShowDialog() == DialogResult.OK) {
                string filename = sfd.FileName;
                this.toolStripStatusLabel1.Text = "一括住所取得開始";
                DisableAllButton();
                bool res = await Task.Run(() => DoGetAllDeliveryAddress(filename, includeUketoriMachi, addressCombine));
                //結果を表示
                if (res) MessageBox.Show("保存しました" + filename, "一括住所取得", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else MessageBox.Show("保存に失敗しました", "一括住所取得", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.toolStripStatusLabel1.Text = "一括住所取得完了";
                EnableAllButton();
            }
        }

        private bool DoGetAllDeliveryAddress(string filename, bool includeUketoriMachi, bool addressCombine) {
            try {
                using (var sw = new System.IO.StreamWriter(filename, false, System.Text.Encoding.GetEncoding("shift_jis"))) {
                    //if (addressCombine == false) sw.WriteLine("商品番号, 郵便番号, 住所1, 住所2,商品ID, 氏名, , , 出品者ニックネーム, ,備考, 値段,利益 , 購入者ニックネーム, 商品名,商品ID");
                    //else sw.WriteLine("商品番号, 郵便番号, 住所, 商品ID, 氏名, , , 出品者ニックネーム, ,備考, 値段,利益 , 購入者ニックネーム, 商品名,商品ID");
                    //FIXME:取り出せていない情報はのっけないが後々修正
                    //FIXME:アドレスコンバイン機能はいまはない
                    sw.WriteLine("郵便番号, 住所, 商品ID, 氏名, , , 出品者ニックネーム, , 値段,利益 , 商品名");
                    foreach (DataGridViewRow row in ExhibittedDataGridView.SelectedRows) {
                        FrilItem item = ExhibittedItemDataBindList[row.Index];
                        if (sellerIDtoAPIDictionary.ContainsKey(item.user_id.ToString())) {
                            FrilAPI api = sellerIDtoAPIDictionary[item.user_id.ToString()];
                            api = Common.checkFrilAPI(api);
                            var info = api.GetTransactionInfo(item.item_id);
                            //支払い待ちの商品の住所は取得しない
                            if (item.t_status == 2) continue;
                            //受け取りまち除外オプションがあれば除外
                            if (includeUketoriMachi == false && item.t_status != 3) continue;
                            //string parent_id = new ExhibitLogDBHelper().getParentIDFromExhibitLog(item.itemid);
                            if (addressCombine == false) sw.WriteLine(string.Join(",", Common.makeAddressExcelCSVLine(item, info, api)));
                            else sw.WriteLine(string.Join(",", Common.makeAddressExcelCSVLineCombineAddress(item, info, api)));
                        }
                    }
                }
                return true;
            } catch (Exception ex) {
                return false;
            }
        }

        #endregion

        private void 出金管理ToolStripMenuItem_Click(object sender, EventArgs e) {
            //if (!LicenseForm.checkCanUseWithErrorWindow()) return;
            string nowpass = Settings.getIkkatuShukkinPassword();
            if (string.IsNullOrEmpty(nowpass) == false) {
                string pass = Microsoft.VisualBasic.Interaction.InputBox("パスワードを入力してください", MainForm.ProductName, "");
                if (nowpass != pass) {
                    MessageBox.Show("パスワードが異なります", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            var form = new IkkatuShukkin();
            form.Show();
        }

        private void item_register_from_excel_button_Click(object sender, EventArgs e) {
            new ItemRegisterFromExcelForm(this).Show();
        }

        private void register_from_xml_button_Click(object sender, EventArgs e) {
            new ReservationRegisterFromExcelForm(this).Show();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e) {
            AdjustLayout();
        }
    }

}

