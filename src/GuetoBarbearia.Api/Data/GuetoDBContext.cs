using GuetoBarbearia.Api.models;
using Microsoft.EntityFrameworkCore;
using GuetoBarbearia.Api;
namespace GuetoBarbearia.Api.Data;

public class GuetoDbContext : DbContext
{
    
    public GuetoDbContext(DbContextOptions<GuetoDbContext> options) : base(options)
    {
    }
    public DbSet<Servico> Servicos { get; set; }
    public DbSet<Agendamento> Agendamentos { get; set; }
    
}