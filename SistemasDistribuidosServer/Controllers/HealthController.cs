using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SistemasDistribuidosServer.Interfaces.Servicos;
using System.Net.Http;

namespace SistemasDistribuidosServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IEleicoesService _eleicoesService;
        private readonly string _portaServidor;
        private readonly ILogger<HealthController> _logger;

        public HealthController(IEleicoesService eleicoesService, IConfiguration configuration, ILogger<HealthController> logger)
        {
            _eleicoesService = eleicoesService;
            _portaServidor = configuration["PortaServidor"];
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Check()
        {
            bool ehPrincipal = _eleicoesService.EhServidorPrincipal();
            return Ok(new
            {
                Status = "Healthy",
                Porta = _portaServidor,
                ServidorPrincipal = _eleicoesService.GetPortaPrincipal(),
                EhPrincipal = ehPrincipal
            });
        }

        [HttpGet("cluster")]
        public async Task<ActionResult> GetClusterStatus()
        {
            var servidores = new List<string> { "5001", "5002", "5003" };
            var status = new Dictionary<string, object>();
            
            foreach (var porta in servidores)
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.Timeout = TimeSpan.FromSeconds(2);
                        var response = await httpClient.GetAsync($"http://localhost:{porta}/health");
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            status[porta] = new
                            {
                                Status = "Online",
                                Content = content
                            };
                        }
                        else
                        {
                            status[porta] = new { Status = "Error", Code = (int)response.StatusCode };
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Erro ao verificar servidor {porta}: {ex.Message}");
                    status[porta] = new { Status = "Offline", Error = ex.Message };
                }
            }

            return Ok(new
            {
                Timestamp = DateTime.Now,
                ThisServer = _portaServidor,
                CurrentLeader = _eleicoesService.GetPortaPrincipal(),
                Servers = status
            });
        }

        [HttpPost("Eleicao")]
        public async Task<ActionResult> IniciarEleicao()
        {
            bool resultado = await _eleicoesService.IniciarEleicao();
            return Ok(new
            {
                Sucesso = resultado,
                NovoLider = resultado ? _portaServidor : _eleicoesService.GetPortaPrincipal().ToString()
            });
        }
    }
}