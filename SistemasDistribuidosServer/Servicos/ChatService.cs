using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Entidades.DTOs;
using SistemasDistribuidosServer.Interfaces.Repositorios;
using SistemasDistribuidosServer.Interfaces.Servicos;

namespace SistemasDistribuidosServer.Servicos
{
    public class ChatService(IUsuarioRepository _usuarioRepository, IChatRepository _chatRepository) : IChatService
    {
        public Mensagem EnviarMensagem(string enviando, string recebendo, string mensagem)
        {
            if (enviando == recebendo)
                throw new ArgumentException("Usuário que envia e que recebe não podem ser iguais.");

            UsuarioDTO usuario1 = _usuarioRepository.GetByLogin(enviando).MapToDTO();
            UsuarioDTO usuario2 = _usuarioRepository.GetByLogin(recebendo).MapToDTO();

            Chat chat = _chatRepository.GetByUsuarios(usuario1.Login, usuario2.Login);
            chat ??= _chatRepository.CriarChat(usuario1.Login, usuario2.Login);

            Mensagem msg = new Mensagem(usuario1, mensagem);
            chat.AddMensagem(msg);

            return msg;
        }

        public List<Mensagem> GetChat(string usuario1, string usuario2)
        {
            if (usuario1 == usuario2)
                throw new ArgumentException("Usuário que envia e que recebe não podem ser iguais.");

            UsuarioDTO user1 = _usuarioRepository.GetByLogin(usuario1).MapToDTO();
            UsuarioDTO user2 = _usuarioRepository.GetByLogin(usuario2).MapToDTO();

            Chat chat = _chatRepository.GetByUsuarios(user1.Login, user2.Login);
            if (chat == null)
                return [];

            return chat.Mensagens;
        }

        public Chat GetChatEntity(string usuario1, string usuario2)
        {
            if (usuario1 == usuario2)
                throw new ArgumentException("Usuário que envia e que recebe não podem ser iguais.");

            UsuarioDTO user1 = _usuarioRepository.GetByLogin(usuario1).MapToDTO();
            UsuarioDTO user2 = _usuarioRepository.GetByLogin(usuario2).MapToDTO();

            Chat chat = _chatRepository.GetByUsuarios(user1.Login, user2.Login);
            return chat;
        }

        public List<Chat> GetChatsByUser(string userLogin)
        {
            List<Chat> chats = _chatRepository.GetChatsByUser(userLogin);
            return chats;
        }
    }
}
