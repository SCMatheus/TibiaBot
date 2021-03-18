using System;
using BotTibia.Enum;

namespace BotTibia.Classes
{
    [Serializable()]
    public class Backpack
    {
        public EnumTipoBackpack Tipo { get; set; }
        public EnumBackpacks Bp { get; set; }
        public CoordenadasDeElementos Coordenadas {get; set;}

        public Backpack(EnumTipoBackpack tipo, EnumBackpacks bp)
        {
            Tipo = tipo;
            Bp = bp;
        }
    }
}
