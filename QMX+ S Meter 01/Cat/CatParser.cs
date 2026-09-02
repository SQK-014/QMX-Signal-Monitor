namespace QMX__S_Meter_01.Cat
{
    internal static class CatParser
    {
        public static int ParseSm(string resp)
        {
            // 例: "SM030;"
            string num = resp.Substring(2, resp.Length - 3);
            return int.Parse(num);
        }

        public static int ParseSa(string resp)
        {
            // 例: "SA050;"
            string num = resp.Substring(2, resp.Length - 3);
            return int.Parse(num);
        }
    }
}
