using SistemasDistribuidosServer.Entidades;

namespace SistemasDistribuidosServer.Interfaces.Servicos
{
    public interface INotificadorService
    {
        void NotificarSeguidores(Usuario criador, Postagem postagem);
    }
}
