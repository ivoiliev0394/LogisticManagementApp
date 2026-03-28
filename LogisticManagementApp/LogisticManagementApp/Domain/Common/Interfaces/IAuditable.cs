namespace LogisticManagementApp.Domain.Common.Interfaces
{
    public interface IAuditable
    {
        DateTime CreatedAtUtc { get; set; }
        DateTime? UpdatedAtUtc { get; set; }
    }
}
