namespace GuetoBarbearia.Api.Services;

public interface IServicos
{
    public List<Servico> ObterTodos();
    public Servico ObterPorId(int id);
    public Servico Adicionar(Servico servico);
    public Servico Atualizar(int id, Servico servicoAtualizado);
    public bool Remover(int id);

}