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

app.MapGet("/api/servicos", () => servicos);

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
    Servico? servicoEncontrado = servicos.Find(servico => servico.Id == novoAgendamento.ServicoId);
    
    if(servicoEncontrado == null)
    {
        return Results.BadRequest("Servico Não Encontrado");
    }

    DateOnly hoje = DateOnly.FromDateTime(DateTime.Today);

    if(novoAgendamento.Data < hoje)
    {
        return Results.BadRequest("Não é possível agendar para uma data passada");
    }

    bool horarioOcupado = agendamentos.Any(agendamento =>
        agendamento.Data == novoAgendamento.Data && agendamento.Horario == novoAgendamento.Horario);

    if (horarioOcupado)
    {
        return Results.Conflict("Esse horário já está ocupado");
    }

    

    
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

app.MapGet("/api/agendamentos/{id}", (int id) =>
{
    Agendamento? agendamentoEncontrado = agendamentos.Find(agendamento => agendamento.Id == id);

    if (agendamentoEncontrado == null)
    {
        return Results.NotFound("Agendamento Não Encontrado");
    }

    return Results.Ok(agendamentoEncontrado);
});

app.MapDelete("/api/agendamentos/{id}", (int id) =>
{
    Agendamento? agendamentoEncontrado = agendamentos.Find(agendamento => agendamento.Id == id);

    if (agendamentoEncontrado == null)
    {
        return Results.NotFound("Agendamento Não Encontrado");
    }

    else
    {
        agendamentos.Remove(agendamentoEncontrado);
        return Results.NoContent();
    }
});

app.MapPut("/api/agendamentos/{id}", (int id, Agendamento agendamentoAtualizado) =>
{
    Agendamento? agendamentoEncontrado = agendamentos.Find(agendamento => agendamento.Id == id);

    if (agendamentoEncontrado == null)
    {
        return Results.NotFound("Agendamento Não Encontrado");
    }

    Servico? servicoEncontrado = servicos.Find(servico => servico.Id == agendamentoAtualizado.ServicoId);

    if (servicoEncontrado == null)
    {
        return Results.BadRequest("Serviço não Encontrado");
    }
    

    DateOnly hoje = DateOnly.FromDateTime(DateTime.Today);

    if (agendamentoAtualizado.Data < hoje)
    {
        return Results.BadRequest("Não é possivel Atualizar uma Data que ja Passou");
    }
    
    bool horarioOcupado = agendamentos.Any(agendamento =>
        agendamento.Data == agendamentoAtualizado.Data 
        && agendamento.Horario == agendamentoAtualizado.Horario 
        && agendamento.Id != id);

    if (horarioOcupado)
    {
        return Results.Conflict("Esse Horario Ja Esta Ocupado");
    }
    
    agendamentoEncontrado.ServicoId = agendamentoAtualizado.ServicoId;
    agendamentoEncontrado.Data = agendamentoAtualizado.Data;
    agendamentoEncontrado.Horario = agendamentoAtualizado.Horario;

    
    
    
    return Results.Ok(agendamentoEncontrado);
});


app.Run();