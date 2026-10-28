using System.ComponentModel.DataAnnotations;

namespace GuetoBarbearia.Api.models;

public class Login
{
    [Required]
    public string Usuario {get; set;}
    [Required]
    public string Senha {get; set;}
}