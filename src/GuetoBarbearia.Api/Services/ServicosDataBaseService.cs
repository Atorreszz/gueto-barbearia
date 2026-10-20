using GuetoBarbearia.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GuetoBarbearia.Api.Services;

public class ServicosDataBaseService : IServicos
{
    private GuetoDbContext context;

    public ServicosDataBaseService(GuetoDbContext context)
    {
        this.context = context;
    }

    public async Task<List<Servico>> ObterTodos()
    {
        return await context.Servicos.ToListAsync();
    }

    public async Task<Servico?> ObterPorId(int id)
    {
        return await context.Servicos.FindAsync(id);
    }

    public async Task<Servico> Adicionar(Servico servico)
    {
        await context.Servicos.AddAsync(servico);
        await context.SaveChangesAsync();
        return servico;
    }

    public async Task<Servico?> Atualizar(int id, Servico servicoAtualizado)
    {
        var servico =  await context.Servicos.FindAsync(id);
        if (servico == null)
        {
            return  null;
        }
        servico.Nome = servicoAtualizado.Nome;
        servico.Preco = servicoAtualizado.Preco;
        servico.DuracaoMinutos = servicoAtualizado.DuracaoMinutos;
        await context.SaveChangesAsync();
        return servico;
    }

    public async Task<bool> Remover(int id)
    {
        var servico = await context.Servicos.FindAsync(id);
        if (servico is null)
        {
            return false;
        }
        context.Servicos.Remove(servico);
        await context.SaveChangesAsync();
        return true;
    }
}