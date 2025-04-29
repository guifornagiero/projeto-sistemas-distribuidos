using SistemasDistribuidosServer.Entidades;

namespace SistemasDistribuidosServer.Interfaces.Repositorios
{
    public interface IPostagemRepository
    {
        List<Postagem> GetPostagens();
        Postagem Publicar(Postagem postagem);
    }
}
