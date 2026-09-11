namespace QMX_S_Meter_01.UI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelSM;
        private System.Windows.Forms.Panel panelSA;
        private System.Windows.Forms.Label lblAgc;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnAgcZeroCal;
        private System.Windows.Forms.Timer timer1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panelSM = new Panel();
            panelSA = new Panel();
            lblAgc = new Label();
            btnSettings = new Button();
            btnAgcZeroCal = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            panelSA.SuspendLayout();
            SuspendLayout();
            // 
            // panelSM
            // 
            panelSM.Location = new Point(15, 2);
            panelSM.Margin = new Padding(4);
            panelSM.Name = "panelSM";
            panelSM.Size = new Size(430, 50);
            panelSM.TabIndex = 0;
            // 
            // panelSA
            // 
            panelSA.Controls.Add(lblAgc);
            panelSA.Location = new Point(15, 77);
            panelSA.Margin = new Padding(4);
            panelSA.Name = "panelSA";
            panelSA.Size = new Size(430, 50);
            panelSA.TabIndex = 1;
            // 
            // lblAgc
            // 
            lblAgc.AutoSize = true;
            lblAgc.BackColor = Color.Transparent;
            lblAgc.Font = new Font("Consolas", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAgc.ForeColor = Color.White;
            lblAgc.Location = new Point(20, 2);
            lblAgc.Name = "lblAgc";
            lblAgc.Size = new Size(28, 15);
            lblAgc.TabIndex = 0;
            lblAgc.Text = "AGC";
            // 
            // btnSettings
            // 
            btnSettings.Location = new Point(16, 144);
            btnSettings.Margin = new Padding(4);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(77, 26);
            btnSettings.TabIndex = 2;
            btnSettings.Text = "COM Port";
            btnSettings.UseVisualStyleBackColor = true;
            btnSettings.Click += btnSettings_Click;
            // 
            // btnAgcZeroCal
            // 
            btnAgcZeroCal.Location = new Point(123, 144);
            btnAgcZeroCal.Margin = new Padding(4);
            btnAgcZeroCal.Name = "btnAgcZeroCal";
            btnAgcZeroCal.Size = new Size(108, 26);
            btnAgcZeroCal.TabIndex = 3;
            btnAgcZeroCal.Text = "AGC Zero Cal";
            btnAgcZeroCal.UseVisualStyleBackColor = true;
            btnAgcZeroCal.Click += btnAgcZeroCal_Click;
            // 
            // timer1
            // 
            timer1.Interval = 500;
            timer1.Tick += timer1_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(458, 172);
            Controls.Add(btnAgcZeroCal);
            Controls.Add(btnSettings);
            Controls.Add(panelSA);
            Controls.Add(panelSM);
            Margin = new Padding(4);
            Name = "Form1";
            Text = "QMX Signal Monitor";
            Load += Form1_Load;
            panelSA.ResumeLayout(false);
            panelSA.PerformLayout();
            ResumeLayout(false);
        }
    }
}