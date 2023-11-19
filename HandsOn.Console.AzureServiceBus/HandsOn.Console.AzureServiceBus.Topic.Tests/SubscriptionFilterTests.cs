namespace HandsOn.Console.AzureServiceBus.Topic.Tests;

public class SubscriptionFilterTests : BaseTest
{
    private readonly ServiceBusSender _sender;
    private ServiceBusReceiver _correlationFilterSubscriptionReceiver;
    private ServiceBusReceiver _sqlFilterSubscriptionReceiver;
    
    public SubscriptionFilterTests()
    {
        _sender = Client.CreateSender(TopicName);
    }
    
    public override async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _correlationFilterSubscriptionReceiver.DisposeAsync();
        await _sqlFilterSubscriptionReceiver.DisposeAsync();

        await base.DisposeAsync();
    }

    [Fact]
    public async Task OnlyOneFilterHasToMatchToSendMessageToSubscription()
    {
        var timeout = TimeSpan.FromSeconds(1);
        const string paymentSubject = "Payment";
        const string customerSubject = "Customer";
        const string paymentMessageType = "PaymentType";
        const string messageBody = "message";
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody)
        {
            Subject = paymentSubject,
        });
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody)
        {
            ApplicationProperties =
            {
                { "type", paymentMessageType }
            }
        });
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody)
        {
            Subject = customerSubject,
        });
        
        _correlationFilterSubscriptionReceiver = Client.CreateReceiver(TopicName, CorrelationFilterSubscriptionName);
        
        var paymentSubjectMessage = await _correlationFilterSubscriptionReceiver.ReceiveMessageAsync(timeout);
        await _correlationFilterSubscriptionReceiver.CompleteMessageAsync(paymentSubjectMessage);
        
        paymentSubjectMessage.Should().NotBeNull();
        paymentSubjectMessage.Body.ToString().Should().Be(messageBody);
        paymentSubjectMessage.Subject.Should().Be(paymentSubject);
        
        var paymentTypeMessage = await _correlationFilterSubscriptionReceiver.ReceiveMessageAsync(timeout);
        await _correlationFilterSubscriptionReceiver.CompleteMessageAsync(paymentTypeMessage);
        
        paymentTypeMessage.Should().NotBeNull();
        paymentTypeMessage.Body.ToString().Should().Be(messageBody);
        paymentTypeMessage.ApplicationProperties["type"].Should().Be(paymentMessageType);
        
        var customerSubjectMessage = await _correlationFilterSubscriptionReceiver.ReceiveMessageAsync(timeout);
        
        customerSubjectMessage.Should().BeNull();
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
        
        _correlationFilterSubscriptionReceiver = Client.CreateReceiver(TopicName, CorrelationFilterSubscriptionName);
        
        var paymentMessage = await _correlationFilterSubscriptionReceiver.ReceiveMessageAsync(timeout);
        await _correlationFilterSubscriptionReceiver.CompleteMessageAsync(paymentMessage);
        
        paymentMessage.Should().NotBeNull();
        paymentMessage.Body.ToString().Should().Be(messageBody);
        paymentMessage.ApplicationProperties["type"].Should().Be(paymentMessageType);
        
        var customerMessage = await _correlationFilterSubscriptionReceiver.ReceiveMessageAsync(timeout);
        
        customerMessage.Should().BeNull();
    }

    [Fact]
    public async Task SqlFilterCanFilterOnMessageHeaderProperties()
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
        
        _sqlFilterSubscriptionReceiver = Client.CreateReceiver(TopicName, SqlFilterSubscriptionName);
        
        var paymentMessage = await _sqlFilterSubscriptionReceiver.ReceiveMessageAsync(timeout);
        await _sqlFilterSubscriptionReceiver.CompleteMessageAsync(paymentMessage);
        
        paymentMessage.Should().NotBeNull();
        paymentMessage.Body.ToString().Should().Be(messageBody);
        paymentMessage.Subject.Should().Be(paymentSubject);
        
        var customerMessage = await _sqlFilterSubscriptionReceiver.ReceiveMessageAsync(timeout);
        
        customerMessage.Should().BeNull();
    }
    
    [Fact]
    public async Task SqlFilterCanFilterOnMessageApplicationProperties()
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
        
        _sqlFilterSubscriptionReceiver = Client.CreateReceiver(TopicName, CorrelationFilterSubscriptionName);
        
        var paymentMessage = await _sqlFilterSubscriptionReceiver.ReceiveMessageAsync(timeout);
        await _sqlFilterSubscriptionReceiver.CompleteMessageAsync(paymentMessage);
        
        paymentMessage.Should().NotBeNull();
        paymentMessage.Body.ToString().Should().Be(messageBody);
        paymentMessage.ApplicationProperties["type"].Should().Be(paymentMessageType);
        
        var customerMessage = await _sqlFilterSubscriptionReceiver.ReceiveMessageAsync(timeout);
        
        customerMessage.Should().BeNull();
    }
}