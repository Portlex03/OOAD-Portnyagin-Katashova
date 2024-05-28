namespace SpaceBattle.Lib.Tests;

using Hwdtech;
using Moq;

public class RemoveCommandInObjectTests
{
    [Fact]
    public void KeyIsNotInDictionaryTest()
    {
        Mock<IDictionary<string, IUObject>> mockDict = new();
        string key = "1";

        mockDict.Setup(dict => dict.Remove(It.IsAny<string>())).Returns(false).Verifiable();

        RemoveCommandInObject removeCommandInObject = new(mockDict.Object, key);

        try
        {
            removeCommandInObject.Execute();
        }
        catch (Exception ex)
        {
            Assert.Fail("Test should be performed without exceptions" + ex.Message);
        }

        mockDict.Verify(dict => dict.Remove(It.Is<string>(factArgs => factArgs == key)), Times.Exactly(1));
    }

    [Fact]
    public void SuccessfulTest()
    {
        Mock<IDictionary<string, IUObject>> mockDict = new();
        string key = "1";

        mockDict.Setup(dict => dict.Remove(It.IsAny<string>())).Returns(true).Verifiable();

        RemoveCommandInObject removeCommandInObject = new(mockDict.Object, key);

        try
        {
            removeCommandInObject.Execute();
        }
        catch (Exception ex)
        {
            Assert.Fail("Test should be performed without exceptions" + ex.Message);
        }

        mockDict.Verify(dict => dict.Remove(It.Is<string>(factArgs => factArgs == key)), Times.Exactly(1));
    }
}
