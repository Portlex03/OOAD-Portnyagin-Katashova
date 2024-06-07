namespace SpaceBattle.Lib.Tests;

using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class SetupGameStrategyTests
{
    public SetupGameStrategyTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }

    private void SetupAngRegisterGetScope(Mock<IStrategy> strategy, object scope)
    {
        strategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(scope).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.GetScope",
            (object[] args) => strategy.Object.Invoke(args)
        ).Execute();
    }

    private void SetupAndRegisterForMoveCommandStartable(
        Mock<IMoveCommandStartable> mockStartable,
        Mock<IUObject> uObject,
        Mock<ICommand> mockOperationMovementICommand,
        Mock<ICommand> mockInitialValuesSetICommand,
        Mock<IQueue> mockGameQueueIQueue)
    {
        mockStartable.Setup(s => s.Target).Returns(uObject.Object);
        mockStartable.Setup(s => s.InitialValues).Returns(
            new Dictionary<string, object>()
        );

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Get.MoveCommandStartable",
            (object[] args) => mockStartable.Object
        ).Execute();


        mockOperationMovementICommand.Setup(cmd => cmd.Execute()).Verifiable();
        mockInitialValuesSetICommand.Setup(cmd => cmd.Execute()).Verifiable();
        mockGameQueueIQueue.Setup(queue => queue.Put(It.IsAny<ICommand>())).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Operations.Movement",
            (object[] args) => mockOperationMovementICommand.Object
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "InitialValues.Set",
            (object[] args) => mockInitialValuesSetICommand.Object
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Game.Queue",
            (object[] args) => mockGameQueueIQueue.Object
        ).Execute();
    }

    private void SuccessfulTests(Action act)
    {
        try
        {
            act();
        }
        catch (Exception ex)
        {
            Assert.Fail("Test should be performed without exceptions: " + ex.Message);
        }
    }

    [Fact]
    public void ArgsEmptyStrategyGetScopeThrowsExceptionTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();


        Assert.Throws<IndexOutOfRangeException>(() => setupGameStrategy.Invoke());


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs.Count() == 0)), Times.Never());
    }

    [Fact]
    public void StrategyGetScopeThrowsExceptionTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        mockGetScopeStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Throws<InvalidOperationException>().Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.GetScope",
            (object[] args) => mockGetScopeStrategy.Object.Invoke(args)
        ).Execute();


        SetupGameStrategy setupGameStrategy = new();


        Assert.Throws<InvalidOperationException>(() => setupGameStrategy.Invoke(scopeId.Object));


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));
    }

    [Fact]
    public void SuccessfulSetupGameStrategyTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();


        SuccessfulTests(() => setupGameStrategy.Invoke(scopeId.Object));


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));
    }

    [Fact]
    public void CommandGetStartMoveTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();

        setupGameStrategy.Invoke(scopeId.Object);


        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        Mock<IStrategy> mockGetMoveCommandStartableStrategy = new();
        Mock<IUObject> mockObject = new();
        Mock<Dictionary<string, object>> mockProperty = new();
        Mock<IMoveCommandStartable> mockMoveCommandStartable = new();

        mockGetMoveCommandStartableStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(mockMoveCommandStartable.Object).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Get.MoveCommandStartable",
            (object[] args) => mockGetMoveCommandStartableStrategy.Object.Invoke(args)
        ).Execute();


        Assert.IsType<MoveCommandStart>(IoC.Resolve<ICommand>("Command.Get.StartMove", mockObject.Object, mockProperty.Object));

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));

        mockGetMoveCommandStartableStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == mockObject.Object && factArgs[1] == mockProperty.Object)), Times.Exactly(1));
    }

    [Fact]
    public void EmptyArgsCommandGetStartMoveThrowsExceptionTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();

        setupGameStrategy.Invoke(scopeId.Object);


        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        Mock<IStrategy> mockGetMoveCommandStartableStrategy = new();
        Mock<IUObject> mockObject = new();
        Mock<Dictionary<string, object>> mockProperty = new();
        Mock<IMoveCommandStartable> mockMoveCommandStartable = new();

        mockGetMoveCommandStartableStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(mockMoveCommandStartable.Object).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Get.MoveCommandStartable",
            (object[] args) => mockGetMoveCommandStartableStrategy.Object.Invoke(args)
        ).Execute();


        Assert.Throws<IndexOutOfRangeException>(() => IoC.Resolve<ICommand>("Command.Get.StartMove"));

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));

        mockGetMoveCommandStartableStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == mockObject.Object && factArgs[1] == mockProperty.Object)), Times.Never());
    }

    [Fact]
    public void CommandGetStopMoveTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();

        setupGameStrategy.Invoke(scopeId.Object);


        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        Mock<IStrategy> mockGetMoveCommandEndableStrategy = new();
        Mock<IUObject> mockObject = new();
        Mock<Dictionary<string, object>> mockProperty = new();
        Mock<IMoveCommandEndable> mockMoveCommandEndable = new();

        mockGetMoveCommandEndableStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(mockMoveCommandEndable.Object).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Get.MoveCommandEndable",
            (object[] args) => mockGetMoveCommandEndableStrategy.Object.Invoke(args)
        ).Execute();


        Assert.IsType<EndMoveCommand>(IoC.Resolve<ICommand>("Command.Get.StopMove", mockObject.Object, mockProperty.Object));

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));

        mockGetMoveCommandEndableStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == mockObject.Object && factArgs[1] == mockProperty.Object)), Times.Exactly(1));
    }

    [Fact]
    public void EmptyArgsCommandGetStopMoveThrowsExceptionTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();

        setupGameStrategy.Invoke(scopeId.Object);


        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        Mock<IStrategy> mockGetMoveCommandEndableStrategy = new();
        Mock<IUObject> mockObject = new();
        Mock<Dictionary<string, object>> mockProperty = new();
        Mock<IMoveCommandEndable> mockMoveCommandEndable = new();

        mockGetMoveCommandEndableStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(mockMoveCommandEndable.Object).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Get.MoveCommandEndable",
            (object[] args) => mockGetMoveCommandEndableStrategy.Object.Invoke(args)
        ).Execute();


        Assert.Throws<IndexOutOfRangeException>(() => IoC.Resolve<ICommand>("Command.Get.StopMove"));

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));

        mockGetMoveCommandEndableStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == mockObject.Object && factArgs[1] == mockProperty.Object)), Times.Never());
    }

    [Fact]
    public void CommandGetRotateTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();

        setupGameStrategy.Invoke(scopeId.Object);


        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        Mock<IStrategy> mockGetRotatableStrategy = new();
        Mock<IUObject> mockObject = new();
        Mock<Dictionary<string, object>> mockProperty = new();
        Mock<IRotatable> mockRotatable = new();

        mockGetRotatableStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(mockRotatable.Object).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Get.Rotatable",
            (object[] args) => mockGetRotatableStrategy.Object.Invoke(args)
        ).Execute();


        Assert.IsType<RotateCommand>(IoC.Resolve<ICommand>("Command.Get.Rotate", mockObject.Object, mockProperty.Object));

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));

        mockGetRotatableStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == mockObject.Object && factArgs[1] == mockProperty.Object)), Times.Exactly(1));
    }

    [Fact]
    public void EmptyArgsCommandGetRotateThrowsExceptionTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();

        setupGameStrategy.Invoke(scopeId.Object);


        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        Mock<IStrategy> mockGetRotatableStrategy = new();
        Mock<IUObject> mockObject = new();
        Mock<Dictionary<string, object>> mockProperty = new();
        Mock<IRotatable> mockRotatable = new();

        mockGetRotatableStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(mockRotatable.Object).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Get.Rotatable",
            (object[] args) => mockGetRotatableStrategy.Object.Invoke(args)
        ).Execute();


        Assert.Throws<IndexOutOfRangeException>(() => IoC.Resolve<ICommand>("Command.Get.Rotate"));

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));

        mockGetRotatableStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == mockObject.Object && factArgs[1] == mockProperty.Object)), Times.Never());
    }

    [Fact]
    public void CommandGetShotTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();

        setupGameStrategy.Invoke(scopeId.Object);


        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        Mock<IStrategy> mockGetShotableStrategy = new();
        Mock<IUObject> mockObject = new();
        Mock<Dictionary<string, object>> mockProperty = new();
        Mock<IShotable> mockShotable = new();

        mockGetShotableStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(mockShotable.Object).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Get.Shotable",
            (object[] args) => mockGetShotableStrategy.Object.Invoke(args)
        ).Execute();


        Assert.IsType<ShotCommand>(IoC.Resolve<ICommand>("Command.Get.Shot", mockObject.Object, mockProperty.Object));

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));

        mockGetShotableStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == mockObject.Object && factArgs[1] == mockProperty.Object)), Times.Exactly(1));
    }

    [Fact]
    public void EmptyArgsCommandGetShotThrowsExceptionTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();

        setupGameStrategy.Invoke(scopeId.Object);


        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        Mock<IStrategy> mockGetShotableStrategy = new();
        Mock<IUObject> mockObject = new();
        Mock<Dictionary<string, object>> mockProperty = new();
        Mock<IShotable> mockShotable = new();

        mockGetShotableStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(mockShotable.Object).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Get.Shotable",
            (object[] args) => mockGetShotableStrategy.Object.Invoke(args)
        ).Execute();


        Assert.Throws<IndexOutOfRangeException>(() => IoC.Resolve<ICommand>("Command.Get.Shot"));

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));

        mockGetShotableStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == mockObject.Object && factArgs[1] == mockProperty.Object)), Times.Never());
    }

    [Fact]
    public void StartMoveStrategyTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();

        setupGameStrategy.Invoke(scopeId.Object);


        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();


        Mock<IUObject> uObject = new();

        Mock<IMoveCommandStartable> mockStartable = new();
        Mock<ICommand> mockOperationMovementICommand = new();
        Mock<ICommand> mockInitialValuesSetICommand = new();
        Mock<IQueue> mockGameQueueIQueue = new();

        SetupAndRegisterForMoveCommandStartable(
            mockStartable,
            uObject,
            mockOperationMovementICommand,
            mockInitialValuesSetICommand,
            mockGameQueueIQueue);


        SuccessfulTests(() => IoC.Resolve<object>("SpaceBattle.StartMove", uObject.Object, new Dictionary<string, IUObject>()));

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));

        mockOperationMovementICommand.Verify(cmd => cmd.Execute(), Times.Never());

        mockInitialValuesSetICommand.Verify(cmd => cmd.Execute(), Times.Exactly(1));

        mockGameQueueIQueue.Verify(queue => queue.Put(It.Is<ICommand>(
            factArgs => factArgs == mockOperationMovementICommand.Object)), Times.Exactly(1));
    }



    [Fact]
    public void EmptyArgsStartMoveStrategyThrowsExceptionTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();

        setupGameStrategy.Invoke(scopeId.Object);


        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();


        Mock<IUObject> uObject = new();

        Mock<IMoveCommandStartable> mockStartable = new();
        mockStartable.Setup(s => s.Target).Returns(uObject.Object);
        mockStartable.Setup(s => s.InitialValues).Returns(
            new Dictionary<string, object>()
        );

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Get.MoveCommandStartable",
            (object[] args) => mockStartable.Object
        ).Execute();


        Mock<ICommand> mockOperationMovementICommand = new();
        Mock<ICommand> mockInitialValuesSetICommand = new();
        Mock<IQueue> mockGameQueueIQueue = new();

        mockOperationMovementICommand.Setup(cmd => cmd.Execute()).Verifiable();
        mockInitialValuesSetICommand.Setup(cmd => cmd.Execute()).Verifiable();
        mockGameQueueIQueue.Setup(queue => queue.Put(It.IsAny<ICommand>())).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Operations.Movement",
            (object[] args) => mockOperationMovementICommand.Object
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "InitialValues.Set",
            (object[] args) => mockInitialValuesSetICommand.Object
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Game.Queue",
            (object[] args) => mockGameQueueIQueue.Object
        ).Execute();


        Assert.Throws<IndexOutOfRangeException>(() => IoC.Resolve<object>("SpaceBattle.StartMove"));

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));

        mockOperationMovementICommand.Verify(cmd => cmd.Execute(), Times.Never());

        mockInitialValuesSetICommand.Verify(cmd => cmd.Execute(), Times.Never());

        mockGameQueueIQueue.Verify(queue => queue.Put(It.Is<ICommand>(
            factArgs => factArgs == mockOperationMovementICommand.Object)), Times.Never());
    }

    [Fact]
    public void StopMoveStrategyTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();

        setupGameStrategy.Invoke(scopeId.Object);


        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();


        Mock<IUObject> uObject = new();

        Mock<IMoveCommandEndable> mockEndable = new();
        mockEndable.Setup(s => s.Target).Returns(uObject.Object);
        mockEndable.Setup(s => s.RequiredValues).Returns(
            new Dictionary<string, object>()
        );

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Get.MoveCommandEndable",
            (object[] args) => mockEndable.Object
        ).Execute();


        Mock<IInjectable> mockInjectable = new();
        Mock<ICommand> mockCommand = new();

        mockInjectable.Setup(inject => inject.Inject(It.IsAny<object>())).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Game.UObject.GetProperty",
            (object[] args) => mockInjectable.Object
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Game.Command.EmptyCommand",
            (object[] args) => mockCommand.Object
        ).Execute();


        SuccessfulTests(() => IoC.Resolve<object>("SpaceBattle.StopMove", uObject.Object, new Dictionary<string, IUObject>()));

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));

        mockInjectable.Verify(inject => inject.Inject(It.Is<object>(
            factArgs => (ICommand)factArgs == mockCommand.Object)), Times.Exactly(1));
    }

    [Fact]
    public void EmptyArgsStopMoveStrategyThrowsExceptionTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();

        setupGameStrategy.Invoke(scopeId.Object);


        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();


        Mock<IUObject> uObject = new();

        Mock<IMoveCommandEndable> mockEndable = new();
        mockEndable.Setup(s => s.Target).Returns(uObject.Object);
        mockEndable.Setup(s => s.RequiredValues).Returns(
            new Dictionary<string, object>()
        );

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Get.MoveCommandEndable",
            (object[] args) => mockEndable.Object
        ).Execute();


        Mock<IInjectable> mockInjectable = new();
        Mock<ICommand> mockCommand = new();

        mockInjectable.Setup(inject => inject.Inject(It.IsAny<object>())).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Game.UObject.GetProperty",
            (object[] args) => mockInjectable.Object
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Game.Command.EmptyCommand",
            (object[] args) => mockCommand.Object
        ).Execute();


        Assert.Throws<IndexOutOfRangeException>(() => IoC.Resolve<object>("SpaceBattle.StopMove"));

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));

        mockInjectable.Verify(inject => inject.Inject(It.Is<object>(
            factArgs => (ICommand)factArgs == mockCommand.Object)), Times.Never());
    }

    [Fact]
    public void RotateStrategyTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();

        setupGameStrategy.Invoke(scopeId.Object);


        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        Mock<IUObject> uObject = new();


        Mock<IRotatable> mockRotatable = new();
        mockRotatable.Setup(s => s.N).Returns(360).Verifiable();
        mockRotatable.Setup(s => s.AngularVelocity).Returns(90).Verifiable();
        mockRotatable.SetupGet(s => s.Angle).Returns(0).Verifiable();
        mockRotatable.SetupSet(s => s.Angle = It.IsAny<int>()).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Get.Rotatable",
            (object[] args) => mockRotatable.Object
        ).Execute();


        SuccessfulTests(() => IoC.Resolve<object>("SpaceBattle.Rotate", uObject.Object, new Dictionary<string, IUObject>()));

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));

        mockRotatable.Verify(s => s.N, Times.Exactly(3));
        mockRotatable.Verify(s => s.AngularVelocity, Times.Exactly(1));
        mockRotatable.Verify(s => s.Angle, Times.Exactly(1));
        mockRotatable.VerifySet(s => s.Angle = 90, Times.Exactly(1));
    }

    [Fact]
    public void EmptyArgsRotateStrategyThrowsExceptionTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();

        setupGameStrategy.Invoke(scopeId.Object);


        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        Mock<IUObject> uObject = new();


        Mock<IRotatable> mockRotatable = new();
        mockRotatable.Setup(s => s.N).Returns(360).Verifiable();
        mockRotatable.Setup(s => s.AngularVelocity).Returns(90).Verifiable();
        mockRotatable.SetupGet(s => s.Angle).Returns(0).Verifiable();
        mockRotatable.SetupSet(s => s.Angle = It.IsAny<int>()).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Get.Rotatable",
            (object[] args) => mockRotatable.Object
        ).Execute();


        Assert.Throws<IndexOutOfRangeException>(() => IoC.Resolve<object>("SpaceBattle.Rotate"));

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));

        mockRotatable.Verify(s => s.N, Times.Never());
        mockRotatable.Verify(s => s.AngularVelocity, Times.Never());
        mockRotatable.Verify(s => s.Angle, Times.Never());
        mockRotatable.VerifySet(s => s.Angle = 90, Times.Never());
    }

    [Fact]
    public void ShotStrategyTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();

        setupGameStrategy.Invoke(scopeId.Object);


        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        Mock<IUObject> uObject = new();


        Mock<IShotable> mockShotable = new();
        mockShotable.Setup(s => s.isShootable).Returns(true).Verifiable();
        mockShotable.Setup(s => s.projectileSpeed).Returns(5).Verifiable();
        mockShotable.Setup(s => s.projectileStartPoint).Returns(new Vector(1, 1)).Verifiable();
        mockShotable.Setup(s => s.direction).Returns(new Vector(1, 1)).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Get.Shotable",
            (object[] args) => mockShotable.Object
        ).Execute();

        Mock<IUObject> uObjectTorpedo = new();
        Mock<Dictionary<string, object>> mockGetObjects = new();
        Mock<Dictionary<string, object>> mockGetInitialValue = new();
        string torpedoId = "id1";

        IoC.Resolve<ICommand>("IoC.Register", "Games.Create.Object",
            (object[] args) => uObjectTorpedo.Object
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Create.Torpedo",
            (object[] args) => mockGetInitialValue.Object
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => torpedoId
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.GetObjects",
            (object[] args) => mockGetObjects.Object
        ).Execute();


        Mock<IMoveCommandStartable> mockStartable = new();
        Mock<ICommand> mockOperationMovementICommand = new();
        Mock<ICommand> mockInitialValuesSetICommand = new();
        Mock<IQueue> mockGameQueueIQueue = new();

        SetupAndRegisterForMoveCommandStartable(
            mockStartable,
            uObject,
            mockOperationMovementICommand,
            mockInitialValuesSetICommand,
            mockGameQueueIQueue);


        SuccessfulTests(() => IoC.Resolve<object>("SpaceBattle.Shot", uObject.Object, new Dictionary<string, IUObject>()));

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));

        mockShotable.Verify(s => s.isShootable, Times.Never());
        mockShotable.Verify(s => s.projectileSpeed, Times.Never());
        mockShotable.Verify(s => s.projectileStartPoint, Times.Never());
        mockShotable.Verify(s => s.direction, Times.Never());
    }

    [Fact]
    public void EmptyArgsShotStrategyThrowsExceptionTest()
    {
        Mock<object> scopeId = new();
        Mock<object> quantumTime = new();

        object scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        Mock<IStrategy> mockGetScopeStrategy = new();

        SetupAngRegisterGetScope(mockGetScopeStrategy, scope);


        SetupGameStrategy setupGameStrategy = new();

        setupGameStrategy.Invoke(scopeId.Object);


        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        Mock<IUObject> uObject = new();


        Mock<IShotable> mockShotable = new();
        mockShotable.Setup(s => s.isShootable).Returns(true).Verifiable();
        mockShotable.Setup(s => s.projectileSpeed).Returns(5).Verifiable();
        mockShotable.Setup(s => s.projectileStartPoint).Returns(new Vector(1, 1)).Verifiable();
        mockShotable.Setup(s => s.direction).Returns(new Vector(1, 1)).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Get.Shotable",
            (object[] args) => mockShotable.Object
        ).Execute();

        Mock<IUObject> uObjectTorpedo = new();
        Mock<Dictionary<string, object>> mockGetObjects = new();
        Mock<Dictionary<string, object>> mockGetInitialValue = new();
        string torpedoId = "id1";

        IoC.Resolve<ICommand>("IoC.Register", "Games.Create.Object",
            (object[] args) => uObjectTorpedo.Object
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Create.Torpedo",
            (object[] args) => mockGetInitialValue.Object
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => torpedoId
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.GetObjects",
            (object[] args) => mockGetObjects.Object
        ).Execute();


        Mock<IMoveCommandStartable> mockStartable = new();
        Mock<ICommand> mockOperationMovementICommand = new();
        Mock<ICommand> mockInitialValuesSetICommand = new();
        Mock<IQueue> mockGameQueueIQueue = new();

        SetupAndRegisterForMoveCommandStartable(
            mockStartable,
            uObject,
            mockOperationMovementICommand,
            mockInitialValuesSetICommand,
            mockGameQueueIQueue);


        Assert.Throws<IndexOutOfRangeException>(() => IoC.Resolve<object>("SpaceBattle.Shot"));

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();


        mockGetScopeStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == scopeId.Object)), Times.Exactly(1));

        mockShotable.Verify(s => s.isShootable, Times.Never());
        mockShotable.Verify(s => s.projectileSpeed, Times.Never());
        mockShotable.Verify(s => s.projectileStartPoint, Times.Never());
        mockShotable.Verify(s => s.direction, Times.Never());
    }
}
