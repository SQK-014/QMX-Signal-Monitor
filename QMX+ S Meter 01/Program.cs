using System;
using System.Windows.Forms;
using QMX_S_Meter_01.UI;

namespace QMX_S_Meter_01
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}
