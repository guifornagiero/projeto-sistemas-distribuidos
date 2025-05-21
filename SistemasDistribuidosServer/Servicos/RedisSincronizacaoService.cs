using System.Text.Json;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Interfaces.Repositorios;
using SistemasDistribuidosServer.Interfaces.Servicos;

namespace SistemasDistribuidosServer.Servicos
{
    public class RedisSincronizacaoService : ISincronizacaoService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IPostagemRepository _postagemRepository;
        private readonly IChatRepository _chatRepository;
        private readonly string _portaServidor;
        private readonly ISubscriber _subscriber;

        public RedisSincronizacaoService(
            IConfiguration configuration,
            IPostagemRepository postagemRepository,
            IChatRepository chatRepository)
        {
            _redis = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis"));
            _postagemRepository = postagemRepository;
            _chatRepository = chatRepository;
            _portaServidor = configuration["PortaServidor"];
            _subscriber = _redis.GetSubscriber();
        }

        public async Task Inicializar()
        {
            // Inscreve nos canais de sincronização
            await _subscriber.SubscribeAsync("postagens", (channel, message) =>
            {
                var postagem = JsonSerializer.Deserialize<Postagem>(message);
                if (postagem != null)
                {
                    _postagemRepository.Publicar(postagem);
                }
            });

            await _subscriber.SubscribeAsync("chats", (channel, message) =>
            {
                var chat = JsonSerializer.Deserialize<Chat>(message);
                if (chat != null)
                {
                    _chatRepository.CriarChat(chat.Usuario1, chat.Usuario2);
                }
            });

            await _subscriber.SubscribeAsync("mensagens", (channel, message) =>
            {
                var (chatId, mensagem) = JsonSerializer.Deserialize<(string, Mensagem)>(message);
                var chat = _chatRepository.GetByUsuarios(chatId.Split('-')[0], chatId.Split('-')[1]);
                if (chat != null)
                {
                    chat.AddMensagem(mensagem);
                }
            });
        }

        public async Task PublicarPostagem(Postagem postagem)
        {
            var message = JsonSerializer.Serialize(postagem);
            await _subscriber.PublishAsync("postagens", message);
        }

        public async Task PublicarChat(Chat chat)
        {
            var message = JsonSerializer.Serialize(chat);
            await _subscriber.PublishAsync("chats", message);
        }

        public async Task PublicarMensagem(Chat chat, Mensagem mensagem)
        {
            var message = JsonSerializer.Serialize((chat.Usuario1 + "-" + chat.Usuario2, mensagem));
            await _subscriber.PublishAsync("mensagens", message);
        }
    }
} 