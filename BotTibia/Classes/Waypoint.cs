using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotTibia.Classes
{ 
    [Serializable()]
    public class Waypoint
    {
        public string Type { get; set; }
        public string Label { get; set; }
        public Coordenada Coordenadas { get; set; }
        public Range Range { get; set; }
        public string Action { get; set; }
    }
}
