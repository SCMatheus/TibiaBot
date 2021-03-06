namespace BotTibia.Elementos
{
    public class PersonagemStatus
    {
        public CoordenadasDeElementos Coordenadas { get; set; }
        public string ParaKey { get; set; }
        public bool ParaStatus { get; set; }
        public bool EhEk { get; set; }

        public PersonagemStatus()
        {
            Coordenadas = new CoordenadasDeElementos();
            Coordenadas.Width = 115;
            ParaStatus = false;
            EhEk = false;
        }

    }
}
