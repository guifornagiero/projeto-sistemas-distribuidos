namespace SistemasDistribuidosServer.Entidades
{
    public class Usuario
    {
        public int Id { get; set; } = new Random().Next(100, 1000);
        public string Nome { get; set; }
        public string Login { get; set; }

        public Usuario(string nome, string login)
        {
            Nome = nome;
            Login = login;
        }
    }
}
