using System;
using System.Runtime.InteropServices;

namespace QMX_S_Meter_01.Utils
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DEVPROPKEY
    {
        public Guid fmtid;
        public uint pid;

        public DEVPROPKEY(string guid, uint pid)
        {
            this.fmtid = new Guid(guid);
            this.pid = pid;
        }
    }

    internal static class DeviceTreeHelper
    {
        [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
        private static extern int CM_Locate_DevNodeW(out uint pdnDevInst, string pDeviceID, uint ulFlags);

        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Get_Parent(out uint pdnDevInst, uint dnDevInst, uint ulFlags);

        [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
        private static extern int CM_Get_DevNode_PropertyW(
            uint dnDevInst, ref DEVPROPKEY PropertyKey, out ulong PropertyType,
            IntPtr PropertyBuffer, ref uint PropertyBufferSize, uint ulFlags);

        [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
        private static extern int CM_Get_DevNode_Registry_PropertyW(
            uint dnDevInst, uint ulProperty, out uint pulRegDataType,
            IntPtr buffer, ref uint pulLength, uint ulFlags);

        private const int CR_SUCCESS = 0;
        private const uint SPDRP_DEVICEDESC = 0x00000000;
        private const uint SPDRP_FRIENDLYNAME = 0x0000000C;
        private const int MaxLevelsToSearch = 5;

        // ★"Bus reported device description" = USBディスクリプタのiProduct文字列そのもの
        //   (デバイスマネージャーの「詳細」タブや、ブラウザのシリアルポート選択画面が
        //    表示しているのと同じ情報源)
        private static readonly DEVPROPKEY DEVPKEY_Device_BusReportedDeviceDesc =
            new DEVPROPKEY("540b947e-8b40-45bc-a8a2-6a0b894cbda2", 4);

        private static readonly string[] GenericNameKeywords =
        {
            "ホスト コントローラー", "Host Controller",
            "ハブ", "Hub",
            "複合デバイス", "Composite Device",
            "ルート", "Root",
            "シリアル デバイス", "Serial Device"
        };

        public static string GetParentDeviceName(string deviceInstanceId)
        {
            try
            {
                if (CM_Locate_DevNodeW(out uint devInst, deviceInstanceId, 0) != CR_SUCCESS)
                    return null;

                // ★まず、ポート自身のデバイスノードでBusReportedDeviceDescを試す
                string busName = ReadStringProperty(devInst, DEVPKEY_Device_BusReportedDeviceDesc);
                if (!string.IsNullOrEmpty(busName))
                    return busName;

                // ★見つからなければ、親を辿りながら同じプロパティを探す
                uint currentInst = devInst;
                for (int level = 0; level < MaxLevelsToSearch; level++)
                {
                    if (CM_Get_Parent(out uint parentDevInst, currentInst, 0) != CR_SUCCESS)
                        return null;

                    busName = ReadStringProperty(parentDevInst, DEVPKEY_Device_BusReportedDeviceDesc);
                    if (!string.IsNullOrEmpty(busName))
                        return busName;

                    // ★BusReportedDeviceDescが無い場合の保険として、従来のFriendlyName/DeviceDescも確認
                    string fallbackName = ReadRegistryProperty(parentDevInst, SPDRP_FRIENDLYNAME)
                                          ?? ReadRegistryProperty(parentDevInst, SPDRP_DEVICEDESC);

                    if (!string.IsNullOrEmpty(fallbackName) && IsGenericName(fallbackName))
                        return null; // ホストコントローラー等に到達 → これ以上は無意味

                    currentInst = parentDevInst;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static bool IsGenericName(string name)
        {
            foreach (string keyword in GenericNameKeywords)
            {
                if (name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        private static string ReadStringProperty(uint devInst, DEVPROPKEY key)
        {
            uint size = 1024;
            IntPtr buffer = Marshal.AllocHGlobal((int)size);
            try
            {
                int result = CM_Get_DevNode_PropertyW(
                    devInst, ref key, out _, buffer, ref size, 0);

                return result == CR_SUCCESS ? Marshal.PtrToStringUni(buffer) : null;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private static string ReadRegistryProperty(uint devInst, uint property)
        {
            uint size = 512;
            IntPtr buffer = Marshal.AllocHGlobal((int)size);
            try
            {
                int result = CM_Get_DevNode_Registry_PropertyW(
                    devInst, property, out _, buffer, ref size, 0);

                return result == CR_SUCCESS ? Marshal.PtrToStringUni(buffer) : null;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}