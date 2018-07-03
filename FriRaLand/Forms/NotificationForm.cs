using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RakuLand.Forms {
    public partial class NotificationForm : Form {
        //通知の取得はMainForm側で行い、こちらでは表示するだけ,再取得ボタンが押されたときは再取得する
        List<FrilAPI.RakumaNotificationResponse> notifyList = new List<FrilAPI.RakumaNotificationResponse>();
        List<FrilAPI.RakumaNotificationResponse> notifyListShibori = new List<FrilAPI.RakumaNotificationResponse>();

        public NotificationForm(List<FrilAPI.RakumaNotificationResponse> notifyList) {
            this.notifyList = notifyList;//参照わたしにすることでこっちで再取得した場合にMainForm.notifyListも更新される
            //this.notifyList = new List<FrilAPI.FrilNotification>(notifyList);
            this.notifyListShibori = new List<FrilAPI.RakumaNotificationResponse>(notifyList);//はじめはすべて表示する
            InitializeComponent();
            this.comboBox1.Items.Add("すべて");
            this.comboBox1.Items.Add("コメント");
            this.comboBox1.Items.Add("取引メッセージ");
            this.comboBox1.Items.Add("評価後メッセージ");
            this.comboBox1.Items.Add("支払い完了");
            this.comboBox1.Items.Add("ニュース");
            this.comboBox1.SelectedIndex = 0;
        }
        private void ReLoadNotification() {
            this.dataGridView1.RowCount = notifyListShibori.Count;
            this.dataGridView1.Refresh();
            this.dataGridView1.ClearSelection();
        }
        private void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            if (notifyListShibori.Count <= e.RowIndex) return;
            try {
                var notification = notifyListShibori[e.RowIndex];
                switch (e.ColumnIndex) {
                    case 0:
                        e.Value = notification.api.account.nickname;
                        break;
                    case 1:
                        switch (notification.type_id) {
                            case "1":
                                e.Value = "いいね";
                                break;
                            case "2":
                                e.Value = "コメント";
                                break;
                            case "3":
                                e.Value = "フォロー通知";
                                break;
                            case "10":
                                e.Value = "取引メッセージ";
                                break;
                            case "25":
                                e.Value = "運営メッセ―ジ";
                                break;
                            case "26":
                                e.Value = "ポイント期限等";
                                break;
                            case "TransactionMessageAdd":
                                e.Value = "評価後メッセージ";
                                break;
                            case "WaitShippingCVSATM":
                                e.Value = "支払い完了";
                                break;
                            default:
                                e.Value = notification.type_id;
                                break;
                        }
                        break;
                    case 2:
                        e.Value = notification.message;
                        break;
                    case 3:
                        e.Value = notification.created_at;
                        break;
                }
            } catch (Exception ex) {

            }
        }

        private void NotificationForm_Load(object sender, EventArgs e) {
            ReLoadNotification();
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e) {
            //合わせて表示する
            if (e.RowIndex >= notifyListShibori.Count) return;
            var notification = notifyListShibori[e.RowIndex];
            if (notification.type_id == "2") {
                CommentForm f = new CommentForm(notification.api, notification.item_id);
                f.Show();
                //FIXIT:transactionformのif判定まだです
            } else if (notification.type_id == "IncomingMessage" || notification.type_id == "TransactionMessageAdd" || notification.type_id == "WaitShippingCVSATM") {
                TransactionMessageForm f = new TransactionMessageForm(notification.api, notification.item_id);
                f.Show();
            }
        }

        private async void button1_Click(object sender, EventArgs e) {
            this.button1.Enabled = false;
            await Task.Run(() => DoGetAllNotification());
            ReLoadNotification();
            this.button1.Enabled = true;
        }
        private void DoGetAllNotification() {
            //通知を取得する
            notifyList.Clear();
            notifyListShibori.Clear();
            //すべてのアカウントについて通知を取得
            var accountList = AccountManageForm.accountLoader();
            foreach (var account in accountList) {
                FrilAPI api = new FrilAPI(account);
                api = Common.checkFrilAPI(api);
                var accountNotifications = api.getNotifications();
                //var accountTodoLists = api.getToDoLists();
                //var accountNews = api.getNews();
                notifyList.AddRange(accountNotifications);
                //notifyList.AddRange(accountTodoLists);
                //notifyList.AddRange(accountNews);
            }
            //通知時間で降順ソート
            //notifyList.Sort((a, b) => DateTime.Compare(b.created, a.created));
            //notifyListShibori = new List<FrilAPI.FrilNotification>(notifyList);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            //絞り込みを実行
            notifyListShibori.Clear();
            foreach (var notification in notifyList) {
                bool flag = false;
                if (this.comboBox1.SelectedIndex == 0) {
                    flag = true;
                } else if (this.comboBox1.SelectedIndex == 1 && notification.type_id == "2") {
                    flag = true;
                } else if (this.comboBox1.SelectedIndex == 2 && notification.type_id == "IncomingMessage") {
                    flag = true;
                } else if (this.comboBox1.SelectedIndex == 3 && notification.type_id == "TransactionMessageAdd") {
                    flag = true;
                } else if (this.comboBox1.SelectedIndex == 4 && notification.type_id == "WaitShippingCVSATM") {
                    flag = true;
                } else if (this.comboBox1.SelectedIndex == 5 && notification.type_id == "個別メッセージ") {
                    flag = true;
                }
                if (flag) this.notifyListShibori.Add(notification);
            }
            ReLoadNotification();
        }

        private void openNotificationButton_Click(object sender, EventArgs e) {
            foreach (DataGridViewRow row in this.dataGridView1.SelectedRows) {
                var notification = notifyListShibori[row.Index];
                if (notification.type_id == "2") {
                    CommentForm f = new CommentForm(notification.api, notification.item_id);
                    f.Show();
                } else if (notification.type_id == "IncomingMessage" || notification.type_id == "TransactionMessageAdd" || notification.type_id == "WaitShippingCVSATM") {
                    TransactionMessageForm f = new TransactionMessageForm(notification.api, notification.item_id);
                    f.Show();
                } else if (notification.type_id == "個別メッセージ") {
                    MessageBox.Show(notification.message);
                }
            }
        }
    }
}
