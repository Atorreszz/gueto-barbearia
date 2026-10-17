using GuetoBarbearia.Api.models;

namespace GuetoBarbearia.Api.Services;

public interface IAgendamento 
{
    public List<Agendamento> ObterTodos();
    public Agendamento ObterPorId(int id);
    public  Agendamento Adicionar(Agendamento agendamento);
    public Agendamento Atualizar(int id, Agendamento agendamentoAtualizado);
    public bool Remover(int id);
}