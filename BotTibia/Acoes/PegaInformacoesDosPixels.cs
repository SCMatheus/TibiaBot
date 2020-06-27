using System.Drawing;
using BotTibia.Elementos;

namespace BotTibia.Acoes
{
    public static class PegaInformacoesDosPixels
    {
        public static Color PegaCorDoPixel(Bitmap tela, CoordenadasXeY pixel)
        {
            return tela.GetPixel(pixel.X, pixel.Y);
        }
    }
}
