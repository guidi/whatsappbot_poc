using Botzin.Enum;

namespace Botzin.Classe
{
    public class Mensagem
    {
        public String Id { get; set; }
        public String Texto { get; set; }
        public String Comando { get; set; }
        public String Personagem { get; set; }
        public StatusMensagem Status { get; set; }
        public AcaoBOT Acao { get; set; }
    }
}
