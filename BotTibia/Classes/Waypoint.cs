using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotTibia.Enum;

namespace BotTibia.Classes
{ 
    [Serializable()]
    public class Waypoint
    {
        public int Index { get; set; }
        public EnumWaypoints Type { get; set; }
        public string Label { get; set; }
        public Coordenada Coordenada { get; set; }
        public Range Range { get; set; }
        public string Action { get; set; }
    }
}
