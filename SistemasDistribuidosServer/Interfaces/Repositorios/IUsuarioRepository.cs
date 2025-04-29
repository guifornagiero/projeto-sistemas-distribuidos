using SistemasDistribuidosServer.Entidades;

namespace SistemasDistribuidosServer.Interfaces.Repositorios
{
    public interface IUsuarioRepository
    {
        List<Usuario> GetAll();
        Usuario GetById(int id);
        Usuario GetByLogin(string login);
    }
}
