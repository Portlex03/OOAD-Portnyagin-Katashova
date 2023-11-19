using Moq;
namespace SpaceBattle.Lib.Tests;

public class RotateCommandTest
{
    [Fact]
    public void GameObjectCanRotateAroundItsOwnAxis()
    {
        var Rotateable = new Mock<IRotatable>();
        Rotateable.SetupGet(t => t.Angle).Returns(45).Verifiable();
        Rotateable.SetupGet(t => t.AngularVelocity).Returns(45).Verifiable();
        Rotateable.SetupGet(t => t.N).Returns(8).Verifiable();
        var cmd = new RotateCommand(Rotateable.Object);

        cmd.Execute();

        Rotateable.VerifySet(t => t.Angle = 90, Times.Once);
        Rotateable.VerifyAll();
    }

    [Fact]
    public void ImpossibleToDetermineAngleOfInclinationOfGameObject()
    {
        var Rotateable = new Mock<IRotatable>();
        Rotateable.SetupGet(t => t.Angle).Throws(() => new Exception()).Verifiable(); // new Exception("lolkekexception")
        Rotateable.SetupGet(t => t.AngularVelocity).Returns(45).Verifiable();
        Rotateable.SetupGet(t => t.N).Returns(8).Verifiable();
        var cmd = new RotateCommand(Rotateable.Object);

        Assert.Throws<Exception>(cmd.Execute);
    }

    [Fact]
    public void ImpossibleToDetermineInstantaneousAngularVelocity()
    {
        var Rotateable = new Mock<IRotatable>();
        Rotateable.SetupGet(t => t.Angle).Returns(45).Verifiable();
        Rotateable.SetupGet(t => t.AngularVelocity).Throws(() => new Exception()).Verifiable();
        Rotateable.SetupGet(t => t.N).Returns(8).Verifiable();
        var cmd = new RotateCommand(Rotateable.Object);

        Assert.Throws<Exception>(cmd.Execute);
    }

    [Fact]
    public void ImpossibleToSetNewAngleOfInclinationOfGameObject()
    {
        var Rotateable = new Mock<IRotatable>();
        Rotateable.SetupGet(t => t.Angle).Returns(45).Verifiable();
        Rotateable.SetupGet(t => t.AngularVelocity).Returns(45).Verifiable();
        Rotateable.SetupGet(t => t.N).Returns(8).Verifiable();
        var cmd = new RotateCommand(Rotateable.Object);

        Rotateable.SetupSet(t => t.Angle = 90).Throws(() => new Exception()).Verifiable();
        Assert.Throws<Exception>(cmd.Execute);
        Rotateable.VerifyAll();
    }
}
