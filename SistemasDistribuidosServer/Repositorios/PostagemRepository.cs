using SistemasDistribuidosServer.Entidades;
using SistemasDistribuidosServer.Interfaces.Repositorios;
using SistemasDistribuidosServer.Interfaces.Servicos;
using Microsoft.Extensions.Configuration;

namespace SistemasDistribuidosServer.Repositorios
{
    public class PostagemRepository : IPostagemRepository
    {
        private List<Postagem> _timeline = [];
        private readonly IEventoService _eventoService;
        private readonly string _portaServidor;

        public PostagemRepository(IEventoService eventoService, IConfiguration configuration)
        {
            _eventoService = eventoService;
            _portaServidor = configuration["PortaServidor"];
        }

        public List<Postagem> GetPostagens()
        {
            return _timeline;
        }

        public Postagem Publicar(Postagem postagem)
        {
            // Verifica se a postagem já existe
            if (_timeline.Any(p => p.Id == postagem.Id))
            {
                Console.WriteLine($"Servidor {_portaServidor} ignorou postagem duplicada: {postagem.Titulo}");
                return postagem;
            }

            _timeline.Add(postagem);
            _timeline = [.. _timeline.OrderBy(p => p.DataCriacao)];

            Console.WriteLine($"Servidor {_portaServidor} adicionou postagem: {postagem.Titulo}");

            // Publica a postagem para sincronização
            _eventoService.PublicarPostagem(postagem).Wait();

            return postagem;
        }
    }
}
