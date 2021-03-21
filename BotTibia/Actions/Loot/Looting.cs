using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using BotTibia.Actions.AHK;
using BotTibia.Actions.Cavebot;
using BotTibia.Actions.Events;
using BotTibia.Classes;
using BotTibia.Enum;

namespace BotTibia.Actions.Loot
{
    public static class Looting
    {
        public static void Lootear()
        {
            AhkFunctions.SendKey("Shift down", Global._tibiaProcessName);
            LootOnDirection(EnumDirecao.NORTH);
            LootOnDirection(EnumDirecao.NORTHEAST);
            LootOnDirection(EnumDirecao.EAST);
            LootOnDirection(EnumDirecao.SOUTHEAST);
            LootOnDirection(EnumDirecao.SOUTH);
            LootOnDirection(EnumDirecao.SOUTHWEST);
            LootOnDirection(EnumDirecao.WEST);
            LootOnDirection(EnumDirecao.NORTHWEST);
            AhkFunctions.SendKey("Shift up", Global._tibiaProcessName);
            if (Global._moveLootItens.Count > 0)
            {
                MoveLootItens();
                MoveGold();
            }
            if (Global._dropItens.Count > 0)
            {
                DropLoot();
            }
        }
        private static void LootOnDirection(EnumDirecao direcao)
        {
            
            var posicao = PegaElementosDaTela.PegaVisinhosDaPosicaoDoPersonagem(direcao);
            ClickEvent.ClickOnElement(Global._tibiaProcessName, posicao, EnumMouseEvent.Right);
            
        }
        private static void MoveGold()
        {
            CoordenadasDeElementos item;
            CoordenadasDeElementos ehBackpack;
            var GoldBp = Global._backpacks.SingleOrDefault(bp => bp.Tipo == EnumTipoBackpack.Gold).Coordenadas;
            List<string> gold = new List<string>();
            gold.Add("gold_coin");
            gold.Add("platinum_coin");
            gold.ForEach(g =>
            {
                while (true)
                {
                    item = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName,
                                                                Global._backpacks
                                                                .SingleOrDefault(bp => bp.Tipo == EnumTipoBackpack.Main)
                                                                .Coordenadas,
                                                                Global._path + $"\\Images\\Itens\\{g}.png");
                    if (item == null)
                        break;
                    ehBackpack = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName,
                                                                new CoordenadasDeElementos()
                                                                {
                                                                    X = GoldBp.X + GoldBp.Width - 55,
                                                                    Y = GoldBp.Y + GoldBp.Height - 45,
                                                                    Width = 55,
                                                                    Height = 45
                                                                },
                                                                Global._path +
                                                                "\\Images\\Global\\Backpacks\\Corpo\\" +
                                                                Global._backpacks
                                                                .FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Gold)
                                                                .Bp.ToString() + ".png");
                    if (ehBackpack != null)
                    {
                        ClickEvent.ClickOnElement(Global._tibiaProcessName,
                                                  new Point(GoldBp.X + GoldBp.Width - 25, GoldBp.Y + GoldBp.Height - 20),
                                                  EnumMouseEvent.Right);
                        CavebotAction.MoveParaOFimDaBackpack(Global._backpacks
                                                                   .FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Gold));
                    }
                    ClickEvent.ItemMove(Global._tibiaProcessName, new Point(item.X + item.Width / 2, item.Y + item.Height / 2),
                                        new Point(GoldBp.X + GoldBp.Width - 25, GoldBp.Y + GoldBp.Height - 20));
                    Thread.Sleep(300);
                }
            });
        }
        private static void DropLoot()
        {
            CoordenadasDeElementos item;
            Global._dropItens.ForEach(drop =>
            {
                while (true)
                {
                    item = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName,
                                                                Global._backpacks
                                                                .SingleOrDefault(bp => bp.Tipo == EnumTipoBackpack.Main)
                                                                .Coordenadas,
                                                                Global._path + $"\\Images\\Itens\\{drop}.png");
                    if (item == null)
                        break;
                    ClickEvent.ItemMove(Global._tibiaProcessName, new Point(item.X + item.Width / 2, item.Y + item.Height / 2), PegaElementosDaTela.PegaPosicaoDoPersonagem());
                    Thread.Sleep(300);
                }
            });
        }
        private static void MoveLootItens()
        {
            CoordenadasDeElementos item;
            CoordenadasDeElementos ehBackpack;
            var lootBp = Global._backpacks.SingleOrDefault(bp => bp.Tipo == EnumTipoBackpack.Loot).Coordenadas;
            Global._moveLootItens.ForEach(loot =>
            {
                while (true)
                {
                    item = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName,
                                                                Global._backpacks
                                                                .SingleOrDefault(bp => bp.Tipo == EnumTipoBackpack.Main)
                                                                .Coordenadas,
                                                                Global._path + $"\\Images\\Itens\\{loot}.png");
                    if (item == null)
                        break;
                    ehBackpack = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName,
                                                                new CoordenadasDeElementos() { X = lootBp.X + lootBp.Width - 55, 
                                                                                               Y = lootBp.Y + lootBp.Height - 45,
                                                                                               Width = 55,
                                                                                               Height = 45},
                                                                Global._path + 
                                                                "\\Images\\Global\\Backpacks\\Corpo\\" +
                                                                Global._backpacks
                                                                .FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Loot)
                                                                .Bp.ToString() + ".png");
                    if (ehBackpack != null)
                    {
                        ClickEvent.ClickOnElement(Global._tibiaProcessName,
                                                  new Point(lootBp.X + lootBp.Width - 25, lootBp.Y + lootBp.Height - 20),
                                                  EnumMouseEvent.Right);
                        CavebotAction.MoveParaOFimDaBackpack(Global._backpacks
                                                                   .FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Loot));
                    }
                    ClickEvent.ItemMove(Global._tibiaProcessName, new Point(item.X + item.Width / 2, item.Y + item.Height / 2),
                                        new Point(lootBp.X + lootBp.Width - 25, lootBp.Y + lootBp.Height - 20));
                    Thread.Sleep(300);
                }
            });
        }
    }
}
