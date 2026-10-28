using GuetoBarbearia.Api.models;
using Microsoft.EntityFrameworkCore;

namespace GuetoBarbearia.Api.Data;

public class GuetoDbContext : DbContext
{
    public GuetoDbContext(DbContextOptions<GuetoDbContext> options) : base(options)
    {
    }
    public DbSet<Servico> Servicos { get; set; }
    public DbSet<Agendamento> Agendamentos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .HasIndex(usuario => usuario.NomeUsuario)
            .IsUnique();
    }
}
