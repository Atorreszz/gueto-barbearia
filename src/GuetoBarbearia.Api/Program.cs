using GuetoBarbearia.Api;
using GuetoBarbearia.Api.models;

List<Servico> servicos = new List<Servico>();
List<Agendamento> agendamentos = new List <Agendamento>();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/status", () => "Gueto funcionando!");



Servico twist = new Servico();

twist.Id = "twist";
twist.Nome = "Twist";
twist.Preco = 120.00m;
twist.DuracaoMinutos = 140;

Servico barrel = new Servico();
barrel.Id = "barrel";
barrel.Nome = "Barrel Twist";
barrel.Preco = 150.00m;
barrel.DuracaoMinutos = 150;


servicos.Add(twist);
servicos.Add(barrel);

app.MapGet("/api/Servicos", () => servicos);

app.MapGet("/api/servicos/{id}", (string id) =>
{
    Servico? servicoEncontrado  = servicos.Find(servico => servico.Id == id);
    if(servicoEncontrado == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(servicoEncontrado);
});
app.MapPost("/api/agendamentos", (Agendamento novoAgendamento) =>
{
    novoAgendamento.Id = agendamentos.Count + 1;

    agendamentos.Add(novoAgendamento);

    return Results.Created(
        $"/api/agendamentos/{novoAgendamento.Id}",
        novoAgendamento
    );
});
app.MapGet("/api/agendamentos", () =>
{
    return Results.Ok(agendamentos);

});

app.Run();