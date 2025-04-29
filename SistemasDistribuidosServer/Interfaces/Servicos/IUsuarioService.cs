using SistemasDistribuidosServer.Entidades;

namespace SistemasDistribuidosServer.Interfaces.Servicos
{
    public interface IUsuarioService
    {
        List<Usuario> GetAll();
        Usuario GetById(int id);
        Usuario GetByLogin(string login);
    }
}
