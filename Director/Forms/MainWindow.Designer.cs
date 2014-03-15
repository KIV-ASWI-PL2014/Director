﻿namespace Director.Forms
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.scenariosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.newScenarioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.ScenarioView = new System.Windows.Forms.TreeView();
            this.TestProgress = new System.Windows.Forms.ProgressBar();
            this.ContentPanel = new System.Windows.Forms.Panel();
            this.RootContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ScenarioContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.RequestContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.processingIcons = new System.Windows.Forms.Timer(this.components);
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.czechToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.runAllScenariosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutProgram = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.addScenarioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runAllTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addRequestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runThisScenarioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteRequestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyThisRequestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeThisRequestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.RootContextMenu.SuspendLayout();
            this.ScenarioContextMenu.SuspendLayout();
            this.RequestContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.scenariosToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1148, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitMenu});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(143, 6);
            // 
            // scenariosToolStripMenuItem
            // 
            this.scenariosToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runAllScenariosToolStripMenuItem,
            this.toolStripMenuItem3,
            this.newScenarioToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exportToolStripMenuItem,
            this.exportResultsToolStripMenuItem});
            this.scenariosToolStripMenuItem.Name = "scenariosToolStripMenuItem";
            this.scenariosToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.scenariosToolStripMenuItem.Text = "Tests";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(181, 6);
            // 
            // newScenarioToolStripMenuItem
            // 
            this.newScenarioToolStripMenuItem.Name = "newScenarioToolStripMenuItem";
            this.newScenarioToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.newScenarioToolStripMenuItem.Text = "New scenario";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(181, 6);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // exportResultsToolStripMenuItem
            // 
            this.exportResultsToolStripMenuItem.Name = "exportResultsToolStripMenuItem";
            this.exportResultsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.exportResultsToolStripMenuItem.Text = "Export results";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.toolStripMenuItem4,
            this.AboutProgram});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(153, 6);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ContentPanel, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1148, 590);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.ScenarioView, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.TestProgress, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(244, 584);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Scenarios";
            // 
            // ScenarioView
            // 
            this.ScenarioView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScenarioView.Location = new System.Drawing.Point(3, 25);
            this.ScenarioView.Name = "ScenarioView";
            this.ScenarioView.Size = new System.Drawing.Size(238, 531);
            this.ScenarioView.TabIndex = 1;
            this.ScenarioView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScenarioView_MouseUp);
            // 
            // TestProgress
            // 
            this.TestProgress.Location = new System.Drawing.Point(3, 562);
            this.TestProgress.Name = "TestProgress";
            this.TestProgress.Size = new System.Drawing.Size(238, 19);
            this.TestProgress.TabIndex = 2;
            this.TestProgress.Value = 50;
            // 
            // ContentPanel
            // 
            this.ContentPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContentPanel.Location = new System.Drawing.Point(253, 3);
            this.ContentPanel.Name = "ContentPanel";
            this.ContentPanel.Size = new System.Drawing.Size(892, 584);
            this.ContentPanel.TabIndex = 1;
            // 
            // RootContextMenu
            // 
            this.RootContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem2,
            this.addScenarioToolStripMenuItem,
            this.runAllTestsToolStripMenuItem});
            this.RootContextMenu.Name = "RootContextMenu";
            this.RootContextMenu.Size = new System.Drawing.Size(145, 70);
            // 
            // ScenarioContextMenu
            // 
            this.ScenarioContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem1,
            this.addRequestToolStripMenuItem,
            this.runThisScenarioToolStripMenuItem,
            this.pasteRequestToolStripMenuItem});
            this.ScenarioContextMenu.Name = "ScenarioContextMenu";
            this.ScenarioContextMenu.Size = new System.Drawing.Size(166, 92);
            // 
            // RequestContextMenu
            // 
            this.RequestContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.copyThisRequestToolStripMenuItem,
            this.removeThisRequestToolStripMenuItem});
            this.RequestContextMenu.Name = "RequestContextMenu";
            this.RequestContextMenu.Size = new System.Drawing.Size(182, 70);
            // 
            // processingIcons
            // 
            this.processingIcons.Enabled = true;
            this.processingIcons.Interval = 300;
            this.processingIcons.Tick += new System.EventHandler(this.processingIcons_Tick);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.languagesToolStripMenuItem,
            this.toolStripMenuItem5,
            this.settingsToolStripMenuItem1});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // languagesToolStripMenuItem
            // 
            this.languagesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.czechToolStripMenuItem});
            this.languagesToolStripMenuItem.Image = global::Director.Properties.Resources.address_bar;
            this.languagesToolStripMenuItem.Name = "languagesToolStripMenuItem";
            this.languagesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.languagesToolStripMenuItem.Text = "Languages";
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Checked = true;
            this.englishToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.englishToolStripMenuItem.Image = global::Director.Properties.Resources.flag_great_britain;
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.englishToolStripMenuItem.Text = "English";
            // 
            // czechToolStripMenuItem
            // 
            this.czechToolStripMenuItem.Image = global::Director.Properties.Resources.flag_czech_republic;
            this.czechToolStripMenuItem.Name = "czechToolStripMenuItem";
            this.czechToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.czechToolStripMenuItem.Text = "Czech";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(149, 6);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = global::Director.Properties.Resources.new_file;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::Director.Properties.Resources.open;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::Director.Properties.Resources.save;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // exitMenu
            // 
            this.exitMenu.Image = global::Director.Properties.Resources.exit;
            this.exitMenu.Name = "exitMenu";
            this.exitMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.exitMenu.Size = new System.Drawing.Size(146, 22);
            this.exitMenu.Text = "Exit";
            this.exitMenu.Click += new System.EventHandler(this.exitMenu_Click);
            // 
            // runAllScenariosToolStripMenuItem
            // 
            this.runAllScenariosToolStripMenuItem.Image = global::Director.Properties.Resources.run;
            this.runAllScenariosToolStripMenuItem.Name = "runAllScenariosToolStripMenuItem";
            this.runAllScenariosToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.runAllScenariosToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.runAllScenariosToolStripMenuItem.Text = "Run All Scenarios";
            // 
            // settingsToolStripMenuItem1
            // 
            this.settingsToolStripMenuItem1.Image = global::Director.Properties.Resources.cog_edit;
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            this.settingsToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.settingsToolStripMenuItem1.Text = "Settings";
            // 
            // AboutProgram
            // 
            this.AboutProgram.Image = global::Director.Properties.Resources.information;
            this.AboutProgram.Name = "AboutProgram";
            this.AboutProgram.Size = new System.Drawing.Size(156, 22);
            this.AboutProgram.Text = "About Program";
            this.AboutProgram.Click += new System.EventHandler(this.AboutProgram_Click);
            // 
            // editToolStripMenuItem2
            // 
            this.editToolStripMenuItem2.Image = global::Director.Properties.Resources.cog_edit;
            this.editToolStripMenuItem2.Name = "editToolStripMenuItem2";
            this.editToolStripMenuItem2.Size = new System.Drawing.Size(144, 22);
            this.editToolStripMenuItem2.Text = "Edit";
            // 
            // addScenarioToolStripMenuItem
            // 
            this.addScenarioToolStripMenuItem.Image = global::Director.Properties.Resources.add;
            this.addScenarioToolStripMenuItem.Name = "addScenarioToolStripMenuItem";
            this.addScenarioToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.addScenarioToolStripMenuItem.Text = "Add Scenario";
            // 
            // runAllTestsToolStripMenuItem
            // 
            this.runAllTestsToolStripMenuItem.Image = global::Director.Properties.Resources.run;
            this.runAllTestsToolStripMenuItem.Name = "runAllTestsToolStripMenuItem";
            this.runAllTestsToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.runAllTestsToolStripMenuItem.Text = "Run all tests";
            // 
            // editToolStripMenuItem1
            // 
            this.editToolStripMenuItem1.Image = global::Director.Properties.Resources.cog_edit;
            this.editToolStripMenuItem1.Name = "editToolStripMenuItem1";
            this.editToolStripMenuItem1.Size = new System.Drawing.Size(165, 22);
            this.editToolStripMenuItem1.Text = "Edit";
            // 
            // addRequestToolStripMenuItem
            // 
            this.addRequestToolStripMenuItem.Image = global::Director.Properties.Resources.add;
            this.addRequestToolStripMenuItem.Name = "addRequestToolStripMenuItem";
            this.addRequestToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.addRequestToolStripMenuItem.Text = "Add request";
            // 
            // runThisScenarioToolStripMenuItem
            // 
            this.runThisScenarioToolStripMenuItem.Image = global::Director.Properties.Resources.run;
            this.runThisScenarioToolStripMenuItem.Name = "runThisScenarioToolStripMenuItem";
            this.runThisScenarioToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.runThisScenarioToolStripMenuItem.Text = "Run this Scenario";
            // 
            // pasteRequestToolStripMenuItem
            // 
            this.pasteRequestToolStripMenuItem.Image = global::Director.Properties.Resources.page_paste;
            this.pasteRequestToolStripMenuItem.Name = "pasteRequestToolStripMenuItem";
            this.pasteRequestToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.pasteRequestToolStripMenuItem.Text = "Paste request";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Image = global::Director.Properties.Resources.cog_edit;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // copyThisRequestToolStripMenuItem
            // 
            this.copyThisRequestToolStripMenuItem.Image = global::Director.Properties.Resources.page_copy;
            this.copyThisRequestToolStripMenuItem.Name = "copyThisRequestToolStripMenuItem";
            this.copyThisRequestToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.copyThisRequestToolStripMenuItem.Text = "Copy this request";
            // 
            // removeThisRequestToolStripMenuItem
            // 
            this.removeThisRequestToolStripMenuItem.Image = global::Director.Properties.Resources.fail;
            this.removeThisRequestToolStripMenuItem.Name = "removeThisRequestToolStripMenuItem";
            this.removeThisRequestToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.removeThisRequestToolStripMenuItem.Text = "Remove this request";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1148, 614);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Api Director";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.ResizeEnd += new System.EventHandler(this.MainWindow_ResizeEnd);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.RootContextMenu.ResumeLayout(false);
            this.ScenarioContextMenu.ResumeLayout(false);
            this.RequestContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitMenu;
        private System.Windows.Forms.ToolStripMenuItem scenariosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runAllScenariosToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem newScenarioToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportResultsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem AboutProgram;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView ScenarioView;
        private System.Windows.Forms.ProgressBar TestProgress;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip RootContextMenu;
        private System.Windows.Forms.ContextMenuStrip ScenarioContextMenu;
        private System.Windows.Forms.ContextMenuStrip RequestContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addScenarioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runAllTestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addRequestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runThisScenarioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyThisRequestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeThisRequestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteRequestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.Panel ContentPanel;
        private System.Windows.Forms.Timer processingIcons;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem czechToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem1;
    }
}