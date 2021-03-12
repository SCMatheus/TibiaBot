using System;
using System.Xml.Serialization;

namespace BotTibia.Enum
{
    public enum EnumWaypoints
    {
        [XmlEnum(Name = "Node")]
        Node,
        [XmlEnum(Name = "Stand")]
        Stand,
        [XmlEnum(Name = "Action")]
        Action,
    }
}