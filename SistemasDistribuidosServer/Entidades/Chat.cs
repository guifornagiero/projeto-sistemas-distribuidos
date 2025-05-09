using SistemasDistribuidosServer.Entidades.DTOs;

namespace SistemasDistribuidosServer.Entidades
{
    public class Chat
    {
        public int Id { get; private set; } = ChatContador.GetCount();
        public string Usuario1 { get; set; }
        public string Usuario2 { get; set; }
        public List<Mensagem> Mensagens { get; set; } = [];

        public Chat(string usuario1, string usuario2)
        {
            Usuario1 = usuario1;
            Usuario2 = usuario2;
        }

        public void AddMensagem(Mensagem mensagem)
        {
            Mensagens.Add(mensagem);
        }
    }

    public static class ChatContador
    {
        public static int Count { get; set; } = 0;

        public static int GetCount()
        {
            Count++;
            return Count;
        }
    }
}
