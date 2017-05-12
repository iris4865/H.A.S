using System;
using System.Runtime.InteropServices;

namespace Management
{
    public class ConsoleScreen
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0; // 숨기기
        const int SW_SHOW = 1;

        public static void OnScreen(bool show)
        {
            var handle = GetConsoleWindow();
            if (show)
                ShowWindow(handle, SW_SHOW);
            else
                ShowWindow(handle, SW_HIDE);
        }
    }
}
