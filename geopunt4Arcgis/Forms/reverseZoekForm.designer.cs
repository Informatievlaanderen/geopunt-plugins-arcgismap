﻿namespace geopunt4Arcgis
{
    partial class reverseZoekForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(reverseZoekForm));
            this.closeBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.add2MapBtn = new System.Windows.Forms.Button();
            this.adresBox = new System.Windows.Forms.TextBox();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.infoLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.helpLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.diffBox = new System.Windows.Forms.TextBox();
            this.saveBtn = new System.Windows.Forms.Button();
            this.LARALink = new System.Windows.Forms.LinkLabel();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeBtn
            // 
            this.closeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeBtn.Location = new System.Drawing.Point(325, 78);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(75, 23);
            this.closeBtn.TabIndex = 0;
            this.closeBtn.Text = "Sluiten";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Adres:";
            // 
            // add2MapBtn
            // 
            this.add2MapBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.add2MapBtn.Enabled = false;
            this.add2MapBtn.Location = new System.Drawing.Point(220, 78);
            this.add2MapBtn.Name = "add2MapBtn";
            this.add2MapBtn.Size = new System.Drawing.Size(99, 23);
            this.add2MapBtn.TabIndex = 2;
            this.add2MapBtn.Text = "Markeer locatie";
            this.add2MapBtn.UseVisualStyleBackColor = true;
            this.add2MapBtn.Click += new System.EventHandler(this.add2MapBtn_Click);
            // 
            // adresBox
            // 
            this.adresBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.adresBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.adresBox.Location = new System.Drawing.Point(15, 25);
            this.adresBox.Name = "adresBox";
            this.adresBox.ReadOnly = true;
            this.adresBox.Size = new System.Drawing.Size(385, 20);
            this.adresBox.TabIndex = 3;
            this.adresBox.Text = "< De informatie wordt opgehaald, even geduld. >";
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.infoLabel,
            this.helpLbl});
            this.statusBar.Location = new System.Drawing.Point(0, 118);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(411, 24);
            this.statusBar.TabIndex = 5;
            this.statusBar.Text = "statusStrip1";
            // 
            // infoLabel
            // 
            this.infoLabel.ActiveLinkColor = System.Drawing.Color.Black;
            this.infoLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.infoLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(364, 19);
            this.infoLabel.Spring = true;
            this.infoLabel.Text = "   ";
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // helpLbl
            // 
            this.helpLbl.IsLink = true;
            this.helpLbl.Name = "helpLbl";
            this.helpLbl.Size = new System.Drawing.Size(32, 19);
            this.helpLbl.Text = "Help";
            this.helpLbl.Click += new System.EventHandler(this.helpLbl_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Afstand tot geprikt punt:";
            // 
            // diffBox
            // 
            this.diffBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.diffBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.diffBox.Location = new System.Drawing.Point(141, 52);
            this.diffBox.Name = "diffBox";
            this.diffBox.ReadOnly = true;
            this.diffBox.Size = new System.Drawing.Size(259, 20);
            this.diffBox.TabIndex = 7;
            // 
            // saveBtn
            // 
            this.saveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveBtn.Enabled = false;
            this.saveBtn.Location = new System.Drawing.Point(139, 78);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(75, 23);
            this.saveBtn.TabIndex = 9;
            this.saveBtn.Text = "Opslaan";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // LARALink
            // 
            this.LARALink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LARALink.AutoSize = true;
            this.LARALink.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LARALink.Location = new System.Drawing.Point(232, 104);
            this.LARALink.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LARALink.Name = "LARALink";
            this.LARALink.Size = new System.Drawing.Size(196, 13);
            this.LARALink.TabIndex = 10;
            this.LARALink.TabStop = true;
            this.LARALink.Text = "Foute adressen kunt u melden via LARA";
            this.LARALink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LARALink_LinkClicked);
            // 
            // reverseZoekForm
            // 
            this.AcceptButton = this.saveBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeBtn;
            this.ClientSize = new System.Drawing.Size(411, 142);
            this.Controls.Add(this.LARALink);
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.diffBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.adresBox);
            this.Controls.Add(this.add2MapBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.closeBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(802, 181);
            this.MinimumSize = new System.Drawing.Size(304, 181);
            this.Name = "reverseZoekForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Meest nabije adres";
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button add2MapBtn;
        private System.Windows.Forms.TextBox adresBox;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel infoLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox diffBox;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.ToolStripStatusLabel helpLbl;
        private System.Windows.Forms.LinkLabel LARALink;
    }
}