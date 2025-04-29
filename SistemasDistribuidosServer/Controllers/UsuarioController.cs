using Microsoft.AspNetCore.Mvc;
using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Interfaces.Servicos;

namespace SistemasDistribuidosServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController(IUsuarioService _usuarioService) : ControllerBase
    {
        [HttpGet]
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
    }
}
