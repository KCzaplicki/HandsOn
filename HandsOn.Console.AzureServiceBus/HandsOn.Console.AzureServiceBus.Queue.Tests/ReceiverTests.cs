namespace HandsOn.Console.AzureServiceBus.Queue.Tests;

public class ReceiverTests : BaseTest
{
    private readonly ServiceBusSender _sender;
    private ServiceBusSessionReceiver _sessionReceiver;
    
    public ReceiverTests()
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
    public async Task ReceiverReceiveMultipleMessages()
    {
        const int maxMessages = 10;
        var receiveMessagesTimeout = TimeSpan.FromSeconds(2);
        
        var sessionId = Guid.NewGuid().ToString();
        var message1Body = "message 1";
        var message2Body = "message 2";
        var message3Body = "message 3";
        
        await _sender.SendMessageAsync(new ServiceBusMessage(message1Body) { SessionId = sessionId });
        await _sender.SendMessageAsync(new ServiceBusMessage(message2Body) { SessionId = sessionId });
        await _sender.SendMessageAsync(new ServiceBusMessage(message3Body) { SessionId = sessionId });
        
        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId);
        var messages = await _sessionReceiver.ReceiveMessagesAsync(maxMessages, receiveMessagesTimeout);

        foreach (var message in messages)
        {
            await _sessionReceiver.CompleteMessageAsync(message);
        }
        
        messages.Count.Should().Be(3);
        messages[0].Body.ToString().Should().Be(message1Body);
        messages[1].Body.ToString().Should().Be(message2Body);
        messages[2].Body.ToString().Should().Be(message3Body);
    }
    
    [Fact]
    public async Task ReceiveAndDeleteModeDeletesMessageAutomatically()
    {
        var sessionId = Guid.NewGuid().ToString();
        var messageBody = "message";
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody) { SessionId = sessionId });
        
        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId, new ServiceBusSessionReceiverOptions
        {
            ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete,
        });
        var message = await _sessionReceiver.ReceiveMessageAsync();

        message.Should().NotBeNull();
        message.Body.ToString().Should().Be(messageBody);
    }
    
    [Fact]
    public async Task PeekLockModeRequireExplicitCompleteMessage()
    {
        var sessionId = Guid.NewGuid().ToString();
        var messageBody = "message";
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody) { SessionId = sessionId });
        
        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId, new ServiceBusSessionReceiverOptions
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock,
        });
        var message = await _sessionReceiver.ReceiveMessageAsync();
        await _sessionReceiver.CompleteMessageAsync(message);

        message.Should().NotBeNull();
        message.Body.ToString().Should().Be(messageBody);
    }

    [Fact]
    public async Task PeekMessageDoesNotLockMessage()
    {
        var sessionId = Guid.NewGuid().ToString();
        var messageBody = "message";
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody) { SessionId = sessionId });
        
        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId);
        var message = await _sessionReceiver.PeekMessageAsync();
        
        message.Should().NotBeNull();
        message.Body.ToString().Should().Be(messageBody);
        message.LockToken.Should().Be(Guid.Empty.ToString());
        message.LockedUntil.Should().Be(default);
    }
    
    [Fact]
    public async Task PeekMessageCantCompleteMessage()
    {
        var sessionId = Guid.NewGuid().ToString();
        var messageBody = "message";
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody) { SessionId = sessionId });
        
        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId);
        var message = await _sessionReceiver.PeekMessageAsync();
        
        Func<Task> act = async () => await _sessionReceiver.CompleteMessageAsync(message);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
    
    [Fact]
    public async Task ReceiveMessageWillLockMessage()
    {
        var sessionId = Guid.NewGuid().ToString();
        var messageBody = "message";
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody) { SessionId = sessionId });
        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId);
        
        var message = await _sessionReceiver.ReceiveMessageAsync();
        await _sessionReceiver.CompleteMessageAsync(message);
        
        message.Should().NotBeNull();
        message.Body.ToString().Should().Be(messageBody);
        message.LockToken.Should().NotBe(Guid.Empty.ToString());
        message.LockedUntil.Should().NotBe(default);
    }

    [Fact]
    public async Task ReceivedMessageCanBeAbandoned()
    {
        var sessionId = Guid.NewGuid().ToString();
        var messageBody = "message";
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody) { SessionId = sessionId });
        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId);
        
        var message = await _sessionReceiver.ReceiveMessageAsync();
        await _sessionReceiver.AbandonMessageAsync(message);
        
        message.Should().NotBeNull();
        message.Body.ToString().Should().Be(messageBody);
    }
    
    [Fact]
    public async Task ReceivedMessageCanBeDeferred()
    {
        var sessionId = Guid.NewGuid().ToString();
        var messageBody = "message";
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody) { SessionId = sessionId });
        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId);
        
        var message = await _sessionReceiver.ReceiveMessageAsync();
        await _sessionReceiver.DeferMessageAsync(message);
        
        message.Should().NotBeNull();
        message.Body.ToString().Should().Be(messageBody);
        message.SequenceNumber.Should().NotBe(default);
    }
    
    [Fact]
    public async Task ReceivedMessageCanBeDeadLettered()
    {
        var sessionId = Guid.NewGuid().ToString();
        var messageBody = "message";
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody) { SessionId = sessionId });
        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId);
        
        var message = await _sessionReceiver.ReceiveMessageAsync();
        await _sessionReceiver.DeadLetterMessageAsync(message);
        
        message.Should().NotBeNull();
        message.Body.ToString().Should().Be(messageBody);
    }
    
    [Fact]
    public async Task DeferredMessageCanBeReceivedAndCompleted()
    {
        var sessionId = Guid.NewGuid().ToString();
        var messageBody = "message";
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody) { SessionId = sessionId });
        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId);
        
        var message = await _sessionReceiver.ReceiveMessageAsync();
        await _sessionReceiver.DeferMessageAsync(message);
        
        message.Should().NotBeNull();
        message.Body.ToString().Should().Be(messageBody);
        message.SequenceNumber.Should().NotBe(default);
        
        var deferredMessage = await _sessionReceiver.ReceiveDeferredMessageAsync(message.SequenceNumber);
        await _sessionReceiver.CompleteMessageAsync(deferredMessage);
        
        deferredMessage.Should().NotBeNull();
        deferredMessage.Body.ToString().Should().Be(messageBody);
        deferredMessage.SequenceNumber.Should().NotBe(default);
    }

    [Fact]
    public async Task AbandonedMessageWillBeReceivedDeliveryMaxCountTimes()
    {
        const int deliveryMaxCount = 5;
        var receiveMessageTimeout = TimeSpan.FromSeconds(1);
        
        var sessionId = Guid.NewGuid().ToString();
        var messageBody = "message";
        var messageId = Guid.NewGuid().ToString();
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody)
        {
            SessionId = sessionId,
            MessageId = messageId
        });
        
        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId);
        
        for (var i = 0; i < deliveryMaxCount; i++)
        {
            var message = await _sessionReceiver.ReceiveMessageAsync(receiveMessageTimeout);
            message.Should().NotBeNull();
            message.Body.ToString().Should().Be(messageBody);
            message.MessageId.Should().Be(messageId);

            await _sessionReceiver.AbandonMessageAsync(message);
        }
        
        var timeoutResult = await _sessionReceiver.ReceiveMessageAsync(receiveMessageTimeout);
        timeoutResult.Should().BeNull();
    }
}