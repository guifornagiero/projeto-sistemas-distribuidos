using Microsoft.AspNetCore.Mvc;
using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Entidades.DTOs;
using SistemasDistribuidosServer.Interfaces.Servicos;

namespace SistemasDistribuidosServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostagemController(IPostagemService _postagemService) : ControllerBase
    {
        [HttpGet("Timeline")]
        public ActionResult<List<Postagem>> GetPostagens()
        {
            List<Postagem> postagens = _postagemService.GetPostagens();

            if (postagens == null)
                return NotFound();

            return Ok(postagens);
        }

        [HttpPost]
        public ActionResult<Postagem> Publicar([FromBody] PostagemDTO postagem)
        {
            Postagem postCriado = _postagemService.Publicar(postagem.CriadorLogin, postagem);

            if (postCriado == null)
                return NotFound();

            return Ok(postCriado);
        }
    }
}
