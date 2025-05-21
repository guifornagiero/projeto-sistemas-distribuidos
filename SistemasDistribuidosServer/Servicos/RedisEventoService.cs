using System.Text.Json;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Interfaces.Servicos;

namespace SistemasDistribuidosServer.Servicos
{
    public class RedisEventoService : IEventoService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly string _portaServidor;
        private readonly ISubscriber _subscriber;

        public RedisEventoService(IConfiguration configuration)
        {
            _redis = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis"));
            _portaServidor = configuration["PortaServidor"];
            _subscriber = _redis.GetSubscriber();
            Console.WriteLine($"RedisEventoService inicializado na porta {_portaServidor}");
        }

        public async Task PublicarPostagem(Postagem postagem)
        {
            var message = JsonSerializer.Serialize(postagem);
            Console.WriteLine($"Servidor {_portaServidor} publicando postagem: {postagem.Titulo}");
            await _subscriber.PublishAsync("postagens", message);
        }

        public async Task PublicarChat(Chat chat)
        {
            var message = JsonSerializer.Serialize(chat);
            Console.WriteLine($"Servidor {_portaServidor} publicando chat entre {chat.Usuario1} e {chat.Usuario2}");
            await _subscriber.PublishAsync("chats", message);
        }

        public async Task PublicarMensagem(Chat chat, Mensagem mensagem)
        {
            var message = JsonSerializer.Serialize((chat.Usuario1 + "-" + chat.Usuario2, mensagem));
            Console.WriteLine($"Servidor {_portaServidor} publicando mensagem de {mensagem.Remetente.Login}");
            await _subscriber.PublishAsync("mensagens", message);
        }
    }
} 