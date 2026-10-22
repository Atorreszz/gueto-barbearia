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
            .Include(a => a.Servico)
            .Where(a => a.Data >= hoje && a.Status != "Cancelado")
            .OrderBy(a => a.Data)
            .ThenBy(a => a.Horario)
            .ToListAsync();
    }

    public async Task<Agendamento?> ObterPorId(int id)
    {
        return await context.Agendamentos
            .Include(a => a.Servico)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<string>> ObterHorariosDisponiveis(DateOnly data, int servicoId)
    {
        ValidarDataAgendamento(data);

        var servico = await context.Servicos.FindAsync(servicoId)
            ?? throw new InvalidOperationException($"Serviço com ID {servicoId} não encontrado.");

        var agendamentosDoDia = await context.Agendamentos
            .Include(a => a.Servico)
            .Where(a => a.Data == data && a.Status != "Cancelado")
            .ToListAsync();

        var agora = DateTime.Now;
        var hojeData = DateOnly.FromDateTime(agora);
        var horaAtual = TimeOnly.FromDateTime(agora);

        var horariosDisponiveis = new List<string>();
        var cursor = ExpedienteBarbearia.HorarioAbertura;

        while (cursor < ExpedienteBarbearia.HorarioEncerramento)
        {
            var fimDoServico = cursor.AddMinutes(servico.DuracaoMinutos);

            if (fimDoServico > ExpedienteBarbearia.HorarioEncerramento)
                break;

            if (data == hojeData)
            {
                var minutosRestantes = (cursor.ToTimeSpan() - horaAtual.ToTimeSpan()).TotalMinutes;
                if (minutosRestantes < ExpedienteBarbearia.AntecedenciaMinimaMinutos)
                {
                    cursor = cursor.AddMinutes(30);
                    continue;
                }
            }

            bool temConflito = agendamentosDoDia.Any(a =>
            {
                var duracaoExistente = a.Servico?.DuracaoMinutos ?? 60;
                var fimExistente = a.Horario.AddMinutes(duracaoExistente);
                return cursor < fimExistente && fimDoServico > a.Horario;
            });

            if (!temConflito)
                horariosDisponiveis.Add(cursor.ToString("HH:mm"));

            cursor = cursor.AddMinutes(30);
        }

        return horariosDisponiveis;
    }

    public async Task<Agendamento> Adicionar(Agendamento agendamento)
    {
        agendamento.TelefoneCliente = new string(agendamento.TelefoneCliente.Where(char.IsDigit).ToArray());

        if (agendamento.TelefoneCliente.Length != 11)
            throw new InvalidOperationException("O telefone deve conter exatamente 11 dígitos com DDD.");

        ValidarDataAgendamento(agendamento.Data);

        var servico = await context.Servicos.FindAsync(agendamento.ServicoId)
            ?? throw new InvalidOperationException($"Serviço com ID {agendamento.ServicoId} não existe. Selecione um serviço válido.");

        var fimAtendimento = agendamento.Horario.AddMinutes(servico.DuracaoMinutos);

        if (agendamento.Horario < ExpedienteBarbearia.HorarioAbertura)
            throw new InvalidOperationException($"O horário mínimo de atendimento é {ExpedienteBarbearia.HorarioAbertura:HH:mm}.");

        if (fimAtendimento > ExpedienteBarbearia.HorarioEncerramento)
            throw new InvalidOperationException(
                $"O serviço '{servico.Nome}' dura {servico.DuracaoMinutos} minutos e encerraria às {fimAtendimento:HH:mm}, " +
                $"após o horário de encerramento ({ExpedienteBarbearia.HorarioEncerramento:HH:mm}). Escolha um horário anterior.");

        var hoje = DateOnly.FromDateTime(DateTime.Now);
        if (agendamento.Data == hoje)
        {
            var agora = TimeOnly.FromDateTime(DateTime.Now);
            var minutosRestantes = (agendamento.Horario.ToTimeSpan() - agora.ToTimeSpan()).TotalMinutes;

            if (minutosRestantes < ExpedienteBarbearia.AntecedenciaMinimaMinutos)
                throw new InvalidOperationException(
                    $"É necessário agendar com pelo menos {ExpedienteBarbearia.AntecedenciaMinimaMinutos} minutos de antecedência. " +
                    $"Escolha um horário mais tarde ou outra data.");
        }

        var agendamentosDoDia = await context.Agendamentos
            .Include(a => a.Servico)
            .Where(a => a.Data == agendamento.Data && a.Status != "Cancelado")
            .ToListAsync();

        bool temConflito = agendamentosDoDia.Any(a =>
        {
            var duracaoExistente = a.Servico?.DuracaoMinutos ?? 60;
            var fimExistente = a.Horario.AddMinutes(duracaoExistente);
            var fimNovo = agendamento.Horario.AddMinutes(servico.DuracaoMinutos);
            return agendamento.Horario < fimExistente && fimNovo > a.Horario;
        });

        if (temConflito)
            throw new InvalidOperationException(
                "Esse horário já está reservado ou conflita com outro agendamento. Escolha outro horário disponível.");

        var agendamentosAtivosDoCliente = await context.Agendamentos
            .CountAsync(a =>
                a.TelefoneCliente == agendamento.TelefoneCliente &&
                a.Data >= hoje &&
                a.Status == "Agendado");

        if (agendamentosAtivosDoCliente >= ExpedienteBarbearia.MaxAgendamentosPorCliente)
            throw new InvalidOperationException(
                $"Você já possui {agendamentosAtivosDoCliente} agendamento(s) futuro(s) ativo(s). " +
                $"O limite é {ExpedienteBarbearia.MaxAgendamentosPorCliente} por cliente.");

        agendamento.Status = "Agendado";

        await context.Agendamentos.AddAsync(agendamento);
        await context.SaveChangesAsync();

        await context.Entry(agendamento).Reference(a => a.Servico).LoadAsync();

        return agendamento;
    }

    public async Task<Agendamento?> Atualizar(int id, Agendamento agendamentoAtualizado)
    {
        var agendamento = await context.Agendamentos.FindAsync(id);
        if (agendamento is null)
            return null;

        if (agendamento.Status is "Finalizado" or "Cancelado")
            throw new InvalidOperationException(
                $"Não é possível alterar um agendamento com status '{agendamento.Status}'.");

        var telefoneLimpo = new string(agendamentoAtualizado.TelefoneCliente.Where(char.IsDigit).ToArray());
        if (telefoneLimpo.Length != 11)
            throw new InvalidOperationException("O telefone deve conter exatamente 11 dígitos com DDD.");

        agendamento.Data = agendamentoAtualizado.Data;
        agendamento.Horario = agendamentoAtualizado.Horario;
        agendamento.NomeCliente = agendamentoAtualizado.NomeCliente;
        agendamento.ServicoId = agendamentoAtualizado.ServicoId;
        agendamento.TelefoneCliente = telefoneLimpo;

        if (agendamentoAtualizado.Status is not null)
            agendamento.Status = agendamentoAtualizado.Status;

        await context.SaveChangesAsync();
        return agendamento;
    }

    public async Task<bool> Remover(int id)
    {
        var agendamento = await context.Agendamentos.FindAsync(id);
        if (agendamento is null)
            return false;

        agendamento.Status = "Cancelado";
        await context.SaveChangesAsync();
        return true;
    }

    private static void ValidarDataAgendamento(DateOnly data)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Now);

        if (data < hoje)
            throw new InvalidOperationException("Não é possível agendar para uma data que já passou.");

        if (data > hoje.AddDays(ExpedienteBarbearia.MaximoDiasFuturos))
            throw new InvalidOperationException(
                $"Não é possível agendar com mais de {ExpedienteBarbearia.MaximoDiasFuturos} dias de antecedência.");

        if (!ExpedienteBarbearia.EhDiaDeTrabalho(data))
            throw new InvalidOperationException(
                $"A data {data:dd/MM/yyyy} é um feriado nacional. Por favor, escolha outra data.");
    }
}
