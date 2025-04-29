using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Entidades.DTOs;

namespace SistemasDistribuidosServer.Interfaces.Servicos
{
    public interface IPostagemService
    {
        List<Postagem> GetPostagens();
        Postagem Publicar(PostagemDTO postagem);
    }
}
