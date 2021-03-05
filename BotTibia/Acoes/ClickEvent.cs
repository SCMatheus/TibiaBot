using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BotTibia.Acoes
{
    public static class ClickEvent
    {
        private const int WM_LBUTTONDOWN = 0x201; //Left mousebutton down
        private const int WM_LBUTTONUP = 0x202;   //Left mousebutton up
        const int WM_RBUTTONDOWN = 0x0204;
        const int WM_RBUTTONUP = 0x0205;
        const int WM_MOUSEMOVE = 0x0200;

        [DllImport("User32.DLL")]
        static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        public static IntPtr MakeLParam(int x, int y) => (IntPtr)((y << 16) | (x & 0xFFFF));

        public static void PerformRightClick(IntPtr hwnd, Point point)
        {
            var pointPtr = MakeLParam(point.X, point.Y);
            SendMessage(hwnd, WM_MOUSEMOVE, IntPtr.Zero, pointPtr);
            SendMessage(hwnd, WM_RBUTTONDOWN, IntPtr.Zero, pointPtr);
            SendMessage(hwnd, WM_RBUTTONUP, IntPtr.Zero, pointPtr);
        }
        public static void PerformLeftClick(IntPtr hwnd, Point point)
        {
            var pointPtr = MakeLParam(point.X, point.Y);
            SendMessage(hwnd, WM_MOUSEMOVE, IntPtr.Zero, pointPtr);
            SendMessage(hwnd, WM_LBUTTONDOWN, IntPtr.Zero, pointPtr);
            SendMessage(hwnd, WM_LBUTTONUP, IntPtr.Zero, pointPtr);
        }
    }
}
