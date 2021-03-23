using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;
using BotTibia.Actions.AHK;
using BotTibia.Actions.Events;
using BotTibia.Actions.Print;
using BotTibia.Actions.Scan;
using BotTibia.Classes;
using BotTibia.Enum;

namespace BotTibia.Actions.Cavebot
{
    public static class CavebotAction
    {
        public static void ExecAction(EnumAction action,Coordenada coordenada, string parans)
        {
            if (action != EnumAction.Say)
            {
                parans = parans.Replace(" ", "");
            }
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
                        ClickEvent.ClickOnElement(Global._tibiaProcessName, coordUse, EnumMouseEvent.Right);
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
                        ClickEvent.ClickOnElement(Global._tibiaProcessName, coordUseItem, EnumMouseEvent.Left);
                    break;
                case EnumAction.Deposit:
                    InvokeMethod(new Func<bool>(EncontraEVaiAteDepot));
                    break;
                case EnumAction.OpenBackpacks:
                    InvokeMethod(new Func<bool>(OpenAllBackpacks));
                    break;
                case EnumAction.CloseBackpacks:
                    InvokeMethod(new Func<bool>(CloseAllBackpacks));
                    break;
                case EnumAction.GotoLabel:
                    GotoLabel(parametros[0]);
                    break;
                case EnumAction.TargetOn:
                    Global._isTarget = true;
                    break;
                case EnumAction.TargetOff:
                    Global._isTarget = false;
                    break;
                case EnumAction.CheckSupply:
                    Global._isTarget = false;
                    break;
                case EnumAction.CheckCap:
                    var isLowCap = CheckCap(int.Parse(parametros[0]));
                    if(isLowCap)
                        GotoLabel(parametros[1]);
                    break;
            }
        }
        private static bool CheckCap(int cap)
        {
            var tela = CapturaTela.CaptureWindow(Global._tibiaProcessName);
            var capImg = CapturaTela.CortaTela(tela, Global._cap.X, Global._cap.Y, Global._cap.Width, Global._cap.Height);
            capImg.Save("cap.png");
            var valorCap = Ocr.IdentificaNumeroEmImg(capImg);
            if(valorCap == -1)
                throw new Exception("Não foi possível encontrar a cap");
            return valorCap <= cap;
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
        private static bool EncontraEVaiAteDepot()
        {
            if (Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Loot).Bp == EnumBackpacks.none)
                return true;
            var count = 0;
            CoordenadasDeElementos depot = null;
            while (count <= 3)
            {
                depot = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._mainWindow, Global._path + $"\\Images\\ConfigsGerais\\Depot_{count}.png");
                count++;
            }
            if (depot == null)
                return false;
            bool result = true;
            ClickEvent.Click(Global._tibiaProcessName, new System.Drawing.Point((depot.X + depot.Width / 2)-8, (depot.Y + depot.Height / 2)-31), EnumMouseEvent.Left);
            Thread.Sleep(5000);
            result = CloseAllBackpacks();
            if (!result)
                return false;
            Thread.Sleep(500);
            result = OpenMainBackpack();
            if (!result)
                return false;
            Thread.Sleep(500);
            result = OpenBackpack(Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Loot));
            if (!result)
                return false;
            Thread.Sleep(500);
            result = OpenDepot();
            if (!result)
                return false;
            Thread.Sleep(500);
            DepositItens();

            return true;
        }
        private static void DepositItens()
        {
            var depot = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Configs\\depot_spot_to_deposit.png");
            var lootBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Loot).Bp.ToString() + ".png");
            CoordenadasDeElementos ehBackpack = null;
            CoordenadasDeElementos ehVazio = null;
            while (true)
            {
                ehVazio = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, lootBP.X, lootBP.Y, 45, 50, Global._path + "\\Images\\Global\\Configs\\spot_bp_vazia.png");
                if (ehVazio != null)
                    break;
                ehBackpack = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, lootBP.X, lootBP.Y, 45, 50, Global._path + "\\Images\\Global\\Backpacks\\Corpo\\" + Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Loot).Bp.ToString() + ".png");
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
        private static bool OpenDepot()
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
                    return false;
                ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point(depotBrowse.X + depotBrowse.Width / 2, depotBrowse.Y + depotBrowse.Height / 2), EnumMouseEvent.Right);
                Thread.Sleep(500);
                var depotOpen = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, depotBrowse.X-50, depotBrowse.Y-50, depotBrowse.Width+100, depotBrowse.Height+100, Global._path + "\\Images\\Global\\Configs\\depot_to_open.png");
                ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point(depotOpen.X + depotOpen.Width / 2, depotOpen.Y + depotOpen.Height / 2), EnumMouseEvent.Right);
            }
            else
            {
                return false;
            }
            return true;

        }
        public static bool CloseAllBackpacks()
        {
            bool result = true;
            result = CloseBackpack(Global._backpacks.FirstOrDefault(backpack => backpack.Tipo == EnumTipoBackpack.Main));
            if (!result)
                return false;
            result = CloseBackpack(Global._backpacks.FirstOrDefault(backpack => backpack.Tipo == EnumTipoBackpack.Loot));
            if (!result)
                return false;
            result = CloseBackpack(Global._backpacks.FirstOrDefault(backpack => backpack.Tipo == EnumTipoBackpack.Supply));
            if (!result)
                return false;
            result = CloseBackpack(Global._backpacks.FirstOrDefault(backpack => backpack.Tipo == EnumTipoBackpack.Gold));
            if (!result)
                return false;
            result = CloseBackpack(Global._backpacks.FirstOrDefault(backpack => backpack.Tipo == EnumTipoBackpack.Ammo));
            if (!result)
                return false;
            return true;
        }
        private static bool CloseBackpack(Backpack backpack)
        {
            CoordenadasDeElementos coordBP = null;
            if (backpack.Bp != EnumBackpacks.none)
            {
                coordBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height,
                                                                Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + backpack.Bp.ToString() + ".png");
                if (coordBP == null)
                    return true;

                coordBP.Width = 180;
                var close = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, coordBP, Global._path + "\\Images\\ConfigsGerais\\Close.png");
                ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point(close.X + close.Width / 2, close.Y + close.Height / 2), EnumMouseEvent.Left);
                backpack.Coordenadas = null;
            }
            return true;
        }
        private static bool OpenMainBackpack()
        {
            var mainBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Main).Bp.ToString() + ".png");
            if (mainBP != null)
            {
                ExpandeBackpack(Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Main));
                return true;
            }
            var coorMain = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Corpo\\" + Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Main).Bp.ToString() + ".png");
            if (coorMain == null)
                return false;
            ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point((coorMain.X + coorMain.Width / 2), coorMain.Y + coorMain.Height / 2), EnumMouseEvent.Right);
            Thread.Sleep(500);
            Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Main).Coordenadas = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Main).Bp.ToString() + ".png");
            Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Main).Coordenadas.Width = 175;
            ExpandeBackpack(Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Main));
            return true;
        }
        private static void ExpandeBackpack(Backpack backpack)
        {
            backpack.Coordenadas.Height = 270;
            var pushBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, backpack.Coordenadas, Global._path + "\\Images\\ConfigsGerais\\setaDown.png");
            ClickEvent.ItemMove(Global._tibiaProcessName, new Point((pushBP.X + pushBP.Width / 2), (pushBP.Y + pushBP.Height + 3)), new Point((pushBP.X + pushBP.Width / 2) - 8, Global._tela.Height + 31));
            Thread.Sleep(100);
            pushBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, backpack.Coordenadas, Global._path + "\\Images\\ConfigsGerais\\setaDown.png");
            //Calcula tamanho final da backpack
            backpack.Coordenadas.Height = pushBP.Y+ pushBP.Height + 3 - backpack.Coordenadas.Y;
            backpack.Coordenadas.Width = pushBP.X+pushBP.Width + 3 - backpack.Coordenadas.X;

        }
        public static void InvokeMethod(Delegate method, params object[] args)
        {
            var result = (bool)method.DynamicInvoke(args);
            if (result)
                return;
            else
            {
                result = (bool)method.DynamicInvoke(args);
                if (result)
                    return;
            }
            throw new Exception("Ocorreu um erro.");           

        }
        private static bool OpenBackpack(Backpack backpack)
        {
            if (Global._backpacks.SingleOrDefault(bp => bp.Tipo == EnumTipoBackpack.Main ).Coordenadas == null)
            {
                OpenMainBackpack();
                Thread.Sleep(500);
            }
            ExpandeBackpack(Global._backpacks.SingleOrDefault(bp => bp.Tipo == EnumTipoBackpack.Main));
            Thread.Sleep(500);
            backpack.Coordenadas = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + backpack.Bp.ToString() + ".png");
            if (backpack.Coordenadas != null)
            {
                backpack.Coordenadas.Width = 175;
                ExpandeBackpack(backpack);
                return true;
            }
            var backpackToOpen = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._backpacks.SingleOrDefault(bp => bp.Tipo == EnumTipoBackpack.Main).Coordenadas, Global._path + "\\Images\\Global\\Backpacks\\Corpo\\" + backpack.Bp.ToString() + ".png");
            if (backpackToOpen == null)
                return false;
            ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point((backpackToOpen.X + backpackToOpen.Width / 2), backpackToOpen.Y + backpackToOpen.Height / 2), EnumMouseEvent.Right);
            Thread.Sleep(300);
            backpack.Coordenadas = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + backpack.Bp.ToString() + ".png");
            backpack.Coordenadas.Width = 175;
            ExpandeBackpack(backpack);
            return true;
        }
        private static bool OpenBackpackInNewTab(Backpack backpack)
        {
            if (Global._backpacks.SingleOrDefault(bp => bp.Tipo == EnumTipoBackpack.Main).Coordenadas == null)
            {
                OpenMainBackpack();
                Thread.Sleep(500);
            }
            Thread.Sleep(500);
            backpack.Coordenadas = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + backpack.Bp.ToString() + ".png");
            if (backpack.Coordenadas != null)
            {
                backpack.Coordenadas.Width = 175;
                ExpandeBackpack(backpack);
                return true;
            }
            var backpackToOpen = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._backpacks.SingleOrDefault(bp => bp.Tipo == EnumTipoBackpack.Main).Coordenadas, Global._path + "\\Images\\Global\\Backpacks\\Corpo\\" + backpack.Bp.ToString() + ".png");
            if (backpackToOpen == null)
                return false;
            AhkFunctions.SendKey("Ctrl Down", Global._tibiaProcessName);
            ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point(backpackToOpen.X + backpackToOpen.Width / 2, backpackToOpen.Y + backpackToOpen.Height / 2), EnumMouseEvent.Right);
            AhkFunctions.SendKey("Ctrl Up", Global._tibiaProcessName);
            Thread.Sleep(500);
            var openNewWindow = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Configs\\open_new_window.png");
            ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point(openNewWindow.X + openNewWindow.Width / 2, openNewWindow.Y + openNewWindow.Height / 2), EnumMouseEvent.Left);
            Thread.Sleep(300);
            backpack.Coordenadas = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + backpack.Bp.ToString() + ".png");
            backpack.Coordenadas.Width = 175;
            ExpandeBackpack(backpack);
            if(backpack.Tipo == EnumTipoBackpack.Gold || backpack.Tipo == EnumTipoBackpack.Loot)
            {
                MoveParaOFimDaBackpack(backpack);
            }
            return true;
        }
        public static bool OpenAllBackpacks()
        {
            var mainBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, 0, 0, Global._tela.Width, Global._tela.Height, Global._path + "\\Images\\Global\\Backpacks\\Nome\\" + Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Main).Bp.ToString() + ".png");
            if (mainBP == null)
                OpenMainBackpack();
            if (Global._backpacks.Count <= 0)
                return true;
            bool result = true;
            Global._backpacks.ForEach(backpack => { 
                if(backpack.Tipo != EnumTipoBackpack.Main && backpack.Bp != EnumBackpacks.none)
                {
                    if (!result)
                        return;
                    result = OpenBackpackInNewTab(backpack);
                    Thread.Sleep(300);
                    
                }
            });
            return true;
        }

        public static void MoveParaOFimDaBackpack(Backpack backpack)
        {
            var pushBP = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, backpack.Coordenadas, Global._path + "\\Images\\ConfigsGerais\\setaDown.png");
            var count = 0;
            while (count < 10)
            {
                ClickEvent.ClickOnElement(Global._tibiaProcessName, new Point((pushBP.X + pushBP.Width / 2), (pushBP.Y + pushBP.Height / 2)), EnumMouseEvent.Left);
                count++;
            }
        } 
        public static void GotoLabel(string label)
        {
            Cavebot.Index = Cavebot.Waypoints.FirstOrDefault(waypoint => waypoint.TypeAction == EnumAction.Label && waypoint.Parametros.ToLower() == label.ToLower()).Index - 1;
        }
    }
}
