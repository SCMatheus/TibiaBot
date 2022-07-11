using System;
using System.Xml.Serialization;
using BotTibia.Enums;

namespace BotTibia.Classes
{
    [Serializable()]
    public class Backpack
    {
        public EnumTipoBackpack Tipo { get; set; }
        public EnumBackpacks Bp { get; set; }
        [XmlIgnore]
        public CoordenadasDeElementos Coordenadas {get; set;}
    }
}
