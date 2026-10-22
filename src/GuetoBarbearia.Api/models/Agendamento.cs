using System.ComponentModel.DataAnnotations;

namespace GuetoBarbearia.Api.models;

public class Agendamento
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Não é possível adicionar um agendamento sem o nome do cliente.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome do cliente deve ter entre 3 e 100 caracteres.")]
    public string NomeCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "Não é possível adicionar um agendamento sem o telefone do cliente.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "Informe um celular com DDD e 11 dígitos apenas (ex: 24999405127).")]
    public string TelefoneCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "O serviço é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um serviço válido.")]
    public int ServicoId { get; set; }

    public Servico? Servico { get; set; }

    [Required(ErrorMessage = "A data do agendamento é obrigatória.")]
    public DateOnly Data { get; set; }

    [Required(ErrorMessage = "O horário do agendamento é obrigatório.")]
    public TimeOnly Horario { get; set; }

    public string Status { get; set; } = "Agendado";
}
