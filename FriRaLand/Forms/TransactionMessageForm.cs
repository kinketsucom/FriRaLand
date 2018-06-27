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
    public partial class TransactionMessageForm : Form {
        private FrilAPI Frilapi;
        private string itemid;
        private FrilAPI.TransactionInfo info;
        private FrilItem item;
        private List<string> message_template_list;
        public TransactionMessageForm(FrilAPI api, string itemid) {
            InitializeComponent();
            this.Frilapi = api;
            this.itemid = itemid;
        }

        private void submit_Click(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(this.richTextBox1.Text)) {
                MessageBox.Show("取引メッセージを入力してください", "エラー",
                                   MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Exclamation,
                                   MessageBoxDefaultButton.Button2);
                return;
            }
            if (this.richTextBox1.Text.Length > 1000) {
                MessageBox.Show("取引メッセージは1000文字以内で入力してください", "エラー",
                                   MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Exclamation,
                                   MessageBoxDefaultButton.Button2);
                return;
            }
            //コメントを送信
            string html = Frilapi.GetTransactionPage(this.itemid);
            Dictionary<string, string> param_dic = GetTransactionTokenFromHTML(html);

            Frilapi.SendTransactionMessage(itemid, this.richTextBox1.Text,param_dic);
            //コメント一覧を再取得してGUIに反映
            this.dataGridView1.Rows.Clear();
            //コメントを取得
            var comments = Frilapi.GetTransactionMessages(itemid);
            //コメントをGUIに反映
            RefreshCommentDataGridview(comments);
            //コメントフォームをクリア
            this.richTextBox1.Clear();
        }


        private Dictionary<string, string> GetTransactionTokenFromHTML(string html) {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            int num = 0;
            num = html.IndexOf("<form id=\"comment-form\"", num);
            int num2 = html.IndexOf("</form>", num);
            string text = html.Substring(num, num2 - num);
            num = 0;
            while (text.IndexOf("<input type=\"hidden\"", num) >= 0) {
                num = text.IndexOf("<input type=\"hidden\"", num);
                num2 = text.IndexOf("/>", num) + "/>".Length;
                string text2 = text.Substring(num, num2 - num);
                num = num2;
                int num3 = text2.IndexOf("name=\"") + "name=\"".Length;
                int num4 = text2.IndexOf("\"", num3);
                string key = text2.Substring(num3, num4 - num3);
                num3 = text2.IndexOf("value=\"") + "value=\"".Length;
                num4 = text2.IndexOf("\"", num3);
                string value = text2.Substring(num3, num4 - num3);
                dictionary.Add(key, value);
            }
            return dictionary;
        }





        private void TransactionMessage_Load(object sender, EventArgs e) {
            //商品情報を取得
            this.item = Frilapi.GetItemInfobyItemIDWithDetail(itemid);
            SetGUIParams();
            ////ウインドウサイズを記憶するか
            //this.saveWindowSizeCheckbox.Checked = Settings.getSaveMessageWindowSize();
            //if (Settings.getSaveMessageWindowSize()) {
            //    this.Width = Settings.getMessageWIndowSizeWidth();
            //    this.Height = Settings.getMessageWIndowSizeHeight();
            //}
            //販売されたときのDB操作を行う
            ExhibitService.updateDBOnSold(Frilapi, item);
            AdjustGUISize();
        }
        private void RefreshCommentDataGridview(List<FrilAPI.Comment> comments) {
            this.dataGridView1.Rows.Clear();
            foreach (var c in comments) {

                this.dataGridView1.Rows.Add(c.screen_name, c.created_at.ToString("MM/dd HH:mm"), c.comment);
            }
        }
        //商品情報や取引情報をもとにGUIにパラメータ設定
        private void SetGUIParams() {
            item.loadImageFromThumbnail();
            //商品情報をGUIに反映
            this.pictureBox1.Image = item.Image;
            this.itemNameLabel.Text = item.item_name;
            this.sellerLabel.Text = item.screen_name;
            this.buyerAccountNickNameTextBox.Text = item.buyer_name;
            this.kakakuTextBox.Text = item.s_price.ToString();// +"円";
            this.riekiTextBox.Text = Common.getRieki(item.s_price).ToString();
            //コメントを取得
            var comments = Frilapi.GetTransactionMessages(itemid);
            //コメントをGUIに反映
            RefreshCommentDataGridview(comments);

            //購入者情報を取得
            this.info = Frilapi.GetTransactionInfo(itemid);
            if (!string.IsNullOrEmpty(info.buyername)) {
                buyerNameTextBox.Text = info.buyername;
            } else {
                buyerNameTextBox.Enabled = false;
            }
            if (!string.IsNullOrEmpty(info.address)) {
                buyerAddressRichTextBox.Text = info.address;
            } else {
                buyerAddressRichTextBox.Enabled = false;
            }
            //取引状態に応じて取引状態のラベルと、コントロールボタンの状態を変更する
            if (!string.IsNullOrEmpty(info.status)) {
                switch (info.status) {
                    case FrilAPI.TradingStatus.Wait_Paymet:
                        statusLabel.Text = FrilItem.StatusMessage.wait_payment;
                        control_button.Enabled = false;
                        control_button.Text = "購入者の支払いをお待ちください";
                        //住所を表示しない
                        this.buyerAddressRichTextBox.Text = "";
                        this.copyButton.Enabled = false;
                        this.buyerAddressRichTextBox.Enabled = false;
                        statusLabel.ForeColor = Color.Red;
                        break;
                    case FrilAPI.TradingStatus.Wait_Shipping:
                        statusLabel.Text = FrilItem.StatusMessage.wait_shipping;
                        control_button.Enabled = true;
                        control_button.Text = "商品を発送したので発送通知をする";
                        break;
                    case FrilAPI.TradingStatus.Wait_Review:
                        statusLabel.Text = FrilItem.StatusMessage.wait_review;
                        control_button.Enabled = false;
                        control_button.Text = "受け取り評価後,購入者を評価できます";
                        break;
                    case FrilAPI.TradingStatus.Wait_Done:
                        statusLabel.Text = FrilItem.StatusMessage.wait_done;
                        control_button.Enabled = true;
                        control_button.Text = "購入者を評価する";
                        break;
                    case FrilAPI.TradingStatus.Done:
                        statusLabel.Text = FrilItem.StatusMessage.done;
                        control_button.Enabled = false;
                        control_button.Text = "取引完了";
                        statusLabel.ForeColor = Color.Red;
                        break;
                    default:
                        statusLabel.Text = info.status;
                        Log.Logger.Error("不明なstatusを確認");
                        break;
                }
            }
            if (!string.IsNullOrEmpty(info.created)) {
                createdDateLabel.Text = info.created;
            }
            //定型文よみこみ
            this.comboBox1.Items.Clear();
            this.message_template_list = Settings.getTransactionMessageTemplate();
            var title_list = Settings.getTransactionMessageTemplateTitle();
            foreach (var str in title_list) this.comboBox1.Items.Add(str);
            if (title_list.Count > 0) this.comboBox1.SelectedIndex = 0;
            this.richTextBox1.Text = "";
            //商品備考読み込み
            var itemNoteDBHelper = new ItemNoteDBHelper();
            var itemNote = itemNoteDBHelper.getItemNote(this.itemid);
            if (itemNote != null) {
                this.BikouTextBox.Text = itemNote.bikou;
                this.address_copyed_checkbox.Checked = itemNote.address_copyed;
            }
        }

        private void control_button_Click(object sender, EventArgs e) {
            string html = this.Frilapi.GetTransactionPage(this.itemid);
            Dictionary<string, string> param_dic = GetShipmentFromHTML(html);
            if (param_dic.Count != 0) { //通知送る前
                param_dic.Add("tracking_number", "");
                this.Frilapi.SendItemShippedNotification(this.itemid, param_dic);
                //出品通知か購入者評価以外ボタンはおしても反応しない(disableにしているが）
                //if (info.status != "wait_shipping" && info.status != "wait_done") return;//FIXME:なんか取引状態の確認ができてないのであやしい
                //if (info.status == "wait_shipping") {
                SetGUIParams();
            } else{//通知送った後は0になる
                //} else if (info.status == "wait_done") {
                //取引メッセージを入力させる
                var messageForm = new InputMessageForm("購入者評価メッセージを入力してください");
                messageForm.SetComboBoxItems(Settings.getBuyerReviewMessageTemplateTitle(), Settings.getBuyerReviewMessageTemplate());
                if (messageForm.ShowDialog() != DialogResult.OK) return;
                string reviewMessage = messageForm.message;
                Frilapi.SendReview(itemid, reviewMessage);
                var itemNoteDBHelper = new ItemNoteDBHelper();
                var shuppinrirekiDBHelper = new ShuppinRirekiDBHelper();
                //出品履歴,商品備考データが存在する場合は該当レコードを削除
                itemNoteDBHelper.deleteItemNote(itemid);

                SetGUIParams();

            }
            //}
        }
        private Dictionary<string, string> GetShipmentFromHTML(string html) {
            try {//FIXME:あやしいtryぶっちゃけ取引中と発送後のちがいわからんかった
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                int num = 0;
                num = html.IndexOf("<form id=\"ship-form\"", num);
                int num2 = html.IndexOf("</form>", num);
                string text = html.Substring(num, num2 - num);
                num = 0;
                while (text.IndexOf("<input type=\"hidden\"", num) >= 0) {
                    num = text.IndexOf("<input type=\"hidden\"", num);
                    num2 = text.IndexOf("/>", num) + "/>".Length;
                    string text2 = text.Substring(num, num2 - num);
                    num = num2;
                    int num3 = text2.IndexOf("name=\"") + "name=\"".Length;
                    int num4 = text2.IndexOf("\"", num3);
                    string key = text2.Substring(num3, num4 - num3);
                    num3 = text2.IndexOf("value=\"") + "value=\"".Length;
                    num4 = text2.IndexOf("\"", num3);
                    string value = text2.Substring(num3, num4 - num3);
                    dictionary.Add(key, value);
                }
                num = 0;
                while (text.IndexOf("<option ", num) >= 0) {
                    num = text.IndexOf("<option ", num);
                    num2 = text.IndexOf("</option>", num) + "</option>".Length;
                    string text3 = text.Substring(num, num2 - num);
                    if (text3.IndexOf("selected=\"selected\"") >= 0) {
                        int num5 = text3.IndexOf("value=\"") + "value=\"".Length;
                        int num6 = text3.IndexOf("\"", num5);
                        string value2 = text3.Substring(num5, num6 - num5);
                        dictionary.Add("item%5Bdelivery_method%5D", value2);
                        break;
                    }
                    num = num2;
                }
                num = 0;
                num = text.IndexOf("<input name=\"utf8\"", num);
                num2 = text.IndexOf("/>", num) + "/>".Length;
                string text4 = text.Substring(num, num2 - num);
                num = text4.IndexOf("value=\"") + "value=\"".Length;
                num2 = text4.IndexOf("\"", num);
                string value3 = text4.Substring(num, num2 - num);
                dictionary.Add("utf8", value3);
                return dictionary;
            }catch(Exception ex) {
                Log.Logger.Error(ex);
                Console.WriteLine(ex);
                return new Dictionary<string, string>();//FIXME:なんだこの通知処理は。。。基地外じみている
            }
        }


        private void TransactionMessageForm_SizeChanged(object sender, EventArgs e) {
            AdjustGUISize();
        }
        private void AdjustGUISize() {
            this.dataGridView1.Width = this.Width - 400;
            this.panel1.Location = new Point(this.dataGridView1.Width + this.dataGridView1.Location.X, this.Height - this.panel1.Height - 45);
            this.panel2.Location = new Point(this.dataGridView1.Width + this.dataGridView1.Location.X, this.Height - this.panel2.Height - this.panel1.Height - 40);
            this.dataGridView1.Height = this.Height - 280;
        }
        private void copyButton_Click(object sender, EventArgs e) {
            //IDataObject data = Clipboard.GetDataObject();

            //if (data != null) {
            //    //関連付けられているすべての形式を列挙する
            //    foreach (string fmt in data.GetFormats()) {
            //        Console.WriteLine(fmt);
            //    }
            //}
            ///*親IDの取得*/
            ////string parent_id = new ExhibitLogDBHelper().getParentIDFromExhibitLog(itemid);
            //FrilItem item = Frilapi.GetItemInfobyItemIDWithDetail(itemid);
            //string[] str = Common.makeAddressExcelCSVLine(item, info, Frilapi);
            ////ANSI形式にエンコードする
            ////byte[] bs = System.Text.Encoding.Default.GetBytes(str);
            ////MemoryStreamに変換する
            ////System.IO.MemoryStream ms = new System.IO.MemoryStream(bs);
            //System.IO.MemoryStream ms = Common.CreateHtmlClipboardFormatToOneLineTable(str);
            ////DataObjectを作成し、MemoryStreamのデータをセットする
            //DataObject data2 = new DataObject();
            //data2.SetData(DataFormats.Html, ms);
            ////クリップボードにコピーする
            //Clipboard.SetDataObject(data2);
        }

        private void TransactionMessageForm_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.W && e.Control) {
                this.Close();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.comboBox1.SelectedIndex < 0) return;
            this.richTextBox1.Text = this.message_template_list[this.comboBox1.SelectedIndex];
        }

        private void TransactionMessageForm_FormClosing(object sender, FormClosingEventArgs e) {
            //閉じる前に商品の備考をDBに保存する
            var itemNoteDBHelper = new ItemNoteDBHelper();
            var itemNote = itemNoteDBHelper.getItemNote(this.itemid);
            if (itemNote == null) {
                //商品備考は保存されていない,新規保存
                if ((string.IsNullOrEmpty(BikouTextBox.Text.Trim()) == false) || this.address_copyed_checkbox.Checked) {//備考欄が空欄でないまたは住居転記済みのときのみ保存
                    //備考を保存する
                    var newItemNote = new ItemNoteDBHelper.ItemNoteClass();
                    newItemNote.itemid = this.itemid;
                    newItemNote.bikou = BikouTextBox.Text.Trim();
                    newItemNote.address_copyed = this.address_copyed_checkbox.Checked;
                    itemNoteDBHelper.updateItemNote(newItemNote);
                }
            } else {
                //商品備考をアップデート
                itemNote.bikou = this.BikouTextBox.Text.Trim();
                itemNote.address_copyed = this.address_copyed_checkbox.Checked;
                itemNoteDBHelper.updateItemNote(itemNote);
            }
            //閉じる前にウィンドウサイズを記憶するか・ウィンドウサイズを記憶
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            settingsDBHelper.updateSettings(Common.save_message_window_size, this.saveWindowSizeCheckbox.Checked.ToString());
            if (this.saveWindowSizeCheckbox.Checked) {
                settingsDBHelper.updateSettings(Common.message_window_size_width, this.Width.ToString());
                settingsDBHelper.updateSettings(Common.message_window_size_height, this.Height.ToString());
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            MessageBox.Show("コピーボタンをクリックするとクリップボードに取引情報がコピーされます\n"
                + "1つめのセルにコピーには商品番号がコピーされます\n"
                + "商品番号とは、商品名または商品説明の末尾にカッコ()で番号を書いている場合にその部分が抽出されます\n"
                + "コピー1では商品名から、コピー2では商品説明から商品番号が抽出されます");
        }

        private void copyButton2_Click(object sender, EventArgs e) {

        }

        private void cancelTransactionButton_Click(object sender, EventArgs e) {
            if (DialogResult.OK != MessageBox.Show("取引のキャンセルを行いますか？", MainForm.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)) return;
            if (Frilapi.cancelTransaction(item.item_id)) {
                MessageBox.Show("取引のキャンセルに成功しました", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show("取引のキャンセルに失敗しました", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
