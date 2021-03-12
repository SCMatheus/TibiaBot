using System;
using System.Xml.Serialization;

namespace BotTibia.Enum
{
    public enum EnumAction
    {
        [XmlEnum(Name = "Empty")]
        Empty,
        [XmlEnum(Name = "Label")]
        Label,
        [XmlEnum(Name = "Say")]
        Say,
        [XmlEnum(Name = "TurnTo")]
        TurnTo,
        [XmlEnum(Name = "UseItem")]
        UseItem,
        [XmlEnum(Name = "Use")]
        Use,
        [XmlEnum(Name = "Wait")]
        Wait,
        [XmlEnum(Name = "Deposit")]
        Deposit,
        [XmlEnum(Name = "GotoLabel")]
        GotoLabel
    }
}
