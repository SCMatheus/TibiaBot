using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotTibia.Classes
{
    [Serializable()]
    public class Coordenada : IEquatable<Coordenada>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public bool Equals(Coordenada other)
        {
            return other.X == X && other.Y == Y && other.Z == Z;
        }
        public override string ToString()
        {
            return $"x:{X} y:{Y} z:{Z}";
        }
    }
}
