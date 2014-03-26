namespace Director.Forms.InfoPanels
{
    partial class ScenarioPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.RunningFrequency = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.ScenarioName = new System.Windows.Forms.TextBox();
            this.ScenarioNameError = new System.Windows.Forms.Label();
            this.DText = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.DText, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(694, 470);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.RunningFrequency);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(10, 135);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.groupBox2.Size = new System.Drawing.Size(674, 50);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Running frequency";
            // 
            // RunningFrequency
            // 
            this.RunningFrequency.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RunningFrequency.FormattingEnabled = true;
            this.RunningFrequency.Items.AddRange(new object[] {
            "Never",
            "30m",
            "1h",
            "3h",
            "6h",
            "12h",
            "24h"});
            this.RunningFrequency.Location = new System.Drawing.Point(10, 18);
            this.RunningFrequency.Name = "RunningFrequency";
            this.RunningFrequency.Size = new System.Drawing.Size(654, 21);
            this.RunningFrequency.TabIndex = 0;
            this.RunningFrequency.Text = "Never";
            this.RunningFrequency.SelectedIndexChanged += new System.EventHandler(this.RunningFrequency_SelectedIndexChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(694, 50);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Image = global::Director.Properties.Resources.lightning_go;
            this.label1.Location = new System.Drawing.Point(10, 8);
            this.label1.Margin = new System.Windows.Forms.Padding(10, 8, 3, 0);
            this.label1.MinimumSize = new System.Drawing.Size(32, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 39);
            this.label1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(48, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 12, 0, 10);
            this.label2.Size = new System.Drawing.Size(195, 47);
            this.label2.TabIndex = 1;
            this.label2.Text = "Scenario settings";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(10, 60);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.groupBox1.Size = new System.Drawing.Size(674, 55);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Scenario name";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.ScenarioName, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.ScenarioNameError, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(10, 18);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(654, 32);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // ScenarioName
            // 
            this.ScenarioName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScenarioName.Location = new System.Drawing.Point(0, 0);
            this.ScenarioName.Margin = new System.Windows.Forms.Padding(0);
            this.ScenarioName.Name = "ScenarioName";
            this.ScenarioName.Size = new System.Drawing.Size(654, 20);
            this.ScenarioName.TabIndex = 1;
            this.ScenarioName.TextChanged += new System.EventHandler(this.ScenarioName_Leave);
            this.ScenarioName.Leave += new System.EventHandler(this.ScenarioName_Leave);
            // 
            // ScenarioNameError
            // 
            this.ScenarioNameError.AutoSize = true;
            this.ScenarioNameError.Dock = System.Windows.Forms.DockStyle.Right;
            this.ScenarioNameError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ScenarioNameError.ForeColor = System.Drawing.Color.Red;
            this.ScenarioNameError.Location = new System.Drawing.Point(549, 18);
            this.ScenarioNameError.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.ScenarioNameError.Name = "ScenarioNameError";
            this.ScenarioNameError.Size = new System.Drawing.Size(102, 14);
            this.ScenarioNameError.TabIndex = 2;
            this.ScenarioNameError.Text = "Name is required";
            this.ScenarioNameError.Visible = false;
            // 
            // DText
            // 
            this.DText.AutoSize = true;
            this.DText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DText.Location = new System.Drawing.Point(3, 195);
            this.DText.Name = "DText";
            this.DText.Size = new System.Drawing.Size(688, 275);
            this.DText.TabIndex = 3;
            this.DText.Text = "label3";
            // 
            // ScenarioPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ScenarioPanel";
            this.Size = new System.Drawing.Size(694, 470);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TextBox ScenarioName;
        private System.Windows.Forms.Label ScenarioNameError;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox RunningFrequency;
        private System.Windows.Forms.Label DText;

    }
}
