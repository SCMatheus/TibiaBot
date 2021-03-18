using System;
using System.Collections.Generic;
using System.Drawing;
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
                Thread.Sleep(500);
                OpenDepot();
                Thread.Sleep(500);
                DepositItens();

            }
            else
            {
                throw new Exception("Nenhum depot foi encontrado!");
            }
        }
        private static void DepositItens()
        {
            var depot = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Configs\\depot_spot_to_deposit.png");
            var lootBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.LootBackpack.ToString() + ".png");
            CoordenadasDeElementos ehBackpack = null;
            CoordenadasDeElementos ehVazio = null;
            while (true)
            {
                ehVazio = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, lootBP.X, lootBP.Y, 45, 50, Global._path + "\\Images\\Global\\Configs\\spot_bp_vazia.png");
                if (ehVazio != null)
                    break;
                ehBackpack = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, lootBP.X, lootBP.Y, 45, 50, Global._path + "\\Images\\Global\\Backpacks\\Corpo\\" + Global._backpacks.LootBackpack.ToString() + ".png");
                if (ehBackpack != null)
                {
                    ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point(ehBackpack.X + ehBackpack.Width / 2, ehBackpack.Y + ehBackpack.Height / 2), EnumMouseEvent.Right);
                }
                else
                {
                    ClickEvent.ItemMove(Global._tibiaProcessName, new Point((lootBP.X + 25), (lootBP.Y+25)), new Point((depot.X + depot.Width / 2), (depot.Y + depot.Height/2)));
                    Thread.Sleep(300);
                }
            }

        }
        private static void OpenDepot()
        {
            var coordPersonagem = PegaElementosDaTela.PegaPosicaoDoPersonagem();
            CoordenadasDeElementos Depot = null;
            int count = 0;
            int tam = 0;
            Point[] posicoes = { new Point(-33, -98), new Point(-33, 33), new Point(33, -40), new Point(-98, -33) };
            while (count <= 3)
            {
                if (Global._mainWindow.Width > 600)
                {
                    Depot = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, coordPersonagem.X + posicoes[count].X, coordPersonagem.Y + posicoes[count].Y, 70, 70, Global._path + "\\Images\\ConfigsGerais\\depot_box_" + count.ToString() + ".png");
                    tam = 1;
                    if (Depot != null)
                        break;
                }else if(Global._mainWindow.Width > 350)
                {
                    //TODO:
                    tam = 2;
                }
                else
                {
                    //TODO:
                    tam = 3;
                }
                count++;
            }
            if(Depot != null && tam == 1)
            {
                AhkFunctions.SendKey("Ctrl Down", Global._tibiaProcessName);
                ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point(Depot.X + Depot.Width / 2, Depot.Y + Depot.Height / 2), EnumMouseEvent.Right);
                AhkFunctions.SendKey("Ctrl Up", Global._tibiaProcessName);
                Thread.Sleep(500);
                var browse = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._mainWindow.X, Global._mainWindow.Y, Global._mainWindow.Width, Global._mainWindow.Height, Global._path + "\\Images\\Global\\Configs\\browse_field.png");
                ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point(browse.X + browse.Width / 2, browse.Y + browse.Height / 2), EnumMouseEvent.Left);
                Thread.Sleep(500);
                count = 0;
                CoordenadasDeElementos depotBrowse = null;
                while (count <= 3)
                {
                    depotBrowse = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + $"\\Images\\Global\\Configs\\depot_in_browse_{count}.png");
                    if (depotBrowse != null)
                        break;
                    count++;
                }
                if (depotBrowse == null)
                    throw new Exception("Não foi possível encontrar o Depot para depositar");
                ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point(depotBrowse.X + depotBrowse.Width / 2, depotBrowse.Y + depotBrowse.Height / 2), EnumMouseEvent.Right);
                Thread.Sleep(500);
                var depotOpen = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, depotBrowse.X-50, depotBrowse.Y-50, depotBrowse.Width+100, depotBrowse.Height+100, Global._path + "\\Images\\Global\\Configs\\depot_to_open.png");
                ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point(depotOpen.X + depotOpen.Width / 2, depotOpen.Y + depotOpen.Height / 2), EnumMouseEvent.Right);
            }

        }
        private static void CloseAllBackpacks()
        {
            CloseBackpack(Global._backpacks.MainBackpack);
            CloseBackpack(Global._backpacks.LootBackpack);
            CloseBackpack(Global._backpacks.SupplyBackpack);
            CloseBackpack(Global._backpacks.GoldBackpack);
            CloseBackpack(Global._backpacks.AmmoBackpack);
        }
        private static void CloseBackpack(EnumBackpacks backpack)
        {
            CoordenadasDeElementos coordBP = null;
            if (backpack != EnumBackpacks.none)
            {
                coordBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height,
                                                                Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + backpack.ToString() + ".png");
                if (coordBP != null)
                {
                    coordBP.Width = 180;
                    var close = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, coordBP, Global._path + "\\Images\\ConfigsGerais\\Close.png");
                    ClickEvent.ClickOnElement(Global._tibiaProcessName, new System.Drawing.Point(close.X + close.Width / 2, close.Y + close.Height / 2), EnumMouseEvent.Left);
                }
            }
        }
        private static void OpenMainBackpack()
        {
            var mainBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.MainBackpack.ToString() + ".png");
            if (mainBP != null)
                return;
            var coorMain = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Corpo\\" + Global._backpacks.MainBackpack.ToString() + ".png");
            if (coorMain == null)
                throw new Exception("Não foi possível encontrar a Main Backpack");
            ClickEvent.ClickOnElement(Global._tibiaProcessName, new System.Drawing.Point((coorMain.X + coorMain.Width / 2), coorMain.Y + coorMain.Height / 2), EnumMouseEvent.Right);
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
            mainBP.Width = 180;
            var pushBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, mainBP, Global._path + "\\Images\\ConfigsGerais\\setaDown.png");
            ClickEvent.ItemMove(Global._tibiaProcessName, new Point((pushBP.X + pushBP.Width / 2), (pushBP.Y + pushBP.Height + 3)), new System.Drawing.Point((pushBP.X + pushBP.Width / 2) - 8, Global._tela.Height+31));
            Thread.Sleep(500);
            var backpackToOpen = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Corpo\\" + backpack.ToString() + ".png");
            if (backpackToOpen == null)
                throw new Exception($"Não foi possível encontrar a {backpack.ToString()}");
            ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point((backpackToOpen.X + backpackToOpen.Width / 2), backpackToOpen.Y + backpackToOpen.Height / 2), EnumMouseEvent.Right);
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
            ClickEvent.ClickOnElement(Global._tibiaProcessName, new System.Drawing.Point((pushBP.X + pushBP.Width / 2), (pushBP.Y + pushBP.Height + 3)), EnumMouseEvent.LeftDown);
            Thread.Sleep(500);
            ClickEvent.ClickOnElement(Global._tibiaProcessName, new System.Drawing.Point((pushBP.X - pushBP.Width / 2), Global._tela.Height+31), EnumMouseEvent.LeftUp);
            Thread.Sleep(500);
            var backpackToOpen = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height+31, Global._path + "\\Images\\Global\\Backpacks\\Corpo\\" + backpack.ToString() + ".png");
            if (backpackToOpen == null)
                throw new Exception($"Não foi possível encontrar a {backpack.ToString()}");
            ClickEvent.ClickOnElement(Global._tibiaProcessName, new System.Drawing.Point((backpackToOpen.X + backpackToOpen.Width / 2) , (backpackToOpen.Y + backpackToOpen.Height / 2)), EnumMouseEvent.Right);
        }
    }
}
