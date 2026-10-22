using GuetoBarbearia.Api.models;

namespace GuetoBarbearia.Api.Services;

public interface IAgendamento
{
    public Task<List<Agendamento>> ObterTodos();
    public Task<Agendamento?> ObterPorId(int id);
    public Task<Agendamento> Adicionar(Agendamento agendamento);
    public Task<Agendamento?> Atualizar(int id, Agendamento agendamentoAtualizado);
    public Task<bool> Remover(int id);
    public Task<List<string>> ObterHorariosDisponiveis(DateOnly data, int servicoId);
}
