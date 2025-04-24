using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Interfaces.Repositorios;

namespace SistemasDistribuidosServer.Repositorios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly List<Usuario> _usuarios = [new("Guilherme"), new("Paulo"), new("Pedro"), new("Gian")];

        public List<Usuario> GetAll()
        {
            return _usuarios;
        }
    }
}
