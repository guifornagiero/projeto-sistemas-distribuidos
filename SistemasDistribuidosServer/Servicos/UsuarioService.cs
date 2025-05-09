using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Interfaces.Repositorios;
using SistemasDistribuidosServer.Interfaces.Servicos;

namespace SistemasDistribuidosServer.Servicos
{
    public class UsuarioService(IUsuarioRepository _repository) : IUsuarioService
    {
        public List<Usuario> GetAll()
        {
            return _repository.GetAll();
        }

        public Usuario GetById(int id)
        {
            return _repository.GetById(id);
        }

        public Usuario GetByLogin(string login)
        {
            return _repository.GetByLogin(login.ToLower());
        }

        public Usuario Seguir(string login, string loginToFollow)
        {
            Usuario user = _repository.GetByLogin(login) ?? throw new Exception("Usuário que está tentando seguir não encontrado.");
            Usuario userToFollow = _repository.GetByLogin(loginToFollow) ?? throw new Exception("Usuário para seguir não encontrado.");

            if (user.Seguindo.Exists(u => u.Login == loginToFollow))
                throw new Exception($"{user.Login} já segue {userToFollow.Login}.");

            userToFollow.AddSeguidor(user.MapToDTO());
            user.Seguir(userToFollow.MapToDTO());

            _repository.Update(userToFollow);
            _repository.Update(user);

            return user;
        }

        public List<Notificacao> GetNotificacoes(string login)
        {
            Usuario user = _repository.GetByLogin(login);
            return user.Notificacoes;
        }
    }
}
