namespace SpaceBattle.Lib.Tests;
using Moq;

public class MoveCommandTest
{
    [Fact]
    public void TheGameObjectCanMoveInStraightLine()
    {
        //pre

        var movable = new Mock<IMovable>();
        movable.SetupGet(m => m.Position).Returns(new Vector(12, 5)).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Vector(-5, 3)).Verifiable();
        var mc = new MoveCommand(movable.Object);

        //act

        mc.Execute();

        //post
        //movable // pos == (7, 8)

        movable.VerifySet(m => m.Position = new Vector(7, 8), Times.Once);
        movable.VerifyAll();
    }

    [Fact]
    public void ImpossibleToDetermineCurrentPositionOfGameObject()
    {
        //pre

        var movable = new Mock<IMovable>();
        movable.SetupGet(m => m.Position).Throws(() => new Exception()).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Vector(-5, 3)).Verifiable();
        var mc = new MoveCommand(movable.Object);

        //act
        //post

        Assert.Throws<Exception>(mc.Execute);
    }

    [Fact]
    public void ImpossibleToDetermineInstantaneousSpeedOfGameObject()
    {
        //pre

        var movable = new Mock<IMovable>();
        movable.SetupGet(m => m.Position).Returns(new Vector(12, 5)).Verifiable();
        movable.SetupGet(m => m.Velocity).Throws(() => new Exception()).Verifiable();
        var mc = new MoveCommand(movable.Object);

        //act
        //post

        Assert.Throws<Exception>(mc.Execute);
    }

    [Fact]
    public void ImpossibleToChangePositionOfGameObject()
    {
        //pre

        var movable = new Mock<IMovable>();
        movable.SetupGet(m => m.Position).Returns(new Vector(12, 5)).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Vector(-5, 3)).Verifiable();
        var mc = new MoveCommand(movable.Object);

        //act

        movable.SetupSet(m => m.Position = new Vector(7, 8)).Throws(() => new Exception()).Verifiable();

        //post

        Assert.Throws<Exception>(mc.Execute);
        movable.VerifyAll();
    }
}
