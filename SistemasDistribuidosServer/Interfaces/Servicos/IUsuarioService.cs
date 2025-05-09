using SistemasDistribuidosServer.Entidades;

namespace SistemasDistribuidosServer.Interfaces.Servicos
{
    public interface IUsuarioService
    {
        List<Usuario> GetAll();
        Usuario GetById(int id);
        Usuario GetByLogin(string login);
        Usuario Seguir(string login, string loginToFollow);
        List<Notificacao> GetNotificacoes(string login);
    }
}
