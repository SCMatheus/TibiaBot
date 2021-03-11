using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotTibia.Classes
{
    [Serializable()]
    public class Range
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Range(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return $"{X}x{Y}";
        }
    }
}
