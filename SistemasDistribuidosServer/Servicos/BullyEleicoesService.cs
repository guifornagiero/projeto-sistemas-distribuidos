using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using SistemasDistribuidosServer.Interfaces.Servicos;

namespace SistemasDistribuidosServer.Servicos
{
    public class BullyEleicoesService : IEleicoesService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ISubscriber _subscriber;
        private readonly string _portaServidor;
        private readonly List<string> _servidoresConhecidos;
        private int _servidorPrincipal;
        private bool _eleicaoEmAndamento = false;
        private Dictionary<string, bool> _votosRecebidos = new();
        private readonly IConfiguration _configuration;
        private HashSet<string> _servidoresAtivos = new();
        private DateTime _ultimaVerificacaoServidores = DateTime.MinValue;
        
        private const string CANAL_ELEICAO = "eleicao";
        private const string CANAL_VITORIA = "vitoria";
        private const string CANAL_HEARTBEAT = "heartbeat";
        private const string CANAL_VOTO = "voto";
        private const string CANAL_SERVIDOR_ATIVO = "servidor_ativo";
        private readonly ILogger<BullyEleicoesService> _logger;

        public BullyEleicoesService(IConfiguration configuration, ILogger<BullyEleicoesService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            
            try
            {
                string redisConnectionString = configuration.GetConnectionString("Redis");
                if (string.IsNullOrEmpty(redisConnectionString))
                {
                    redisConnectionString = "localhost:6379,abortConnect=false";
                    _logger.LogWarning("String de conexão Redis não encontrada na configuração. Usando padrão: " + redisConnectionString);
                }
                else
                {
                    _logger.LogInformation("Conectando ao Redis usando: " + redisConnectionString);
                }
                
                _redis = ConnectionMultiplexer.Connect(redisConnectionString);
                _subscriber = _redis.GetSubscriber();
                _portaServidor = configuration["PortaServidor"] ?? "5001";
                _logger.LogInformation($"Conexão Redis estabelecida com sucesso para o servidor na porta {_portaServidor}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao conectar ao Redis: {ex.Message}");
                throw;
            }
            
            // Portas dos servidores conhecidos - poderia vir da configuração em um sistema real
            _servidoresConhecidos = new List<string> { "5001", "5002", "5003" };
            
            // Inicialmente, o servidor com menor porta é o principal
            _servidorPrincipal = _servidoresConhecidos.Select(int.Parse).Min();
            
            _logger.LogInformation($"BullyEleicoesService inicializado na porta {_portaServidor}. Servidor principal inicial: {_servidorPrincipal}");
        }

        public async Task IniciarServicoEleicao()
        {
            _logger.LogInformation($"Iniciando serviço de eleição para o servidor na porta {_portaServidor}...");
            
            try {
                // Registra este servidor como ativo
                await _subscriber.PublishAsync(CANAL_SERVIDOR_ATIVO, _portaServidor);
                _servidoresAtivos.Add(_portaServidor);
                _logger.LogInformation($"Servidor {_portaServidor} registrado como ativo");
            
            // Inscreve nos canais de eleição
            await _subscriber.SubscribeAsync(CANAL_ELEICAO, async (channel, message) => 
            {
                string portaIniciadora = message.ToString();
                _logger.LogInformation($"Servidor {_portaServidor} recebeu mensagem de eleição de {portaIniciadora}");
                
                int portaRecebida = int.Parse(portaIniciadora);
                int minhaPorta = int.Parse(_portaServidor);
                
                if (portaRecebida < minhaPorta)
                {
                    // Se a porta que está realizando a eleição é menor que a minha, devo enviar meu voto
                    await _subscriber.PublishAsync(CANAL_VOTO, _portaServidor);
                    _logger.LogInformation($"Servidor {_portaServidor} enviou voto para {portaIniciadora}");
                }
                else if (portaRecebida > minhaPorta)
                {
                    // Se a porta que está realizando a eleição é maior que a minha, inicio minha própria eleição
                    await IniciarEleicao();
                    _logger.LogInformation($"Servidor {_portaServidor} iniciou eleição própria em resposta a {portaIniciadora}");
                }
            });

            await _subscriber.SubscribeAsync(CANAL_VITORIA, (channel, message) => 
            {
                string novoLider = message.ToString();
                _logger.LogInformation($"Servidor {_portaServidor} recebeu anúncio de vitória de {novoLider}");
                _servidorPrincipal = int.Parse(novoLider);
                _eleicaoEmAndamento = false;
                _logger.LogInformation($"Novo servidor principal definido: {_servidorPrincipal}");
            });

            await _subscriber.SubscribeAsync(CANAL_VOTO, (channel, message) => 
            {
                string voto = message.ToString();
                _logger.LogInformation($"Servidor {_portaServidor} recebeu voto de {voto}");
                
                if (_eleicaoEmAndamento)
                {
                    _votosRecebidos[voto] = true;
                    _logger.LogInformation($"Voto de {voto} registrado. Total de votos: {_votosRecebidos.Count}");
                    
                    // Verifica se recebeu votos de todos os servidores com ID maior
                    bool todosVotaram = true;
                    foreach (var servidor in _servidoresConhecidos)
                    {
                        if (int.Parse(servidor) > int.Parse(_portaServidor) && !_votosRecebidos.ContainsKey(servidor))
                        {
                            todosVotaram = false;
                            break;
                        }
                    }
                    
                    if (todosVotaram)
                    {
                        // Se recebeu votos de todos os servidores com ID maior, declara-se vencedor
                        _subscriber.PublishAsync(CANAL_VITORIA, _portaServidor);
                        _servidorPrincipal = int.Parse(_portaServidor);
                        _eleicaoEmAndamento = false;
                        _logger.LogInformation($"Servidor {_portaServidor} se declara vencedor da eleição");
                    }
                }
            });

            // Heartbeat para verificar se o servidor principal está vivo
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (!EhServidorPrincipal())
                        {
                            bool servidorPrincipalVivo = await VerificarServidorPrincipal();
                            if (!servidorPrincipalVivo && !_eleicaoEmAndamento)
                            {
                                _logger.LogWarning($"Servidor principal {_servidorPrincipal} não respondeu. Iniciando eleição...");
                                await IniciarEleicao();
                            }
                        }
                        else
                        {
                            // Se este servidor é o principal, envia heartbeat
                            await _subscriber.PublishAsync(CANAL_HEARTBEAT, _portaServidor);
                            _logger.LogInformation($"Servidor principal {_portaServidor} enviou heartbeat");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Erro no heartbeat: {ex.Message}");
                    }
                    
                    await Task.Delay(5000); // Verifica a cada 5 segundos
                }
            });

            // Responde a heartbeats
            await _subscriber.SubscribeAsync(CANAL_HEARTBEAT, (channel, message) => 
            {
                string porta = message.ToString();
                if (porta == _servidorPrincipal.ToString())
                {
                    _logger.LogInformation($"Heartbeat recebido do servidor principal: {porta}");
                }
            });
            
            // Inscreve no canal de servidores ativos
            await _subscriber.SubscribeAsync(CANAL_SERVIDOR_ATIVO, (channel, message) => 
            {
                string porta = message.ToString();
                _servidoresAtivos.Add(porta);
                _logger.LogInformation($"Servidor {porta} registrado como ativo. Total de servidores ativos: {_servidoresAtivos.Count}");
            });

            // Anuncia regularmente sua presença e verifica servidores inativos
            _ = Task.Run(async () => 
            {
                while (true)
                {
                    await _subscriber.PublishAsync(CANAL_SERVIDOR_ATIVO, _portaServidor);
                    await VerificarServidoresAtivos();
                    await Task.Delay(10000); // Anuncia a cada 10 segundos
                }
            });

            _logger.LogInformation($"Servidor {_portaServidor} está escutando eventos de eleição");
            }
            catch (Exception ex) {
                _logger.LogError($"Erro ao iniciar serviço de eleição: {ex.Message}");
                throw;
            }
        }
        
        private async Task VerificarServidoresAtivos()
        {
            if (DateTime.Now - _ultimaVerificacaoServidores < TimeSpan.FromSeconds(30))
            {
                return; // Evita verificações muito frequentes
            }
            
            _ultimaVerificacaoServidores = DateTime.Now;
            HashSet<string> servidoresConfirmados = new();
            
            foreach (var servidor in _servidoresAtivos)
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.Timeout = TimeSpan.FromSeconds(2);
                        var response = await httpClient.GetAsync($"http://localhost:{servidor}/health");
                        if (response.IsSuccessStatusCode)
                        {
                            servidoresConfirmados.Add(servidor);
                        }
                        else
                        {
                            _logger.LogWarning($"Servidor {servidor} não respondeu ao health check");
                        }
                    }
                }
                catch
                {
                    _logger.LogWarning($"Falha ao verificar servidor {servidor}");
                }
            }
            
            // Atualiza a lista de servidores ativos
            _servidoresAtivos = servidoresConfirmados;
            _logger.LogInformation($"Servidores ativos: {string.Join(", ", _servidoresAtivos)}");
            
            // Se o servidor principal não estiver ativo, inicia eleição
            if (!_servidoresAtivos.Contains(_servidorPrincipal.ToString()) && !_eleicaoEmAndamento)
            {
                _logger.LogWarning($"Servidor principal {_servidorPrincipal} não está ativo. Iniciando eleição...");
                await IniciarEleicao();
            }
        }

        public async Task<bool> IniciarEleicao()
        {
            if (_eleicaoEmAndamento)
            {
                _logger.LogInformation($"Eleição já em andamento no servidor {_portaServidor}");
                return false;
            }

            _eleicaoEmAndamento = true;
            _votosRecebidos.Clear();
            
            _logger.LogInformation($"Servidor {_portaServidor} iniciando eleição");
            _logger.LogInformation($"Servidores ativos no momento da eleição: {string.Join(", ", _servidoresAtivos)}");
            
            // Publica mensagem de eleição
            await _subscriber.PublishAsync(CANAL_ELEICAO, _portaServidor);
            
            // Verifica se existem servidores ativos com IDs maiores
            bool existeMaior = _servidoresAtivos.Any(s => int.Parse(s) > int.Parse(_portaServidor));
            
            if (!existeMaior)
            {
                _logger.LogInformation($"Não existem servidores ativos com ID maior que {_portaServidor}. Declarando vitória.");
                // Se não existem servidores com IDs maiores, este se declara vencedor
                await AnunciarVitoria();
                return true;
            }
            
            _logger.LogInformation($"Aguardando respostas de servidores com ID maior que {_portaServidor}...");
            // Espera por um tempo para receber respostas
            await Task.Delay(2000);
            
            // Se não receber resposta de nenhum servidor maior, declara-se vencedor
            bool recebeuResposta = _votosRecebidos.Any(v => int.Parse(v.Key) > int.Parse(_portaServidor));
            
            if (!recebeuResposta)
            {
                _logger.LogInformation($"Não recebi resposta de nenhum servidor com ID maior. Declarando vitória.");
                await AnunciarVitoria();
                return true;
            }
            else
            {
                _logger.LogInformation($"Recebi respostas de servidores com ID maior: {string.Join(", ", _votosRecebidos.Keys.Where(k => int.Parse(k) > int.Parse(_portaServidor)))}");
                _eleicaoEmAndamento = false;
            }
            
            return false;
        }

        public async Task AnunciarVitoria()
        {
            try {
                _logger.LogInformation($"Servidor {_portaServidor} anunciando vitória na eleição!!!");
                _servidorPrincipal = int.Parse(_portaServidor);
                _eleicaoEmAndamento = false;
                await _subscriber.PublishAsync(CANAL_VITORIA, _portaServidor);
                
                // Notifica os outros servidores que este é o novo líder
                _logger.LogInformation($"Nova hierarquia: Servidor {_portaServidor} é o principal. Servidores secundários: {string.Join(", ", _servidoresAtivos.Where(s => s != _portaServidor))}");
            }
            catch (Exception ex) {
                _logger.LogError($"Erro ao anunciar vitória: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> VerificarServidorPrincipal()
        {
            try
            {
                if (_servidorPrincipal == int.Parse(_portaServidor))
                {
                    return true; // Este servidor é o principal
                }

                // Verifica se o servidor principal está vivo usando uma requisição HTTP
                string url = $"http://localhost:{_servidorPrincipal}/health";
                
                _logger.LogInformation($"Verificando saúde do servidor principal na porta {_servidorPrincipal}...");
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(2);
                    var response = await httpClient.GetAsync(url);
                    bool sucesso = response.IsSuccessStatusCode;
                    _logger.LogInformation($"Servidor principal {_servidorPrincipal} {(sucesso ? "está ativo" : "não respondeu corretamente")}");
                    return sucesso;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning($"Servidor principal {_servidorPrincipal} não respondeu à verificação HTTP: {ex.Message}");
                return false;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning($"Timeout ao verificar servidor principal {_servidorPrincipal}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Erro ao verificar servidor principal {_servidorPrincipal}: {ex.Message}");
                return false;
            }
        }

        public async Task RegistrarVoto(string portaServidor)
        {
            if (_eleicaoEmAndamento)
            {
                _votosRecebidos[portaServidor] = true;
                _logger.LogInformation($"Voto registrado do servidor {portaServidor}");
            }
        }

        public int GetPortaPrincipal()
        {
            return _servidorPrincipal;
        }

        public bool EhServidorPrincipal()
        {
            return int.Parse(_portaServidor) == _servidorPrincipal;
        }

        public async Task NotificarNovoLider(string portaServidor)
        {
            _logger.LogInformation($"Notificando novo líder: {portaServidor}");
            await _subscriber.PublishAsync(CANAL_VITORIA, portaServidor);
        }
    }
}