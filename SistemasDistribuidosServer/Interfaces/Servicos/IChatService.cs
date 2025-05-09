using Microsoft.AspNetCore.Mvc;
using SistemasDistribuidosServer.Entidades;

namespace SistemasDistribuidosServer.Interfaces.Servicos
{
    public interface IChatService
    {
        List<Mensagem> GetChat(string usuario1, string usuario2);
        Mensagem EnviarMensagem(string enviando, string recebendo, string mensagem);
    }
}
