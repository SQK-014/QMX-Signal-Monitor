namespace QMX_S_Meter_01.UI
{
    partial class WarningForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label label1;

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
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            // ★サイズはコード側(SetMessage)で都度計算して設定するため、
            //   ここでは初期値のみ。AutoSizeは使わない。
            this.label1.AutoSize = false;
            this.label1.Font = new System.Drawing.Font("Yu Gothic UI", 11F);
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(400, 100);
            this.label1.TabIndex = 0;
            this.label1.Text = "QMX+ is not powered on.\nPlease turn on the QMX+.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WarningForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 150);
            this.Controls.Add(this.label1);
            this.Name = "WarningForm";
            this.Text = "Warning";
            this.Load += new System.EventHandler(this.WarningForm_Load);
            this.ResumeLayout(false);
        }
    }
}