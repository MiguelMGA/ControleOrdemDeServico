namespace OsService.Domain.Enums;

public enum ServiceOrderStatus
{
    Open = 0,
    InProgress = 1,
    Finished = 2
}

public static class ServiceOrderStatusExtensions
{
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

    public static bool IsFinalState(this ServiceOrderStatus status)
        => status is ServiceOrderStatus.Finished;
}