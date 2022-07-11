using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BotTibia.Enums;

namespace BotTibia.Classes
{ 
    [Serializable()]
    public class Waypoint
    {
        public int Index { get; set; }
        [XmlElement()]
        public EnumWaypoints Type { get; set; }
        [XmlElement()]
        public EnumAction TypeAction { get; set; }
        public Coordenada Coordenada { get; set; }
        public EnumMarks Mark { get; set; }
        public Range Range { get; set; }
        public string Parametros { get; set; }
    }
}
