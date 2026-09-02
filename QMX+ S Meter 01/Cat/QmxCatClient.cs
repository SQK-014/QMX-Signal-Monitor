using System;
using System.IO.Ports;
using System.Text;

namespace QMX__S_Meter_01.Cat
{
    internal class QmxCatClient
    {
        private readonly SerialPort _port;

        public QmxCatClient(string portName, int baud = 115200)
        {
            _port = new SerialPort(portName, baud, Parity.None, 8, StopBits.One);
            _port.NewLine = ";";           // TS‑480/QMX+ は終端が ;
            _port.Encoding = Encoding.ASCII;
            _port.DtrEnable = true;
            _port.RtsEnable = true;
        }

        public void Open()
        {
            if (!_port.IsOpen)
                _port.Open();
        }

        public void Close()
        {
            if (_port.IsOpen)
                _port.Close();
        }

        public string Send(string cmd)
        {
            if (!_port.IsOpen)
                throw new InvalidOperationException("CATポートが開いていません。");

            _port.Write(cmd);

            // 終端 ; まで読む（TS‑480/QMX+ 正式仕様）
            string resp = _port.ReadLine();
            return resp + ";";   // ReadLine は ; を含まないので付加
        }
    }
}
