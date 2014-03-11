namespace Director
{
    namespace Forms
    {
        partial class SplashScreen
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
            this.versionInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // versionInfo
            // 
            this.versionInfo.AutoSize = true;
            this.versionInfo.BackColor = System.Drawing.Color.Transparent;
            this.versionInfo.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.versionInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.versionInfo.ForeColor = System.Drawing.Color.Transparent;
            this.versionInfo.Location = new System.Drawing.Point(321, 9);
            this.versionInfo.Name = "versionInfo";
            this.versionInfo.Size = new System.Drawing.Size(121, 13);
            this.versionInfo.TabIndex = 0;
            this.versionInfo.Text = "Version: 0.0.1 alpha";
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Director.Properties.Resources.splashscreen;
            this.ClientSize = new System.Drawing.Size(450, 190);
            this.ControlBox = false;
            this.Controls.Add(this.versionInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximumSize = new System.Drawing.Size(450, 190);
            this.MinimumSize = new System.Drawing.Size(450, 190);
            this.Name = "SplashScreen";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

            }

            #endregion

            private System.Windows.Forms.Label versionInfo;
        }
    }
}