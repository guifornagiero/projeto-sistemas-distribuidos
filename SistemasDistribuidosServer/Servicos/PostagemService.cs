using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Entidades.DTOs;
using SistemasDistribuidosServer.Interfaces.Repositorios;
using SistemasDistribuidosServer.Interfaces.Servicos;

namespace SistemasDistribuidosServer.Servicos
{
    public class PostagemService(IUsuarioService _usuarioService, 
                                 IPostagemRepository _repository,
                                 INotificadorService _notificadorService) : IPostagemService
    {
        public List<Postagem> GetPostagens()
        {
            return _repository.GetPostagens();
        }

        public Postagem Publicar(PostagemDTO postagem)
        {
            ValidarCampos(postagem);

            Usuario criador = _usuarioService.GetByLogin(postagem.CriadorLogin) 
                ?? throw new KeyNotFoundException($"Criador não encontrado com login - {postagem.CriadorLogin}");

            Postagem post = Postagem.FromDTO(postagem, criador);
            _repository.Publicar(post);

            _notificadorService.NotificarSeguidores(criador, post);

            return post;
        }

        private void ValidarCampos(PostagemDTO postagem)
        {
            if (string.IsNullOrWhiteSpace(postagem.CriadorLogin))
                throw new ArgumentException("O login do criador não pode ser vazio ou nulo.", nameof(postagem.CriadorLogin));

            if (string.IsNullOrWhiteSpace(postagem.Titulo))
                throw new ArgumentException("O título da postagem não pode ser vazio ou nulo.", nameof(postagem.Titulo));

            if (string.IsNullOrWhiteSpace(postagem.Conteudo))
                throw new ArgumentException("O conteúdo da postagem não pode ser vazio ou nulo.", nameof(postagem.Conteudo));
        }
    }
}
