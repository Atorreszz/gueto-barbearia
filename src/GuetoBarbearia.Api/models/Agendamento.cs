using System;

namespace GuetoBarbearia.Api.models;

public class Agendamento
{
    public int Id { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string TelefoneCliente { get; set; } = string.Empty;
    public string ServicoId { get; set; } = string.Empty;
    public DateOnly Data { get; set; }
    public TimeOnly Horario { get; set; }
}
