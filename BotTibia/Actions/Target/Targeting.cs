using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using BotTibia.Actions.AHK;
using BotTibia.Actions.Events;
using BotTibia.Actions.Loot;
using BotTibia.Classes;
using BotTibia.Enums;

namespace BotTibia.Actions.Target
{
    public static class Targeting
    {
        public static Combo[] Combos = new Combo[4];
        public static Support[] Supports = new Support[4];
        private static int _comboIndex = 0;
        private static DateTime _timeToNextSpell;
        public static void AttackAndLooting()
        {
            var loot = false;
            if (Global.SelectedClient == EnumSuportedClients.Global) {
                AhkFunctions.SendKey("PgUp", Global._tibiaProcessName);
                Thread.Sleep(150);
            }

            var contTempo = 0;
            while (Global._isTarget)
            {
                CoordenadasDeElementos isTarget;
                //if (Global.SelectedClient == EnumSuportedClients.ArchLight) {
                //    isTarget = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._battle, Global._path + "\\Images\\ConfigsGerais\\targetado.png");
                //    if (isTarget != null)
                //        Thread.Sleep(150);
                //    else {
                //        Thread.Sleep(300);
                //        isTarget = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._battle, Global._path + "\\Images\\ConfigsGerais\\targetado.png");
                //        if (isTarget == null) break;
                //    };
                //}
                isTarget = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._battle, Global._path + "\\Images\\ConfigsGerais\\targetado.png");
                if (isTarget != null) {
                    DoSupport();
                    DoCombo();
                }
                if (isTarget == null)
                {
                    contTempo = 0;
                    if (loot && Global._isLoot) Looting.Lootear();
                    
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
                    if (contTempo <= 150) continue;
                    ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point(isTarget.X + isTarget.Width / 2, isTarget.Y + isTarget.Height / 2), EnumMouseEvent.Left);
                    ClickEvent.MouseMove(Global._tibiaProcessName, new Point(0,0));
                    break;
                }
            }
        }

        private static void DoSupport()
        {
            var now = DateTime.Now;
            var supports = Supports.Where(s => s != null && !s.IsNull()).ToArray();
            if (!supports.Any()) 
                return;

            foreach (var support in supports) {
                if (support.LastUse != null && support.LastUse + support.TimeSpanCooldown() > now)
                    continue;

                AhkFunctions.SendKey(support.Hotkey, Global._tibiaProcessName);
                support.LastUse = DateTime.Now;
                Thread.Sleep(200);
            }
        }

        private static void DoCombo() {
            var now = DateTime.Now;
            var combos = Combos.Where(c => c != null && !c.IsNull()).ToArray();
            if (!combos.Any())
                return;

            if (_comboIndex >= combos.Count())
                _comboIndex = 0;

            if (_timeToNextSpell != null && _timeToNextSpell > now)
                return;
            if (combos[_comboIndex].LastUse != null && combos[_comboIndex].LastUse + combos[_comboIndex].TimeSpanCooldown() > now)
                return;

            AhkFunctions.SendKey(combos[_comboIndex].Hotkey, Global._tibiaProcessName);

            combos[_comboIndex].LastUse = DateTime.Now;
            _timeToNextSpell = combos[_comboIndex].LastUse + combos[_comboIndex].TimeSpantimeToNextSpell();

            _comboIndex++;
        }

        public static void AjustaBattle()
        {
            var battle = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela, Global._path + "\\Images\\Global\\Configs\\battle_name.png", 10);
            if (battle == null)
            {
                var openBattle = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela, Global._path + "\\Images\\Global\\Configs\\battle_to_open.png");
                if (openBattle == null)
                {
                    throw new Exception("Não foi possível encontrar o battle");
                }
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

        public class Combo {
            public string Hotkey { get; set; } = null;
            public int? Cooldown { get; set; } = null;
            public int? TimeToNextSpell { get; set; } = null;
            private TimeSpan? _timeToNextSpell { get; set; }
            public TimeSpan TimeSpantimeToNextSpell() {
                if (_timeToNextSpell == null)
                    _timeToNextSpell = TimeSpan.FromSeconds(TimeToNextSpell.Value);
                return _timeToNextSpell.Value;
            }

            private TimeSpan? _timeSpanCooldown { get; set; }
            public TimeSpan TimeSpanCooldown() {
                if (_timeSpanCooldown == null)
                    _timeSpanCooldown = TimeSpan.FromSeconds(Cooldown.Value);
                return _timeSpanCooldown.Value;
            }

            public DateTime LastUse { get; set; }
            public bool IsNull() => string.IsNullOrEmpty(Hotkey) || Cooldown == null || TimeToNextSpell == null;
        }

        public class Support {
            public string Hotkey { get; set; } = null;
            public int? Cooldown { get; set; }
            private TimeSpan? _timeSpanCooldown { get; set; }
            public TimeSpan TimeSpanCooldown() {
                if (_timeSpanCooldown == null)
                    _timeSpanCooldown = TimeSpan.FromSeconds(Cooldown.Value);
                return _timeSpanCooldown.Value;
            }

            public DateTime LastUse { get; set; }
            public bool IsNull() => string.IsNullOrEmpty(Hotkey) || Cooldown == null;
        }
    }
}
