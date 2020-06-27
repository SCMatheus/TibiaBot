using System.Drawing;

namespace BotTibia.Elementos
{
    public class ManaBar
    {
        public CoordenadasDeElementos Coordenadas { get; set; }
        public Color pixel { get; set; }
        public CoordenadasXeY ManaHeal { get; set; }

        public ManaBar()
        {
            ManaHeal = new CoordenadasXeY();
            pixel = new Color();
            Coordenadas = new CoordenadasDeElementos();
        }

        public void CalculaPixelsDoHeal(int healPercent)
        {
            //Calcula pixel do heal mais alto
            this.ManaHeal.X = (int)((healPercent / 100.0) * this.Coordenadas.Width) + this.Coordenadas.X;
            this.ManaHeal.Y = (Coordenadas.Height / 2) + Coordenadas.Y;
        }
    }
}
