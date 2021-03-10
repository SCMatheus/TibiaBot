using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BotTibia.Actions.Events;
using BotTibia.Actions.Print;
using BotTibia.Classes;

namespace BotTibia.Actions.Cavebot
{
    public static class Cavebot
    {
        private static List<Waypoint> Waypoints = new List<Waypoint>();
        private static int Index = 0;
        public static void AddWaypoint(Waypoint waypoint)
        {
            Waypoints.Add(waypoint);
        }
        public static void ExecutaWaypoint()
        {
            if (!(Waypoints.Count() > Index))
                Index = 0;
            var waypoint = Waypoints.ElementAt(Index);
            bool chegou;
            switch (waypoint.Type)
            {
                case "Node":
                    chegou = Node(waypoint.Coordenadas, waypoint.Range);
                    if (chegou)
                    {
                        Index++;
                    }
                    break;
                case "Stand":
                    chegou = Stand(waypoint.Coordenadas);
                    if (chegou)
                    {
                        Index++;
                    }
                    break;
                case "Walk":
                    chegou = Node(waypoint.Coordenadas, waypoint.Range);
                    if (chegou)
                    {
                        Index++;
                    }
                    break;
                case "Use":
                    chegou = Node(waypoint.Coordenadas, waypoint.Range);
                    if (chegou)
                    {
                        Index++;
                    }
                    break;
                case "UseItem":
                    chegou = Node(waypoint.Coordenadas, waypoint.Range);
                    if (chegou)
                    {
                        Index++;
                    }
                    break;
                case "CheckPos":
                    chegou = Node(waypoint.Coordenadas, waypoint.Range);
                    if (chegou)
                    {
                        Index++;
                    }
                    break;
                case "CheckSupply":
                    chegou = Node(waypoint.Coordenadas, waypoint.Range);
                    if (chegou)
                    {
                        Index++;
                    }
                    break;
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
                                                                    Global._caminho + "\\Images\\MapAndar\\floor-" +
                                                                    item.ToString() + ".png");
                if (encontrado != null)
                {
                    return item;
                }
            }
            return -1;
        }

        public static void Hunting()
        {
            try
            {
                while (true)
                {
                    ExecutaWaypoint();
                }
            }catch(ThreadAbortException ex)
            {

            }
        }
        public static bool Node(Coordenada coordenadaFinal, Range range)
        {
            var andar = PegaAndarDoMap();
            if (andar == -1)
            {
                throw new Exception("Não foi possivel encontrar o andar no cavebot");
            }
            var coordenadasAtuais = PegaElementosDaTela.PegaCoordenadasDoPersonagem(Global._tibiaProcessName, Global._miniMap, andar, Global._ultimaCoordenadaDoPersonagem);
            if (coordenadasAtuais == null)
                return false;
            Global._ultimaCoordenadaDoPersonagem = coordenadasAtuais;
            var EhcoodenadaFinal = (Math.Abs(coordenadaFinal.X - coordenadasAtuais.X) + 1
                                       <= range.X) &&
                                       (Math.Abs(coordenadaFinal.Y - coordenadasAtuais.Y) + 1
                                       <= range.Y);
            if (!(EhcoodenadaFinal && coordenadasAtuais.Z == coordenadaFinal.Z))
            {
                var click = new Point()
                {
                    X = Global._miniMap.X + Global._miniMap.Width / 2 + (coordenadaFinal.X - coordenadasAtuais.X) - 8,
                    Y = Global._miniMap.Y + Global._miniMap.Height / 2 + (coordenadaFinal.Y - coordenadasAtuais.Y) - 31,
                };
                ClickEvent.Click(Global._tibiaProcessName, click, Enum.MouseEvent.Left);
                return false;
            }
            return true;
        }
        public static bool Stand(Coordenada coordenada)
        {
            var andar = PegaAndarDoMap();
            if (andar == -1)
            {
                throw new Exception("Não foi possivel encontrar o andar no cavebot");
            }
            var coordenadasAtuais = PegaElementosDaTela.
                                    PegaCoordenadasDoPersonagem(Global._tibiaProcessName, Global._miniMap, andar, Global._ultimaCoordenadaDoPersonagem);
            var EhcoodenadaFinal = (coordenada.X == coordenadasAtuais.X) &&
                                    (coordenada.Y == coordenadasAtuais.Y);
            if (!(EhcoodenadaFinal && andar == coordenada.Z))
            {
                var click = new Point()
                {
                    X = Global._miniMap.X + Global._miniMap.Width / 2 + (coordenada.X - coordenadasAtuais.X) - 8,
                    Y = (Global._miniMap.Y + Global._miniMap.Height / 2 + (coordenada.Y - coordenadasAtuais.Y) - 31),
                };
                ClickEvent.Click(Global._tibiaProcessName, click, Enum.MouseEvent.Left);
                return false;
            }
            return true;
        }
    }
}
