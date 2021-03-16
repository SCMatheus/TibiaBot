using System;
using System.Linq;
using System.Threading;
using BotTibia.Actions.AHK;
using BotTibia.Actions.Events;
using BotTibia.Classes;
using BotTibia.Enum;

namespace BotTibia.Actions.Cavebot
{
    public static class CavebotAction
    {
        public static void ExecAction(EnumAction action,Coordenada coordenada, string parans)
        {
            parans = parans.Replace(" ", "");
            var parametros = parans.Split(',');
                switch (action)
            {
                case EnumAction.Say:
                        AhkFunctions.SendMessage(parametros[0]+ "{Enter}", Global._tibiaProcessName);
                        AhkFunctions.SendKey("Enter", Global._tibiaProcessName);
                    break;
                case EnumAction.Wait:
                    Thread.Sleep(int.Parse(parametros[0]));
                    break;
                case EnumAction.TurnTo:
                    AhkFunctions.SendKey("Ctrl Down", Global._tibiaProcessName);
                    Thread.Sleep(50);
                    if (parametros[0].ToLower() == "north")
                    {
                        AhkFunctions.SendKey("Up", Global._tibiaProcessName);
                    }else if (parametros[0].ToLower() == "south")
                    {
                        AhkFunctions.SendKey("Down", Global._tibiaProcessName);
                    }
                    else if (parametros[0].ToLower() == "east")
                    {
                        AhkFunctions.SendKey("Right", Global._tibiaProcessName);
                    }
                    else if (parametros[0].ToLower() == "west")
                    {
                        AhkFunctions.SendKey("Left", Global._tibiaProcessName);
                    }
                    Thread.Sleep(50);
                    AhkFunctions.SendKey("Ctrl Up", Global._tibiaProcessName);
                    break;
                case EnumAction.Use:
                        var coordUse = PegaElementosDaTela.PegaVisinhosDaPosicaoDoPersonagem((EnumDirecao)System.Enum.Parse(typeof(EnumDirecao),parametros[0].ToUpper()));
                        ClickEvent.Click(Global._tibiaProcessName, coordUse, EnumMouseEvent.Right);
                    break;
                case EnumAction.UseItem:
                    string variavel;
                        if (parametros[0].StartsWith("$"))
                        {
                            variavel = PegaVariavelPorChave(parametros[0]);
                        }
                        else
                        {
                            variavel = parametros[0];
                        }
                        AhkFunctions.SendKey(variavel.ToUpper(),Global._tibiaProcessName);
                        var coordUseItem = PegaElementosDaTela.PegaVisinhosDaPosicaoDoPersonagem((EnumDirecao)System.Enum.Parse(typeof(EnumDirecao), parametros[1].ToUpper()));
                        ClickEvent.Click(Global._tibiaProcessName, coordUseItem, EnumMouseEvent.Left);
                    break;
                case EnumAction.Deposit:
                    EncontraEVaiAteDepot();
                    break;
                case EnumAction.GotoLabel:

                    break;
            }
        }
        public static string PegaVariavelPorChave(string chave)
        {
            string variavel = chave.Replace("$", "");
            variavel = Global._variaveisGlobais.FirstOrDefault(x => x.Chave.Equals(variavel.ToLower()))?.Valor;
            if (string.IsNullOrWhiteSpace(variavel) && variavel == null)
            {
                throw new Exception($"Não foi possivel encontrar a variável {chave}");
            }
            return variavel;
        }
        private static void EncontraEVaiAteDepot()
        {
            var depot = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName,Global._mainWindow, Global._path+ "\\Images\\ConfigsGerais\\Depot.png");
            if (depot != null)
            {
                ClickEvent.Click(Global._tibiaProcessName, new System.Drawing.Point(depot.X + depot.Width / 2, depot.Y + depot.Height / 2), EnumMouseEvent.Left);
                Thread.Sleep(500);
                CloseAllBackpacks();
            }
            else
            {
                throw new Exception("Nenhum depot foi encontrado!");
            }
        }
        private static void CloseAllBackpacks()
        {
            CoordenadasDeElementos mainBP = null;
            CoordenadasDeElementos lootBP = null;
            CoordenadasDeElementos goldBP = null;
            CoordenadasDeElementos supplyBP = null;
            CoordenadasDeElementos ammoBP = null;
            if (Global._backpacks.MainBackpack != EnumBackpacks.none)
            {
                mainBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height,
                                                              Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.MainBackpack.ToString() + ".png");
                if (mainBP != null)
                    CloseBackpack(mainBP);
            }
            if (Global._backpacks.LootBackpack != EnumBackpacks.none)
            {
                lootBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height,
                                                              Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.LootBackpack.ToString() + ".png");
                if (lootBP != null)
                    CloseBackpack(lootBP);
            }
            if (Global._backpacks.GoldBackpack != EnumBackpacks.none)
            {
                goldBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height,
                                                              Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.LootBackpack.ToString() + ".png");
                if (goldBP != null)
                    CloseBackpack(goldBP);
            }
            if (Global._backpacks.SupplyBackpack != EnumBackpacks.none)
            {
                supplyBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height,
                                                                Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.LootBackpack.ToString() + ".png");
                if (supplyBP != null)
                    CloseBackpack(supplyBP);
            }
            if (Global._backpacks.AmmoBackpack != EnumBackpacks.none)
            {
                ammoBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height,
                                                              Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.LootBackpack.ToString() + ".png");
                if(ammoBP != null)
                    CloseBackpack(ammoBP);
            }
        }
        private static void CloseBackpack(CoordenadasDeElementos backpack)
        {
            backpack.Width = 180;
            var close = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, backpack, Global._path + "\\Images\\ConfigsGerais\\Close.png");
            ClickEvent.Click(Global._tibiaProcessName, new System.Drawing.Point(close.X + close.Width / 2, close.Y + close.Height / 2), EnumMouseEvent.Left);
        }
    }
}
