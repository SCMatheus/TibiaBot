using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace BotTibia.Classes
{
    public static class Global
    {
        public static VidaBar _vida = new VidaBar() { pixel = Color.FromArgb(255, 219, 79, 79) };
        public static ManaBar _mana = new ManaBar() { pixel = Color.FromArgb(255, 83, 80, 218) };
        public static PersonagemStatus _status = new PersonagemStatus();
        public static int _fireTimer { get; set; }
        public static Thread _threadHeal { get; set; }
        public static Thread _threadCavebot { get; set; }
        public static string _telaPrincipal { get; set; }
        public static string _tibiaProcessName { get; set; }
        public static CoordenadasDeElementos _tela = new CoordenadasDeElementos();
        public static CoordenadasDeElementos _mainWindow = new CoordenadasDeElementos();
        public static CoordenadasDeElementos _miniMap = new CoordenadasDeElementos();
        public static CoordenadasDeElementos _andarDoMap = new CoordenadasDeElementos();
        public static CoordenadasDeElementos _battle = new CoordenadasDeElementos();
        public static Coordenada _ultimaCoordenadaDoPersonagem = new Coordenada();
        public static string _path;
        public static bool _isTarget = false;
        public static bool _isLoot = false;
        public static bool _isCavebot = false;
        public static List<Variavel> _variaveisGlobais = new List<Variavel>();
        public static List<Backpack> _backpacks = new List<Backpack>();
        public static List<CoordenadasDeElementos> _backpacksCoord = new List<CoordenadasDeElementos>(); //TODO CLASS COORDENADAS E BACKPACK
        public static List<string> _dropItens = new List<string>();
        public static List<string> _moveLootItens = new List<string>();
        public static Point _coordenadaDoPersonagem = new Point();
        public static CoordenadasDeElementos _cap = new CoordenadasDeElementos();
        public static bool _isEatFood = false;
        public static string _FoodKey = "F1";
    }
}
