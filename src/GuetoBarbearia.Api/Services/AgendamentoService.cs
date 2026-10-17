using GuetoBarbearia.Api.models;

namespace GuetoBarbearia.Api.Services;

public class AgendamentoService : IAgendamento
{
    private static List<Agendamento> agendamentos = new List<Agendamento>()
    {
        new Agendamento()
        {
            Data = new DateOnly(2026,10,18), Horario = new TimeOnly(09, 00), Id = 1, NomeCliente = "Arthur",
            ServicoId = "1", TelefoneCliente = "24998372214"
        }
    };
    
    public List<Agendamento> ObterTodos()
    {
        return agendamentos;
    }

    public Agendamento ObterPorId(int id)
    {
        return agendamentos.FirstOrDefault(x => x.Id == id);
    }

    public Agendamento Adicionar(Agendamento agendamento)
    {
        agendamentos.Add(agendamento);
        return agendamento;
    }

    public Agendamento Atualizar(int id,Agendamento agendamentoAtualizado)
    {
        var agendamento = agendamentos.FirstOrDefault(x => x.Id == id);
        if (agendamento is null)
        {
            return null;
        }

        agendamento.TelefoneCliente = agendamentoAtualizado.TelefoneCliente;
        agendamento.ServicoId = agendamentoAtualizado.ServicoId;
        agendamento.Horario =  agendamentoAtualizado.Horario;
        agendamento.Data =  agendamentoAtualizado.Data;
        agendamento.NomeCliente = agendamentoAtualizado.NomeCliente;
        
        return  agendamento;
    }

    public bool Remover(int id)
    {
        var agendamento = agendamentos.FirstOrDefault(x => x.Id == id);
        if (agendamento is null)
        {
            return false;
        }

        agendamentos.Remove(agendamento);
        return true;
    }
}