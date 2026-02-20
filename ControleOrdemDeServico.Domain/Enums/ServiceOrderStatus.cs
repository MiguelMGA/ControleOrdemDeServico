namespace OsService.Domain.Enums;

public enum ServiceOrderStatus
{
    Open = 0,
    InProgress = 1,
    Finished = 2,
    Canceled = 3
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
            (ServiceOrderStatus.Open, ServiceOrderStatus.Canceled) => true,
            (ServiceOrderStatus.InProgress, ServiceOrderStatus.Canceled) => true,
            _ => false
        };
    }

    public static bool IsFinalState(this ServiceOrderStatus status)
        => status is ServiceOrderStatus.Finished
        or ServiceOrderStatus.Canceled;
}