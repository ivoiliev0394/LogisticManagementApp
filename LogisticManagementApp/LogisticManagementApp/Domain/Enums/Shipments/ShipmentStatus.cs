namespace LogisticManagementApp.Domain.Enums.Shipments
{
    public enum ShipmentStatus
    {
        Draft = 1,
        Created = 2,
        Booked = 3,
        ReadyForPickup = 4,
        PickedUp = 5,
        InTransit = 6,
        ArrivedAtHub = 7,
        OutForDelivery = 8,
        Delivered = 9,
        Cancelled = 10,
        Exception = 11
    }
}
