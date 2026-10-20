using GuetoBarbearia.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GuetoBarbearia.Api.Services;

public class LimpezaAgendamentosBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LimpezaAgendamentosBackgroundService> _logger;

    public LimpezaAgendamentosBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<LimpezaAgendamentosBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<GuetoDbContext>();

                var hoje = DateOnly.FromDateTime(DateTime.Now);

                // Exclui todos os agendamentos anteriores à data de hoje
                var deletados = await context.Agendamentos
                    .Where(a => a.Data < hoje)
                    .ExecuteDeleteAsync(stoppingToken);

                if (deletados > 0)
                {
                    _logger.LogInformation("{Count} agendamento(s) expirado(s) foram excluídos do banco de dados.", deletados);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar excluir agendamentos expirados no banco de dados.");
            }

            // Executa a verificação a cada 1 hora
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
