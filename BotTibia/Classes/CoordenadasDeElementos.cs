namespace BotTibia.Classes
{
    public class CoordenadasDeElementos
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public override bool Equals(object obj) {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType())) {
                return false;
            } else {
                CoordenadasDeElementos p = (CoordenadasDeElementos)obj;
                return (X == p.X) && (Y == p.Y) && (Width == p.Width) && (Height == p.Height);
            }
        }
    }
}
