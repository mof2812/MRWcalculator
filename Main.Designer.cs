namespace MRWCalculator
{
    partial class Main
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuCharacteristics = new System.Windows.Forms.MenuStrip();
            this.SettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CellCharacteristicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InterfaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblCellType = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblBaudrate = new System.Windows.Forms.Label();
            this.menuCharacteristics.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuCharacteristics
            // 
            this.menuCharacteristics.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuCharacteristics.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuCharacteristics.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SettingsToolStripMenuItem});
            this.menuCharacteristics.Location = new System.Drawing.Point(0, 0);
            this.menuCharacteristics.Name = "menuCharacteristics";
            this.menuCharacteristics.Size = new System.Drawing.Size(1330, 33);
            this.menuCharacteristics.TabIndex = 0;
            this.menuCharacteristics.Text = "menuCharacteristics";
            // 
            // SettingsToolStripMenuItem
            // 
            this.SettingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CellCharacteristicsToolStripMenuItem,
            this.InterfaceToolStripMenuItem});
            this.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem";
            this.SettingsToolStripMenuItem.Size = new System.Drawing.Size(132, 32);
            this.SettingsToolStripMenuItem.Text = "Einstellungen";
            // 
            // CellCharacteristicsToolStripMenuItem
            // 
            this.CellCharacteristicsToolStripMenuItem.Name = "CellCharacteristicsToolStripMenuItem";
            this.CellCharacteristicsToolStripMenuItem.Size = new System.Drawing.Size(321, 34);
            this.CellCharacteristicsToolStripMenuItem.Text = "Wägezellen-Kennwerte";
            this.CellCharacteristicsToolStripMenuItem.Click += new System.EventHandler(this.CellCharacteristicsToolStripMenuItem_Click);
            // 
            // InterfaceToolStripMenuItem
            // 
            this.InterfaceToolStripMenuItem.Name = "InterfaceToolStripMenuItem";
            this.InterfaceToolStripMenuItem.Size = new System.Drawing.Size(321, 34);
            this.InterfaceToolStripMenuItem.Text = "Schnittstelle zur Wägezelle";
            this.InterfaceToolStripMenuItem.Click += new System.EventHandler(this.InterfaceToolStripMenuItem_Click);
            // 
            // lblCellType
            // 
            this.lblCellType.AutoSize = true;
            this.lblCellType.Location = new System.Drawing.Point(44, 235);
            this.lblCellType.Name = "lblCellType";
            this.lblCellType.Size = new System.Drawing.Size(70, 25);
            this.lblCellType.TabIndex = 1;
            this.lblCellType.Text = "label1";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblBaudrate
            // 
            this.lblBaudrate.AutoSize = true;
            this.lblBaudrate.Location = new System.Drawing.Point(44, 288);
            this.lblBaudrate.Name = "lblBaudrate";
            this.lblBaudrate.Size = new System.Drawing.Size(99, 25);
            this.lblBaudrate.TabIndex = 2;
            this.lblBaudrate.Text = "Baudrate";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(1330, 907);
            this.Controls.Add(this.lblBaudrate);
            this.Controls.Add(this.lblCellType);
            this.Controls.Add(this.menuCharacteristics);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuCharacteristics;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Main";
            this.Text = "MRWcalculator";
            this.menuCharacteristics.ResumeLayout(false);
            this.menuCharacteristics.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuCharacteristics;
        private System.Windows.Forms.ToolStripMenuItem SettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CellCharacteristicsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem InterfaceToolStripMenuItem;
        private System.Windows.Forms.Label lblCellType;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblBaudrate;
    }
}

