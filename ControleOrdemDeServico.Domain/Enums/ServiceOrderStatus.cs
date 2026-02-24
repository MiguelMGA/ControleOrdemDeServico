namespace OsService.Domain.Enums;

/// <summary>
/// Representa os possíveis estados de uma Ordem de Serviço.
/// </summary>
public enum ServiceOrderStatus
{
    /// <summary>
    /// Ordem de Serviço aberta, aguardando início da execução.
    /// </summary>
    Open = 0,

    /// <summary>
    /// Ordem de Serviço em execução.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Ordem de Serviço finalizada.
    /// Estado final, não permite novas transições.
    /// </summary>
    Finished = 2
}

/// <summary>
/// Métodos de extensão responsáveis por validar regras de transição
/// e comportamento do status da Ordem de Serviço.
/// </summary>
public static class ServiceOrderStatusExtensions
{
    /// <summary>
    /// Verifica se é permitido realizar a transição
    /// do status atual para o próximo status informado.
    /// </summary>
    /// <param name="current">Status atual da Ordem de Serviço.</param>
    /// <param name="next">Próximo status desejado.</param>
    /// <returns>
    /// True se a transição for válida; caso contrário, False.
    /// </returns>
    /// <remarks>
    /// Regras de transição válidas:
    /// - Open → InProgress
    /// - InProgress → Finished
    /// </remarks>
    public static bool CanTransitionTo(
        this ServiceOrderStatus current,
        ServiceOrderStatus next)
    {
        return (current, next) switch
        {
            (ServiceOrderStatus.Open, ServiceOrderStatus.InProgress) => true,
            (ServiceOrderStatus.InProgress, ServiceOrderStatus.Finished) => true,
            _ => false
        };
    }

    /// <summary>
    /// Indica se o status atual representa um estado final,
    /// no qual não são permitidas novas transições.
    /// </summary>
    /// <param name="status">Status da Ordem de Serviço.</param>
    /// <returns>
    /// True se o status for final; caso contrário, False.
    /// </returns>
    public static bool IsFinalState(this ServiceOrderStatus status)
        => status is ServiceOrderStatus.Finished;
}