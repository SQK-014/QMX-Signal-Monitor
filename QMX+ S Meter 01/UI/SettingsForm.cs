using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using QMX_S_Meter_01.Utils;

namespace QMX_S_Meter_01.UI
{
    public partial class SettingsForm : Form
    {
        // ★表示名(例: "QMX Transceiver (COM4)")とポート名("COM4")を対応付ける
        private readonly Dictionary<string, string> displayToPortMap = new Dictionary<string, string>();

        public SettingsForm()
        {
            InitializeComponent();

            this.Text = "COM Port Settings";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            LoadPortList();
        }

        // ★WMI + cfgmgr32(親デバイス探索)を使い、COMポート名と表示名の対応表を作る
        private Dictionary<string, string> GetPortFriendlyNames()
        {
            var result = new Dictionary<string, string>();

            try
            {
                using (var searcher = new ManagementObjectSearcher(
                    "SELECT Name, PNPDeviceID FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'"))
                {
                    foreach (ManagementObject device in searcher.Get())
                    {
                        string fullName = device["Name"]?.ToString();
                        string pnpId = device["PNPDeviceID"]?.ToString();
                        if (string.IsNullOrEmpty(fullName)) continue;

                        Match match = Regex.Match(fullName, @"\(COM\d+\)$");
                        if (!match.Success) continue;

                        string portName = match.Value.Trim('(', ')');

                        // ★親デバイスの名前を優先。取れなければ元の名前("USB Serial Device"等)のまま
                        string parentName = !string.IsNullOrEmpty(pnpId)
                            ? DeviceTreeHelper.GetParentDeviceName(pnpId)
                            : null;

                        string display = !string.IsNullOrEmpty(parentName)
                            ? $"{parentName} ({portName})"
                            : fullName;

                        result[portName] = display;
                    }
                }
            }
            catch
            {
                // WMI/API失敗時は空の対応表のまま返す(呼び出し側でポート名そのままにフォールバック)
            }

            return result;
        }

        private void LoadPortList()
        {
            // ★現在の選択(表示名)から、対応するポート名を逆引きしておく
            string currentSelection = comboBoxPorts.SelectedItem as string;
            string currentPort = FindPortForDisplay(currentSelection);

            comboBoxPorts.Items.Clear();
            displayToPortMap.Clear();

            string[] ports = SerialPort.GetPortNames();

            if (ports.Length == 0)
            {
                comboBoxPorts.Items.Add("(No COM ports found)");
                comboBoxPorts.SelectedIndex = 0;
                comboBoxPorts.Enabled = false;
                btnOk.Enabled = false;
                return;
            }

            comboBoxPorts.Enabled = true;
            btnOk.Enabled = true;

            Array.Sort(ports, (a, b) => ExtractPortNumber(a).CompareTo(ExtractPortNumber(b)));

            var friendlyNames = GetPortFriendlyNames();

            foreach (string port in ports)
            {
                string display = friendlyNames.TryGetValue(port, out string name) ? name : port;
                displayToPortMap[display] = port;
                comboBoxPorts.Items.Add(display);
            }

            string savedPort = Properties.Settings.Default.ComPort;
            string savedDisplay = FindDisplayForPort(savedPort);

            if (savedDisplay != null)
            {
                comboBoxPorts.SelectedItem = savedDisplay;
            }
            else
            {
                string currentDisplay = FindDisplayForPort(currentPort);
                comboBoxPorts.SelectedItem = currentDisplay ?? comboBoxPorts.Items[0];
            }
        }

        // ★ポート名("COM4")から表示名("QMX Transceiver (COM4)")を探す
        private string FindDisplayForPort(string portName)
        {
            if (string.IsNullOrWhiteSpace(portName)) return null;

            foreach (var kv in displayToPortMap)
            {
                if (kv.Value == portName) return kv.Key;
            }
            return null;
        }

        // ★表示名からポート名を逆引きする
        private string FindPortForDisplay(string display)
        {
            if (string.IsNullOrWhiteSpace(display)) return null;
            return displayToPortMap.TryGetValue(display, out string port) ? port : null;
        }

        private int ExtractPortNumber(string portName)
        {
            string digits = new string(Array.FindAll(portName.ToCharArray(), char.IsDigit));
            return int.TryParse(digits, out int n) ? n : int.MaxValue;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPortList();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (comboBoxPorts.SelectedItem == null)
                return;

            string display = comboBoxPorts.SelectedItem.ToString();

            // ★表示名から実際のポート名("COM4"など)に変換して保存する
            string actualPort = displayToPortMap.TryGetValue(display, out string port) ? port : display;

            Properties.Settings.Default.ComPort = actualPort;
            Properties.Settings.Default.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}