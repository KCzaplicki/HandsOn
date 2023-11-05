namespace HandsOn.Console.AzureServiceBus.Queue.Tests;

public class MessageDeduplicationTests : BaseTest
{
    private readonly ServiceBusSender _sender;
    private readonly ServiceBusReceiver _receiver;
    
    public MessageDeduplicationTests()
    {
        _sender = Client.CreateSender(SimpleQueueName);
        _receiver = Client.CreateReceiver(SimpleQueueName);
    }
    
    public override async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _receiver.DisposeAsync();

        await base.DisposeAsync();
    }

    [Fact]
    public async Task MessageDeduplicationDetectsMessagesWithTheSameMessageId()
    {
        const int maxReceiveMessages = 10;
        var messageId = Guid.NewGuid().ToString();
        var messageBody = "message";
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody) { MessageId = messageId });
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody) { MessageId = messageId });
        
        var messages = await _receiver.ReceiveMessagesAsync(maxReceiveMessages);
        
        var messagesWithId = messages
            .Where(m => m.MessageId == messageId)
            .ToList();

        await _receiver.CompleteMessageAsync(messagesWithId[0]);
        
        messagesWithId.Should().HaveCount(1);
        messagesWithId[0].Body.ToString().Should().Be(messageBody);
    }
}