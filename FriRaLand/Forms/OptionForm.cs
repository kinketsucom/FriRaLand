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
    public partial class OptionForm : Form {
        private MainForm mainform;
        public OptionForm(MainForm mainform) {
            this.mainform = mainform;
            InitializeComponent();
        }
        private void OptionForm_Load(object sender, EventArgs e) {
            //minValue
            this.numericUpDown1.Minimum = 0;
            //this.numericUpDown2.Minimum = 0;
            //this.numericUpDown3.Minimum = 0;
            //this.numericUpDown4.Minimum = 0;
            //this.numericUpDown5.Minimum = 0;
            //this.numericUpDown6.Minimum = 0;
            this.TopMost = true;
            //DBから復元
            this.numericUpDown1.Value = Settings.getIkkatuShuppinInterval();
            //this.numericUpDown2.Value = Settings.getIkkatuHassouInterval();
            //this.numericUpDown3.Value = Settings.getIkkatuHyoukaInterval();
            //this.numericUpDown4.Value = Settings.getIkkatuShukkinInterval();
            //this.numericUpDown5.Value = Settings.getIkkatuTorikesiInterval();
            //this.numericUpDown6.Value = Settings.getReexhibitCheckInterval();
            this.useNotificationCheckbox.Checked = Settings.useNotification();
            this.NotificationIntercalNumeric.Value = Settings.getNotificationInterval();
        }

        private void useNotificationCheckbox_CheckedChanged(object sender, EventArgs e) {
            if (this.useNotificationCheckbox.Checked) this.panel2.Enabled = true;
            else this.panel2.Enabled = false;
        }

        private void OptionForm_FormClosing(object sender, FormClosingEventArgs e) {
            var settingDBHelper = new SettingsDBHelper();
            settingDBHelper.updateSettings(Common.ikkatu_shuppin_interval, this.numericUpDown1.Value.ToString());
            //settingDBHelper.updateSettings(Common.ikkatu_hassou_interval, this.numericUpDown2.Value.ToString());
            //settingDBHelper.updateSettings(Common.ikkatu_hyouka_interval, this.numericUpDown3.Value.ToString());
            //settingDBHelper.updateSettings(Common.ikkatu_shukkin_interval, this.numericUpDown4.Value.ToString());
            //settingDBHelper.updateSettings(Common.ikkatu_torikesi_interval, this.numericUpDown5.Value.ToString());
            //settingDBHelper.updateSettings(Common.reexhibit_check_interval, this.numericUpDown6.Value.ToString());

            settingDBHelper.updateSettings(Common.use_notification, this.useNotificationCheckbox.Checked.ToString());
            settingDBHelper.updateSettings(Common.notification_interval, this.NotificationIntercalNumeric.Value.ToString());
            //通知タイマーの再セット
            mainform.SetNotificationTimer();
        }
    }
}
