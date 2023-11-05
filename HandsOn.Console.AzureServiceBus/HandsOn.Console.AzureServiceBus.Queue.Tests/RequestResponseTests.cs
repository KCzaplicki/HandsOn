namespace HandsOn.Console.AzureServiceBus.Queue.Tests;

public class RequestResponseTests : BaseTest
{
    private readonly ServiceBusSender _sender;
    
    private ServiceBusSessionReceiver _requestSessionReceiver;
    private ServiceBusSessionReceiver _responseSessionReceiver;

    public RequestResponseTests()
    {
        _sender = Client.CreateSender(SessionQueueName);
    }

    public override async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _requestSessionReceiver.DisposeAsync();
        await _responseSessionReceiver.DisposeAsync();

        await base.DisposeAsync();
    }

    [Fact]
    public async Task RequestMessageSetReplySessionIdAndResponseIsSendInTheSession()
    {
        var requestSessionId = Guid.NewGuid().ToString();
        var responseSessionId = Guid.NewGuid().ToString();
        var requestMessageBody = "Request message";
        var responseMessageBody = "Response message";
        
        // 1. Request message is send to the queue with ReplyToSessionId property set
        await _sender.SendMessageAsync(new ServiceBusMessage(requestMessageBody)
        {
            SessionId = requestSessionId,
            ReplyToSessionId = responseSessionId
        });
        
        // 2. Request message is received from the queue
        _requestSessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, requestSessionId);
        var requestMessage = await _requestSessionReceiver.ReceiveMessageAsync();
        await _requestSessionReceiver.CompleteMessageAsync(requestMessage);
        
        // 3. Response message is send to the queue with SessionId property set
        await _sender.SendMessageAsync(new ServiceBusMessage(responseMessageBody)
        {
            SessionId = responseSessionId
        });
        
        // 4. Response message is received from the session
        _responseSessionReceiver = await Client.AcceptSessionAsync(SessionQueueName, responseSessionId);
        var responseMessage = await _responseSessionReceiver.ReceiveMessageAsync();
        await _responseSessionReceiver.CompleteMessageAsync(responseMessage);

        requestMessage.Body.ToString().Should().Be(requestMessageBody);
        requestMessage.ReplyToSessionId.Should().Be(responseSessionId);
        responseMessage.Body.ToString().Should().Be(responseMessageBody);
        responseMessage.SessionId.Should().Be(responseSessionId);
    }
}