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
    }
}
