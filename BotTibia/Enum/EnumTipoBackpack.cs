using System;
using System.Xml.Serialization;

namespace BotTibia.Enum
{
    public enum EnumTipoBackpack
    {
        [XmlEnum(Name = "Main")]
        Main,
        [XmlEnum(Name = "Loot")]
        Loot,
        [XmlEnum(Name = "Gold")]
        Gold,
        [XmlEnum(Name = "Supply")]
        Supply,
        [XmlEnum(Name = "Ammo")]
        Ammo
    }
}
