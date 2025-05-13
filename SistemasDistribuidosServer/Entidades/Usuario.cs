using SistemasDistribuidosServer.Entidades.DTOs;

namespace SistemasDistribuidosServer.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public List<UsuarioDTO> Seguidores { get; set; } = [];
        public List<UsuarioDTO> Seguindo { get; set; } = [];
        public List<Notificacao> Notificacoes { get; set; } = [];

        public Usuario(int id, string nome, string login)
        {
            Nome = nome;
            Login = login;
        }

        public UsuarioDTO MapToDTO()
            => new UsuarioDTO
                {
                    Nome = Nome,
                    Login = Login,
                    Id = Id
                };

        public void AddSeguidor(UsuarioDTO seguidor)
        {
            Seguidores.Add(seguidor);
        }

        public void Seguir(UsuarioDTO usuario)
        {
            Seguindo.Add(usuario);
        }

        public void Notificar(Notificacao notificacao)
        {
            Notificacoes.Add(notificacao);
        }
    }
}
