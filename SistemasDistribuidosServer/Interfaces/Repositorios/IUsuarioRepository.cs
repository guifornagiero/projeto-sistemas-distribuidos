using SistemasDistribuidosServer.Entidades;

namespace SistemasDistribuidosServer.Interfaces.Repositorios
{
    public interface IUsuarioRepository
    {
        List<Usuario> GetAll();
    }
}
