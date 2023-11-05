namespace HandsOn.Console.AzureServiceBus.Queue.Tests;

public class SenderTests : BaseTest
{   
    private readonly ServiceBusSender _sender;
    private ServiceBusSessionReceiver _sessionReceiver;

    public SenderTests()
    {
        _sender = Client.CreateSender(SessionQueueName);
    }
    
    public override async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _sessionReceiver.DisposeAsync();

        await base.DisposeAsync();
    }
    
    [Fact]
    public async Task ScheduledMessageWillBeReceivedAfterScheduledTime()
    {
        var messageBody = "message";
        var sessionId = Guid.NewGuid().ToString();
        var receiveMessageTimeout = TimeSpan.FromSeconds(1);
        var scheduledEnqueueDelay = TimeSpan.FromSeconds(5);
        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId);
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody)
        {
            SessionId = sessionId,
            ScheduledEnqueueTime = DateTimeOffset.UtcNow.Add(scheduledEnqueueDelay)
        });
        
        var message = await _sessionReceiver.ReceiveMessageAsync(receiveMessageTimeout);
        message.Should().BeNull();
        
        await Task.Delay(scheduledEnqueueDelay.Add(receiveMessageTimeout));
        
        message = await _sessionReceiver.ReceiveMessageAsync(receiveMessageTimeout);
        await _sessionReceiver.CompleteMessageAsync(message);
        
        message.Body.ToString().Should().Be(messageBody);
    }
    
    [Fact]
    public async Task MessageBatchSendMultipleMessages()
    {
        var sessionId = Guid.NewGuid().ToString();
        var message1Body = "message 1";
        var message2Body = "message 2";
        var message3Body = "message 3";
        
        var messageBatch = await _sender.CreateMessageBatchAsync();
        messageBatch.TryAddMessage(new ServiceBusMessage(message1Body) { SessionId = sessionId });
        messageBatch.TryAddMessage(new ServiceBusMessage(message2Body) { SessionId = sessionId });
        messageBatch.TryAddMessage(new ServiceBusMessage(message3Body) { SessionId = sessionId });
        
        await _sender.SendMessagesAsync(messageBatch);

        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId);

        var message1 = await _sessionReceiver.ReceiveMessageAsync();
        await _sessionReceiver.CompleteMessageAsync(message1);
        var message2 = await _sessionReceiver.ReceiveMessageAsync();
        await _sessionReceiver.CompleteMessageAsync(message2);
        var message3 = await _sessionReceiver.ReceiveMessageAsync();
        await _sessionReceiver.CompleteMessageAsync(message3);
        
        message1.Body.ToString().Should().Be(message1Body);
        message2.Body.ToString().Should().Be(message2Body);
        message3.Body.ToString().Should().Be(message3Body);
    }

    [Fact]
    public async Task SenderSendMultipleMessages()
    {
        var sessionId = Guid.NewGuid().ToString();
        var message1Body = "message 1";
        var message2Body = "message 2";
        var message3Body = "message 3";
        
        await _sender.SendMessagesAsync(new []
        {
            new ServiceBusMessage(message1Body) { SessionId = sessionId },
            new ServiceBusMessage(message2Body) { SessionId = sessionId },
            new ServiceBusMessage(message3Body) { SessionId = sessionId }
        });

        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId);

        var message1 = await _sessionReceiver.ReceiveMessageAsync();
        await _sessionReceiver.CompleteMessageAsync(message1);
        var message2 = await _sessionReceiver.ReceiveMessageAsync();
        await _sessionReceiver.CompleteMessageAsync(message2);
        var message3 = await _sessionReceiver.ReceiveMessageAsync();
        await _sessionReceiver.CompleteMessageAsync(message3);
        
        message1.Body.ToString().Should().Be(message1Body);
        message2.Body.ToString().Should().Be(message2Body);
        message3.Body.ToString().Should().Be(message3Body);
    }
    
    [Fact]
    public async Task SendMessagesAsyncThrowsServiceBusExceptionWhenMessageSizeExceedsMaximum()
    {
        var sessionId = Guid.NewGuid().ToString();
        var messageBody = new string('*', 100000);
        
        Func<Task> act = async () => await _sender.SendMessagesAsync(new []
        {
            new ServiceBusMessage(messageBody) { SessionId = sessionId },
            new ServiceBusMessage(messageBody) { SessionId = sessionId },
            new ServiceBusMessage(messageBody) { SessionId = sessionId },
            new ServiceBusMessage(messageBody) { SessionId = sessionId }
        });
        
        await act.Should().ThrowAsync<ServiceBusException>();
    }

    [Fact]
    public async Task MessageBatchWillNotAddMessageWhenMessagesSizeExceedsMaximum()
    {
        var messageBody = new string('*', 100000);
        var messageBatch = await _sender.CreateMessageBatchAsync();
        
        var message1Result = messageBatch.TryAddMessage(new ServiceBusMessage(messageBody));
        var message2Result = messageBatch.TryAddMessage(new ServiceBusMessage(messageBody));
        var message3Result = messageBatch.TryAddMessage(new ServiceBusMessage(messageBody));
        
        message1Result.Should().BeTrue();
        message2Result.Should().BeTrue();
        message3Result.Should().BeFalse();
    }

    [Fact]
    public async Task MessageWillBeDeletedAfterExceedsTimeToLive()
    {
        var sessionId = Guid.NewGuid().ToString();
        var messageBody = "message";
        var timeToLive = TimeSpan.FromSeconds(2);
        var receiveMessageTimeout = TimeSpan.FromSeconds(1);
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody)
        {
            SessionId = sessionId,
            TimeToLive = timeToLive
        });

        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId);

        await Task.Delay(timeToLive.Add(receiveMessageTimeout));
        
        var message = await _sessionReceiver.ReceiveMessageAsync(receiveMessageTimeout);
        message.Should().BeNull();
    }
}