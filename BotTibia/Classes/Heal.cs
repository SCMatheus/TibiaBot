using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BotTibia.Classes
{
    [Serializable()]
    public class Heal
    {
        public string Percent { get; set; }
        public string Hotkey { get; set; }
    }
}
