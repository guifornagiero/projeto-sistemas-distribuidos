using Microsoft.AspNetCore.Mvc;
using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Entidades.DTOs;
using SistemasDistribuidosServer.Interfaces.Servicos;

namespace SistemasDistribuidosServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController(IUsuarioService _usuarioService) : ControllerBase
    {
        [HttpGet("GetAll")]
        public ActionResult<List<Usuario>> GetUsuarios()
        {
            List<Usuario> usuarios = _usuarioService.GetAll();

            if (usuarios == null || usuarios.Count == 0)
                return NotFound();

            Console.WriteLine(usuarios);

            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public ActionResult<Usuario> GetById([FromRoute] int id)
        {
            Usuario usuario = _usuarioService.GetById(id);

            if (usuario == null) 
                return NotFound();

            Console.WriteLine(usuario);

            return Ok(usuario);
        }

        [HttpPost("Seguir")]
        public ActionResult<Usuario> Seguir([FromBody] SeguirDTO dto)
        {
            Usuario user = _usuarioService.Seguir(dto.LoginQuerSeguir, dto.LoginParaSeguir);
            return Ok(user);
        }

        [HttpGet("Notificacoes/{login}")]
        public ActionResult<List<Notificacao>> GetNotificacoes([FromRoute] string login)
        {
            List<Notificacao> notificacoes = _usuarioService.GetNotificacoes(login);
            return Ok(notificacoes);
        }

        [HttpGet("Login/{login}")]
        public ActionResult<Usuario> GetByLogin([FromRoute] string login)
        {
            Usuario usuario = _usuarioService.GetByLogin(login);
            return Ok(usuario);
        }
    }
}
