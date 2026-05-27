using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WorkspaceLauncher.Models
{
    /// <summary>
    /// Windows Shell32 API를 활용하여 파일/바로가기의 아이콘을 WPF ImageSource로 추출합니다.
    /// 추가 NuGet 패키지 없이 P/Invoke만으로 동작합니다.
    /// </summary>
    public static class IconHelper
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(
            string pszPath, uint dwFileAttributes,
            ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        private const uint SHGFI_ICON = 0x000000100;
        private const uint SHGFI_LARGEICON = 0x000000000;

        /// <summary>
        /// 지정된 파일 경로에서 아이콘을 추출하여 WPF ImageSource로 반환합니다.
        /// .exe, .lnk(바로가기), 폴더 경로 등을 지원합니다.
        /// </summary>
        public static ImageSource? GetIconFromPath(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return null;

                if (!System.IO.File.Exists(filePath) && !System.IO.Directory.Exists(filePath))
                    return null;

                SHFILEINFO shinfo = new SHFILEINFO();
                IntPtr result = SHGetFileInfo(
                    filePath, 0, ref shinfo,
                    (uint)Marshal.SizeOf(shinfo),
                    SHGFI_ICON | SHGFI_LARGEICON);

                if (result == IntPtr.Zero || shinfo.hIcon == IntPtr.Zero)
                    return null;

                ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                    shinfo.hIcon,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                DestroyIcon(shinfo.hIcon);

                if (imageSource.CanFreeze)
                    imageSource.Freeze();

                return imageSource;
            }
            catch
            {
                return null;
            }
        }
    }
}
