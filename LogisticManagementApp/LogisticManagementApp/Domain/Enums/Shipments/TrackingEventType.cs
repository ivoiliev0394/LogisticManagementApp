namespace LogisticManagementApp.Domain.Enums.Shipments
{
    public enum TrackingEventType
    {
        Created = 1,
        Booked = 2,
        ReadyForPickup = 3,
        PickedUp = 4,
        Departed = 5,
        Arrived = 6,
        InTransit = 7,
        OutForDelivery = 8,
        Delivered = 9,
        Exception = 10,
        CustomsHold = 11,
        DocumentReceived = 12
    }
}
