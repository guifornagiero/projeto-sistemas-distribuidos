using Microsoft.Extensions.Hosting;
using SistemasDistribuidosServer.Interfaces.Servicos;

namespace SistemasDistribuidosServer.Servicos
{
    public class EleicoesStartupService : IHostedService
    {
        private readonly IEleicoesService _eleicoesService;
        private readonly ILogger<EleicoesStartupService> _logger;

        public EleicoesStartupService(IEleicoesService eleicoesService, ILogger<EleicoesStartupService> logger)
        {
            _eleicoesService = eleicoesService;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando serviço de eleições...");
            
            try
            {
                // Inicializa o serviço de eleição
                await _eleicoesService.IniciarServicoEleicao();
                
                // Verifica se o servidor principal está ativo
                bool servidorPrincipalAtivo = await _eleicoesService.VerificarServidorPrincipal();
                
                if (!servidorPrincipalAtivo)
                {
                    _logger.LogWarning("Servidor principal não está ativo. Iniciando eleição...");
                    await _eleicoesService.IniciarEleicao();
                }
                else
                {
                    _logger.LogInformation($"Servidor principal ativo: {_eleicoesService.GetPortaPrincipal()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao iniciar serviço de eleições: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Parando serviço de eleições...");
            return Task.CompletedTask;
        }
    }
}