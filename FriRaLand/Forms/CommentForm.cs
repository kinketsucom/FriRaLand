using RakuLand.DBHelper;
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
    public partial class CommentForm : Form {
        private FrilAPI Frilapi;
        private FrilItem item;
        private MainForm mainform;
        private string itemid;
        private List<FrilAPI.Comment> comments;
        private List<string> comment_template_list;
        public CommentForm(FrilAPI api, string itemid, MainForm mainform) {
            InitializeComponent();
            this.Frilapi = api;
            this.itemid = itemid;
            this.mainform = mainform;
        }
        public CommentForm(FrilAPI api, string itemid) {
            InitializeComponent();
            this.Frilapi = api;
            this.itemid = itemid;
        }
        private void CommentForm_Load(object sender, EventArgs e) {
            //商品情報を取得
            this.item = Frilapi.GetItemInfobyItemIDWithDetail(itemid);
            item.loadImageFromThumbnail();
            //商品情報をGUIに反映
            this.pictureBox1.Image = item.Image;
            this.itemNameLabel.Text = item.item_name;
            this.sellerLabel.Text = item.screen_name;//FIXIT:seller.nameをへんこうしたけど
            this.kakakuTextBox.Text = item.s_price.ToString();// +"円";
            this.riekiTextBox.Text = Common.getRieki(item.s_price).ToString();
            //コメントを取得
            comments = Frilapi.GetComments(itemid);
            //コメントをGUIに反映
            RefreshCommentDataGridview(comments);
            //定型文よみこみ
            this.comment_template_list = Settings.getCommentTemplate();
            var title_list = Settings.getCommentTemplateTitle();
            this.comboBox1.Items.Clear();
            foreach (var str in title_list) this.comboBox1.Items.Add(str);
            if (title_list.Count > 0) this.comboBox1.SelectedIndex = 0;
            this.richTextBox1.Text = "";
            //商品備考読み込み
            var itemNoteDBHelper = new ItemNoteDBHelper();
            var itemNote = itemNoteDBHelper.getItemNote(this.itemid);
            if (itemNote != null) {
                this.BikouTextBox.Text = itemNote.bikou;
            }

            //出品中以外は赤字に
            if (item.status_message != "出品中") {
                itemStatusTextBox.ForeColor = Color.Red;
                itemStatusTextBox.BackColor = SystemColors.Control;
            }
            //商品状態読み込み
            itemStatusTextBox.Text = item.status_message;
            //公開停止中である場合のみ商品を再公開するボタンを有効に
            if (item.status_message == "公開停止中") reSaleItemButton.Enabled = true;
            else reSaleItemButton.Enabled = false;
            //ウインドウサイズを記憶するか
            this.saveWindowSizeCheckbox.Checked = Settings.getSaveMessageWindowSize();
            if (Settings.getSaveMessageWindowSize()) {
                this.Width = Settings.getMessageWIndowSizeWidth();
                this.Height = Settings.getMessageWIndowSizeHeight();
            }
            //値下げ除外リストに含まれる場合はcheckboxをオン
            var nesage_ng_list = Settings.getNesageNGList().ToArray();
            if (Array.IndexOf(nesage_ng_list, item.item_id) >= 0) this.ignoreNesageCheckBox.Checked = true;
            AdjustGUISize();
        }

        private void submit_Click(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(this.richTextBox1.Text)) {
                MessageBox.Show("コメントを入力してください", "エラー",
                                   MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Exclamation,
                                   MessageBoxDefaultButton.Button2);
                return;
            }
            if (this.richTextBox1.Text.Length > 1000) {
                MessageBox.Show("コメントは1000文字以内で入力してください", "エラー",
                                   MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Exclamation,
                                   MessageBoxDefaultButton.Button2);
                return;
            }
            //コメントを送信
            Frilapi.AddComment(itemid, this.richTextBox1.Text);
            //コメント一覧を再取得してGUIに反映
            this.dataGridView1.Rows.Clear();
            //コメントを取得
            var comments = Frilapi.GetComments(itemid);
            //コメントをGUIに反映
            RefreshCommentDataGridview(comments);
            //コメントフォームをクリア
            this.richTextBox1.Clear();

        }
        private void RefreshCommentDataGridview(List<FrilAPI.Comment> comments) {
            this.dataGridView1.Rows.Clear();
            comments.Reverse();
            foreach (var c in comments) {

                this.dataGridView1.Rows.Add(c.screen_name, c.created_at.ToString("MM/dd HH:mm"), c.comment);
            }
        }

        private void CommentForm_SizeChanged(object sender, EventArgs e) {
            AdjustGUISize();
        }
        private void AdjustGUISize() {
            
            //this.dataGridView1.Width = this.Width - 400;
            //this.panel1.Location = new Point(this.dataGridView1.Width + this.dataGridView1.Location.X, this.Height - this.panel1.Height - 45);
            //this.panel2.Location = new Point(this.dataGridView1.Width + this.dataGridView1.Location.X, this.Height - this.panel2.Height - this.panel1.Height - 40);

            //this.CommentInfoLabel.Location = new Point(this.dataGridView1.Width + this.dataGridView1.Location.X, this.Height - this.toolStripStatusLabel1.Height-this.CommentInfoLabel.Height); 
            //this.dataGridView1.Height = this.Height - this.toolStripStatusLabel1.Height-this.CommentInfoLabel.Height-50;
        }

        private void CommentForm_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.W && e.Control) {
                this.Close();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.comboBox1.SelectedIndex < 0) return;
            this.richTextBox1.Text = this.comment_template_list[this.comboBox1.SelectedIndex];
        }

        private void CommentForm_FormClosing(object sender, FormClosingEventArgs e) {
            //閉じる前に商品の備考をDBに保存する
            var itemNoteDBHelper = new ItemNoteDBHelper();
            var itemNote = itemNoteDBHelper.getItemNote(this.itemid);
            if (itemNote == null) {
                //商品備考は保存されていない,新規保存
                if (string.IsNullOrEmpty(BikouTextBox.Text.Trim()) == false) {//備考欄が空欄でないときのみ保存
                    //備考を保存する
                    var newItemNote = new ItemNoteDBHelper.ItemNoteClass();
                    newItemNote.itemid = this.itemid;
                    newItemNote.bikou = BikouTextBox.Text.Trim();
                    itemNoteDBHelper.updateItemNote(newItemNote);
                }
            } else {
                //商品備考をアップデート
                itemNote.bikou = this.BikouTextBox.Text.Trim();
                itemNoteDBHelper.updateItemNote(itemNote);
            }
            //閉じる前にウィンドウサイズを記憶するか・ウィンドウサイズを記憶
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            settingsDBHelper.updateSettings(Common.save_message_window_size, this.saveWindowSizeCheckbox.Checked.ToString());
            if (this.saveWindowSizeCheckbox.Checked) {
                settingsDBHelper.updateSettings(Common.message_window_size_width, this.Width.ToString());
                settingsDBHelper.updateSettings(Common.message_window_size_height, this.Height.ToString());
            }
            //値下げしないチェックボックスがONで値下げNGリストに入っていなかったらリストに追加
            if (ignoreNesageCheckBox.Checked) {
                var nesage_ng_list = Settings.getNesageNGList();
                if (Array.IndexOf(nesage_ng_list.ToArray(), item.item_id) < 0) nesage_ng_list.Add(item.item_id);
                Settings.setNesageNGList(nesage_ng_list);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            //var editform = new ItemEditForm(item, Frilapi);
            //editform.mainform = mainform;
            //editform.Show();
        }

        private void reSaleItemButton_Click(object sender, EventArgs e) {
            ////商品を再公開する
            //if (DialogResult.OK != MessageBox.Show("公開停止中の商品を再公開しますか?", MainForm.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)) return;
            //if (Frilapi.reSaleFrilItem(item.item_id)) {
            //    MessageBox.Show("商品の再公開に成功しました", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    //リロード
            //    CommentForm_Load(sender, e);
            //} else {
            //    MessageBox.Show("商品の再公開に失敗しました", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void CommentForm_SizeChanged_1(object sender, EventArgs e) {
            AdjustGUISize();
        }

        private void button1_Click_1(object sender, EventArgs e) {
            var editform = new ItemEditForm(item, this.Frilapi);
            //editform.mainform = mainform;
            editform.Show();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            Console.WriteLine(e.RowIndex);
            Console.WriteLine(comments[e.RowIndex].id);
            if (Frilapi.DeleteComment(comments[e.RowIndex].item_id, comments[e.RowIndex].id)) {
                toolStripStatusLabel1.Text = "コメント削除成功";
            } else {
                toolStripStatusLabel1.Text = "コメント削除失敗(お問い合わせください)";
            }
            //コメント一覧を再取得してGUIに反映
            this.dataGridView1.Rows.Clear();
            //コメントを取得
            comments = Frilapi.GetComments(itemid);
            //コメントをGUIに反映
            RefreshCommentDataGridview(comments);
        }

        private void CommentInfoLabel_MouseHover(object sender, EventArgs e) {
            toolStripStatusLabel1.Text = "ダブルクリックでコメントを削除することができます。";
        }

        private void CommentInfoLabel_MouseLeave(object sender, EventArgs e) {
            toolStripStatusLabel1.Text = "";
        }
    }
}
