namespace LogisticManagementApp.Domain.Enums.Orders
{
    public enum OrderStatus
    {
        Draft = 1,
        Submitted = 2,
        Confirmed = 3,
        ScheduledForPickup = 4,
        InProgress = 5,
        Completed = 6,
        Cancelled = 7
    }
}
