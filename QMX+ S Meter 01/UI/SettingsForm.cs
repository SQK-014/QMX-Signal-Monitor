using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace QMX_S_Meter_01.UI
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            comboPorts.Items.Clear();
            comboPorts.Items.AddRange(SerialPort.GetPortNames());

            comboPorts.Text = Properties.Settings.Default.ComPort;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ComPort = comboPorts.Text;
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
