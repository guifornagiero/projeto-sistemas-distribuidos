using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Entidades.DTOs;
using SistemasDistribuidosServer.Interfaces.Repositorios;

namespace SistemasDistribuidosServer.Repositorios
{
    public class ChatRepository : IChatRepository
    {
        private List<Chat> _chats = [];

        public Chat CriarChat(string usuario1, string usuario2)
        {
            Chat chat = new Chat(usuario1, usuario2);
            _chats.Add(chat);
            return chat;
        }

        public Chat GetByUsuarios(string usuario1, string usuario2)
        {
            Chat chat = _chats.FirstOrDefault(c => c.Usuario1 == usuario1 && c.Usuario2 == usuario2);
            chat ??= _chats.FirstOrDefault(c => c.Usuario1 == usuario2 && c.Usuario2 == usuario1);

            return chat;
        }

        public List<Chat> GetChatsByUser(string userLogin)
        {
            List<Chat> chats = _chats.Where(c => c.Usuario1 == userLogin || c.Usuario2 == userLogin).ToList();
            return chats;
        }
    }
}
