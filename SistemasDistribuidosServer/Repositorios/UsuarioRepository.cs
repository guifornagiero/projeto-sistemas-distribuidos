using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Interfaces.Repositorios;

namespace SistemasDistribuidosServer.Repositorios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly List<Usuario> _usuarios = [new(1, "Guilherme", "guifornagiero"), 
                                                    new(2, "Paulo", "paulobrito"), 
                                                    new(3, "Pedro", "pedrobento"), 
                                                    new(4, "Gian", "gianluca")];

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

        public Usuario Update(Usuario usuario)
        {
            var usuarioExistente = _usuarios.FirstOrDefault(u => u.Id == usuario.Id);

            if (usuarioExistente == null)
                throw new InvalidOperationException("Usuário não existe no database para dar update.");

            usuarioExistente.Nome = usuario.Nome;
            usuarioExistente.Login = usuario.Login;
            usuarioExistente.Seguidores = usuario.Seguidores;
            usuarioExistente.Seguindo = usuario.Seguindo;

            return usuarioExistente;
        }

        public Usuario Insert(Usuario usuario)
        {
            if (_usuarios.Any(u => u.Login == usuario.Login))
                throw new InvalidOperationException("Já existe um usuário com esse login.");

            _usuarios.Add(usuario);
            return usuario;
        }
    }
}
