using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Interfaces.Repositorios;

namespace SistemasDistribuidosServer.Repositorios
{
    public class PostagemRepository : IPostagemRepository
    {
        private List<Postagem> _timeline = [];

        public List<Postagem> GetPostagens()
        {
            return _timeline;
        }

        public Postagem Publicar(Postagem postagem)
        {
            _timeline.Add(postagem);
            _timeline = [.. _timeline.OrderBy(p => p.DataCriacao)];

            return postagem;
        }
    }
}
