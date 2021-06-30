namespace XBMC_Remote
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnSave = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.LblIp = new System.Windows.Forms.Label();
            this.LblPort = new System.Windows.Forms.Label();
            this.LblUser = new System.Windows.Forms.Label();
            this.LblPassword = new System.Windows.Forms.Label();
            this.TBIp = new System.Windows.Forms.TextBox();
            this.TBPort = new System.Windows.Forms.TextBox();
            this.TBUser = new System.Windows.Forms.TextBox();
            this.TBPassword = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BtnSave
            // 
            this.BtnSave.Location = new System.Drawing.Point(115, 89);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(75, 23);
            this.BtnSave.TabIndex = 0;
            this.BtnSave.Text = "Save";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(214, 89);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 1;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // LblIp
            // 
            this.LblIp.AutoSize = true;
            this.LblIp.Location = new System.Drawing.Point(14, 14);
            this.LblIp.Name = "LblIp";
            this.LblIp.Size = new System.Drawing.Size(20, 13);
            this.LblIp.TabIndex = 2;
            this.LblIp.Text = "IP:";
            // 
            // LblPort
            // 
            this.LblPort.AutoSize = true;
            this.LblPort.Location = new System.Drawing.Point(271, 11);
            this.LblPort.Name = "LblPort";
            this.LblPort.Size = new System.Drawing.Size(29, 13);
            this.LblPort.TabIndex = 3;
            this.LblPort.Text = "Port:";
            // 
            // LblUser
            // 
            this.LblUser.AutoSize = true;
            this.LblUser.Location = new System.Drawing.Point(14, 40);
            this.LblUser.Name = "LblUser";
            this.LblUser.Size = new System.Drawing.Size(32, 13);
            this.LblUser.TabIndex = 4;
            this.LblUser.Text = "User:";
            // 
            // LblPassword
            // 
            this.LblPassword.AutoSize = true;
            this.LblPassword.Location = new System.Drawing.Point(14, 66);
            this.LblPassword.Name = "LblPassword";
            this.LblPassword.Size = new System.Drawing.Size(56, 13);
            this.LblPassword.TabIndex = 5;
            this.LblPassword.Text = "Password:";
            // 
            // TBIp
            // 
            this.TBIp.Location = new System.Drawing.Point(81, 11);
            this.TBIp.Name = "TBIp";
            this.TBIp.Size = new System.Drawing.Size(184, 20);
            this.TBIp.TabIndex = 6;
            // 
            // TBPort
            // 
            this.TBPort.Location = new System.Drawing.Point(306, 11);
            this.TBPort.Name = "TBPort";
            this.TBPort.Size = new System.Drawing.Size(85, 20);
            this.TBPort.TabIndex = 7;
            // 
            // TBUser
            // 
            this.TBUser.Location = new System.Drawing.Point(81, 37);
            this.TBUser.Name = "TBUser";
            this.TBUser.Size = new System.Drawing.Size(310, 20);
            this.TBUser.TabIndex = 8;
            // 
            // TBPassword
            // 
            this.TBPassword.Location = new System.Drawing.Point(81, 63);
            this.TBPassword.Name = "TBPassword";
            this.TBPassword.Size = new System.Drawing.Size(310, 20);
            this.TBPassword.TabIndex = 9;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 120);
            this.Controls.Add(this.TBPassword);
            this.Controls.Add(this.TBUser);
            this.Controls.Add(this.TBPort);
            this.Controls.Add(this.TBIp);
            this.Controls.Add(this.LblPassword);
            this.Controls.Add(this.LblUser);
            this.Controls.Add(this.LblPort);
            this.Controls.Add(this.LblIp);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnSave);
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Label LblIp;
        private System.Windows.Forms.Label LblPort;
        private System.Windows.Forms.Label LblUser;
        private System.Windows.Forms.Label LblPassword;
        private System.Windows.Forms.TextBox TBIp;
        private System.Windows.Forms.TextBox TBPort;
        private System.Windows.Forms.TextBox TBUser;
        private System.Windows.Forms.TextBox TBPassword;
    }
}