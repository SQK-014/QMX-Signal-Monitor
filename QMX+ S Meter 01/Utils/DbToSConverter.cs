namespace QMX__S_Meter_01.Utils
{
    internal static class DbToSConverter
    {
        public static double DbmToS(double dbm)
        {
            // ITU S9 = -73 dBm
            if (dbm >= -73)
                return 9 + ((dbm + 73) / 10.0);   // ★正しい式：S9以降は10dB/step

            // S0〜S9は6dB/step
            return (dbm + 127) / 6.0;
        }
    }
}
