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

namespace FriRaLand.Forms {
    public partial class AccountManageForm : Form {
        public AccountManageForm() {
            InitializeComponent();
        }
        private List<MainForm.Account> accountList = new List<MainForm.Account>();
        public MainForm mainform;
        private void AccountManageForm_Load(object sender, EventArgs e) {
            accountColumnBoxReflesh();
            groupListBoxRefresh();
        }

        static public List<MainForm.Account> accountLoader() {
            return new AccountDBHelper().loadAccounts();
        }
        private void accountColumnBoxReflesh() {
            //ComboBoxクリア
            this.accountListBox2.Items.Clear();
            //アカウントリストクリア
            accountList.Clear();
            accountList = accountLoader();
            foreach (var a in accountList) {
                //accountListBox1.Items.Add(a);
                accountListBox2.Items.Add(a);
            }
            this.accountDataGridView1.DataSource = accountList;
            //DataSource設定したあとにしないと順序狂う
            this.accountDataGridView1.Columns["nickname"].DisplayIndex = 0;
            this.accountDataGridView1.Columns["expiration_date"].DisplayIndex = 1;
            this.accountDataGridView1.Columns["token_update_date"].DisplayIndex = 2;
            this.accountDataGridView1.Columns["exhibit_cnt"].DisplayIndex = 3;
            this.accountDataGridView1.Columns["hanbai_num"].DisplayIndex = 4;
        }

        private void groupListBoxRefresh() {
            this.groupListBox.Items.Clear();
            List<GroupKindDBHelper.GroupKind> gklist = new GroupKindDBHelper().loadGroupKind();
            foreach (var gk in gklist) this.groupListBox.Items.Add(gk);
        }

        private void accountAddButton_Click(object sender, EventArgs e) {
            if (checkTokenRefreshNow()) return;
            //メルカリログイン試行
            MercariAPI m = new MercariAPI();
            bool loginResult = m.tryMercariLogin(emailTextBox.Text.Trim(), passwordTextBox.Text.Trim());
            if (loginResult) {
                //ログイン成功
                MessageBox.Show("ログインに成功しました。", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MainForm.Account account = new MainForm.Account();
                account.email = emailTextBox.Text.Trim();
                account.password = passwordTextBox.Text.Trim();
                account.access_token = m.access_token;
                account.global_access_token = m.global_access_token;
                account.sellerid = m.sellerid;
                account.nickname = m.nickname;
                account.expiration_date = Common.getDateFromUnixTimeStamp(m.expiration_date);
                //既にあったら追加しない
                AccountDBHelper accountDBHelper = new AccountDBHelper();
                if (accountDBHelper.getAccountDBId(account) != 0) {
                    MessageBox.Show("既にアカウントが存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                account.token_update_date = account.expiration_date;
                accountDBHelper.addAccount(account);
                //ComboBoxリフレッシュ
                accountColumnBoxReflesh();
            } else {
                //ログイン失敗
                MessageBox.Show("ログインに失敗しました。\n（同じIPアドレスから連続でログインを試すとエラーが発生することがあります。\n15分おきに2つずつアカウントを登録するとエラーが起きにくいです。）", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void accountDeleteButton_Click(object sender, EventArgs e) {
            if (checkTokenRefreshNow()) return;
            if (accountDataGridView1.SelectedRows.Count <= 0) return;
            //削除確認画面を出す
            DialogResult result = MessageBox.Show("アカウントを一覧から削除しますか?", "質問",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes) {
                foreach (var selectedrow in this.accountDataGridView1.SelectedRows) {
                    AccountDBHelper accountDBHelper = new AccountDBHelper();
                    int delDBId = accountDBHelper.getAccountDBId(accountList[((DataGridViewRow)selectedrow).Index]);
                    //アカウントDBからの削除
                    accountDBHelper.deleteAccount(new List<int> { delDBId });
                    //グループ配属DBからの削除
                    new GroupBelongDBHelper().deleteGroupBelongByAccountID(delDBId);
                    //削除したアカウントに関する出品予約が存在すれば削除する
                    var reservationDBHelper = new ReservationDBHelper();
                    List<ReservationSettingForm.ReservationSetting> reservationList = reservationDBHelper.loadReservations();
                    List<int> deleteReservationIdList = new List<int>();
                    foreach (var reservation in reservationList) if (delDBId == reservation.accountDBId) deleteReservationIdList.Add(reservation.DBId);
                    reservationDBHelper.deleteReservation(deleteReservationIdList);
                }
                //ComboBoxリフレッシュ
                accountColumnBoxReflesh();
            } else if (result == DialogResult.No) {
                return;
            }
        }
        private bool checkTokenRefreshNow() {
            if (this.accountTokenRefleshButton.BackColor == Color.Red) {
                MessageBox.Show("トークン一括更新中です", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            return false;
        }
        private void accountTokenRefleshButton_Click(object sender, EventArgs e) {
            //バックグラウンドで動作中ならキャンセルリクエスト送る
            if (this.accountTokenRefleshButton.BackColor == Color.Red) {
                backgroundWorker1.CancelAsync();
                this.accountTokenRefleshButton.BackColor = Color.Transparent;
                this.accountTokenRefleshButton.Text = "トークン更新";
                return;
            }
            if (accountDataGridView1.SelectedRows.Count > 1) {
                int num = accountDataGridView1.SelectedRows.Count;
                if (MessageBox.Show("アカウントを" + num.ToString() + "件選択しています\n15分おきに2つずつトークンを自動更新します\n開始しますか?", MainForm.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                    if (backgroundWorker1.IsBusy == false) {
                        this.accountTokenRefleshButton.BackColor = Color.Red;
                        this.accountTokenRefleshButton.Text = "更新停止";
                        backgroundWorker1.RunWorkerAsync();
                        return;
                    } else {
                        MessageBox.Show("しばらく待ってから実行してください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

            } else if (accountDataGridView1.SelectedRows.Count <= 0) {
                MessageBox.Show("アカウントを1つ以上選択してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //トークン更新確認画面を出す
            DialogResult result = MessageBox.Show("トークンを更新しますか？最大15秒かかります（エラー発生時以外は行わないことを推奨します）", "質問",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes) {
                AccountDBHelper accountDBHelper = new AccountDBHelper();
                MainForm.Account updateAccount = accountList[this.accountDataGridView1.SelectedRows[0].Index];
                MercariAPI api = new MercariAPI(updateAccount);
                //string[] proxy_list = Common.getProxyServer().ToArray();
                bool ok = false;
                /*if (proxy_list.Length != 0) {
                    for (int trynum = 0; trynum < 5; trynum++) {
                        MercariAPI newapi = Common.checkMercariAPI(api, true, proxy_list[trynum]);
                        if (newapi.access_token != api.access_token) {
                            ok = true;
                            break;
                        }
                    }
                }
                else {*/
                MercariAPI newapi = Common.checkMercariAPI(api, true);
                if (newapi.access_token != api.access_token) ok = true;
                //}
                if (ok) {
                    MessageBox.Show("トークンを更新しました。", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    accountColumnBoxReflesh();
                    return;
                } else {
                    MessageBox.Show("トークンの更新に失敗しました。\n同じIPアドレスから連続でログインを試すとエラーが発生することがあります。15分おきに2つずつアカウントを登録するとエラーが起きにくいです。）", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    accountColumnBoxReflesh();
                }
                return;
            } else if (result == DialogResult.No) {
                return;
            }
        }

        private void AccountManageForm_FormClosed(object sender, FormClosedEventArgs e) {
            this.mainform.Enabled = true;
            this.mainform.OnBackFromAccountManageForm();
        }

        private void emailTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == ',') {
                //カンマは入力できないように
                e.Handled = true;
            }
        }

        private void accountListBox1_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void accountListBox1_Format(object sender, ListControlConvertEventArgs e) {
            MainForm.Account ac = (MainForm.Account)e.ListItem;
            e.Value = ac.nickname;
        }

        private void button1_Click(object sender, EventArgs e) {
            if (checkTokenRefreshNow()) return;
            if (accountDataGridView1.SelectedRows.Count != 1) {
                MessageBox.Show("アカウントを1つだけ選択してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JPGファイル(*.jpg;*.jpeg)|*.jpg;*.jpeg|すべてのファイル(*.*)|*.*";
            ofd.Title = "プロフィール画像を選んでください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK) {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する
                MainForm.Account selectedAccount = accountList[this.accountDataGridView1.SelectedRows[0].Index];
                MercariAPI api = new MercariAPI(selectedAccount);
                string filename = ofd.FileName;
                string path = MercariAPI.getExhibitionImageFromPath(filename);
                bool rst = api.updateProfilePhoto(path);
                if (rst) {
                    MessageBox.Show("プロフィール画像を更新しました。", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } else {
                    MessageBox.Show("プロフィール画像の更新に失敗しました。", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void withdrawbutton_Click(object sender, EventArgs e) {
            if (checkTokenRefreshNow()) return;
            if (accountDataGridView1.SelectedRows.Count != 1) {
                MessageBox.Show("アカウントを1つだけ選択してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MainForm.Account selectedAccount = accountList[this.accountDataGridView1.SelectedRows[0].Index];
            MercariAPI api = new MercariAPI(selectedAccount);
            WithDrawForm f = new WithDrawForm(api);
            f.Show();
        }

        private void accountListBox2_Format(object sender, ListControlConvertEventArgs e) {
            MainForm.Account ac = (MainForm.Account)e.ListItem;
            e.Value = ac.nickname;
        }

        private void addAccountToNewGroup_Click(object sender, EventArgs e) {
            //アカウント一覧から新しくつくるグループに加えるメンバを追加
            if (accountListBox2.SelectedItems.Count <= 0) return;
            //現在すでにメンバにいるやつは追加しない
            Dictionary<int, MainForm.Account> existBelongListBoxDic = new Dictionary<int, MainForm.Account>();
            foreach (MainForm.Account account in GroupBelongListBox.Items) existBelongListBoxDic[account.DBId] = account;
            foreach (MainForm.Account account in accountListBox2.SelectedItems) {
                if (!existBelongListBoxDic.ContainsKey(account.DBId)) {
                    //追加する
                    GroupBelongListBox.Items.Add(account);
                }
            }
            //アカウント一覧の選択を解除
            accountListBox2.ClearSelected();
        }

        private void newGroupListBox_Format(object sender, ListControlConvertEventArgs e) {
            MainForm.Account ac = (MainForm.Account)e.ListItem;
            e.Value = ac.nickname;
        }

        private void deleteAccountToNewGroup_Click(object sender, EventArgs e) {
            if (GroupBelongListBox.SelectedItems.Count <= 0) return;
            //選択されたアカウントを新しく作成するグループのアカウント一覧から削除する
            while (GroupBelongListBox.SelectedItems.Count != 0) {
                GroupBelongListBox.Items.RemoveAt(GroupBelongListBox.SelectedIndices[0]);
            }
        }

        private void groupCreateButton_Click(object sender, EventArgs e) {
            if (checkTokenRefreshNow()) return;
            if (this.groupEditMode == false) {
                //グループ名が空ならアウト
                if (string.IsNullOrEmpty(this.groupNameTextBox.Text)) {
                    MessageBox.Show("グループ名を入力してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //アカウント一覧が空ならアウト
                if (this.GroupBelongListBox.Items.Count <= 0) {
                    MessageBox.Show("1つ以上のアカウントをグループに追加してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                GroupBelongDBHelper groupbelongDBHelper = new GroupBelongDBHelper();
                GroupKindDBHelper groupkindDBHelper = new GroupKindDBHelper();
                //まずグループを追加
                int groupid = groupkindDBHelper.addGroupKind(this.groupNameTextBox.Text.Trim());
                if (groupid < 0) {
                    MessageBox.Show("グループの追加に失敗しました", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //グループ配属情報を追加
                foreach (var item in GroupBelongListBox.Items) {
                    GroupBelongDBHelper.GroupBelong gb = new GroupBelongDBHelper.GroupBelong();
                    gb.AccountID = ((MainForm.Account)item).DBId;
                    gb.GroupID = groupid;
                    groupbelongDBHelper.addGroupBelong(gb);
                }

                MessageBox.Show("グループを作成しました: " + this.groupNameTextBox.Text, MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.groupNameTextBox.Clear();
                this.GroupBelongListBox.Items.Clear();

                //リフレッシュ
                groupListBoxRefresh();
            } else {
                //グループ名が空ならアウト
                if (string.IsNullOrEmpty(this.groupNameTextBox.Text)) {
                    MessageBox.Show("グループ名を入力してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //アカウント一覧が空ならアウト
                if (this.GroupBelongListBox.Items.Count <= 0) {
                    MessageBox.Show("1つ以上のアカウントをグループに追加してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                GroupBelongDBHelper groupbelongDBHelper = new GroupBelongDBHelper();
                GroupKindDBHelper groupkindDBHelper = new GroupKindDBHelper();
                //グループ名を更新
                new GroupKindDBHelper().updateGroupName(this.edittingGroupId, this.groupNameTextBox.Text);
                //グループ配属情報を更新
                new GroupBelongDBHelper().deleteGroupBelongByGroupID(this.edittingGroupId); //一旦配属メンバーすべて削除
                foreach (var item in GroupBelongListBox.Items) {
                    GroupBelongDBHelper.GroupBelong gb = new GroupBelongDBHelper.GroupBelong();
                    gb.AccountID = ((MainForm.Account)item).DBId;
                    gb.GroupID = this.edittingGroupId;
                    groupbelongDBHelper.addGroupBelong(gb);
                }

                MessageBox.Show("グループを更新しました: " + this.groupNameTextBox.Text, MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.groupNameTextBox.Clear();
                this.GroupBelongListBox.Items.Clear();

                //リフレッシュ
                groupListBoxRefresh();
                //更新完了したのでeditモード終了
                this.groupEditMode = false;
                this.groupCreateButton.Text = "作成";
                this.groupCreateButton.BackColor = Color.Transparent;
            }
        }

        private void groupListBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((GroupKindDBHelper.GroupKind)e.ListItem).GroupName;
        }

        private void deleteGroupButton_Click(object sender, EventArgs e) {
            if (!checkGroupEditMode()) return;
            if (checkTokenRefreshNow()) return;
            //グループを選択していなければエラー
            if (this.groupListBox.SelectedItems.Count <= 0) {
                MessageBox.Show("削除するグループを選択してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //選択したグループのIDを取得して削除
            int delete_groupid = ((GroupKindDBHelper.GroupKind)groupListBox.SelectedItem).GroupId;
            new GroupKindDBHelper().deleteGroupKind(delete_groupid);
            new GroupBelongDBHelper().deleteGroupBelongByGroupID(delete_groupid);
            groupListBoxRefresh();
        }

        private void button5_Click(object sender, EventArgs e) {
            if (checkTokenRefreshNow()) return;
            if (accountDataGridView1.SelectedRows.Count != 1) {
                MessageBox.Show("アカウントを1つだけ選択してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //上へ
            int nowselected = this.accountDataGridView1.SelectedRows[0].Index;
            if (nowselected < 1) return;//2個目からしか無理
            int DBId1 = accountList[nowselected - 1].DBId;
            int DBId2 = accountList[nowselected].DBId;
            new AccountDBHelper().swapNumber(DBId1, DBId2);
            accountColumnBoxReflesh();
            this.accountDataGridView1.ClearSelection();
            this.accountDataGridView1.Rows[nowselected - 1].Selected = true;
            this.accountDataGridView1.FirstDisplayedScrollingRowIndex = nowselected - 1;
        }

        private void button6_Click(object sender, EventArgs e) {
            if (checkTokenRefreshNow()) return;
            if (accountDataGridView1.SelectedRows.Count != 1) {
                MessageBox.Show("アカウントを1つだけ選択してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //下へ
            int nowselected = this.accountDataGridView1.SelectedRows[0].Index;
            if (nowselected < 0 || nowselected == accountList.Count - 1) return;
            int DBId1 = accountList[nowselected + 1].DBId;
            int DBId2 = accountList[nowselected].DBId;
            new AccountDBHelper().swapNumber(DBId1, DBId2);
            accountColumnBoxReflesh();
            this.accountDataGridView1.ClearSelection();
            this.accountDataGridView1.Rows[nowselected + 1].Selected = true;
            this.accountDataGridView1.FirstDisplayedScrollingRowIndex = nowselected + 1;
        }

        private void accountDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) {

        }
        private BackgroundWorker bgWorker;
        //GUIの更新はこちらで行う
        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
            accountColumnBoxReflesh();
        }
        //時間のかかる処理を実行する
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            bgWorker = (BackgroundWorker)sender;
            List<MainForm.Account> shorilist = new List<MainForm.Account>();
            foreach (var selectedrow in this.accountDataGridView1.SelectedRows) {
                AccountDBHelper accountDBHelper = new AccountDBHelper();
                shorilist.Add(accountList[((DataGridViewRow)selectedrow).Index]);
            }
            shorilist.Reverse();
            //string[] proxy_list = Common.getProxyServer().ToArray();
            int ren = 0;
            int num = 0;
            int proxy_num = 0;
            foreach (var updateAccount in shorilist) {
                try {
                    MercariAPI api = new MercariAPI(updateAccount);
                    /*if (Settings.getUseProxyManual() && proxy_list.Length != 0) {
                        for (int trynum = 0; trynum < 5; trynum++) {
                            MercariAPI newapi = Common.checkMercariAPI(api, true, proxy_list[proxy_num % proxy_list.Length]);
                            //5回までリトライ
                            if (newapi.access_token != api.access_token) break;
                            proxy_num++;
                        }
                    }
                    else {*/
                    MercariAPI newapi = Common.checkMercariAPI(api, true);//, proxy_list[proxy_num % proxy_list.Length]);
                    //}
                    num++;
                    bgWorker.ReportProgress(num * 100 / shorilist.Count);
                    DateTime oldtime = DateTime.Now;
                    while (true) {
                        if (ren == 0) {
                            //もう1つ処理
                            ren = 1;
                            break;
                        }
                        if (DateTime.Now > oldtime.AddMinutes(15)) {
                            if (ren == 1) ren = 0;
                            break;
                        }
                        if (bgWorker.CancellationPending) {
                            //キャンセルリクエストがあったらキャンセルする
                            e.Cancel = true;
                            return;
                        }
                    }
                } catch (Exception ex) {
                    Log.Logger.Error(ex.Message);
                    Log.Logger.Error("token refresh error");
                }
            }

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            //ボタンの色を戻す
            this.accountTokenRefleshButton.BackColor = Color.Transparent;
            this.accountTokenRefleshButton.Text = "トークン更新";
        }


        private void AccountManageForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (checkTokenRefreshNow()) e.Cancel = true;
            if (backgroundWorker1.IsBusy) {
                MessageBox.Show("しばらく待ってから実行してください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }
        public void OnBackFromTokenUpdateDateModifyForm() {
            accountColumnBoxReflesh();
        }
        private void accountDataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            try {
                switch (e.ColumnIndex) {
                    case 2:
                        //アカウント名でソート
                        if (accountList.Count <= 0) return;
                        //降順昇順の切り替え
                        if (String.Compare(accountList[0].nickname, accountList[accountList.Count - 1].nickname) < 0) accountList.Sort((a, b) => String.Compare(b.nickname, a.nickname));
                        else accountList.Sort((a, b) => String.Compare(a.nickname, b.nickname));
                        break;
                    case 3:
                        //トークン期限でソート
                        //アカウント名でソート
                        if (accountList.Count <= 0) return;
                        //降順昇順の切り替え
                        if (DateTime.Compare(accountList[0].expiration_date, accountList[accountList.Count - 1].expiration_date) < 0) accountList.Sort((a, b) => DateTime.Compare(b.expiration_date, a.expiration_date));
                        else accountList.Sort((a, b) => DateTime.Compare(a.expiration_date, b.expiration_date));
                        break;
                    case 0:
                        //出品数でソート
                        if (accountList.Count <= 0) return;
                        //降順昇順の切り替え
                        if (accountList[0].exhibit_cnt < accountList[accountList.Count - 1].exhibit_cnt) accountList.Sort((a, b) => b.exhibit_cnt - a.exhibit_cnt);
                        else accountList.Sort((a, b) => a.exhibit_cnt - b.exhibit_cnt);
                        break;
                    case 1:
                        //販売数でソート
                        if (accountList.Count <= 0) return;
                        //降順昇順の切り替え
                        if (accountList[0].hanbai_num < accountList[accountList.Count - 1].hanbai_num) accountList.Sort((a, b) => b.hanbai_num - a.hanbai_num);
                        else accountList.Sort((a, b) => a.hanbai_num - b.hanbai_num);
                        break;
                }
            } catch (Exception ex) {

            }
            this.accountDataGridView1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e) {
            //選択したグループを上へ
            if (checkTokenRefreshNow()) return;
            if (!checkGroupEditMode()) return;
            if (groupListBox.SelectedIndices.Count != 1) {
                MessageBox.Show("グループを1つだけ選択してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //上へ
            int nowselected = this.groupListBox.SelectedIndices[0];
            if (nowselected < 1) return;//2個目からしか無理
            int DBId1 = ((GroupKindDBHelper.GroupKind)groupListBox.Items[nowselected - 1]).GroupId;
            int DBId2 = ((GroupKindDBHelper.GroupKind)groupListBox.Items[nowselected]).GroupId;
            new GroupKindDBHelper().swapNumber(DBId1, DBId2);
            groupListBoxRefresh();
            this.groupListBox.ClearSelected();
            this.groupListBox.SetSelected(nowselected - 1, true);
            this.groupListBox.TopIndex = nowselected - 1;
        }

        private void button2_Click(object sender, EventArgs e) {
            //選択したグループを下へ
            if (!checkGroupEditMode()) return;
            if (checkTokenRefreshNow()) return;
            if (groupListBox.SelectedIndices.Count != 1) {
                MessageBox.Show("グループを1つだけ選択してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //下へ
            int nowselected = this.groupListBox.SelectedIndices[0];
            if (nowselected < 0 || nowselected == accountList.Count - 1) return;
            int DBId1 = ((GroupKindDBHelper.GroupKind)groupListBox.Items[nowselected + 1]).GroupId;
            int DBId2 = ((GroupKindDBHelper.GroupKind)groupListBox.Items[nowselected]).GroupId;
            new GroupKindDBHelper().swapNumber(DBId1, DBId2);
            groupListBoxRefresh();
            this.groupListBox.ClearSelected();
            this.groupListBox.SetSelected(nowselected + 1, true);
            this.groupListBox.TopIndex = nowselected + 1;
        }
        private bool groupEditMode = false;
        private int edittingGroupId = -1;
        private void editGroupButton_Click(object sender, EventArgs e) {
            //既に編集中なら弾く
            if (!checkGroupEditMode()) return;
            if (this.groupListBox.SelectedIndices.Count != 1) {
                MessageBox.Show("編集するグループを1つ以上選択してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //グループ編集用意
            this.groupEditMode = true;
            this.groupCreateButton.Text = "変更保存";
            this.groupCreateButton.BackColor = Color.Red;
            this.GroupBelongListBox.Items.Clear();
            //選択しているグループに属しているメンバをListBoxに追加
            var selectedGroupKind = ((GroupKindDBHelper.GroupKind)groupListBox.SelectedItem);
            this.edittingGroupId = selectedGroupKind.GroupId;
            this.groupNameTextBox.Text = selectedGroupKind.GroupName;
            List<int> groupBelongAccountDBIdList = new GroupBelongDBHelper().loadGroupBelongDictionary()[selectedGroupKind.GroupId];
            List<MainForm.Account> belongAccountList = new AccountDBHelper().selectItem(groupBelongAccountDBIdList);
            foreach (var account in belongAccountList) this.GroupBelongListBox.Items.Add(account);
        }
        private bool checkGroupEditMode() {
            if (this.groupEditMode) {
                MessageBox.Show("編集中のグループを保存してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void token_update_date_modifyButton_Click(object sender, EventArgs e) {
            if (this.accountDataGridView1.SelectedRows.Count <= 0) {
                MessageBox.Show("トークン更新日時を変更するアカウントを選択してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            List<MainForm.Account> updateaccounts = new List<MainForm.Account>();
            foreach (DataGridViewRow row in this.accountDataGridView1.SelectedRows) {
                updateaccounts.Add(accountList[((DataGridViewRow)row).Index]);
            }
            new TokenUpdateDateModifyForm(this, updateaccounts).Show();
        }

        private void メールアドレス確認ToolStripMenuItem_Click(object sender, EventArgs e) {
            if (checkTokenRefreshNow()) return;
            if (accountDataGridView1.SelectedRows.Count != 1) {
                MessageBox.Show("アカウントを1つだけ選択してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MainForm.Account selectedAccount = accountList[this.accountDataGridView1.SelectedRows[0].Index];
            MessageBox.Show(selectedAccount.email);
        }
    }
}
