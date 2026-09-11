using System;
using System.Drawing;
using System.Windows.Forms;

namespace QMX_S_Meter_01.UI
{
    public partial class WarningForm : Form
    {
        private const int Padding = 20;
        private const int MaxTextWidth = 400; // ★この幅で折り返す

        public WarningForm()
        {
            InitializeComponent();

            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;

            SetMessage(
                "QMX+ is not powered on.\n" +
                "Please turn on the QMX+.");
        }

        // ★TextRenderer.MeasureTextで実際の折り返し後サイズを正確に測定し、
        //   その結果を使ってlabelとフォームのサイズを直接設定する
        //   (AutoSize/PreferredWidth/PreferredHeightには頼らない)
        public void SetMessage(string message)
        {
            label1.Text = message;

            Size measured = TextRenderer.MeasureText(
                message,
                label1.Font,
                new Size(MaxTextWidth, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);

            label1.AutoSize = false;
            label1.Location = new Point(Padding, Padding);
            label1.Size = new Size(Math.Min(measured.Width, MaxTextWidth), measured.Height);

            this.ClientSize = new Size(
                label1.Width + Padding * 2,
                label1.Height + Padding * 2);
        }

        private void WarningForm_Load(object sender, EventArgs e)
        {
            // 何もしない：電源ON判定は Form1 側で行う
        }
    }
}