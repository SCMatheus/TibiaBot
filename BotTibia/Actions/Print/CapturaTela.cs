using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BotTibia.Classes;

namespace BotTibia.Actions.Print
{
    public static class CapturaTela
    {
        private static Bitmap telaInteira;
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        public static Bitmap CaptureWindow(string processTitle)
        {
            Process[] processlist = Process.GetProcesses();
            var hWnd = processlist.First(proc => proc.MainWindowTitle.Equals(processTitle)).MainWindowHandle;
            if (telaInteira != null)
                telaInteira.Dispose();

            Rectangle rctForm = Rectangle.Empty;

            using (Graphics grfx = Graphics.FromHdc(GetWindowDC(hWnd)))
            {
                rctForm = Rectangle.Round(grfx.VisibleClipBounds);
            }

            Bitmap pImage = new Bitmap(rctForm.Width, rctForm.Height, PixelFormat.Format24bppRgb);
            Graphics graphics = Graphics.FromImage(pImage);

            IntPtr hDC = graphics.GetHdc();
            //paint control onto graphics using provided options        
            try
            {
                PrintWindow(hWnd, hDC, (uint)0);
            }
            finally
            {
                graphics.ReleaseHdc(hDC);
            }
            telaInteira = pImage;
            graphics.Dispose();
            return pImage;
        }
        public static Bitmap CapturaDeTela()
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Graphics g = Graphics.FromImage(telaInteira))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
            return telaInteira;
        }

        public static Bitmap CortaTela(Bitmap tela, int x, int y, int width, int height)
        {
            var cropArea = new RectangleF(x, y, width, height);
            var telaCortada = tela.Clone(cropArea, tela.PixelFormat);
            return telaCortada;
        }

        public static Bitmap CortaStatusBar(Bitmap tela, PersonagemStatus status)
        {
            var cropArea = new RectangleF(status.Coordenadas.X, status.Coordenadas.Y, status.Coordenadas.Width, status.Coordenadas.Height);
            var telaCortada = tela.Clone(cropArea, tela.PixelFormat);
            return telaCortada;
        }
    }
}
