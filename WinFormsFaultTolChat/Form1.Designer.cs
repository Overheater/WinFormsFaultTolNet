namespace WinFormsFaultTolChat
{
    partial class Form1
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
            this.SendButton = new System.Windows.Forms.Button();
            this.MessageBox = new System.Windows.Forms.TextBox();
            this.CopyNumber = new System.Windows.Forms.NumericUpDown();
            this.Messages = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.CopyNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // SendButton
            // 
            this.SendButton.BackColor = System.Drawing.Color.SteelBlue;
            this.SendButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SendButton.Enabled = false;
            this.SendButton.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SendButton.ForeColor = System.Drawing.SystemColors.Control;
            this.SendButton.Location = new System.Drawing.Point(12, 679);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(460, 41);
            this.SendButton.TabIndex = 1;
            this.SendButton.Text = "Send";
            this.SendButton.UseVisualStyleBackColor = false;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click_1);
            // 
            // MessageBox
            // 
            this.MessageBox.AcceptsReturn = true;
            this.MessageBox.AcceptsTab = true;
            this.MessageBox.BackColor = System.Drawing.SystemColors.MenuBar;
            this.MessageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MessageBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MessageBox.Location = new System.Drawing.Point(12, 635);
            this.MessageBox.Multiline = true;
            this.MessageBox.Name = "MessageBox";
            this.MessageBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.MessageBox.Size = new System.Drawing.Size(460, 38);
            this.MessageBox.TabIndex = 2;
            this.MessageBox.TextChanged += new System.EventHandler(this.MessageBox_TextChanged);
            // 
            // CopyNumber
            // 
            this.CopyNumber.BackColor = System.Drawing.SystemColors.MenuBar;
            this.CopyNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CopyNumber.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CopyNumber.Location = new System.Drawing.Point(154, 726);
            this.CopyNumber.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.CopyNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CopyNumber.Name = "CopyNumber";
            this.CopyNumber.Size = new System.Drawing.Size(172, 29);
            this.CopyNumber.TabIndex = 3;
            this.CopyNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // Messages
            // 
            this.Messages.AcceptsTab = true;
            this.Messages.BackColor = System.Drawing.SystemColors.MenuBar;
            this.Messages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Messages.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Messages.ForeColor = System.Drawing.SystemColors.Info;
            this.Messages.Location = new System.Drawing.Point(13, 13);
            this.Messages.Name = "Messages";
            this.Messages.ReadOnly = true;
            this.Messages.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.Messages.Size = new System.Drawing.Size(459, 616);
            this.Messages.TabIndex = 4;
            this.Messages.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(484, 761);
            this.Controls.Add(this.Messages);
            this.Controls.Add(this.CopyNumber);
            this.Controls.Add(this.MessageBox);
            this.Controls.Add(this.SendButton);
            this.Name = "Form1";
            this.Text = "ChatTol";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.CopyNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button SendButton;
        private System.Windows.Forms.TextBox MessageBox;
        private System.Windows.Forms.NumericUpDown CopyNumber;
        private System.Windows.Forms.RichTextBox Messages;
    }
}

