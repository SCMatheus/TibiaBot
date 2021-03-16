using System;
using BotTibia.Enum;

namespace BotTibia.Classes
{
    [Serializable()]
    public class Backpacks
    {
        public EnumBackpacks MainBackpack { get; set; }
        public EnumBackpacks SupplyBackpack { get; set; }
        public EnumBackpacks LootBackpack { get; set; }
        public EnumBackpacks GoldBackpack { get; set; }
        public EnumBackpacks AmmoBackpack { get; set; }
    }
}
