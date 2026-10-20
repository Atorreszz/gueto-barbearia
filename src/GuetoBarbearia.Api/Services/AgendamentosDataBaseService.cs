using GuetoBarbearia.Api.Data;
using GuetoBarbearia.Api.models;
using Microsoft.EntityFrameworkCore;

namespace GuetoBarbearia.Api.Services;

public class AgendamentosDataBaseService : IAgendamento
{
    private readonly GuetoDbContext context;

    public AgendamentosDataBaseService(GuetoDbContext context)
    {
        this.context = context;
    }

    public async Task<List<Agendamento>> ObterTodos()
    {
        var hoje = DateOnly.FromDateTime(DateTime.Now);
        return await context.Agendamentos
            .Where(a => a.Data >= hoje)
            .ToListAsync();
    }

    public async Task<Agendamento?> ObterPorId(int id)
    {
        return await context.Agendamentos.FindAsync(id);
    }

    public async Task<Agendamento> Adicionar(Agendamento agendamento)
    {
        await context.Agendamentos.AddAsync(agendamento);
        await context.SaveChangesAsync();
        return agendamento;
    }

    public async Task<Agendamento?> Atualizar(int id, Agendamento agendamentoAtualizado)
    {
        var agendamento = await context.Agendamentos.FindAsync(id);
        if (agendamento is null)
        {
            return null;
        }

        agendamento.Data = agendamentoAtualizado.Data;
        agendamento.Horario = agendamentoAtualizado.Horario;
        agendamento.NomeCliente = agendamentoAtualizado.NomeCliente;
        agendamento.ServicoId = agendamentoAtualizado.ServicoId;
        agendamento.TelefoneCliente = agendamentoAtualizado.TelefoneCliente;

        await context.SaveChangesAsync();
        return agendamento;
    }

    public async Task<bool> Remover(int id)
    {
        var agendamento = await context.Agendamentos.FindAsync(id);
        if (agendamento is null)
        {
            return false;
        }

        context.Agendamentos.Remove(agendamento);
        await context.SaveChangesAsync();
        return true;
    }
}
