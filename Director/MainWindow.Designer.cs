namespace Director
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuNewScenario = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuOpenScenarios = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSaveScenarios = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuExitProgram = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFile});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // MenuFile
            // 
            resources.ApplyResources(this.MenuFile, "MenuFile");
            this.MenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuNewScenario,
            this.MenuOpenScenarios,
            this.MenuSaveScenarios,
            this.toolStripMenuItem1,
            this.MenuExitProgram});
            this.MenuFile.Name = "MenuFile";
            // 
            // MenuNewScenario
            // 
            resources.ApplyResources(this.MenuNewScenario, "MenuNewScenario");
            this.MenuNewScenario.Name = "MenuNewScenario";
            // 
            // MenuOpenScenarios
            // 
            resources.ApplyResources(this.MenuOpenScenarios, "MenuOpenScenarios");
            this.MenuOpenScenarios.Name = "MenuOpenScenarios";
            // 
            // MenuSaveScenarios
            // 
            resources.ApplyResources(this.MenuSaveScenarios, "MenuSaveScenarios");
            this.MenuSaveScenarios.Name = "MenuSaveScenarios";
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // MenuExitProgram
            // 
            resources.ApplyResources(this.MenuExitProgram, "MenuExitProgram");
            this.MenuExitProgram.Name = "MenuExitProgram";
            // 
            // MainWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MenuFile;
        private System.Windows.Forms.ToolStripMenuItem MenuNewScenario;
        private System.Windows.Forms.ToolStripMenuItem MenuOpenScenarios;
        private System.Windows.Forms.ToolStripMenuItem MenuSaveScenarios;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem MenuExitProgram;
    }
}

