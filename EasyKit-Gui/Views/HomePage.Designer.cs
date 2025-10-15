namespace EasyKit_Gui.Views
{
    partial class HomePage
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

    private System.Windows.Forms.Panel sidebarPanel;
    private System.Windows.Forms.Panel topBarPanel;
    private System.Windows.Forms.Panel mainPanel;
    private System.Windows.Forms.ListBox navListBox;
    private System.Windows.Forms.Label appTitleLabel;
    private System.Windows.Forms.Label currentDirLabel;
    private System.Windows.Forms.Button changeDirButton;

        private void InitializeComponent()
        {
            sidebarPanel = new Panel();
            navListBox = new ListBox();
            topBarPanel = new Panel();
            appTitleLabel = new Label();
            currentDirLabel = new Label();
            changeDirButton = new Button();
            mainPanel = new Panel();
            sidebarPanel.SuspendLayout();
            topBarPanel.SuspendLayout();
            SuspendLayout();
            // 
            // sidebarPanel
            // 
            sidebarPanel.BackColor = Color.FromArgb(30, 30, 40);
            sidebarPanel.Controls.Add(navListBox);
            sidebarPanel.Dock = DockStyle.Left;
            sidebarPanel.Location = new Point(0, 0);
            sidebarPanel.Name = "sidebarPanel";
            sidebarPanel.Size = new Size(180, 650);
            sidebarPanel.TabIndex = 2;
            // 
            // navListBox
            // 
            navListBox.BackColor = Color.FromArgb(30, 30, 40);
            navListBox.BorderStyle = BorderStyle.None;
            navListBox.Dock = DockStyle.Fill;
            navListBox.Font = new Font("Segoe UI", 11F);
            navListBox.ForeColor = Color.White;
            navListBox.ItemHeight = 20;
            navListBox.Items.AddRange(new object[] { "Git", "Composer", "Laravel", "Npm", "Tool Marketplace", "Settings" });
            navListBox.Location = new Point(0, 0);
            navListBox.Name = "navListBox";
            navListBox.Size = new Size(180, 650);
            navListBox.TabIndex = 0;
            // 
            // topBarPanel
            // 
            topBarPanel.BackColor = Color.FromArgb(40, 40, 60);
            topBarPanel.Controls.Add(appTitleLabel);
            topBarPanel.Controls.Add(currentDirLabel);
            topBarPanel.Controls.Add(changeDirButton);
            topBarPanel.Dock = DockStyle.Top;
            topBarPanel.Location = new Point(180, 0);
            topBarPanel.Name = "topBarPanel";
            topBarPanel.Size = new Size(820, 48);
            topBarPanel.TabIndex = 1;
            // 
            // appTitleLabel
            // 
            appTitleLabel.AutoSize = true;
            appTitleLabel.Dock = DockStyle.Left;
            appTitleLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            appTitleLabel.ForeColor = Color.White;
            appTitleLabel.Location = new Point(0, 0);
            appTitleLabel.Name = "appTitleLabel";
            appTitleLabel.Padding = new Padding(16, 8, 0, 0);
            appTitleLabel.Size = new Size(104, 38);
            appTitleLabel.TabIndex = 0;
            appTitleLabel.Text = "EasyKit";
            // 
            // currentDirLabel
            // 
            currentDirLabel.AutoSize = true;
            currentDirLabel.Dock = DockStyle.Right;
            currentDirLabel.Font = new Font("Segoe UI", 10F);
            currentDirLabel.ForeColor = Color.LightGray;
            currentDirLabel.Location = new Point(692, 0);
            currentDirLabel.Name = "currentDirLabel";
            currentDirLabel.Padding = new Padding(0, 12, 8, 0);
            currentDirLabel.Size = new Size(8, 31);
            currentDirLabel.TabIndex = 1;
            currentDirLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // changeDirButton
            // 
            changeDirButton.BackColor = Color.FromArgb(60, 80, 120);
            changeDirButton.Dock = DockStyle.Right;
            changeDirButton.FlatStyle = FlatStyle.Flat;
            changeDirButton.Font = new Font("Segoe UI", 9F);
            changeDirButton.ForeColor = Color.White;
            changeDirButton.Location = new Point(700, 0);
            changeDirButton.Margin = new Padding(0, 8, 8, 8);
            changeDirButton.Name = "changeDirButton";
            changeDirButton.Size = new Size(120, 48);
            changeDirButton.TabIndex = 2;
            changeDirButton.Text = "Change Folder";
            changeDirButton.UseVisualStyleBackColor = false;
            // 
            // mainPanel
            // 
            mainPanel.BackColor = Color.FromArgb(50, 50, 70);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(180, 48);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new Size(820, 602);
            mainPanel.TabIndex = 0;
            // 
            // HomePage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 650);
            Controls.Add(mainPanel);
            Controls.Add(topBarPanel);
            Controls.Add(sidebarPanel);
            Name = "HomePage";
            Text = "EasyKit";
            sidebarPanel.ResumeLayout(false);
            topBarPanel.ResumeLayout(false);
            topBarPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}