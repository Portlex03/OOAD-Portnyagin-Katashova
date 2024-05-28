namespace SpaceBattle.Lib.Tests;

using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class InitGameScopeStrategyTests
{
    public InitGameScopeStrategyTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }

    [Fact]
    public void GetGameIdTest()
    {
        Mock<IStrategy> mockGetGameIdStrategy = new();
        String gameId = "id";
        Mock<object> quantumTime = new();
        
        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(gameId).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        InitGameScopeStrategy initGameScopeStrategy= new InitGameScopeStrategy();

        object scope = initGameScopeStrategy.Invoke(quantumTime.Object);
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        String expectGameId = IoC.Resolve<String>("SpaceBattle.GetGameId");
        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();

        Assert.True(expectGameId.Equals(gameId));

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Exactly(1));
    }

    [Fact]
    public void GetQuantumTimeTest()
    {
        Mock<IStrategy> mockGetGameIdStrategy = new();
        String gameId = "id";
        Mock<object> quantumTime = new();

        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(gameId).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        InitGameScopeStrategy initGameScopeStrategy= new InitGameScopeStrategy();

        object scope = initGameScopeStrategy.Invoke(quantumTime.Object);
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        object expectQuantumTime = IoC.Resolve<object>("SpaceBattle.GetQuantumTime");
        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();

        Assert.True(expectQuantumTime.Equals(quantumTime.Object));

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Exactly(1));
    }

    [Fact]
    public void GetQueueTest()
    {
        Mock<IStrategy> mockGetGameIdStrategy = new();
        String gameId = "id";
        Mock<object> quantumTime = new();

        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(gameId).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        InitGameScopeStrategy initGameScopeStrategy= new InitGameScopeStrategy();

        object scope = initGameScopeStrategy.Invoke(quantumTime.Object);
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        Queue<ICommand> expectQueue = IoC.Resolve<Queue<ICommand>>("SpaceBattle.GetQueue");
        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();

        Assert.IsType<Queue<ICommand>>(expectQueue);
        Assert.True(expectQueue.Count == 0);

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Exactly(1));
    }

    [Fact]
    public void GetObjectsTest()
    {
        Mock<IStrategy> mockGetGameIdStrategy = new();
        String gameId = "id";
        Mock<object> quantumTime = new();

        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(gameId).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        InitGameScopeStrategy initGameScopeStrategy= new InitGameScopeStrategy();

        object scope = initGameScopeStrategy.Invoke(quantumTime.Object);
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        Dictionary<string, IUObject> expectObjects = IoC.Resolve<Dictionary<string, IUObject>>("SpaceBattle.GetObjects");
        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();

        Assert.IsType<Dictionary<string, IUObject>>(expectObjects);
        Assert.True(expectObjects.Count == 0);

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Exactly(1));
    }

    [Fact]
    public void GetObjectTest()
    {
        Mock<IStrategy> mockGetGameIdStrategy = new();
        String gameId = "id";
        Mock<object> quantumTime = new();
        
        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(gameId).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        InitGameScopeStrategy initGameScopeStrategy= new InitGameScopeStrategy();

        object scope = initGameScopeStrategy.Invoke(quantumTime.Object);
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        Dictionary<string, IUObject> objectsTest = IoC.Resolve<Dictionary<string, IUObject>>("SpaceBattle.GetObjects");

        Mock<IUObject> mockObject = new();

        objectsTest["id1"] = mockObject.Object;

        object getObject = IoC.Resolve<object>("SpaceBattle.GetObject", "id1");
        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();

        Assert.True(objectsTest.Count() == 1);
        Assert.True(objectsTest["id1"].Equals(getObject));

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Exactly(1));
    }

    [Fact]
    public void RemoveObjectTest()
    {
        Mock<IStrategy> mockGetGameIdStrategy = new();
        String gameId = "id";
        Mock<object> quantumTime = new();
        
        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(gameId).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        InitGameScopeStrategy initGameScopeStrategy= new InitGameScopeStrategy();

        object scope = initGameScopeStrategy.Invoke(quantumTime.Object);
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        Assert.IsType<RemoveCommandInObject>(IoC.Resolve<RemoveCommandInObject>("SpaceBattle.RemoveObject", "id3"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Exactly(1));
    }

    [Fact]
    public void QueueDequeueTest()
    {
        Mock<IStrategy> mockGetGameIdStrategy = new();
        String gameId = "id";
        Mock<object> quantumTime = new();
        
        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(gameId).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        InitGameScopeStrategy initGameScopeStrategy= new InitGameScopeStrategy();

        object scope = initGameScopeStrategy.Invoke(quantumTime.Object);
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        Queue<ICommand> queueTest = IoC.Resolve<Queue<ICommand>>("SpaceBattle.GetQueue");

        Mock<ICommand> mockCommand = new();

        queueTest.Enqueue(mockCommand.Object);

        ICommand dequeue = IoC.Resolve<ICommand>("SpaceBattle.QueueDequeue", queueTest);
        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();

        Assert.True(mockCommand.Object.Equals(dequeue));

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Exactly(1));
    }

    [Fact]
    public void QueueEnqueueTest()
    {
        Mock<IStrategy> mockGetGameIdStrategy = new();
        String gameId = "id";
        Mock<object> quantumTime = new();
        
        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(gameId).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        InitGameScopeStrategy initGameScopeStrategy= new InitGameScopeStrategy();

        object scope = initGameScopeStrategy.Invoke(quantumTime.Object);
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<ICommand> mockCommand = new();

        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        Assert.IsType<QueueEnqueueCommand>(IoC.Resolve<QueueEnqueueCommand>("SpaceBattle.QueueEnqueue", mockCommand.Object));
        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Exactly(1));
    }

    [Fact]
    public void GetIdThrowsErrorTest()
    {
        Mock<IStrategy> mockGetGameIdStrategy = new();
        Mock<object> quantumTime = new();
        
        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Throws<InvalidOperationException>().Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        InitGameScopeStrategy initGameScopeStrategy = new InitGameScopeStrategy();

        Assert.Throws<InvalidOperationException>(() => initGameScopeStrategy.Invoke(quantumTime.Object));

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Exactly(1));
    }

    [Fact]
    public void quantumTimeThrowsErrorTest()
    {
        Mock<IStrategy> mockGetGameIdStrategy = new();
        String gameId = "id";
        
        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(gameId).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        InitGameScopeStrategy initGameScopeStrategy= new InitGameScopeStrategy();

        Assert.Throws<IndexOutOfRangeException>(() => initGameScopeStrategy.Invoke());

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Exactly(1));
    }

    [Fact]
    public void AnyArgsEmptyGetObjectThrowsExceptionTest()
    {
        Mock<IStrategy> mockGetGameIdStrategy = new();
        String gameId = "id";
        Mock<object> quantumTime = new();
        
        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(gameId).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        InitGameScopeStrategy initGameScopeStrategy= new InitGameScopeStrategy();

        object scope = initGameScopeStrategy.Invoke(quantumTime.Object);
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        Dictionary<string, IUObject> objectsTest = IoC.Resolve<Dictionary<string, IUObject>>("SpaceBattle.GetObjects");

        Assert.Throws<IndexOutOfRangeException>(() => IoC.Resolve<object>("SpaceBattle.GetObject"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Exactly(1));
    }

    [Fact]
    public void AnyArgsEmptyRemoveObjectThrowsExceptionTest()
    {
        Mock<IStrategy> mockGetGameIdStrategy = new();
        String gameId = "id";
        Mock<object> quantumTime = new();
        
        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(gameId).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        InitGameScopeStrategy initGameScopeStrategy= new InitGameScopeStrategy();

        object scope = initGameScopeStrategy.Invoke(quantumTime.Object);
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        Assert.Throws<IndexOutOfRangeException>(() => IoC.Resolve<ICommand>("SpaceBattle.RemoveObject"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Exactly(1));
    }

    [Fact]
    public void QueueDequeueThrowsErrorTest()
    {
        Mock<IStrategy> mockGetGameIdStrategy = new();
        String gameId = "id";
        Mock<object> quantumTime = new();
        
        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(gameId).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        InitGameScopeStrategy initGameScopeStrategy= new InitGameScopeStrategy();

        object scope = initGameScopeStrategy.Invoke(quantumTime.Object);
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        Queue<ICommand> queueTest = IoC.Resolve<Queue<ICommand>>("SpaceBattle.GetQueue");

        Assert.Throws<InvalidOperationException>(() => IoC.Resolve<ICommand>("SpaceBattle.QueueDequeue"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Exactly(1));
    }

    [Fact]
    public void AnyArgsEmptyQueueEnqueueThrowsExceptionTest()
    {
        Mock<IStrategy> mockGetGameIdStrategy = new();
        String gameId = "id";
        Mock<object> quantumTime = new();
        
        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(gameId).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        InitGameScopeStrategy initGameScopeStrategy= new InitGameScopeStrategy();

        object scope = initGameScopeStrategy.Invoke(quantumTime.Object);
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        Assert.Throws<IndexOutOfRangeException>(() => IoC.Resolve<ICommand>("SpaceBattle.QueueEnqueue"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Exactly(1));
    }
}
