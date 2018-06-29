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
    public partial class MessageBoxWithProgressBar : Form {
        public MessageBoxWithProgressBar(string title, string message) {
            InitializeComponent();
            this.Text = title;
            this.label1.Text = message;
            this.TopMost = true;
        }

        private void MessageBoxWithProgressBar_Load(object sender, EventArgs e) {

        }
        public void changeMessage(string message) {
            this.label1.Text = message;
        }
        public void changeProgressBarValue(int value) {
            this.progressBar1.Value = value;
        }

        private void MessageBoxWithProgressBar_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
        }
    }
}
