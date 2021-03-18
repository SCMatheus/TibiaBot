using System;
using System.Xml.Serialization;

namespace BotTibia.Enum
{
    [Serializable()]
    public enum EnumBackpacks
    {
        [XmlEnum(Name = "None")]
        none,
        [XmlEnum(Name = "Backpack")]
        backpack,
        [XmlEnum(Name = "Backpack of holding")]
        backpack_of_holding,
        [XmlEnum(Name = "Beach")]
        beach,
        [XmlEnum(Name = "Birthday")]
        birthday,
        [XmlEnum(Name = "Buggy")]
        buggy,
        [XmlEnum(Name = "Blue")]
        blue,
        [XmlEnum(Name = "Brocade")]
        brocade,
        [XmlEnum(Name = "Camouflage")]
        camouflage,
        [XmlEnum(Name = "Crown")]
        crown,
        [XmlEnum(Name = "Demon")]
        demon,
        [XmlEnum(Name = "Deepling")]
        deepling,
        [XmlEnum(Name = "Dragon")]
        dragon,
        [XmlEnum(Name = "Expedition")]
        expedition,
        [XmlEnum(Name = "Fur")]
        fur,
        [XmlEnum(Name = "Golden")]
        golden,
        [XmlEnum(Name = "Green")]
        green,
        [XmlEnum(Name = "Grey")]
        grey,
        [XmlEnum(Name = "Jewelled")]
        jewelled,
        [XmlEnum(Name = "Minotaur")]
        minotaur,
        [XmlEnum(Name = "Orange")]
        orange,
        [XmlEnum(Name = "Purple")]
        purple,
        [XmlEnum(Name = "Pirate")]
        pirate,
        [XmlEnum(Name = "Red")]
        red,
        [XmlEnum(Name = "Yellow")]
        yellow
    }
}
