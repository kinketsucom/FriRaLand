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
            this.useNotificationCheckbox.Checked = Settings.useNotification();
            this.NotificationIntercalNumeric.Value = Settings.getNotificationInterval();
        }

        private void useNotificationCheckbox_CheckedChanged(object sender, EventArgs e) {
            if (this.useNotificationCheckbox.Checked) this.panel2.Enabled = true;
            else this.panel2.Enabled = false;
        }

        private void OptionForm_FormClosing(object sender, FormClosingEventArgs e) {
            var settingDBHelper = new SettingsDBHelper();
            settingDBHelper.updateSettings(Common.use_notification, this.useNotificationCheckbox.Checked.ToString());
            settingDBHelper.updateSettings(Common.notification_interval, this.NotificationIntercalNumeric.Value.ToString());
        }
    }
}
