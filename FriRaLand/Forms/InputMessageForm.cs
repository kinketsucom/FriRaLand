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
    public partial class InputMessageForm : Form {
        public string message;
        private List<string> body_list;
        public InputMessageForm(string text) {
            InitializeComponent();
            this.label1.Text = text;
            this.DialogResult = DialogResult.Cancel;
        }

        private void InputMessageForm_Load(object sender, EventArgs e) {
            this.TopMost = true;
        }

        private void button1_Click(object sender, EventArgs e) {
            this.message = this.richTextBox1.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        public void SetComboBoxItems(List<string> titles, List<string> bodies) {
            foreach (var str in titles) this.comboBox1.Items.Add(str);
            this.body_list = bodies;
            if (this.comboBox1.Items.Count > 0) this.comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            this.richTextBox1.Text = this.body_list[this.comboBox1.SelectedIndex];
        }
    }
}
