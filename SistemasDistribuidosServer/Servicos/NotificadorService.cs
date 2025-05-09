using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Entidades.DTOs;
using SistemasDistribuidosServer.Interfaces.Repositorios;
using SistemasDistribuidosServer.Interfaces.Servicos;

namespace SistemasDistribuidosServer.Servicos
{
    public class NotificadorService(IUsuarioRepository _userRepository) : INotificadorService
    {
        public void NotificarSeguidores(Usuario criador, Postagem postagem)
        {
            Notificacao notificacao = new Notificacao($"O usuário {criador.Login} fez uma nova postagem. Confira!", postagem.Titulo, postagem.Conteudo);

            foreach (UsuarioDTO seguidor in criador.Seguidores)
            {
                Usuario user = _userRepository.GetByLogin(seguidor.Login);
                user.Notificar(notificacao);

                _userRepository.Update(user);
            }
        }
    }
}
