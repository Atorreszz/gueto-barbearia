using System.ComponentModel.DataAnnotations;

namespace GuetoBarbearia.Api;

public class Servico
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Não é possível adicionar um serviço sem nome.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome do serviço deve ter entre 2 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Preco { get; set; }

    [Required]
    [Range(5, 480, ErrorMessage = "A duração deve ser entre 5 e 480 minutos.")]
    public int DuracaoMinutos { get; set; }
}
