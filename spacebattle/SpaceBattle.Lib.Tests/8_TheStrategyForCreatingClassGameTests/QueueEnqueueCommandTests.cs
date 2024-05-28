namespace SpaceBattle.Lib.Tests;

using Hwdtech;
using Moq;

public class QueueEnqueueCommandTests
{
    [Fact]
    public void SuccessfulTest()
    {
        Queue<ICommand> queue = new();
        Mock<ICommand> mockCommand = new();

        QueueEnqueueCommand queueEnqueueCommand= new(queue, mockCommand.Object);

        try
        {
            queueEnqueueCommand.Execute();
        }
        catch (Exception ex)
        {
            Assert.Fail("Test should be performed without exceptions" + ex.Message);
        }

        Assert.True(queue.Dequeue().Equals(mockCommand.Object));
    }
}
