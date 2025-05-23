using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Entidades.DTOs;

namespace SistemasDistribuidosServer.Interfaces.Repositorios
{
    public interface IChatRepository
    {
        Chat GetByUsuarios(string usuario1, string usuario2);
        Chat CriarChat(string usuario1, string usuario2);
        List<Chat> GetChatsByUser(string userLogin);
    }
}
