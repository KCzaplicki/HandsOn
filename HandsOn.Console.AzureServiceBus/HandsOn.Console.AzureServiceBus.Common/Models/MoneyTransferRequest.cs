namespace HandsOn.Console.AzureServiceBus.Common.Models;

public sealed class MoneyTransferRequest : BaseModel
{
    public string SenderId { get; set; }

    public string RecipientId { get; set; }

    public double Value { get; set; }

    public string Currency { get; set; }

    public DateTime RequestedAt { get; set; }

    public override string ToString() =>
        $"SenderId: {SenderId}, RecipientId: {RecipientId}, Value: {Value}, Currency: {Currency}, RequestedAt: {RequestedAt}";
}