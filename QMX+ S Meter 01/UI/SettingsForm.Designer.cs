namespace QMX_S_Meter_01.UI
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label labelPrompt;
        private System.Windows.Forms.ComboBox comboBoxPorts;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;

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
            this.labelPrompt = new System.Windows.Forms.Label();
            this.comboBoxPorts = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // labelPrompt
            //
            this.labelPrompt.AutoSize = true;
            this.labelPrompt.Font = new System.Drawing.Font("Yu Gothic UI", 10F);
            this.labelPrompt.Location = new System.Drawing.Point(20, 20);
            this.labelPrompt.Name = "labelPrompt";
            this.labelPrompt.Size = new System.Drawing.Size(120, 20);
            this.labelPrompt.TabIndex = 0;
            this.labelPrompt.Text = "Select COM Port:";
            //
            // comboBoxPorts
            //
            this.comboBoxPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPorts.DropDownWidth = 400;
            this.comboBoxPorts.Font = new System.Drawing.Font("Yu Gothic UI", 10F);
            this.comboBoxPorts.Location = new System.Drawing.Point(20, 50);
            this.comboBoxPorts.Name = "comboBoxPorts";
            this.comboBoxPorts.Size = new System.Drawing.Size(340, 28);
            this.comboBoxPorts.TabIndex = 1;
            //
            // btnRefresh
            //
            this.btnRefresh.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.btnRefresh.Location = new System.Drawing.Point(370, 49);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(80, 30);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // btnOk
            //
            this.btnOk.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.btnOk.Location = new System.Drawing.Point(280, 100);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(80, 32);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // btnCancel
            //
            this.btnCancel.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.btnCancel.Location = new System.Drawing.Point(370, 100);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 32);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // SettingsForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 150);
            this.Controls.Add(this.labelPrompt);
            this.Controls.Add(this.comboBoxPorts);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Name = "SettingsForm";
            this.Text = "COM Port Settings";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}