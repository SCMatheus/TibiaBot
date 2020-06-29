using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using BotTibia.Elementos;

namespace BotTibia.Acoes
{
    public static class CapturaTela
    {
        private static Bitmap telaInteira = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format24bppRgb);
        public static Bitmap CapturaDeTela()
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Graphics g = Graphics.FromImage(telaInteira))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
            return telaInteira;
        }

        public static Bitmap CortaTela(Bitmap tela)
        {
            var cropArea = new RectangleF((tela.Width/2), 0, (tela.Width/2), tela.Height);
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
