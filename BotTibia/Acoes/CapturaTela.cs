using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using BotTibia.Elementos;

namespace BotTibia.Acoes
{
    public static class CapturaTela
    {
        public static Bitmap CapturaDeTela()
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            Bitmap tela = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(tela))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
            return tela;
        }

        public static Bitmap CortaTela(Bitmap tela)
        {
            var cropArea = new RectangleF((tela.Width/3)*2, 0, (tela.Width / 3), tela.Height);
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
