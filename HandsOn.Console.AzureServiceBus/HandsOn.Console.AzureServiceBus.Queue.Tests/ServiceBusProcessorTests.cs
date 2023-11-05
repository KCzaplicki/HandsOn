namespace HandsOn.Console.AzureServiceBus.Queue.Tests;

public class ServiceBusProcessorTests : BaseTest
{
    private readonly ServiceBusSender _sender;
    private ServiceBusProcessor _processor;

    public ServiceBusProcessorTests()
    {
        _sender = Client.CreateSender(SimpleQueueName);
    }
    
    public override async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _processor.DisposeAsync();

        await base.DisposeAsync();
    }

    [Fact]
    public async Task ProcessorReceiveMessages()
    {
        var mre = new ManualResetEvent(false);
        var messageBody = "message";
        ServiceBusReceivedMessage message = null;
        
        _processor = Client.CreateProcessor(SimpleQueueName);
        _processor.ProcessMessageAsync += async args =>
        {
            message = args.Message;
            await args.CompleteMessageAsync(args.Message);
            
            mre.Set();
        };
        _processor.ProcessErrorAsync += async _ =>
        {
            Assert.Fail("Error occurred during processing message.");
        };

        await _processor.StartProcessingAsync();
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody));
        mre.WaitOne();
        
        await _processor.StopProcessingAsync();
        
        message.Should().NotBeNull();
        message!.Body.ToString().Should().Be(messageBody);
    }
    
    [Fact]
    public async Task ProcessorCanAutoCompleteMessages()
    {
        var mre = new ManualResetEvent(false);
        var messageBody = "message";
        ServiceBusReceivedMessage message = null;
        
        _processor = Client.CreateProcessor(SimpleQueueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = true
        });
        _processor.ProcessMessageAsync += async args =>
        {
            message = args.Message;
            
            mre.Set();
        };
        _processor.ProcessErrorAsync += async _ =>
        {
            Assert.Fail("Error occurred during processing message.");
        };

        await _processor.StartProcessingAsync();
        
        await _sender.SendMessageAsync(new ServiceBusMessage(messageBody));
        mre.WaitOne();
        
        await _processor.StopProcessingAsync();
        
        message.Should().NotBeNull();
        message!.Body.ToString().Should().Be(messageBody);
    }
}