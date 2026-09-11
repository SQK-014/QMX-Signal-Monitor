// ============================================================
// QMX+ Sメーター / AGCメーター表示アプリケーション
// QMX+ S-Meter / AGC Meter Display Application
// ------------------------------------------------------------
// 作者: 塙 薫 (JJ1JTG)
// Author: Kaoru Hanawa (JJ1JTG)
//
// 概要: QMX+とCAT通信(SM/SAコマンド)を行い、
//       Sメーターおよびアンテナ入力ゼロ基準のAGCメーターを
//       リアルタイム表示する。
// Description: Communicates with the QMX+ transceiver via CAT
//              commands (SM/SA) to display real-time S-meter
//              and zero-referenced AGC meter readings.
// ============================================================

using System;
using System.IO;
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

        private WarningForm warningForm;

        private readonly string logFilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "qmx_debug_log.txt");

        private const long MaxLogFileSizeBytes = 1 * 1024 * 1024; // ★ログファイルの上限サイズ(1MB)

        public Form1()
        {
            InitializeComponent();

            TrimLogIfTooLarge();   // ★起動時にログサイズをチェックし、大きすぎればリセット

            // ★ Shown はフォームが実際に画面表示された後に発生するイベント。
            //    ここでポートチェック/警告を行うことで、UIが必ず先に表示される。
            this.Shown += Form1_Shown;
        }

        // ★ログファイルが上限サイズを超えていたら削除し、新規に書き始められるようにする
        private void TrimLogIfTooLarge()
        {
            try
            {
                var fileInfo = new FileInfo(logFilePath);

                if (fileInfo.Exists && fileInfo.Length > MaxLogFileSizeBytes)
                {
                    File.Delete(logFilePath);
                }
            }
            catch
            {
                // ログ管理自体の失敗はアプリ動作に影響させない
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // ★ Load ではUI部品の初期化のみ行い、
            //    接続試行やダイアログ表示は一切しない(UI表示をブロックしないため)
            smRenderer = new SmMeterRenderer(smModel);
            saRenderer = new SaMeterRenderer(saModel);

            panelSM.Paint += (s, ev) =>
                smRenderer.Render(ev.Graphics, panelSM.ClientRectangle);

            panelSA.Paint += (s, ev) =>
                saRenderer.Render(ev.Graphics, panelSA.ClientRectangle);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            // ★ この時点でウィンドウは画面に表示済み。
            //    ここから初めて接続確認・警告ダイアログを出す。
            InitializeCatConnection();
        }

        // ★起動時の接続確認をここに集約
        private void InitializeCatConnection()
        {
            string savedPort = Properties.Settings.Default.ComPort;

            // ★保存されたポート名が「未設定」、または「現在のPCに実在しない」場合は
            //   同じ扱いにする。これによりCOM4のような古い/無効な値が残っていても安全。
            bool portIsValid = !string.IsNullOrWhiteSpace(savedPort)
                && Array.IndexOf(System.IO.Ports.SerialPort.GetPortNames(), savedPort) >= 0;

            if (!portIsValid)
            {
                ShowWarning(
                    "COM port is not set.\n\n" +
                    "Please:\n" +
                    " 1. Turn ON the QMX\n" +
                    " 2. Connect the USB/serial cable\n" +
                    " 3. Press the \"COM Port\" button and select the port\n\n" +
                    "(If QMX is not powered on, it will not appear in the port list.)");
                return;
            }

            TryOpenCat(savedPort, showPowerOnHint: true);
        }

        // ★指定ポートでCAT接続を試みる共通処理
        //   showPowerOnHint: 接続失敗時に「QMXの電源を確認してください」の案内を出すかどうか
        private void TryOpenCat(string portName, bool showPowerOnHint)
        {
            try
            {
                cat = new QmxCatClient(portName);
                cat.Open();
                timer1.Start();
            }
            catch (Exception ex)
            {
                cat = null;

                string detail = TranslateException(ex);

                string message = showPowerOnHint
                    ? $"Could not connect to QMX on \"{portName}\".\n\n" +
                      "Please check that:\n" +
                      " - QMX is powered ON\n" +
                      " - The USB/serial cable is connected\n\n" +
                      $"Then press \"COM Port\" to retry or select a different port.\n\n(Details: {detail})"
                    : $"Cannot open COM port \"{portName}\".\n{detail}";

                ShowWarning(message);
            }
        }

        // ★ .NET/OS由来の生の例外メッセージを、どのCOMポート名でも通用する
        //   ユーザー向け文言に変換する。特定のポート番号にハードコードしない。
        private string TranslateException(Exception ex)
        {
            switch (ex)
            {
                case FileNotFoundException _:
                    return "Could not find the COM Port.";

                case UnauthorizedAccessException _:
                    return "The COM Port is already in use by another application.";

                case IOException _ when ex.Message.Contains("Could not find file"):
                    return "Could not find the COM Port.";

                case IOException _:
                    return "The COM Port could not be opened. It may be disconnected or in use.";

                case ArgumentException _:
                    return "The COM Port name is invalid.";

                default:
                    return ex.Message;
            }
        }

        // ★非モーダルの警告フォームを表示(既に表示中ならメッセージだけ更新)
        //   ★警告内容は同時にファイルへも記録する(トラブル時の履歴確認用)
        private void ShowWarning(string message)
        {
            Log(message);

            if (warningForm == null || warningForm.IsDisposed)
            {
                warningForm = new WarningForm();
                warningForm.SetMessage(message);
                warningForm.Show(this);
            }
            else
            {
                warningForm.SetMessage(message);
                warningForm.BringToFront();
            }
        }

        // ★表示中の警告フォームを閉じる
        private void CloseWarning()
        {
            if (warningForm != null && !warningForm.IsDisposed)
            {
                warningForm.Close();
                warningForm = null;
            }
        }

        // ★現在のCAT接続を安全に閉じる
        private void CloseCat()
        {
            timer1.Stop();

            if (cat != null)
            {
                try { cat.Close(); } catch { /* 無視 */ }
                cat = null;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                string smResp = cat.Send("SM;");
                string saResp = cat.Send("SA;");

                int smRaw = CatParser.ParseSm(smResp);
                int saRaw = CatParser.ParseSa(saResp);

                double smDbm = smRaw - 127;
                double saDbm = saRaw;

                smModel.Value = smDbm;

                double baseline = Properties.Settings.Default.AgcBaseline;
                double saAdj = saDbm - baseline;
                if (saAdj < 0) saAdj = 0;

                saModel.Value = saAdj;

                panelSM.Invalidate();
                panelSA.Invalidate();
            }
            catch (Exception ex)
            {
                // 通信中に切断された場合(QMXの電源が落ちた等)はタイマーを止めて再接続を促す
                CloseCat();

                string detail = TranslateException(ex);

                ShowWarning(
                    $"Lost connection to QMX.\n\n" +
                    "Please check the power and cable, then press \"COM Port\" to reconnect.\n\n" +
                    $"(Details: {detail})");
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            CloseWarning();   // ★ボタンを押した時点で警告を消す

            using (var dlg = new SettingsForm())
                dlg.ShowDialog();

            string selectedPort = Properties.Settings.Default.ComPort;

            if (string.IsNullOrWhiteSpace(selectedPort))
                return;   // キャンセル等でポート未設定のまま

            // ★ポートが変わっていなくても、「もう一度接続を試したい」ケース
            //   (QMXの電源を入れ直した後の再試行など)に対応するため、
            //   常に閉じてから開き直す
            CloseCat();
            TryOpenCat(selectedPort, showPowerOnHint: true);
        }

        private void btnAgcZeroCal_Click(object sender, EventArgs e)
        {
            if (cat == null)
                return;

            string saResp = cat.Send("SA;");
            int saRaw = CatParser.ParseSa(saResp);
            double saDbm = saRaw;

            Properties.Settings.Default.AgcBaseline = saDbm;
            Properties.Settings.Default.Save();
        }

        private void Log(string msg)
        {
            try
            {
                string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}  {msg}{Environment.NewLine}";
                File.AppendAllText(logFilePath, line);
            }
            catch
            {
                // Ignore logging failures
            }
        }
    }
}