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
        private static List<string> Waypoints = new List<string>();
        private static int Index = 0;
        public static void AddWaypoint(string waypoint)
        {
            Waypoints.Add(waypoint);
        }
        public static void ExecutaWaypoint()
        {
            if (!(Waypoints.Count() > Index))
                Index = 0;
            var waypoint = Waypoints.ElementAt(Index);
            var actions = waypoint.Split(':');
            bool chegou;
            switch (actions[0])
            {
                case "Node":
                    chegou = Node(actions);
                    if (chegou)
                    {
                        Index++;
                    }
                    break;
                case "Stand":
                    chegou = Node(actions);
                    if (chegou)
                    {
                        Index++;
                    }
                    break;
                case "Walk":
                    chegou = Node(actions);
                    if (chegou)
                    {
                        Index++;
                    }
                    break;
                case "Use":
                    chegou = Node(actions);
                    if (chegou)
                    {
                        Index++;
                    }
                    break;
                case "UseItem":
                    chegou = Node(actions);
                    if (chegou)
                    {
                        Index++;
                    }
                    break;
                case "CheckPos":
                    chegou = Node(actions);
                    if (chegou)
                    {
                        Index++;
                    }
                    break;
                case "CheckSupply":
                    chegou = Node(actions);
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
        public static bool Node(string[] actions)
        {
            var coordenadas = actions[1].Split(',');
            var andar = PegaAndarDoMap();
            var finalCordinates = new Point()
            {
                X = int.Parse(coordenadas[0]),
                Y = int.Parse(coordenadas[1])
            };
            if (andar == -1)
            {
                throw new Exception("Não foi possivel encontrar o andar no cavebot");
            }
            var coordenadasAtuais = PegaElementosDaTela.
                                    PegaCoordenadasDoPersonagem(Global._tibiaProcessName, Global._miniMap, andar);
            var EhcoodenadaFinal = (Math.Abs(finalCordinates.X - coordenadasAtuais.X) + 1
                                       <= int.Parse(coordenadas[3])) &&
                                       (Math.Abs(finalCordinates.Y - coordenadasAtuais.Y) + 1
                                       <= int.Parse(coordenadas[4]));
            if (!(EhcoodenadaFinal && andar == int.Parse(coordenadas[2])))
            {
                var click = new Point()
                {
                    X = Global._miniMap.X + Global._miniMap.Width / 2 + (finalCordinates.X - coordenadasAtuais.X) - 8,
                    Y = (Global._miniMap.Y + Global._miniMap.Height / 2 + (finalCordinates.Y - coordenadasAtuais.Y) - 31),
                };
                //ClickEvent.Click(Global._tibiaProcessName, new Point() { X = 1812, Y = 48-29}, Enum.MouseEvent.Right);
                ClickEvent.Click(Global._tibiaProcessName, click, Enum.MouseEvent.Left);
                return false;
            }
            return true;
        }
        public static bool Stand(string[] actions)
        {
            var coordenadas = actions[1].Split(',');
            var andar = PegaAndarDoMap();
            var finalCordinates = new Point()
            {
                X = int.Parse(coordenadas[0]),
                Y = int.Parse(coordenadas[1])
            };
            if (andar == -1)
            {
                throw new Exception("Não foi possivel encontrar o andar no cavebot");
            }
            var coordenadasAtuais = PegaElementosDaTela.
                                    PegaCoordenadasDoPersonagem(Global._tibiaProcessName, Global._miniMap, andar);
            var EhcoodenadaFinal = (finalCordinates.X == coordenadasAtuais.X) &&
                                   (finalCordinates.Y == coordenadasAtuais.Y);
            if (!(EhcoodenadaFinal && andar == int.Parse(coordenadas[2])))
            {
                var click = new Point()
                {
                    X = Global._miniMap.X + Global._miniMap.Width / 2 + (finalCordinates.X - coordenadasAtuais.X) - 8,
                    Y = (Global._miniMap.Y + Global._miniMap.Height / 2 + (finalCordinates.Y - coordenadasAtuais.Y) - 31),
                };
                //ClickEvent.Click(Global._tibiaProcessName, new Point() { X = 1812, Y = 48-29}, Enum.MouseEvent.Right);
                ClickEvent.Click(Global._tibiaProcessName, click, Enum.MouseEvent.Left);
                return false;
            }
            return true;
        }
    }
}
