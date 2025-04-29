using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Interfaces.Repositorios;

namespace SistemasDistribuidosServer.Repositorios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly List<Usuario> _usuarios = [new("Guilherme", "guifornagiero"), 
                                                    new("Paulo", "paulobrito"), 
                                                    new("Pedro", "pedrobento"), 
                                                    new("Gian", "gianluca")];

        public List<Usuario> GetAll()
        {
            return _usuarios;
        }

        public Usuario GetById(int id)
        {
            return _usuarios.Where(u => u.Id == id).FirstOrDefault();
        }

        public Usuario GetByLogin(string login)
        {
            return _usuarios.Where(u => u.Login == login).FirstOrDefault();
        }
    }
}
