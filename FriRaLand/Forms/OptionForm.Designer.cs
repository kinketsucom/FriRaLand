namespace RakuLand.Forms {
    partial class OptionForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.panel2 = new System.Windows.Forms.Panel();
            this.NotificationIntercalNumeric = new System.Windows.Forms.NumericUpDown();
            this.label24 = new System.Windows.Forms.Label();
            this.useNotificationCheckbox = new System.Windows.Forms.CheckBox();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NotificationIntercalNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.NotificationIntercalNumeric);
            this.panel2.Controls.Add(this.label24);
            this.panel2.Enabled = false;
            this.panel2.Location = new System.Drawing.Point(41, 43);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(377, 38);
            this.panel2.TabIndex = 40;
            // 
            // NotificationIntercalNumeric
            // 
            this.NotificationIntercalNumeric.Location = new System.Drawing.Point(5, 8);
            this.NotificationIntercalNumeric.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.NotificationIntercalNumeric.Name = "NotificationIntercalNumeric";
            this.NotificationIntercalNumeric.Size = new System.Drawing.Size(49, 22);
            this.NotificationIntercalNumeric.TabIndex = 34;
            this.NotificationIntercalNumeric.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("メイリオ", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(60, 4);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(233, 25);
            this.label24.TabIndex = 35;
            this.label24.Text = "分おきに新着通知を確認する";
            // 
            // useNotificationCheckbox
            // 
            this.useNotificationCheckbox.AutoSize = true;
            this.useNotificationCheckbox.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.useNotificationCheckbox.Location = new System.Drawing.Point(13, 13);
            this.useNotificationCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.useNotificationCheckbox.Name = "useNotificationCheckbox";
            this.useNotificationCheckbox.Size = new System.Drawing.Size(187, 29);
            this.useNotificationCheckbox.TabIndex = 39;
            this.useNotificationCheckbox.Text = "通知機能を使用する";
            this.useNotificationCheckbox.UseVisualStyleBackColor = true;
            this.useNotificationCheckbox.CheckedChanged += new System.EventHandler(this.useNotificationCheckbox_CheckedChanged);
            // 
            // OptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 253);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.useNotificationCheckbox);
            this.Name = "OptionForm";
            this.Text = "OptionForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OptionForm_FormClosing);
            this.Load += new System.EventHandler(this.OptionForm_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NotificationIntercalNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.NumericUpDown NotificationIntercalNumeric;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.CheckBox useNotificationCheckbox;
    }
}