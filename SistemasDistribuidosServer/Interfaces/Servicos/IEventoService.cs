using SistemasDistribuidosServer.Entidades;

namespace SistemasDistribuidosServer.Interfaces.Servicos
{
    public interface IEventoService
    {
        Task PublicarPostagem(Postagem postagem);
        Task PublicarChat(Chat chat);
        Task PublicarMensagem(Chat chat, Mensagem mensagem);
    }
}