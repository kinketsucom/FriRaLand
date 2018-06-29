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
            //public IkkatuClass(Common.Account account, string addressstr, string bankstr, int uriage, FrilAPI.Bank bank, FrilAPI.Address address) {
            //    this.nickname = account.nickname;
            //    this.account = account;
            //    this.addressstr = addressstr;
            //    this.bankstr = bankstr;
            //    this.uriage = uriage;
            //    this.bank = bank;
            //    this.address = address;
            //}
            public IkkatuClass(Common.Account account, string bankstr, int uriage) {
                this.nickname = account.nickname;
                this.account = account;
                this.bankstr = bankstr;
                this.uriage = uriage;
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
        private int minexhibit_money = 1000; //この料金以上なら出金する
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
                    api.GetBankInfo();//apiオブジェクトに対応したbank情報を取得
                    ////有効期限が切れていないアカウントでbanklistとる
                    //if (allbanklist == null) allbanklist = api.getOtherBankDictionary();//FIME:null判定はいるのか・・？
                    //売り上げ情報・口座・住所情報取得
                    api.GetBalanceInfo();
                    //int uriage = api.getCurrentSales().current_sales;
                    int uriage = (int)api.account.balance_info.balance;
                    //FrilAPI.Bank bank = api.getBankAccounts();//FIXME:これたぶんいらない
                    //FrilAPI.Address address = api.getAddressWithBank();//FIXME:これたぶんいらないというかフリルに住所は不要
                    //string bankstr = ((bank == null) ? "" : GetBankStr(bank));
                    string bankstr = api.account.bank_info.account_number+api.account.bank_info.name+api.account.bank_info.branch_name
                        +api.account.bank_info.last_name+api.account.bank_info.first_name;
                    // 口座番号,Ａ銀行Ｂ支店
                    //string addressstr = ((address == null) ? "" : address.ToString());
                    bindlist.Add(new IkkatuClass(account, bankstr, uriage));
                    num++;
                } catch {
                    num++;
                }
            }
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.DataSource = bindlist;
            //if (allbanklist == null) {
            //    MessageBox.Show("銀行情報の取得に失敗しました");
            //    this.Close();
            //}
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
                    MessageBox.Show("方法1を選択してください", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            bgWorker = (BackgroundWorker)sender;
            //負の値なら方法1, 非負なら方法2
            bool isMode1 = ((int)e.Argument) < 0;
            bool isMode2 = !isMode1;
            int num = 0;
            foreach (DataGridViewRow row in this.dataGridView1.SelectedRows) {
                bgWorker.ReportProgress(num * 100 / this.dataGridView1.SelectedRows.Count);
                if (bgWorker.CancellationPending) return;
                IkkatuClass ic = bindlist[row.Index];
                FrilAPI api = new FrilAPI(ic.account);
                api = Common.checkFrilAPI(api);
                bindlist[row.Index].status = "";
                if (isMode1) {
                    //方法1
                    //var sales = api.getCurrentSales();
                    if (api.account.balance_info.balance < minexhibit_money) continue;//FIXME:額を指定する
                    int amount = (int)api.account.balance_info.balance - (int)this.numericUpDown2.Value;//引き落とす金額amount
                    if (amount < 0) continue;//FIXME:指定額システム
                    //単純に登録されている住所と銀行を使って振込リクエスト
                    bool ok = false;
                    try {
                        // リクエスト開始
                        api.BankUpdate();
                        ok = api.Withdraw(amount);
                        //bool saveflag = api.SaveBankAccounts(ic.bank, ic.address);
                        //if (saveflag) {
                        //    Log.Logger.Info("一括出金 : " + ic.account.nickname + "の銀行口座及び住所の保存に成功");
                        //    sales = api.getCurrentSales();
                        //    bool request_result = false;
                        //    if (this.radioButton5.Checked) request_result = api.DoBillRequest(ic.bank, sales.current_sales, sales.payment_fee);
                        //    else request_result = api.DoBillRequest(ic.bank, sales.current_sales - (int)this.numericUpDown2.Value, sales.payment_fee);
                        //    if (request_result) {
                        //        ok = true;
                        //        Log.Logger.Info("一括出金 : " + ic.nickname + "の振込申請に成功");
                        //    }
                        //}
                    } catch (Exception ex) {
                        Log.Logger.Error(ex.Message);
                    }
                    if (ok == false) {
                        Log.Logger.Error("一括出金 : " + ic.account.nickname + "の出金に失敗");
                        bindlist[row.Index].status = "失敗";
                    } else {
                        bindlist[row.Index].status = "成功";
                    }
                } else {
                    //方法2
                    //MainForm.Account account = new AccountDBHelper().getAccountFromSellerid(api.sellerid);
                    //if (account.defaultbankaddressId < 0) throw new Exception("default_bankが設定されていない");
                    //var default_data = new DefaultBankAddressBankDBHelper().selectDefaultBankAddress(account.defaultbankaddressId);
                    //bool use_address = default_data.use_address;
                    //var sales = api.getCurrentSales();
                    //if (sales.current_sales < minexhibit_money) continue;
                    //if (sales.current_sales - (int)this.numericUpDown2.Value < 0) continue;
                    //bool ok = false;
                    //try {
                    //    string address_id = "";
                    //    if (default_data.bank == null) throw new Exception("dafault_bankが無効");
                    //    if (use_address) {
                    //        //住所使用する場合
                    //        if (default_data.address == null) throw new Exception("dafault_addressが無効");
                    //        //まず住所一覧を取得
                    //        List<MercariAPI.Address> address_list = api.getDeliverAddressList();
                    //        //住所一覧にデフォルトの住所があるかを調べる
                    //        foreach (var ad in address_list) {
                    //            if (MercariAPI.Address.compare(ad, default_data.address)) {
                    //                address_id = ad.id.ToString();
                    //            }
                    //        }
                    //        if (string.IsNullOrEmpty(address_id)) {
                    //            //デフォルト住所がないので新たに住所を登録
                    //            address_list = api.addDeliverAddress(default_data.address);
                    //            //登録した中からIDを取り出す
                    //            foreach (var ad in address_list) {
                    //                if (MercariAPI.Address.compare(ad, default_data.address)) {
                    //                    address_id = ad.id.ToString();
                    //                }
                    //            }
                    //        }
                    //    } else {
                    //        //住所を使用しない場合, 登録されている住所を使用する
                    //        address_id = ic.address.id.ToString();
                    //    }
                    //    if (address_id == "") throw new Exception("デフォルト住所を追加したあとIDの取得失敗");
                    //    bool saveflag = api.SaveBankAccounts(default_data.bank, address_id);
                    //    bool request_result = false;
                    //    if (saveflag) {
                    //        Log.Logger.Info("一括出金 : " + ic.account.nickname + "の銀行口座及び住所の保存に成功");
                    //        sales = api.getCurrentSales();
                    //        if (this.radioButton5.Checked) request_result = api.DoBillRequest(default_data.bank, sales.current_sales, sales.payment_fee);
                    //        else request_result = api.DoBillRequest(default_data.bank, sales.current_sales - (int)this.numericUpDown2.Value, sales.payment_fee);
                    //        if (request_result) {
                    //            Log.Logger.Info("一括出金 : " + ic.nickname + "の振込申請に成功");
                    //        }
                    //    }
                    //    //最後にダミーを戻す
                    //    bool saveflag2 = api.SaveBankAccounts(ic.bank, ic.address.id.ToString());
                    //    if (request_result && saveflag2) {
                    //        ok = true;
                    //    }
                    //} catch (Exception ex) {
                    //    Log.Logger.Error(ex.Message);
                    //}
                    //if (ok == false) {
                    //    Log.Logger.Error("一括出金 : " + ic.account.nickname + "の出金に失敗");
                    //    bindlist[row.Index].status = "失敗";
                    //    var uriage = api.getCurrentSales();
                    //    if (uriage != null) bindlist[row.Index].uriage = uriage.current_sales;
                    //} else {
                    //    Log.Logger.Info("一括出金 : " + ic.account.nickname + "の出金に成功");
                    //    bindlist[row.Index].status = "成功";
                    //    var uriage = api.getCurrentSales();
                    //    if (uriage != null) bindlist[row.Index].uriage = uriage.current_sales;
                    //}
                }
                System.Threading.Thread.Sleep(Settings.getIkkatuShukkinInterval() * 1000);
            }
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
