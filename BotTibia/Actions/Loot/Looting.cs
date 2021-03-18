using System.Drawing;
using System.Threading;
using BotTibia.Actions.AHK;
using BotTibia.Actions.Events;
using BotTibia.Classes;
using BotTibia.Enum;

namespace BotTibia.Actions.Loot
{
    public static class Looting
    {
        public static void Lootear()
        {
            LootOnDirection(EnumDirecao.NORTH);
            Thread.Sleep(100);
            LootOnDirection(EnumDirecao.NORTHEAST);
            Thread.Sleep(100);
            LootOnDirection(EnumDirecao.EAST);
            Thread.Sleep(100);
            LootOnDirection(EnumDirecao.SOUTHEAST);
            Thread.Sleep(100);
            LootOnDirection(EnumDirecao.SOUTH);
            Thread.Sleep(100);
            LootOnDirection(EnumDirecao.SOUTHWEST);
            Thread.Sleep(100);
            LootOnDirection(EnumDirecao.WEST);
            Thread.Sleep(100);
            LootOnDirection(EnumDirecao.NORTHWEST);
        }
        private static void LootOnDirection(EnumDirecao direcao)
        {
            AhkFunctions.SendKey("Shift down", Global._tibiaProcessName);
            var posicao = PegaElementosDaTela.PegaVisinhosDaPosicaoDoPersonagem(direcao);
            ClickEvent.ClickOnElement(Global._tibiaProcessName, posicao, EnumMouseEvent.Right);
            AhkFunctions.SendKey("Shift up", Global._tibiaProcessName);
        }
    }
}
