namespace RakuLand.Forms {
    partial class AccountManageForm {
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
            this.token_update_date_modifyButton = new System.Windows.Forms.Button();
            this.accountDataGridView1 = new System.Windows.Forms.DataGridView();
            this.nickname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.expiration_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.token_update_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.exhibit_cnt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hanbai_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.withdrawbutton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.accountTokenRefleshButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.accountAddButton = new System.Windows.Forms.Button();
            this.emailTextBox = new System.Windows.Forms.TextBox();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.accountDeleteButton = new System.Windows.Forms.Button();
            this.editGroupButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.deleteGroupButton = new System.Windows.Forms.Button();
            this.groupListBox = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.deleteAccountToNewGroup = new System.Windows.Forms.Button();
            this.groupCreateButton = new System.Windows.Forms.Button();
            this.groupNameTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.GroupBelongListBox = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.accountListBox2 = new System.Windows.Forms.ListBox();
            this.addAccountToNewGroup = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.accountDataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // token_update_date_modifyButton
            // 
            this.token_update_date_modifyButton.Location = new System.Drawing.Point(561, 54);
            this.token_update_date_modifyButton.Margin = new System.Windows.Forms.Padding(4);
            this.token_update_date_modifyButton.Name = "token_update_date_modifyButton";
            this.token_update_date_modifyButton.Size = new System.Drawing.Size(121, 55);
            this.token_update_date_modifyButton.TabIndex = 34;
            this.token_update_date_modifyButton.Text = "トークン更新日時変更";
            this.token_update_date_modifyButton.UseVisualStyleBackColor = true;
            this.token_update_date_modifyButton.Visible = false;
            this.token_update_date_modifyButton.Click += new System.EventHandler(this.token_update_date_modifyButton_Click);
            // 
            // accountDataGridView1
            // 
            this.accountDataGridView1.AllowUserToAddRows = false;
            this.accountDataGridView1.AllowUserToDeleteRows = false;
            this.accountDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.accountDataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nickname,
            this.expiration_date,
            this.token_update_date,
            this.exhibit_cnt,
            this.hanbai_num});
            this.accountDataGridView1.Location = new System.Drawing.Point(12, 46);
            this.accountDataGridView1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.accountDataGridView1.Name = "accountDataGridView1";
            this.accountDataGridView1.ReadOnly = true;
            this.accountDataGridView1.RowHeadersVisible = false;
            this.accountDataGridView1.RowTemplate.Height = 24;
            this.accountDataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.accountDataGridView1.Size = new System.Drawing.Size(525, 208);
            this.accountDataGridView1.TabIndex = 26;
            // 
            // nickname
            // 
            this.nickname.DataPropertyName = "nickname";
            this.nickname.HeaderText = "アカウント名";
            this.nickname.Name = "nickname";
            this.nickname.ReadOnly = true;
            this.nickname.Width = 110;
            // 
            // expiration_date
            // 
            this.expiration_date.DataPropertyName = "expiration_date";
            this.expiration_date.HeaderText = "トークン期限";
            this.expiration_date.Name = "expiration_date";
            this.expiration_date.ReadOnly = true;
            this.expiration_date.Width = 130;
            // 
            // token_update_date
            // 
            this.token_update_date.DataPropertyName = "token_update_date";
            this.token_update_date.HeaderText = "トークン更新日時";
            this.token_update_date.Name = "token_update_date";
            this.token_update_date.ReadOnly = true;
            this.token_update_date.Width = 130;
            // 
            // exhibit_cnt
            // 
            this.exhibit_cnt.DataPropertyName = "exhibit_cnt";
            this.exhibit_cnt.HeaderText = "出品数";
            this.exhibit_cnt.Name = "exhibit_cnt";
            this.exhibit_cnt.ReadOnly = true;
            this.exhibit_cnt.Width = 70;
            // 
            // hanbai_num
            // 
            this.hanbai_num.DataPropertyName = "hanbai_num";
            this.hanbai_num.HeaderText = "販売数";
            this.hanbai_num.Name = "hanbai_num";
            this.hanbai_num.ReadOnly = true;
            this.hanbai_num.Width = 70;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(559, 266);
            this.button6.Margin = new System.Windows.Forms.Padding(4);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(121, 29);
            this.button6.TabIndex = 32;
            this.button6.Text = "下へ";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Visible = false;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(559, 230);
            this.button5.Margin = new System.Windows.Forms.Padding(4);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(121, 29);
            this.button5.TabIndex = 31;
            this.button5.Text = "上へ";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            // 
            // withdrawbutton
            // 
            this.withdrawbutton.Location = new System.Drawing.Point(559, 117);
            this.withdrawbutton.Margin = new System.Windows.Forms.Padding(4);
            this.withdrawbutton.Name = "withdrawbutton";
            this.withdrawbutton.Size = new System.Drawing.Size(121, 29);
            this.withdrawbutton.TabIndex = 28;
            this.withdrawbutton.Text = "出金";
            this.withdrawbutton.UseVisualStyleBackColor = true;
            this.withdrawbutton.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(559, 154);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 29);
            this.button1.TabIndex = 29;
            this.button1.Text = "プロフ画像変更";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // accountTokenRefleshButton
            // 
            this.accountTokenRefleshButton.Location = new System.Drawing.Point(561, 13);
            this.accountTokenRefleshButton.Margin = new System.Windows.Forms.Padding(4);
            this.accountTokenRefleshButton.Name = "accountTokenRefleshButton";
            this.accountTokenRefleshButton.Size = new System.Drawing.Size(121, 29);
            this.accountTokenRefleshButton.TabIndex = 27;
            this.accountTokenRefleshButton.Text = "トークン更新";
            this.accountTokenRefleshButton.UseVisualStyleBackColor = true;
            this.accountTokenRefleshButton.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.accountAddButton);
            this.groupBox1.Controls.Add(this.emailTextBox);
            this.groupBox1.Controls.Add(this.passwordTextBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 259);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(667, 106);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "新規アカウント追加";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 40);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "メールアドレス";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 68);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "パスワード";
            // 
            // accountAddButton
            // 
            this.accountAddButton.Location = new System.Drawing.Point(549, 64);
            this.accountAddButton.Margin = new System.Windows.Forms.Padding(4);
            this.accountAddButton.Name = "accountAddButton";
            this.accountAddButton.Size = new System.Drawing.Size(113, 29);
            this.accountAddButton.TabIndex = 13;
            this.accountAddButton.Text = "追加";
            this.accountAddButton.UseVisualStyleBackColor = true;
            this.accountAddButton.Click += new System.EventHandler(this.accountAddButton_Click);
            // 
            // emailTextBox
            // 
            this.emailTextBox.Location = new System.Drawing.Point(120, 36);
            this.emailTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.emailTextBox.Name = "emailTextBox";
            this.emailTextBox.Size = new System.Drawing.Size(405, 22);
            this.emailTextBox.TabIndex = 11;
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(120, 68);
            this.passwordTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(405, 22);
            this.passwordTextBox.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 15);
            this.label1.TabIndex = 25;
            this.label1.Text = "アカウント一覧";
            // 
            // accountDeleteButton
            // 
            this.accountDeleteButton.Location = new System.Drawing.Point(559, 194);
            this.accountDeleteButton.Margin = new System.Windows.Forms.Padding(4);
            this.accountDeleteButton.Name = "accountDeleteButton";
            this.accountDeleteButton.Size = new System.Drawing.Size(121, 29);
            this.accountDeleteButton.TabIndex = 30;
            this.accountDeleteButton.Text = "削除";
            this.accountDeleteButton.UseVisualStyleBackColor = true;
            this.accountDeleteButton.Visible = false;
            // 
            // editGroupButton
            // 
            this.editGroupButton.Location = new System.Drawing.Point(470, 466);
            this.editGroupButton.Margin = new System.Windows.Forms.Padding(4);
            this.editGroupButton.Name = "editGroupButton";
            this.editGroupButton.Size = new System.Drawing.Size(121, 29);
            this.editGroupButton.TabIndex = 41;
            this.editGroupButton.Text = "編集";
            this.editGroupButton.UseVisualStyleBackColor = true;
            this.editGroupButton.Click += new System.EventHandler(this.editGroupButton_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(470, 430);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(121, 29);
            this.button2.TabIndex = 40;
            this.button2.Text = "下へ";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(470, 395);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(121, 29);
            this.button3.TabIndex = 39;
            this.button3.Text = "上へ";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            // 
            // deleteGroupButton
            // 
            this.deleteGroupButton.Location = new System.Drawing.Point(470, 501);
            this.deleteGroupButton.Margin = new System.Windows.Forms.Padding(4);
            this.deleteGroupButton.Name = "deleteGroupButton";
            this.deleteGroupButton.Size = new System.Drawing.Size(121, 29);
            this.deleteGroupButton.TabIndex = 37;
            this.deleteGroupButton.Text = "削除";
            this.deleteGroupButton.UseVisualStyleBackColor = true;
            this.deleteGroupButton.Click += new System.EventHandler(this.deleteGroupButton_Click);
            // 
            // groupListBox
            // 
            this.groupListBox.FormattingEnabled = true;
            this.groupListBox.ItemHeight = 15;
            this.groupListBox.Location = new System.Drawing.Point(35, 420);
            this.groupListBox.Margin = new System.Windows.Forms.Padding(4);
            this.groupListBox.Name = "groupListBox";
            this.groupListBox.Size = new System.Drawing.Size(425, 109);
            this.groupListBox.TabIndex = 36;
            this.groupListBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.groupListBox_Format);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.deleteAccountToNewGroup);
            this.groupBox2.Controls.Add(this.groupCreateButton);
            this.groupBox2.Controls.Add(this.groupNameTextBox);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.GroupBelongListBox);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.accountListBox2);
            this.groupBox2.Controls.Add(this.addAccountToNewGroup);
            this.groupBox2.Location = new System.Drawing.Point(27, 537);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(556, 301);
            this.groupBox2.TabIndex = 38;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "グループ作成・編集";
            // 
            // deleteAccountToNewGroup
            // 
            this.deleteAccountToNewGroup.Location = new System.Drawing.Point(211, 179);
            this.deleteAccountToNewGroup.Margin = new System.Windows.Forms.Padding(4);
            this.deleteAccountToNewGroup.Name = "deleteAccountToNewGroup";
            this.deleteAccountToNewGroup.Size = new System.Drawing.Size(113, 41);
            this.deleteAccountToNewGroup.TabIndex = 24;
            this.deleteAccountToNewGroup.Text = "削除\r\n ←";
            this.deleteAccountToNewGroup.UseVisualStyleBackColor = true;
            this.deleteAccountToNewGroup.Click += new System.EventHandler(this.deleteAccountToNewGroup_Click);
            // 
            // groupCreateButton
            // 
            this.groupCreateButton.Location = new System.Drawing.Point(413, 268);
            this.groupCreateButton.Margin = new System.Windows.Forms.Padding(4);
            this.groupCreateButton.Name = "groupCreateButton";
            this.groupCreateButton.Size = new System.Drawing.Size(113, 29);
            this.groupCreateButton.TabIndex = 27;
            this.groupCreateButton.Text = "作成";
            this.groupCreateButton.UseVisualStyleBackColor = true;
            this.groupCreateButton.Click += new System.EventHandler(this.groupCreateButton_Click);
            // 
            // groupNameTextBox
            // 
            this.groupNameTextBox.Location = new System.Drawing.Point(91, 21);
            this.groupNameTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.groupNameTextBox.Name = "groupNameTextBox";
            this.groupNameTextBox.Size = new System.Drawing.Size(232, 22);
            this.groupNameTextBox.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 25);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 15);
            this.label7.TabIndex = 19;
            this.label7.Text = "グループ名";
            // 
            // GroupBelongListBox
            // 
            this.GroupBelongListBox.FormattingEnabled = true;
            this.GroupBelongListBox.ItemHeight = 15;
            this.GroupBelongListBox.Location = new System.Drawing.Point(332, 75);
            this.GroupBelongListBox.Margin = new System.Windows.Forms.Padding(4);
            this.GroupBelongListBox.Name = "GroupBelongListBox";
            this.GroupBelongListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.GroupBelongListBox.Size = new System.Drawing.Size(193, 184);
            this.GroupBelongListBox.TabIndex = 26;
            this.GroupBelongListBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.newGroupListBox_Format);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(329, 54);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(160, 15);
            this.label6.TabIndex = 25;
            this.label6.Text = "グループに属するアカウント";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 54);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 15);
            this.label5.TabIndex = 21;
            this.label5.Text = "アカウント一覧";
            // 
            // accountListBox2
            // 
            this.accountListBox2.FormattingEnabled = true;
            this.accountListBox2.ItemHeight = 15;
            this.accountListBox2.Location = new System.Drawing.Point(8, 72);
            this.accountListBox2.Margin = new System.Windows.Forms.Padding(4);
            this.accountListBox2.Name = "accountListBox2";
            this.accountListBox2.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.accountListBox2.Size = new System.Drawing.Size(193, 184);
            this.accountListBox2.TabIndex = 22;
            this.accountListBox2.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.accountListBox_Format);
            // 
            // addAccountToNewGroup
            // 
            this.addAccountToNewGroup.Location = new System.Drawing.Point(211, 130);
            this.addAccountToNewGroup.Margin = new System.Windows.Forms.Padding(4);
            this.addAccountToNewGroup.Name = "addAccountToNewGroup";
            this.addAccountToNewGroup.Size = new System.Drawing.Size(113, 41);
            this.addAccountToNewGroup.TabIndex = 23;
            this.addAccountToNewGroup.Text = "追加\r\n →";
            this.addAccountToNewGroup.UseVisualStyleBackColor = true;
            this.addAccountToNewGroup.Click += new System.EventHandler(this.addAccountToNewGroup_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 401);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 15);
            this.label4.TabIndex = 35;
            this.label4.Text = "グループ一覧";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            // 
            // AccountManageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 849);
            this.Controls.Add(this.editGroupButton);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.deleteGroupButton);
            this.Controls.Add(this.groupListBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.token_update_date_modifyButton);
            this.Controls.Add(this.accountDataGridView1);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.withdrawbutton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.accountTokenRefleshButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.accountDeleteButton);
            this.Name = "AccountManageForm";
            this.Text = "AccountManageForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AccountManageForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AccountManageForm_FormClosed);
            this.Load += new System.EventHandler(this.AccountManageForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.accountDataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button token_update_date_modifyButton;
        private System.Windows.Forms.DataGridView accountDataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn nickname;
        private System.Windows.Forms.DataGridViewTextBoxColumn expiration_date;
        private System.Windows.Forms.DataGridViewTextBoxColumn token_update_date;
        private System.Windows.Forms.DataGridViewTextBoxColumn exhibit_cnt;
        private System.Windows.Forms.DataGridViewTextBoxColumn hanbai_num;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button withdrawbutton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button accountTokenRefleshButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button accountAddButton;
        private System.Windows.Forms.TextBox emailTextBox;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button accountDeleteButton;
        private System.Windows.Forms.Button editGroupButton;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button deleteGroupButton;
        private System.Windows.Forms.ListBox groupListBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button deleteAccountToNewGroup;
        private System.Windows.Forms.Button groupCreateButton;
        private System.Windows.Forms.TextBox groupNameTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListBox GroupBelongListBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox accountListBox2;
        private System.Windows.Forms.Button addAccountToNewGroup;
        private System.Windows.Forms.Label label4;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}