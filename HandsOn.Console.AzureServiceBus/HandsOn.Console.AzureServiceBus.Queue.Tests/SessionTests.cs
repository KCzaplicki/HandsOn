namespace HandsOn.Console.AzureServiceBus.Queue.Tests;

public class SessionTests : BaseTest
{
    private readonly ServiceBusSender _sender;
    private ServiceBusSessionReceiver _sessionReceiver;

    public SessionTests()
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
    public async Task SessionReceiverReceiveMessagesInOrder()
    {
        var message1Body = "message 1";
        var message2Body = "message 2";
        var message3Body = "message 3";
        var sessionId = Guid.NewGuid().ToString();
        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId);

        await SendMessageAsync(message1Body, sessionId);
        await SendMessageAsync(message2Body, sessionId);
        await SendMessageAsync(message3Body, sessionId);

        var message1 = await ReceiveMessageAsync();
        var message2 = await ReceiveMessageAsync();
        var message3 = await ReceiveMessageAsync();

        message1.Body.ToString().Should().Be(message1Body);
        message2.Body.ToString().Should().Be(message2Body);
        message3.Body.ToString().Should().Be(message3Body);
    }
    
    [Fact]
    public async Task SessionReceiverReceiveOnlySessionMessages()
    {
        var message1Body = "message 1";
        var message2Body = "message 2";
        var sessionId = Guid.NewGuid().ToString();
        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId);
        
        await SendMessageAsync(message1Body, sessionId);
        await SendMessageAsync(message2Body, Guid.NewGuid().ToString());

        var message1 = await ReceiveMessageAsync();
        var message2 = await ReceiveMessageAsync();

        message1.Body.ToString().Should().Be(message1Body);
        message2.Should().BeNull();
    }

    [Fact]
    public async Task OnlyOneSessionReceiverCanBeOpenSimultaneously()
    {
        var sessionId = Guid.NewGuid().ToString();
        _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId);
            
        Func<Task> act = async () => await Client.AcceptSessionAsync(SessionQueueName, sessionId);
        await act.Should().ThrowAsync<ServiceBusException>();
    }
    
    [Fact]
    public async Task SessionReceiverSetSessionStateMetadata()
    {
        var message1Body = "message 1";
        var message2Body = "message 2";
        var sessionStateData = "Message 1 processed";
        var sessionId1 = Guid.NewGuid().ToString();

        await SendMessageAsync(message1Body, sessionId1);
        await SendMessageAsync(message2Body, sessionId1);

        ServiceBusReceivedMessage message1 = null, message2 = null;
        BinaryData sessionState = null;
        
        await Task.Run(async () =>
        {
            _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId1);
            message1 = await ReceiveMessageAsync();
            await _sessionReceiver.SetSessionStateAsync(new BinaryData(sessionStateData));
            await _sessionReceiver.CloseAsync();
        });
        await Task.Run(async () =>
        {
            _sessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, sessionId1);
            sessionState = await _sessionReceiver.GetSessionStateAsync();
            message2 = await ReceiveMessageAsync();
        });
        
        message1.Body.ToString().Should().Be(message1Body);
        message2.Body.ToString().Should().Be(message2Body);
        sessionState.ToString().Should().Be(sessionStateData);
    }

    private async Task SendMessageAsync(string body, string sessionId)
    {
        var message = new ServiceBusMessage(body)
        {
            SessionId = sessionId
        };
        await _sender.SendMessageAsync(message);
    }
    
    private async Task<ServiceBusReceivedMessage?> ReceiveMessageAsync(bool completeMessage = true)
    {
        var message = await _sessionReceiver.ReceiveMessageAsync(TimeSpan.FromSeconds(1));
        if (message != null && completeMessage)
        {
            await _sessionReceiver.CompleteMessageAsync(message);
        }

        return message;
    }
}