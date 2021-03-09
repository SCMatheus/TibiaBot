using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
            CoordenadasDeElementos encontrado = null;
            var count = 0;
            while (count <= 15)
            {
                encontrado = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._andarDoMap, 
                                                                    Global._caminho + "\\Images\\MapAndar\\floor-" + 
                                                                    count.ToString() + ".png");
                if (encontrado != null)
                {
                    break;
                }
                count++;
            }
            count = encontrado == null ? -1 : count;
            return count;
        }

        public static void Hunting()
        {
            while (true)
            {
                ExecutaWaypoint();
            }
        }
        public static bool Node(Coordenada coordenada, Range range)
        {
            var andar = PegaAndarDoMap();
            if (andar == -1)
            {
                throw new Exception("Não foi possivel encontrar o andar no cavebot");
            }
            var coordenadasAtuais = PegaElementosDaTela.
                                    PegaCoordenadasDoPersonagem(Global._tibiaProcessName, Global._miniMap, andar);
            var EhcoodenadaFinal = (Math.Abs(coordenada.X - coordenadasAtuais.X) + 1
                                       <= range.X) &&
                                       (Math.Abs(coordenada.Y - coordenadasAtuais.Y) + 1
                                       <= range.Y);
            if (!(EhcoodenadaFinal && andar == coordenada.Z))
            {
                var click = new Point()
                {
                    X = Global._miniMap.X + Global._miniMap.Width / 2 + (coordenada.X - coordenadasAtuais.X) - 8,
                    Y = Global._miniMap.Y + Global._miniMap.Height / 2 + (coordenada.Y - coordenadasAtuais.Y) - 31,
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
                                    PegaCoordenadasDoPersonagem(Global._tibiaProcessName, Global._miniMap, andar);
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
