using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RakuLand.Forms {
    public partial class IkkatuShukkin : Form {
        public IkkatuShukkin() {
            InitializeComponent();
        }
        MessageBoxWithProgressBar messagebox;
        public static Dictionary<string, Dictionary<string, string>> allbanklist;
        private class IkkatuClass {
            public IkkatuClass(Common.Account account, string addressstr, string bankstr, int uriage, FrilAPI.Bank bank, FrilAPI.Address address) {
                this.nickname = account.nickname;
                this.account = account;
                this.addressstr = addressstr;
                this.bankstr = bankstr;
                this.uriage = uriage;
                this.bank = bank;
                this.address = address;
            }
            public Common.Account account;
            public string nickname { get; set; }
            public string addressstr { get; set; }
            public string bankstr { get; set; }
            public int uriage { get; set; }
            public string status { get; set; }
            public FrilAPI.Bank bank;
            public FrilAPI.Address address;
        }
        SortableBindingList<IkkatuClass> bindlist = new SortableBindingList<IkkatuClass>();
        private int minexhibit_money = 0; //この料金以上なら出金する
        private void IkkatuShukkin_Load(object sender, EventArgs e) {
            //しばらくお待ちください
            this.messagebox = new MessageBoxWithProgressBar("一括出金", "しばらくお待ちください");
            this.messagebox.StartPosition = FormStartPosition.CenterParent;
            this.messagebox.Show();
            this.numericUpDown1.Maximum = 9999999;
            this.numericUpDown2.Maximum = 9999999;
        }

        private void IkkatuShukkin_Shown(object sender, EventArgs e) {
            //アカウント情報を取得,それぞれの売上金も取得する
            var accountList = AccountManageForm.accountLoader();
            int num = 0;
            foreach (var account in accountList) {
                try {
                    this.messagebox.changeProgressBarValue(num * 100 / accountList.Count);
                    var api = new FrilAPI(account);
                    api.getBankDictionary();
                    //有効期限が切れていないアカウントでbanklistとる
                    if (allbanklist == null) allbanklist = api.getOtherBankDictionary();
                    //売り上げ情報・口座・住所情報取得
                    int uriage = api.getCurrentSales().current_sales;
                    FrilAPI.Bank bank = api.getBankAccounts();
                    FrilAPI.Address address = api.getAddressWithBank();
                    string bankstr = ((bank == null) ? "" : GetBankStr(bank));
                    string addressstr = ((address == null) ? "" : address.ToString());
                    bindlist.Add(new IkkatuClass(account, addressstr, bankstr, uriage, bank, address));
                    num++;
                } catch {
                    num++;
                }
            }
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.DataSource = bindlist;
            if (allbanklist == null) {
                MessageBox.Show("銀行情報の取得に失敗しました");
                this.Close();
            }
            //しばらくお待ちくださいを閉じる
            this.messagebox.Dispose();
        }


        public static string GetBankStr(FrilAPI.Bank bank) {
            if (bank == null) return "";
            foreach (var initial in allbanklist.Keys) {
                foreach (var b in allbanklist[(string)initial]) {
                    if (b.Value == bank.bankid) {
                        return (string)b.Key + " " + bank.branch_id + " " + bank.account_number;
                    }
                }
            }
            return "";
        }

        private void button1_Click(object sender, EventArgs e) {
            if (this.button1.BackColor == Color.Red) {
                //停止リクエスト
                this.backgroundWorker1.CancelAsync();
                //GUI戻す
                this.button1.BackColor = Color.Transparent;
                this.dataGridView1.Enabled = true;
                this.radioButton1.Enabled = true;
                this.radioButton2.Enabled = true;
                this.radioButton3.Enabled = true;
                this.radioButton4.Enabled = true;
                this.numericUpDown1.Enabled = true;
                this.button1.Text = "一括出金";
            } else {
                //開始
                //1つも選択してなければおわり
                int selected_num = this.dataGridView1.SelectedRows.Count;
                if (selected_num < 1) {
                    MessageBox.Show("一括出金するアカウントを選択してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (this.radioButton1.Checked == false && this.radioButton2.Checked == false) {
                    MessageBox.Show("方法1または方法2を選択してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (this.radioButton3.Checked == false && this.radioButton4.Checked == false) {
                    MessageBox.Show("出金条件を選択してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (this.radioButton5.Checked == false && this.radioButton6.Checked == false) {
                    MessageBox.Show("出金金額を選択してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //確認
                if (this.radioButton1.Checked) {
                    if (MessageBox.Show("選択した" + selected_num.ToString() + "件のアカウントを方法1で出金します.よろしいですか?", MainForm.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) {
                        return;
                    }
                } else {
                    if (MessageBox.Show("選択した" + selected_num.ToString() + "件のアカウントを方法2で出金します.よろしいですか?", MainForm.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) {
                        return;
                    }
                }
                if (this.backgroundWorker1.IsBusy) {
                    MessageBox.Show("しばらくたってから実行してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (this.radioButton2.Checked) {
                    //方法2の時はアカウント一覧をみて、デフォルト銀行が指定されていないアカウントがあれば警告を出す
                    bool warning_flag = false;
                    var account_list = AccountManageForm.accountLoader();
                    foreach (var account in account_list) if (account.defaultbankaddressId < 0) warning_flag = true;
                    if (warning_flag) MessageBox.Show("方法2で指定するデフォルト銀行・住所が指定されていないアカウントがあるため、処理に失敗する可能性があります", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    /*var settingsDBHelper = new SettingsDBHelper();
                    string bank_xml = settingsDBHelper.getSettingValue("default_bank");
                    if (bank_xml != null) default_bank = Common.XmlToBank(Common.Base64Decode(bank_xml));
                    if (default_bank == null) {
                        MessageBox.Show("方法2で使用する銀行口座が指定されていません", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string use_address = settingsDBHelper.getSettingValue("use_address");
                    if (use_address == null) {
                        MessageBox.Show("方法2で住所を使用するかが指定されていません", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (use_address == "True") {
                        string address_xml = settingsDBHelper.getSettingValue("default_address");
                        if (address_xml != null) default_address = Common.XmlToAddress(Common.Base64Decode(address_xml));
                        if (default_address == null) {
                            MessageBox.Show("方法2で使用する住所が指定されていません", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }*/
                }
                if (this.radioButton4.Checked) {
                    minexhibit_money = (int)this.numericUpDown1.Value;
                }
                //開始する
                this.button1.BackColor = Color.Red;
                this.dataGridView1.Enabled = false;
                this.radioButton1.Enabled = false;
                this.radioButton2.Enabled = false;
                this.radioButton3.Enabled = false;
                this.radioButton4.Enabled = false;
                this.radioButton5.Enabled = false;
                this.radioButton6.Enabled = false;
                this.numericUpDown1.Enabled = false;
                this.numericUpDown2.Enabled = false;
                this.progressBar1.Value = 0;
                this.button1.Text = "停止";
                if (this.radioButton1.Checked) this.backgroundWorker1.RunWorkerAsync(-1);
                else this.backgroundWorker1.RunWorkerAsync(1);
            }
        }
        BackgroundWorker bgWorker;
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            this.progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            this.button1.BackColor = Color.Transparent;
            this.dataGridView1.Enabled = true;
            this.radioButton1.Enabled = true;
            this.radioButton2.Enabled = true;
            this.radioButton3.Enabled = true;
            this.radioButton4.Enabled = true;
            this.radioButton5.Enabled = true;
            this.radioButton6.Enabled = true;
            this.numericUpDown1.Enabled = true;
            this.numericUpDown2.Enabled = true;
            this.button1.Text = "一括出金";
        }

        //TODO:住所偽装のhogeなのでいまはいらん
        private void button2_Click(object sender, EventArgs e) {
            //var f = new TrueKouzaAddressRegisterForm(allbanklist);
            //f.Show();
        }
    }
}
