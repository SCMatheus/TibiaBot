using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotTibia.Actions.Print;
using BotTibia.Classes;

namespace BotTibia.Actions.Cavebot
{
    public class Cavebot
    {
        public List<string> Waypoints {get; set;}
        public Cavebot()
        {
            Waypoints = new List<string>();
        }
        public void AddWaypoint(string waypoint)
        {

        }
        public void ExecutaWaypoint(Bitmap tela, int index)
        {
            var waypoint = Waypoints.ElementAt(index);
            var actions = waypoint.Split(':');
            switch (actions[0])
            {
                case "Node":
                    var coordenadas = actions[1].Split(',');
                    var coordenadasAtuais = PegaElementosDaTela.
                                            PegaCoordenadasDoPersonagem(Global._tibiaProcessName, Global._miniMap.X,
                                                                        Global._miniMap.Y, Global._miniMap.Width,
                                                                        Global._miniMap.Height,7);
                    break;
            }
        }
    }
}
