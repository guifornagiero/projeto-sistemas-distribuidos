using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<EventoListenerService> _logger;

        public EventoListenerService(
            IConfiguration configuration,
            IPostagemRepository postagemRepository,
            IChatRepository chatRepository,
            ILogger<EventoListenerService> logger)
        {
            _logger = logger;
            
            string redisConnectionString = configuration.GetConnectionString("Redis");
            if (string.IsNullOrEmpty(redisConnectionString))
            {
                redisConnectionString = "localhost:6379,abortConnect=false";
                _logger.LogWarning("String de conexão Redis não encontrada na configuração. Usando padrão: " + redisConnectionString);
            }
            
            _redis = ConnectionMultiplexer.Connect(redisConnectionString);
            _postagemRepository = postagemRepository;
            _chatRepository = chatRepository;
            _subscriber = _redis.GetSubscriber();
            _portaServidor = configuration["PortaServidor"] ?? "5001";
            _logger.LogInformation($"EventoListenerService inicializado na porta {_portaServidor}");
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
                            _logger.LogInformation($"Servidor {_portaServidor} recebeu postagem: {postagem.Titulo}");
                            _postagemRepository.Publicar(postagem);
                            _logger.LogInformation($"Servidor {_portaServidor} sincronizou postagem: {postagem.Titulo}");
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError($"Erro de deserialização de postagem no servidor {_portaServidor}: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Erro ao processar postagem no servidor {_portaServidor}: {ex.Message}");
                    }
                });

                await _subscriber.SubscribeAsync("chats", (channel, message) =>
                {
                    try
                    {
                        var chat = JsonSerializer.Deserialize<Chat>(message);
                        if (chat != null)
                        {
                            _logger.LogInformation($"Servidor {_portaServidor} recebeu chat entre {chat.Usuario1} e {chat.Usuario2}");
                            _chatRepository.CriarChat(chat.Usuario1, chat.Usuario2);
                            _logger.LogInformation($"Servidor {_portaServidor} sincronizou chat entre {chat.Usuario1} e {chat.Usuario2}");
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError($"Erro de deserialização de chat no servidor {_portaServidor}: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Erro ao processar chat no servidor {_portaServidor}: {ex.Message}");
                    }
                });

                await _subscriber.SubscribeAsync("mensagens", (channel, message) =>
                {
                    try
                    {
                        var (chatId, mensagem) = JsonSerializer.Deserialize<(string, Mensagem)>(message);
                        var usuarios = chatId.Split('-');
                        _logger.LogInformation($"Servidor {_portaServidor} recebeu mensagem de {mensagem.Remetente.Login}");
                        
                        var chat = _chatRepository.GetByUsuarios(usuarios[0], usuarios[1]);
                        if (chat != null)
                        {
                            chat.AddMensagem(mensagem);
                            _logger.LogInformation($"Servidor {_portaServidor} sincronizou mensagem de {mensagem.Remetente.Login}");
                        }
                        else
                        {
                            _logger.LogWarning($"Servidor {_portaServidor} não encontrou chat para {usuarios[0]} e {usuarios[1]}");
                            // Criar chat se não existir
                            chat = _chatRepository.CriarChat(usuarios[0], usuarios[1]);
                            chat.AddMensagem(mensagem);
                            _logger.LogInformation($"Servidor {_portaServidor} criou novo chat e adicionou mensagem");
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError($"Erro de deserialização de mensagem no servidor {_portaServidor}: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Erro ao processar mensagem no servidor {_portaServidor}: {ex.Message}");
                    }
                });

                _logger.LogInformation($"Servidor {_portaServidor} está escutando eventos...");

                // Mantém o serviço rodando
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogCritical($"Erro de conexão com Redis no servidor {_portaServidor}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Erro fatal no EventoListenerService do servidor {_portaServidor}: {ex.Message}");
                throw;
            }
        }
    }
} 