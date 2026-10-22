using GuetoBarbearia.Api.Data;
using GuetoBarbearia.Api.models;
using Microsoft.EntityFrameworkCore;

namespace GuetoBarbearia.Api.Services;

public class ServicosDataBaseService : IServicos
{
    private readonly GuetoDbContext context;

    public ServicosDataBaseService(GuetoDbContext context)
    {
        this.context = context;
    }

    public async Task<List<Servico>> ObterTodos()
    {
        return await context.Servicos
            .OrderBy(s => s.Nome)
            .ToListAsync();
    }

    public async Task<Servico?> ObterPorId(int id)
    {
        return await context.Servicos.FindAsync(id);
    }

    public async Task<Servico> Adicionar(Servico servico)
    {
        var nomeNormalizado = servico.Nome.Trim();
        var existe = await context.Servicos
            .AnyAsync(s => s.Nome.ToLower() == nomeNormalizado.ToLower());

        if (existe)
            throw new InvalidOperationException($"Já existe um serviço com o nome '{nomeNormalizado}'.");

        servico.Nome = nomeNormalizado;

        await context.Servicos.AddAsync(servico);
        await context.SaveChangesAsync();
        return servico;
    }

    public async Task<Servico?> Atualizar(int id, Servico servicoAtualizado)
    {
        var servico = await context.Servicos.FindAsync(id);
        if (servico is null)
            return null;

        var nomeNormalizado = servicoAtualizado.Nome.Trim();
        var nomeDuplicado = await context.Servicos
            .AnyAsync(s => s.Nome.ToLower() == nomeNormalizado.ToLower() && s.Id != id);

        if (nomeDuplicado)
            throw new InvalidOperationException($"Já existe outro serviço com o nome '{nomeNormalizado}'.");

        servico.Nome = nomeNormalizado;
        servico.Preco = servicoAtualizado.Preco;
        servico.DuracaoMinutos = servicoAtualizado.DuracaoMinutos;

        await context.SaveChangesAsync();
        return servico;
    }

    public async Task<bool> Remover(int id)
    {
        var servico = await context.Servicos.FindAsync(id);
        if (servico is null)
            return false;

        var hoje = DateOnly.FromDateTime(DateTime.Now);
        var temAgendamentosFuturos = await context.Agendamentos
            .AnyAsync(a => a.ServicoId == id && a.Data >= hoje && a.Status != "Cancelado");

        if (temAgendamentosFuturos)
            throw new InvalidOperationException(
                $"O serviço '{servico.Nome}' possui agendamentos futuros ativos. " +
                "Cancele os agendamentos antes de remover o serviço.");

        context.Servicos.Remove(servico);
        await context.SaveChangesAsync();
        return true;
    }
}
