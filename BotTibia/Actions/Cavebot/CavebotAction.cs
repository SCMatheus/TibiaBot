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
            if (Global._backpacks.LootBackpack == EnumBackpacks.none)
                return;
            var count = 0;
            CoordenadasDeElementos depot = null;
            while (count <= 3)
            {
                depot = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._mainWindow, Global._path + $"\\Images\\ConfigsGerais\\Depot_{count}.png");
                count++;
            }
            if (depot != null)
            {
                ClickEvent.Click(Global._tibiaProcessName, new System.Drawing.Point((depot.X + depot.Width / 2)-8, (depot.Y + depot.Height / 2)-31), EnumMouseEvent.Left);
                Thread.Sleep(5000);
                CloseAllBackpacks();
                Thread.Sleep(500);
                OpenMainBackpack();
                Thread.Sleep(500);
                OpenBackpack(Global._backpacks.LootBackpack);
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
                                                              Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.GoldBackpack.ToString() + ".png");
                if (goldBP != null)
                    CloseBackpack(goldBP);
            }
            if (Global._backpacks.SupplyBackpack != EnumBackpacks.none)
            {
                supplyBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height,
                                                                Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.SupplyBackpack.ToString() + ".png");
                if (supplyBP != null)
                    CloseBackpack(supplyBP);
            }
            if (Global._backpacks.AmmoBackpack != EnumBackpacks.none)
            {
                ammoBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height,
                                                              Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.AmmoBackpack.ToString() + ".png");
                if(ammoBP != null)
                    CloseBackpack(ammoBP);
            }
        }
        private static void CloseBackpack(CoordenadasDeElementos backpack)
        {
            backpack.Width = 180;
            var close = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, backpack, Global._path + "\\Images\\ConfigsGerais\\Close.png");
            ClickEvent.Click(Global._tibiaProcessName, new System.Drawing.Point((close.X + close.Width / 2)-8, (close.Y + close.Height / 2) - 31), EnumMouseEvent.Left);
        }
        private static void OpenMainBackpack()
        {
            var mainBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.MainBackpack.ToString() + ".png");
            if (mainBP != null)
                return;
            var coorMain = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Corpo\\" + Global._backpacks.MainBackpack.ToString() + ".png");
            if (coorMain == null)
                throw new Exception("Não foi possível encontrar a Main Backpack");
            ClickEvent.Click(Global._tibiaProcessName, new System.Drawing.Point((coorMain.X + coorMain.Width / 2) - 8, (coorMain.Y + coorMain.Height / 2) - 31), EnumMouseEvent.Right);
        }
        private static void OpenBackpack(EnumBackpacks backpack)
        {
            var mainBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.MainBackpack.ToString() + ".png");
            if (mainBP == null)
            {
                OpenMainBackpack();
                Thread.Sleep(500);
                mainBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.MainBackpack.ToString() + ".png");
            }
            mainBP.Height = mainBP.Height + 270;
            mainBP.Width = Global._tela.Width;
            var pushBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, mainBP, Global._path + "\\Images\\ConfigsGerais\\setaDown.png");
            ClickEvent.Click(Global._tibiaProcessName, new System.Drawing.Point((pushBP.X + pushBP.Width / 2) - 8, (pushBP.Y + pushBP.Height + 3) - 31), EnumMouseEvent.LeftDown);
            Thread.Sleep(500);
            ClickEvent.Click(Global._tibiaProcessName, new System.Drawing.Point((pushBP.X - pushBP.Width / 2) - 8,Global._tela.Height), EnumMouseEvent.LeftUp);
            Thread.Sleep(500);
            var backpackToOpen = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Corpo\\" + backpack.ToString() + ".png");
            if (backpackToOpen == null)
                throw new Exception($"Não foi possível encontrar a {backpack.ToString()}");
            ClickEvent.Click(Global._tibiaProcessName, new System.Drawing.Point((backpackToOpen.X + backpackToOpen.Width / 2) - 8, (backpackToOpen.Y + backpackToOpen.Height / 2) - 31), EnumMouseEvent.Right);
        }
        private static void OpenBackpackInNewTab(EnumBackpacks backpack)
        {
            var mainBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.MainBackpack.ToString() + ".png");
            if (mainBP == null)
            {
                OpenMainBackpack();
                Thread.Sleep(500);
                mainBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.MainBackpack.ToString() + ".png");
            }
            mainBP.Height = mainBP.Height + 270;
            mainBP.Width = Global._tela.Width;
            var pushBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, mainBP, Global._path + "\\Images\\ConfigsGerais\\setaDown.png");
            ClickEvent.Click(Global._tibiaProcessName, new System.Drawing.Point((pushBP.X + pushBP.Width / 2) - 8, (pushBP.Y + pushBP.Height + 3) - 31), EnumMouseEvent.LeftDown);
            Thread.Sleep(500);
            ClickEvent.Click(Global._tibiaProcessName, new System.Drawing.Point((pushBP.X - pushBP.Width / 2) - 8, Global._tela.Height), EnumMouseEvent.LeftUp);
            Thread.Sleep(500);
            var backpackToOpen = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Corpo\\" + backpack.ToString() + ".png");
            if (backpackToOpen == null)
                throw new Exception($"Não foi possível encontrar a {backpack.ToString()}");
            ClickEvent.Click(Global._tibiaProcessName, new System.Drawing.Point((backpackToOpen.X + backpackToOpen.Width / 2) - 8, (backpackToOpen.Y + backpackToOpen.Height / 2) - 31), EnumMouseEvent.Right);
        }
    }
}
