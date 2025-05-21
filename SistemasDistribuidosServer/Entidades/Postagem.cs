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

        // Parameterless constructor for JSON deserialization
        public Postagem() { }

        // Private constructor for creating from DTO
        private Postagem(PostagemDTO postagem, Usuario criador)
        {
            Titulo = postagem.Titulo;
            Conteudo = postagem.Conteudo;
            CriadorId = criador.Id;
            CriadorNome = criador.Nome;
            CriadorLogin = criador.Login;
        }

        // Factory method to create from DTO
        public static Postagem FromDTO(PostagemDTO postagem, Usuario criador)
        {
            return new Postagem(postagem, criador);
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
