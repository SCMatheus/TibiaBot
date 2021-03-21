using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BotTibia.Actions.AHK;
using BotTibia.Actions.Events;
using BotTibia.Actions.Loot;
using BotTibia.Classes;
using BotTibia.Enum;

namespace BotTibia.Actions.Target
{
    public static class Targeting
    {
        public static void AttackAndLooting()
        {
            bool target = true;
            bool loot = false;
            AhkFunctions.SendKey("PgUp", Global._tibiaProcessName);
            Thread.Sleep(150);
            CoordenadasDeElementos isTarget;
            int contTempo = 0;
            while (target)
            {
                isTarget = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._battle, Global._path + "\\Images\\ConfigsGerais\\targetado.png");
                if (isTarget == null)
                {
                    if (loot && Global._isLoot)
                        Looting.Lootear();
                    AhkFunctions.SendKey("PgUp", Global._tibiaProcessName);
                    Thread.Sleep(150);
                    isTarget = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._battle, Global._path + "\\Images\\ConfigsGerais\\targetado.png");
                    if (isTarget != null)
                    {
                        loot = true;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    Thread.Sleep(200);
                    contTempo++;
                    loot = true;
                    if (contTempo > 150)
                    {
                        ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point(isTarget.X + isTarget.Width / 2, isTarget.Y + isTarget.Height / 2), EnumMouseEvent.Left);
                        break;
                    }
                }
            }
        }
        public static void AjustaBattle()
        {
            var battle = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela, Global._path + "\\Images\\Global\\Configs\\battle_name.png");
            if (battle == null)
            {
                var openBattle = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela, Global._path + "\\Images\\Global\\Configs\\battle_to_open.png");
                ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point(openBattle.X + openBattle.Width / 2, openBattle.Y + openBattle.Height / 2), EnumMouseEvent.Left);
                Thread.Sleep(300);
                battle = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela, Global._path + "\\Images\\Global\\Configs\\battle_name.png");
            }
            Global._battle = battle;
            battle.Height = Global._tela.Height;
            battle.Width = 180;
            var pushBattle = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, battle, Global._path + "\\Images\\ConfigsGerais\\setaDown.png");
            ClickEvent.ItemMove(Global._tibiaProcessName, new Point((pushBattle.X + pushBattle.Width / 2), (pushBattle.Y + pushBattle.Height + 3)), new Point((pushBattle.X + pushBattle.Width / 2) - 8, 0));
            pushBattle = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, battle, Global._path + "\\Images\\ConfigsGerais\\setaDown.png");
            ClickEvent.ItemMove(Global._tibiaProcessName, new Point((pushBattle.X + pushBattle.Width / 2), (pushBattle.Y + pushBattle.Height + 3)), new Point((pushBattle.X + pushBattle.Width / 2) - 8, (pushBattle.Y + 180)));
            Global._battle.Height = 280;
            Global._battle.Width = 175;
        } 
    }
}
