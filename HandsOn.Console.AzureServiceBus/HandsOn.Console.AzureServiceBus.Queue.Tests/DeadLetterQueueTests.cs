namespace HandsOn.Console.AzureServiceBus.Queue.Tests;

public class DeadLetterQueueTests : BaseTest
{
    private readonly ServiceBusSender _sender;
    private ServiceBusSessionReceiver _sessionReceiver;
    
    public DeadLetterQueueTests()
    {
        _sender = Client.CreateSender(QueueName);
    }
    
    public override async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _sessionReceiver.DisposeAsync();

        await base.DisposeAsync();
    }

    [Fact]
    public async Task DeadLetterQueueReceiveDeadLetteredMessages()
    {
        var sessionId = Guid.NewGuid().ToString();
        var messageBody = "message";
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody) { SessionId = sessionId });
        
        _sessionReceiver = await Client.AcceptSessionAsync(QueueName, sessionId);
        var message = await _sessionReceiver.ReceiveMessageAsync();
        await _sessionReceiver.DeadLetterMessageAsync(message);
        
        var deadLetterReceiver = Client.CreateReceiver(QueueName, new ServiceBusReceiverOptions
        {
            SubQueue = SubQueue.DeadLetter
        });
        var deadLetterMessage = await deadLetterReceiver.ReceiveMessageAsync();
        await deadLetterReceiver.CompleteMessageAsync(deadLetterMessage);

        deadLetterMessage.Should().NotBeNull();
        deadLetterMessage.Body.ToString().Should().Be(messageBody);
    }
}