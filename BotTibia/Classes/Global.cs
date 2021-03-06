using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BotTibia.Elementos;

namespace BotTibia.Classes
{
    public static class Global
    {
        public static VidaBar _vida = new VidaBar();
        public static ManaBar _mana = new ManaBar();
        public static PersonagemStatus _status = new PersonagemStatus();
        public static int _fireTimer { get; set; }
        public static Thread _threadHeal { get; set; }
        public static string _telaPrincipal { get; set; }
        public static string _tibiaProcessName { get; set; }
    }
}
