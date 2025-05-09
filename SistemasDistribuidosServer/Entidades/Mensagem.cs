using SistemasDistribuidosServer.Entidades.DTOs;

namespace SistemasDistribuidosServer.Entidades
{
    public class Mensagem
    {
        public int Id { get; set; } = MensagemContador.GetCount();
        public UsuarioDTO Remetente { get; set; }
        public string Texto { get; set; }
        public DateTime EnviadaEm { get; set; } = DateTime.Now;

        public Mensagem(UsuarioDTO remetente, string texto)
        {
            Remetente = remetente;
            Texto = texto;
        }
    }

    public static class MensagemContador
    {
        public static int Count { get; set; } = 0;

        public static int GetCount()
        {
            Count++;
            return Count;
        }
    }
}
