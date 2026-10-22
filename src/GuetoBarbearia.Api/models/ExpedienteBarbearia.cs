namespace GuetoBarbearia.Api.models;

public static class ExpedienteBarbearia
{
    public static readonly TimeOnly HorarioAbertura = new TimeOnly(9, 0);
    public static readonly TimeOnly HorarioEncerramento = new TimeOnly(20, 0);
    public const int AntecedenciaMinimaMinutos = 60;
    public const int MaximoDiasFuturos = 60;
    public const int MaxAgendamentosPorCliente = 2;

    public static bool EhFeriadoNacional(DateOnly data)
    {
        return (data.Month, data.Day) is
            (1, 1)   or
            (4, 21)  or
            (5, 1)   or
            (9, 7)   or
            (10, 12) or
            (11, 2)  or
            (11, 15) or
            (11, 20) or
            (12, 25);
    }

    public static bool EhDiaDeTrabalho(DateOnly data)
    {
        return !EhFeriadoNacional(data);
    }
}
