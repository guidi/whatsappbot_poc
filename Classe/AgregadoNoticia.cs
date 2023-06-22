namespace Botzin.Classe
{
    public class AgregadoNoticia
    {
        public int count { get; set; }
        public List<Noticias> list { get; set; }
    }

    public class Noticias
    {
        public string title { get; set; }
        public string description { get; set; }
        public string link { get; set; }
        public string datetime { get; set; }
        public string date { get; set; }
        public string time { get; set; }
    }
}
