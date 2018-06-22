using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FriRaLand.DBHelper;

    namespace FriRaLand.Forms {
    public partial class ReservationSettingForm : Form {
        private FrilItem item;
        private MainForm mainform;
        private ReservationDBHelper reservationDBHelper;
        public ReservationSettingForm(FrilItem item, MainForm mainform) {
            InitializeComponent();
            this.mainform = mainform;
            this.item = item;
            this.item.loadImageFromFile();
            this.itemNameLabel.Text = item.item_name;
            this.itemPictureBox.Image = item.Image;
            List<MainForm.Account> accountList = new AccountDBHelper().loadAccounts();
            foreach (var ac in accountList) this.accountComboBox.Items.Add(ac);
            if (accountList.Count != 0) this.accountComboBox.SelectedIndex = 0;

            this.reservationDBHelper = new ReservationDBHelper();

            //GUI設定
            exhibitHourNumeric.Minimum = intervalHourNumeric.Minimum = autoDeleteHourNumeric.Minimum = 0;
            exhibitHourNumeric.Maximum = intervalHourNumeric.Maximum = autoDeleteHourNumeric.Maximum = 23;
            exhibitMinuteNumeric.Minimum = intervalMinuteNumeric.Minimum = autoDeleteMinuteNumeric.Minimum = 0;
            exhibitMinuteNumeric.Maximum = intervalMinuteNumeric.Maximum = autoDeleteMinuteNumeric.Maximum = 59;
            intervalDayNumeric.Minimum = 0; autoDeleteDayNumeric.Minimum = 0;
            exhibitTimeNumeric.Minimum = 1;
            exhibitTimeNumeric.Maximum = 100;
            exhibitTimeNumeric.Value = 1;
            dateTimePicker1.Value = DateTime.Now.Date;
            exhibitHourNumeric.Value = DateTime.Now.Hour;
            exhibitMinuteNumeric.Value = DateTime.Now.Minute;
        }
        public struct Status {
            public const int Unexecuted = 0;
            public const int Success = 1;
            public const int Failed = 2;
            public const string Unexecuted_str = "";
            public const string Success_str = "成功";
            public const string Failed_str = "失敗";
        }
        public static string get_exhibit_status_str(int exhibit_status) {
            if (exhibit_status == Status.Unexecuted) return Status.Unexecuted_str;
            if (exhibit_status == Status.Success) return Status.Success_str;
            if (exhibit_status == Status.Failed) return Status.Failed_str;
            return "";
        }

        public class ReservationSetting {
            public ReservationSetting() {
                exhibit_status = Status.Unexecuted;
                exhibit_status_str = Status.Unexecuted_str;
            }
            public const string dafaultDate = "1970/01/01 0:00:00";

            public int DBId;
            public int itemDBId;
            public int accountDBId;
            public DateTime exhibitDate;
            public DateTime deleteDate;
            public DateTime deleteDate2;
            public bool consider_favorite = false;
            public bool consider_comment = false;
            public bool consider_favorite2 = false;
            public bool consider_comment2 = false;
            public bool reexhibit_flag = false;
            public string itemid = "";
            public int exhibit_status; //出品成功or失敗or未実行
            public string imagepath;

            //タイマーでのみ使用しているフラグ
            public bool doexhibit_flag;
            public bool docancel_flag;
            public bool docancel_flag2;

            public string exhibit_status_str { get; set; }
            public string exhibitDateString { get; set; }
            public string deleteDateString { get; set; }
            public string deleteDateString2 { get; set; }
            public string consider_favorite_str { get; set; }
            public string consider_comment_str { get; set; }
            public string consider_favorite_str2 { get; set; }
            public string consider_comment_str2 { get; set; }
            public string reexhibit_flag_str { get; set; }
            public string itemName { get; set; }
            public Image itemImage { get; set; }
            public string accountNickName { get; set; }
            public void loadImageFromFile() {
                try {
                    if (string.IsNullOrEmpty(this.imagepath)) return;
                    var bitmap = new Bitmap(this.imagepath);
                    this.itemImage = FrilCommon.ResizeImage(bitmap, 50, 50);
                    bitmap.Dispose();
                } catch {
                    this.itemImage = null;
                }
            }
        }
        private void ReservationSettingForm_Load(object sender, EventArgs e) {

        }

        private void accountComboBox_Format(object sender, ListControlConvertEventArgs e) {
            MainForm.Account ac = (MainForm.Account)e.ListItem;
            e.Value = ac.nickname;
        }

        private void autoDeleteCheckBox_CheckedChanged(object sender, EventArgs e) {
            if (autoDeleteCheckBox.Checked) panel1.Enabled = true;
            else panel1.Enabled = false;
        }

        private void registerReservationButton_Click(object sender, EventArgs e) {
            if (this.accountComboBox.SelectedIndex < 0) return;
            List<ReservationSetting> addReservationList = new List<ReservationSetting>();
            DateTime d = this.dateTimePicker1.Value;
            DateTime exhibitTime = new DateTime(d.Year, d.Month, d.Day, (int)this.exhibitHourNumeric.Value, (int)this.exhibitMinuteNumeric.Value, 0);
            for (int i = 0; i < (int)this.exhibitTimeNumeric.Value; i++) {
                ReservationSetting rs = new ReservationSetting();
                rs.itemDBId = item.DBId;
                rs.accountDBId = ((MainForm.Account)this.accountComboBox.SelectedItem).DBId;
                rs.exhibitDate = exhibitTime;
                if (this.autoDeleteCheckBox.Checked) rs.deleteDate = exhibitTime.AddDays((int)this.autoDeleteDayNumeric.Value).AddHours((int)this.autoDeleteHourNumeric.Value).AddMinutes((int)this.autoDeleteMinuteNumeric.Value);
                else rs.deleteDate = DateTime.Parse(ReservationSetting.dafaultDate);
                if (this.autoDeleteCheckBox2.Checked) rs.deleteDate2 = exhibitTime.AddDays((int)this.autoDeleteDayNumeric2.Value).AddHours((int)this.autoDeleteHourNumeric2.Value).AddMinutes((int)this.autoDeleteMinuteNumeric2.Value);
                else rs.deleteDate2 = DateTime.Parse(ReservationSetting.dafaultDate);
                rs.consider_comment = !this.considerCommentcheckBox.Checked; //逆なので注意
                rs.consider_favorite = !this.considerFavoritecheckBox.Checked;
                rs.consider_comment2 = !this.considerCommentcheckBox2.Checked; //逆なので注意
                rs.consider_favorite2 = !this.considerFavoritecheckBox2.Checked;

                rs.reexhibit_flag = this.reExhibitCheckBox1.Checked;
                //DBに追加する
                this.reservationDBHelper.addReservation(rs);
                //次の出品時間
                exhibitTime = exhibitTime.AddDays((int)this.intervalDayNumeric.Value).AddHours((int)this.intervalHourNumeric.Value).AddMinutes((int)this.intervalMinuteNumeric.Value);
            }
            MessageBox.Show("出品予約を追加しました", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //作ったら閉じる
            //this.Close();
            //this.mainform.OnBackFromReservationSettingForm();
            //this.mainform.tabControl1.SelectedIndex = 1; //出品ページみせる
        }

        private void ReservationSettingForm_FormClosing(object sender, FormClosingEventArgs e) {
            this.mainform.OnBackFromReservationSettingForm();
        }

        private void autoDeleteCheckBox2_CheckedChanged(object sender, EventArgs e) {
            if (autoDeleteCheckBox2.Checked) panel2.Enabled = true;
            else panel2.Enabled = false;
        }

    }
}
