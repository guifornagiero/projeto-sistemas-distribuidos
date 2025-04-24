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
    }
}
