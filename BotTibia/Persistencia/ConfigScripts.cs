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
        public List<Waypoint> Waypoints { get; set; }
        public List<Variavel> VariaveisGlobais { get; set; }
        public List<Backpack> Backpacks { get; set; }
        public List<string> Drops { get; set; }
        public List<string> Loots { get; set; }
    }
}
