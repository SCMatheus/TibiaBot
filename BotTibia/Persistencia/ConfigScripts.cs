using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using BotTibia.Classes;

namespace BotTibia.Persistencia
{
    [Serializable()]
    [XmlRoot("ConfigScripts")]
    public class ConfigScripts
    {
        public Heal Primeiro { get; set; }
        public Heal Segundo { get; set; }
        public Heal Terceiro { get; set; }
        public Heal ManaHeal { get; set; }
        public string ParaHeal { get; set; }
        public int Firetimer { get; set; }
        public List<string> Waypoints { get; set; }
    }
}
