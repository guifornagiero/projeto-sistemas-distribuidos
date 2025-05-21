using System.Text.Json;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Interfaces.Repositorios;

namespace SistemasDistribuidosServer.Servicos
{
    public class EventoListenerService : BackgroundService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IPostagemRepository _postagemRepository;
        private readonly IChatRepository _chatRepository;
        private readonly ISubscriber _subscriber;
        private readonly string _portaServidor;

        public EventoListenerService(
            IConfiguration configuration,
            IPostagemRepository postagemRepository,
            IChatRepository chatRepository)
        {
            _redis = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis"));
            _postagemRepository = postagemRepository;
            _chatRepository = chatRepository;
            _subscriber = _redis.GetSubscriber();
            _portaServidor = configuration["PortaServidor"];
            Console.WriteLine($"EventoListenerService inicializado na porta {_portaServidor}");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // Inscreve nos canais de sincronização
                await _subscriber.SubscribeAsync("postagens", (channel, message) =>
                {
                    try
                    {
                        var postagem = JsonSerializer.Deserialize<Postagem>(message);
                        if (postagem != null)
                        {
                            Console.WriteLine($"Servidor {_portaServidor} recebeu postagem: {postagem.Titulo}");
                            _postagemRepository.Publicar(postagem);
                            Console.WriteLine($"Servidor {_portaServidor} sincronizou postagem: {postagem.Titulo}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao processar postagem no servidor {_portaServidor}: {ex.Message}");
                    }
                });

                await _subscriber.SubscribeAsync("chats", (channel, message) =>
                {
                    try
                    {
                        var chat = JsonSerializer.Deserialize<Chat>(message);
                        if (chat != null)
                        {
                            Console.WriteLine($"Servidor {_portaServidor} recebeu chat entre {chat.Usuario1} e {chat.Usuario2}");
                            _chatRepository.CriarChat(chat.Usuario1, chat.Usuario2);
                            Console.WriteLine($"Servidor {_portaServidor} sincronizou chat entre {chat.Usuario1} e {chat.Usuario2}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao processar chat no servidor {_portaServidor}: {ex.Message}");
                    }
                });

                await _subscriber.SubscribeAsync("mensagens", (channel, message) =>
                {
                    try
                    {
                        var (chatId, mensagem) = JsonSerializer.Deserialize<(string, Mensagem)>(message);
                        var usuarios = chatId.Split('-');
                        Console.WriteLine($"Servidor {_portaServidor} recebeu mensagem de {mensagem.Remetente.Login}");
                        
                        var chat = _chatRepository.GetByUsuarios(usuarios[0], usuarios[1]);
                        if (chat != null)
                        {
                            chat.AddMensagem(mensagem);
                            Console.WriteLine($"Servidor {_portaServidor} sincronizou mensagem de {mensagem.Remetente.Login}");
                        }
                        else
                        {
                            Console.WriteLine($"Servidor {_portaServidor} não encontrou chat para {usuarios[0]} e {usuarios[1]}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao processar mensagem no servidor {_portaServidor}: {ex.Message}");
                    }
                });

                Console.WriteLine($"Servidor {_portaServidor} está escutando eventos...");

                // Mantém o serviço rodando
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro fatal no EventoListenerService do servidor {_portaServidor}: {ex.Message}");
                throw;
            }
        }
    }
} 