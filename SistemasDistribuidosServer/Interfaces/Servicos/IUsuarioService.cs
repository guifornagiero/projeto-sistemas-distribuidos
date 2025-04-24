using SistemasDistribuidosServer.Entidades;

namespace SistemasDistribuidosServer.Interfaces.Servicos
{
    public interface IUsuarioService
    {
        List<Usuario> GetAll();
    }
}
