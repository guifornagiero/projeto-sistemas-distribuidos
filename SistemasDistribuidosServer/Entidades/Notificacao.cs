namespace SistemasDistribuidosServer.Entidades
{
    public class Notificacao
    {
        public int Id { get; set; } = NotificadorContador.GetCount();
        public string Descricao { get; set; }
        public string PostagemTitulo { get; set; }
        public string PostagemDescricao { get; set; }
        public DateTime CriadaEm { get; set; } = DateTime.Now;

        public Notificacao()
        {
        }

        public Notificacao(string descricao, string postagemTitulo, string postagemDescricao)
        {
            Descricao = descricao;
            PostagemTitulo = postagemTitulo;
            PostagemDescricao = postagemDescricao;
        }
    }

    public static class NotificadorContador
    {
        public static int Count { get; set; } = 0;

        public static int GetCount()
        {
            Count++;
            return Count;
        }
    }
}
