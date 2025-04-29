using SistemasDistribuidosServer.Entidades.DTOs;

namespace SistemasDistribuidosServer.Entidades
{
    public class Postagem
    {
        public int Id { get; set; } = PostagemContador.GetCount();
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public int CriadorId { get; set; }
        public string CriadorNome { get; set; }
        public string CriadorLogin { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public Postagem(PostagemDTO postagem, Usuario criador)
        {
            Titulo = postagem.Titulo;
            Conteudo = postagem.Conteudo;
            CriadorId = criador.Id;
            CriadorNome = criador.Nome;
            CriadorLogin = criador.Login;
        }
    }

    public static class PostagemContador
    {
        public static int Count { get; set; } = 0;

        public static int GetCount()
        {
            Count++;
            return Count;
        }
    }
}
