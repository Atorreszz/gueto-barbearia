using Microsoft.AspNetCore.Mvc;


namespace GuetoBarbearia.Api.Services;

public class ServicosService : IServicos
{
    private static List<Servico> servicos = new List<Servico>()
    {
        new Servico() {Id = 1, Nome = "Twist", DuracaoMinutos = 140, Preco = 120m} 
    };

    public List<Servico> ObterTodos()
    {
        return servicos;
    }

    public Servico ObterPorId(int id)
    {
        return servicos.FirstOrDefault(x => x.Id == id);
    }

    public Servico Adicionar(Servico servico)
    {
        servicos.Add(servico);
        return servico;
    }

    public Servico Atualizar(int id,Servico servicoAtualizado)
    {
        var servico = servicos.FirstOrDefault(x => x.Id == id);
        if (servico is null)
        {
            return null;
        }

        servico.Preco =  servicoAtualizado.Preco;
        servico.DuracaoMinutos =  servicoAtualizado.DuracaoMinutos;
        servico.Nome = servicoAtualizado.Nome;
        
        return  servico;
    }

    public bool Remover(int id)
    {
        var servico = servicos.FirstOrDefault(x => x.Id == id);
        if (servico is null)
        {
            return false;
        }

        servicos.Remove(servico);
        return true;
    }
}