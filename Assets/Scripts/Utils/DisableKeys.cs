using System.Runtime.InteropServices;
using UnityEngine;

namespace Utils
{
    public class DisableKeys : MonoBehaviour
    {
        [DllImport("user32.dll")]
        private static extern bool SystemParametersInfo(
            uint action, uint param, ref STICKYKEYS vparam, uint init);

        private const uint SPI_GETSTICKYKEYS = 0x003A;
        private const uint SPI_SETSTICKYKEYS = 0x003B;

        [StructLayout(LayoutKind.Sequential)]
        private struct STICKYKEYS
        {
            public uint cbSize;
            public uint dwFlags;
        }

        private STICKYKEYS original;

        void Start()
        {
            original = new STICKYKEYS();
            original.cbSize = (uint)Marshal.SizeOf(typeof(STICKYKEYS));
            SystemParametersInfo(SPI_GETSTICKYKEYS, original.cbSize, ref original, 0);

            var sk = new STICKYKEYS();
            sk.cbSize = original.cbSize;
            sk.dwFlags = 0; // disable all Sticky Keys features
            SystemParametersInfo(SPI_SETSTICKYKEYS, sk.cbSize, ref sk, 0);
        }

        void OnApplicationQuit()
        {
            // restore original settings
            SystemParametersInfo(SPI_SETSTICKYKEYS, original.cbSize, ref original, 0);
        }
    }
}
