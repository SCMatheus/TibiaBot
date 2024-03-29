﻿using BotTibia.Actions.Events;
using BotTibia.Classes;
using BotTibia.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace BotTibia.Actions.Cavebot {
    public static class Cavebot
    {
        public static List<Waypoint> Waypoints = new List<Waypoint>();
        public static int Index = 0;
        public static void AtualizaIndex()
        {
            var count = 1;
            Waypoints.ForEach(x => {
                x.Index = count;
                count++;
                });
        }
        public static void AddWaypoint(Waypoint waypoint)
        {
            Waypoints.Add(waypoint);
        }
        public static void ExecutaWaypoint()
        {
            var waypoint = Waypoints.ElementAt(Index);
            bool chegou;
            switch (waypoint.Type)
            {
                case EnumWaypoints.Node:
                    chegou = Node(waypoint.Coordenada, waypoint.Range);
                    if (chegou)
                    {
                        IndexAdd();
                    }
                    break;
                case EnumWaypoints.Stand:
                    chegou = Stand(waypoint.Coordenada);
                    if (chegou)
                    {
                        IndexAdd();
                    }
                    break;
                case EnumWaypoints.Action:
                    CavebotAction.ExecAction(waypoint.TypeAction, waypoint.Coordenada, waypoint.Parametros);
                    if(waypoint.TypeAction != EnumAction.GotoLabel)
                        IndexAdd();
                    break;
                case EnumWaypoints.Mark:
                    chegou = Mark(waypoint.Mark);
                    if (chegou) {
                        IndexAdd();
                    }
                    break;
            }
        }
        public static void IndexAdd()
        {
            if (Index >= Waypoints.Count() - 1)
            {
                Index = 0;
            }
            else
            {
                Index++;
            }
        }
        public static int PegaAndarDoMap()
        {
            CoordenadasDeElementos encontrado;
            List<int> andares= new List<int>()
            {
                7,6,8,5,9,4,10,3,11,2,12,1,13,0,14
            };
            foreach(var item in andares)
            {
                encontrado = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._andarDoMap, 
                                                                    Global._path + "\\Images\\MapAndar\\floor-" +
                                                                    item.ToString() + ".png");
                if (encontrado != null)
                {
                    return item;
                }
            }
            return -1;
        }
        #region Node
        public static bool Node(Coordenada coordenadaFinal, Range range)
        {
            var andar = PegaAndarDoMap();
            if (andar == -1)
            {
                Thread.Sleep(10);
                andar = PegaAndarDoMap();
                if (andar == -1)
                {
                    throw new Exception("Não foi possivel encontrar o andar no cavebot");
                }
            }
            var coordenadasAtuais = PegaElementosDaTela.PegaCoordenadasDoPersonagem(Global._tibiaProcessName, Global._miniMap, andar, Global._ultimaCoordenadaDoPersonagem);
            if (coordenadasAtuais == null)
                return false;
            Boolean EhcoodenadaFinal;
            if (andar != coordenadaFinal.Z)
            {
                EhcoodenadaFinal = (Math.Abs(coordenadaFinal.X - coordenadasAtuais.X) + 1
                                       <= 3) &&
                                       (Math.Abs(coordenadaFinal.Y - coordenadasAtuais.Y) + 1
                                       <= 3);
            }
            else
            {
                EhcoodenadaFinal = (Math.Abs(coordenadaFinal.X - coordenadasAtuais.X) + 1
                                        <= range.X) &&
                                        (Math.Abs(coordenadaFinal.Y - coordenadasAtuais.Y) + 1
                                        <= range.Y);
            }

            if (!EhcoodenadaFinal)
            {
                if (Global._ultimaCoordenadaDoPersonagem.Equals(coordenadasAtuais))
                {
                    var click = new Point()
                    {
                        X = Global._miniMap.X + Global._miniMap.Width / 2 + (coordenadaFinal.X - coordenadasAtuais.X),
                        Y = Global._miniMap.Y + Global._miniMap.Height / 2 + (coordenadaFinal.Y - coordenadasAtuais.Y),
                    };
                    ClickEvent.ClickOnElement(Global._tibiaProcessName, click, EnumMouseEvent.Left);
                }
                Global._ultimaCoordenadaDoPersonagem = coordenadasAtuais;
                return false;
            }
            Global._ultimaCoordenadaDoPersonagem = coordenadasAtuais;
            return true;
        }
        #endregion
        #region Mark
        public static bool Mark(EnumMarks mark) {
            var coordenadas = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._miniMap, Global._path + $"\\Images\\Marks\\{mark.ToString().ToLower()}.png");
            if (coordenadas != null) {
                if (coordenadas.Equals(Global._ultimoMark) || Global._ultimoMark == null) {
                    var click = new Point() {
                        X = coordenadas.X + 4,
                        Y = coordenadas.Y + 2,
                    };
                    ClickEvent.ClickOnElement(Global._tibiaProcessName, click, EnumMouseEvent.Left);
                }
                Global._ultimoMark = coordenadas;
                return false;
            }
            Global._ultimoMark = null;
            return true;
        }
        #endregion
        #region Stand
        public static bool Stand(Coordenada coordenadaFinal)
        {
            var andar = PegaAndarDoMap();
            if (andar == -1)
            {
                Thread.Sleep(100);
                andar = PegaAndarDoMap();
                if (andar == -1)
                {
                    throw new Exception("Não foi possivel encontrar o andar no cavebot");
                }
            }
            var coordenadasAtuais = PegaElementosDaTela.PegaCoordenadasDoPersonagem(Global._tibiaProcessName, Global._miniMap, andar, Global._ultimaCoordenadaDoPersonagem);
            if (coordenadasAtuais == null)
                return false;
            Boolean EhcoodenadaFinal;
            if (andar != coordenadaFinal.Z)
            {
                EhcoodenadaFinal = (Math.Abs(coordenadaFinal.X - coordenadasAtuais.X) + 1
                                       <= 3) &&
                                       (Math.Abs(coordenadaFinal.Y - coordenadasAtuais.Y) + 1
                                       <= 3);
            }
            else
            {
                EhcoodenadaFinal = (coordenadaFinal.X == coordenadasAtuais.X) &&
                        (coordenadaFinal.Y == coordenadasAtuais.Y);
            }
            if (!EhcoodenadaFinal)
            {
                if (Global._ultimaCoordenadaDoPersonagem.Equals(coordenadasAtuais))
                {
                    var click = new Point()
                    {
                        X = Global._miniMap.X + Global._miniMap.Width / 2 + (coordenadaFinal.X - coordenadasAtuais.X),
                        Y = Global._miniMap.Y + Global._miniMap.Height / 2 + (coordenadaFinal.Y - coordenadasAtuais.Y),
                    };
                    ClickEvent.ClickOnElement(Global._tibiaProcessName, click, EnumMouseEvent.Left);
                }
                Global._ultimaCoordenadaDoPersonagem = coordenadasAtuais;
                return false;
            }
            Global._ultimaCoordenadaDoPersonagem = coordenadasAtuais;
            return true;
        }
        #endregion
    }
}
