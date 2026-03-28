namespace LogisticManagementApp.Domain.Enums.Billing
{
    public enum InvoiceStatus
    {
        Draft = 1,
        Issued = 2,
        PartiallyPaid = 3,
        Paid = 4,
        Cancelled = 5,
        Overdue = 6
    }
}
