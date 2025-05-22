using SistemasDistribuidosServer.Entidades;

namespace SistemasDistribuidosServer.Interfaces.Servicos
{
    public interface IEleicoesService
    {
        Task IniciarServicoEleicao();
        Task<bool> IniciarEleicao();
        Task AnunciarVitoria();
        Task<bool> VerificarServidorPrincipal();
        Task RegistrarVoto(string portaServidor);
        int GetPortaPrincipal();
        bool EhServidorPrincipal();
        Task NotificarNovoLider(string portaServidor);
    }
}