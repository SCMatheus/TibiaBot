using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BotTibia.Enum;

namespace BotTibia.Actions.Events
{
    public static class ClickEvent
    {
        private const int WM_LBUTTONDOWN = 0x201; //Left mousebutton down
        private const int WM_LBUTTONUP = 0x202;   //Left mousebutton up
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;
        private const int WM_MOUSEMOVE = 0x0200;
        private static readonly IntPtr MK_LBUTTON = new IntPtr(0x0001);


        [DllImport("User32.DLL")]
        static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        public static IntPtr MakeLParam(int x, int y) => (IntPtr)((y << 16) | (x & 0xFFFF));

        public static void Click(string process, Point point, EnumMouseEvent evento)
        {
            var hwnd = Process.GetProcesses().ToList().FirstOrDefault(p => p.MainWindowTitle.Equals(process)).MainWindowHandle;
            var pointPtr = MakeLParam(point.X, point.Y);
            SendMessage(hwnd, WM_MOUSEMOVE, IntPtr.Zero, pointPtr);
            if (EnumMouseEvent.Left == evento)
            {
                SendMessage(hwnd, WM_LBUTTONDOWN, IntPtr.Zero, pointPtr);
                SendMessage(hwnd, WM_LBUTTONUP, IntPtr.Zero, pointPtr);
            }
            else if (EnumMouseEvent.Right == evento)
            {
                SendMessage(hwnd, WM_RBUTTONDOWN, IntPtr.Zero, pointPtr);
                SendMessage(hwnd, WM_RBUTTONUP, IntPtr.Zero, pointPtr);
            }
            else if (EnumMouseEvent.LeftDown == evento)
            {
                SendMessage(hwnd, WM_LBUTTONDOWN, MK_LBUTTON, pointPtr);
            }
            else if (EnumMouseEvent.LeftUp == evento)
            {
                SendMessage(hwnd, WM_LBUTTONUP, IntPtr.Zero, pointPtr);
            }
            else if (EnumMouseEvent.RightDown == evento)
            {
                SendMessage(hwnd, WM_RBUTTONDOWN, IntPtr.Zero, pointPtr);
            }
            else if (EnumMouseEvent.RightUp == evento)
            {
                SendMessage(hwnd, WM_RBUTTONUP, IntPtr.Zero, pointPtr);
            }
        }
        public static void ClickOnElement(string process, Point point, EnumMouseEvent evento)
        {
            var hwnd = Process.GetProcesses().ToList().FirstOrDefault(p => p.MainWindowTitle.Equals(process)).MainWindowHandle;
            var pointPtr = MakeLParam(point.X-8, point.Y-31);
            SendMessage(hwnd, WM_MOUSEMOVE, IntPtr.Zero, pointPtr);
            if (EnumMouseEvent.Left == evento)
            {
                SendMessage(hwnd, WM_LBUTTONDOWN, MK_LBUTTON, pointPtr);
                SendMessage(hwnd, WM_LBUTTONUP, IntPtr.Zero, pointPtr);
            }
            else if (EnumMouseEvent.Right == evento)
            {
                SendMessage(hwnd, WM_RBUTTONDOWN, IntPtr.Zero, pointPtr);
                SendMessage(hwnd, WM_RBUTTONUP, IntPtr.Zero, pointPtr);
            }
            else if (EnumMouseEvent.LeftDown == evento)
            {
                SendMessage(hwnd, WM_LBUTTONDOWN, MK_LBUTTON, pointPtr);
            }
            else if (EnumMouseEvent.LeftUp == evento)
            {
                SendMessage(hwnd, WM_LBUTTONUP, IntPtr.Zero, pointPtr);
            }
            else if (EnumMouseEvent.RightDown == evento)
            {
                SendMessage(hwnd, WM_RBUTTONDOWN, IntPtr.Zero, pointPtr);
            }
            else if (EnumMouseEvent.RightUp == evento)
            {
                SendMessage(hwnd, WM_RBUTTONUP, IntPtr.Zero, pointPtr);
            }
        }
        public static void ItemMove(string process, Point point,Point move)
        {
            var hwnd = Process.GetProcesses().ToList().FirstOrDefault(p => p.MainWindowTitle.Equals(process)).MainWindowHandle;
            var pointPtr = MakeLParam(point.X - 8, point.Y - 31);
            SendMessage(hwnd, WM_MOUSEMOVE, IntPtr.Zero, pointPtr);

            SendMessage(hwnd, WM_LBUTTONDOWN, MK_LBUTTON, pointPtr);
            SendMessage(hwnd, WM_MOUSEMOVE, IntPtr.Zero, MakeLParam(move.X - 8, move.Y - 31));
            SendMessage(hwnd, WM_LBUTTONUP, IntPtr.Zero, pointPtr);

        }
        public static void MouseMove(string process, Point point)
        {
            var hwnd = Process.GetProcesses().ToList().FirstOrDefault(p => p.MainWindowTitle.Equals(process)).MainWindowHandle;
            var pointPtr = MakeLParam(point.X, point.Y);
            SendMessage(hwnd, WM_MOUSEMOVE, IntPtr.Zero, pointPtr);
        }
        public static void MouseMoveOnElement(string process, Point point)
        {
            var hwnd = Process.GetProcesses().ToList().FirstOrDefault(p => p.MainWindowTitle.Equals(process)).MainWindowHandle;
            var pointPtr = MakeLParam(point.X, point.Y);
            SendMessage(hwnd, WM_MOUSEMOVE, IntPtr.Zero, pointPtr);
        }
    }
}
