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
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<GuetoDbContext>();

                var agora = DateTime.Now;
                var hoje = DateOnly.FromDateTime(agora);
                var horaAtual = TimeOnly.FromDateTime(agora);

                var agendamentosParaFinalizar = await context.Agendamentos
                    .Where(a =>
                        a.Status == "Agendado" &&
                        (a.Data < hoje || (a.Data == hoje && a.Horario < horaAtual)))
                    .ToListAsync(stoppingToken);

                if (agendamentosParaFinalizar.Count > 0)
                {
                    foreach (var ag in agendamentosParaFinalizar)
                        ag.Status = "Finalizado";

                    await context.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation(
                        "{Count} agendamento(s) marcado(s) como Finalizado.",
                        agendamentosParaFinalizar.Count);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao finalizar agendamentos expirados.");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
