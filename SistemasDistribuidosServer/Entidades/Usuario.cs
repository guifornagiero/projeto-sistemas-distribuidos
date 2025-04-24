namespace SistemasDistribuidosServer.Entidades
{
    public class Usuario
    {
        public int Id { get; set; } = new Random().Next(100, 1000);
        public string Nome { get; set; }

        public Usuario(string nome)
        {
            Nome = nome;
        }
    }
}
