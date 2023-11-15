namespace HandsOn.Console.AzureServiceBus.Topic.Tests;

public class SubscriptionFilterTests : BaseTest
{
    private readonly ServiceBusSender _sender;
    private ServiceBusReceiver _correlationFilterSubscriptionReceiver;
    private ServiceBusReceiver _applicationPropertyFilterSubscriptionReceiver;
    
    public SubscriptionFilterTests()
    {
        _sender = Client.CreateSender(TopicName);
    }
    
    public override async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _correlationFilterSubscriptionReceiver.DisposeAsync();
        await _applicationPropertyFilterSubscriptionReceiver.DisposeAsync();

        await base.DisposeAsync();
    }
    
    [Fact]
    public async Task CorrelationFilterCanFilterOnMessageHeaderProperties()
    {
        var timeout = TimeSpan.FromSeconds(1);
        const string paymentSubject = "Payment";
        const string customerSubject = "Customer";
        const string messageBody = "message";
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody)
        {
            Subject = paymentSubject,
        });
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody)
        {
            Subject = customerSubject,
        });
        
        _correlationFilterSubscriptionReceiver = Client.CreateReceiver(TopicName, CorrelationFilterSubscriptionName);
        
        var paymentMessage = await _correlationFilterSubscriptionReceiver.ReceiveMessageAsync(timeout);
        await _correlationFilterSubscriptionReceiver.CompleteMessageAsync(paymentMessage);
        
        paymentMessage.Should().NotBeNull();
        paymentMessage.Body.ToString().Should().Be(messageBody);
        paymentMessage.Subject.Should().Be(paymentSubject);
        
        var customerMessage = await _correlationFilterSubscriptionReceiver.ReceiveMessageAsync(timeout);
        
        customerMessage.Should().BeNull();
    }
    
    [Fact]
    public async Task CorrelationFilterCanFilterOnMessageApplicationProperties()
    {
        var timeout = TimeSpan.FromSeconds(1);
        const string paymentMessageType = "PaymentType";
        const string customerMessageType = "CustomerType";
        const string messageBody = "message";
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody)
        {
            ApplicationProperties =
            {
                { "type", paymentMessageType }
            }
        });
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody)
        {
            ApplicationProperties =
            {
                { "type", customerMessageType }
            }
        });
        
        _applicationPropertyFilterSubscriptionReceiver = Client.CreateReceiver(TopicName, ApplicationPropertyFilterSubscriptionName);
        
        var paymentMessage = await _applicationPropertyFilterSubscriptionReceiver.ReceiveMessageAsync(timeout);
        await _applicationPropertyFilterSubscriptionReceiver.CompleteMessageAsync(paymentMessage);
        
        paymentMessage.Should().NotBeNull();
        paymentMessage.Body.ToString().Should().Be(messageBody);
        paymentMessage.ApplicationProperties["type"].Should().Be(paymentMessageType);
        
        var customerMessage = await _applicationPropertyFilterSubscriptionReceiver.ReceiveMessageAsync(timeout);
        
        customerMessage.Should().BeNull();
    }
}