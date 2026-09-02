using System;
using System.Windows.Forms;
using QMX__S_Meter_01.Cat;
using QMX__S_Meter_01.Meters;

namespace QMX_S_Meter_01.UI
{
    public partial class Form1 : Form
    {
        private QmxCatClient cat;

        private SmMeterModel smModel = new SmMeterModel();
        private SaMeterModel saModel = new SaMeterModel();

        private SmMeterRenderer smRenderer;
        private SaMeterRenderer saRenderer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cat = new QmxCatClient(Properties.Settings.Default.ComPort);

            smRenderer = new SmMeterRenderer(smModel);
            saRenderer = new SaMeterRenderer(saModel);

            panelSM.Paint += (s, ev) =>
                smRenderer.Render(ev.Graphics, panelSM.ClientRectangle);

            panelSA.Paint += (s, ev) =>
                saRenderer.Render(ev.Graphics, panelSA.ClientRectangle);

            try
            {
                cat.Open();
                timer1.Start();
            }
            catch (Exception ex)
            {
                Log("CATを開けません: " + ex.Message);
            }

            Log($"Loaded AGC baseline: {Properties.Settings.Default.AgcBaseline:F1} dBm");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                string smResp = cat.Send("SM;");
                string saResp = cat.Send("SA;");

                int smRaw = CatParser.ParseSm(smResp);
                int saRaw = CatParser.ParseSa(saResp);

                // --- dBm計算 ---
                double smDbm = -73 + (smRaw - 50);
                double saDbm = -73 + (saRaw - 50);

                // --- Sメータ（SM）は補正なし（dBmのまま） ---
                smModel.Value = smDbm;

                // --- AGC（SA）のみ Zero Cal を適用 ---
                double baseline = Properties.Settings.Default.AgcBaseline;

                double saAdj = saDbm - baseline;
                if (saAdj < 0) saAdj = 0;   // AGCは負値を表示しない

                saModel.Value = saAdj;     // dB値としてレンダラーへ渡す

                panelSM.Invalidate();
                panelSA.Invalidate();
            }
            catch (Exception ex)
            {
                Log("CATエラー: " + ex.Message);
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            using (var dlg = new SettingsForm())
                dlg.ShowDialog();
        }

        // ★ AGC Zero Cal は SA の「補正前 dBm」を基準に保存する
        private void btnAgcZeroCal_Click(object sender, EventArgs e)
        {
            string saResp = cat.Send("SA;");
            int saRaw = CatParser.ParseSa(saResp);
            double saDbm = -73 + (saRaw - 50);

            Properties.Settings.Default.AgcBaseline = saDbm;
            Properties.Settings.Default.Save();

            Log($"AGC Zero Cal: baseline set to {saDbm:F1} dBm");
        }

        private void Log(string msg)
        {
            // 必要ならログ実装
        }
    }
}
