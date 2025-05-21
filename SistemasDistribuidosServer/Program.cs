using SistemasDistribuidosServer.Interfaces.Repositorios;
using SistemasDistribuidosServer.Interfaces.Servicos;
using SistemasDistribuidosServer.Repositorios;
using SistemasDistribuidosServer.Servicos;

var builder = WebApplication.CreateBuilder(args);

// Pega a porta da URL
var urls = builder.Configuration["urls"] ?? "http://localhost:5001";
var porta = urls.Split(":").Last();
builder.Configuration["PortaServidor"] = porta;

Console.WriteLine($"Servidor iniciando na porta: {porta}");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy
            .WithOrigins("*") // ou "*" para todos, com cuidado!
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

#region Injecao de dependencia

// Servicos
builder.Services.AddSingleton<IUsuarioService, UsuarioService>();
builder.Services.AddSingleton<IPostagemService, PostagemService>();
builder.Services.AddSingleton<INotificadorService, NotificadorService>();
builder.Services.AddSingleton<IChatService, ChatService>();
builder.Services.AddSingleton<IEventoService, RedisEventoService>();
builder.Services.AddHostedService<EventoListenerService>();

// Repositorios
builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddSingleton<IPostagemRepository, PostagemRepository>();
builder.Services.AddSingleton<IChatRepository, ChatRepository>();

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PermitirFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
