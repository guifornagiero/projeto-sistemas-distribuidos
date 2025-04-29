using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Entidades.DTOs;
using SistemasDistribuidosServer.Interfaces.Repositorios;
using SistemasDistribuidosServer.Interfaces.Servicos;

namespace SistemasDistribuidosServer.Servicos
{
    public class PostagemService(IUsuarioService _usuarioService, 
                                 IPostagemRepository _repository) : IPostagemService
    {
        public List<Postagem> GetPostagens()
        {
            return _repository.GetPostagens();
        }

        public Postagem Publicar(string criadorLogin, PostagemDTO postagem)
        {
            Usuario criador = _usuarioService.GetByLogin(criadorLogin) 
                ?? throw new Exception($"Criador não encontrado com login - {criadorLogin}");

            Postagem post = new(postagem, criador);

            return _repository.Publicar(post);
        }
    }
}
