using System.Drawing;

namespace BotTibia.Elementos
{
    public class VidaBar
    {
        public CoordenadasDeElementos Coordenadas { get; set; }
        public Color pixel { get; set; }

        public CoordenadasXeY HighHeal { get; set; }
        public CoordenadasXeY MediumHeal { get; set; }
        public CoordenadasXeY LowHeal { get; set; }

        public VidaBar()
        {
            HighHeal = new CoordenadasXeY();
            MediumHeal = new CoordenadasXeY();
            LowHeal = new CoordenadasXeY();
            pixel = new Color();
            Coordenadas = new CoordenadasDeElementos();
        }

        public void SetCoordenadasPorImagemDoCoracao(CoordenadasDeElementos coordenadas)
        {
            Coordenadas.X = coordenadas.X + coordenadas.Width + 3;
            Coordenadas.Y = coordenadas.Y;
            Coordenadas.Width = 89;
            Coordenadas.Height = coordenadas.Height;
        }
        public void CalculaPixelsDoHeal(int low, int medium, int high)
        {
            CalculaPixelsDoHealLow(low);
            CalculaPixelsDoHealMedium(medium);
            CalculaPixelsDoHealHigh(high);
        }

        public void CalculaPixelsDoHealHigh(int high)
        {
            //Calcula pixel do heal mais alto
            this.HighHeal.X = (int)((high / 100.0) * this.Coordenadas.Width) + this.Coordenadas.X;
            this.HighHeal.Y = (Coordenadas.Height / 2) + Coordenadas.Y;

        }

        public void CalculaPixelsDoHealMedium(int medium)
        {
            //Calcula pixel do heal medio
            this.MediumHeal.X = (int)((medium / 100.0) * Coordenadas.Width) + Coordenadas.X;
            this.MediumHeal.Y = (Coordenadas.Height / 2) + Coordenadas.Y;

        }
        public void CalculaPixelsDoHealLow(int low)
        {
            //Calcula pixel do heal mais baixo
            this.LowHeal.X = (int)((low / 100.0) * Coordenadas.Width) + Coordenadas.X;
            this.LowHeal.Y = (Coordenadas.Height / 2) + Coordenadas.Y;
        }
    }
}
