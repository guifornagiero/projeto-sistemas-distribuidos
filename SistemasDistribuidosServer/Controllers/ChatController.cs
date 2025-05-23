using Microsoft.AspNetCore.Mvc;
using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Interfaces.Servicos;

namespace SistemasDistribuidosServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController(IChatService _chatService) : ControllerBase
    {
        [HttpGet("{usuario1}&{usuario2}")]
        public ActionResult<List<Mensagem>> GetChatByUser([FromRoute] string usuario1, [FromRoute] string usuario2)
        {
            List<Mensagem> mensagens = _chatService.GetChat(usuario1, usuario2);
            return Ok(mensagens);
        }

        [HttpGet("Chat/{usuario1}&{usuario2}")]
        public ActionResult<List<Mensagem>> GetChatByUsers([FromRoute] string usuario1, [FromRoute] string usuario2)
        {
            Chat chat = _chatService.GetChatEntity(usuario1, usuario2);
            return Ok(chat);
        }

        [HttpPost("EnviarMensagem/{enviando}&{recebendo}")]
        public ActionResult<Mensagem> EnviarMensagem([FromRoute] string enviando, [FromRoute] string recebendo, [FromBody] string mensagem)
        {
            Mensagem msg = _chatService.EnviarMensagem(enviando, recebendo, mensagem);
            return Ok(msg);
        }

        [HttpGet("GetChatsByUser/{userLogin}")]
        public ActionResult<Chat> GetChatsByUser([FromRoute] string userLogin)
        {
            List<Chat> chats = _chatService.GetChatsByUser(userLogin);
            return Ok(chats);
        }
    }
}
