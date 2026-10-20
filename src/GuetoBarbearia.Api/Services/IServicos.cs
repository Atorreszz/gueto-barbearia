namespace GuetoBarbearia.Api.Services;

public interface IServicos
{
    public Task<List<Servico>> ObterTodos();
    public Task<Servico?> ObterPorId(int id);
    public Task<Servico> Adicionar(Servico novoservico);
    public Task<Servico?> Atualizar(int id, Servico servicoAtualizado);
    public Task<bool> Remover(int id);
}