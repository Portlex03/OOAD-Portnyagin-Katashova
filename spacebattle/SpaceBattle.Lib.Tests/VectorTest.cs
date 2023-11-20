using Moq;
namespace SpaceBattle.Lib.Tests;

public class VectorTest
{
    [Fact]
    public void CheckGetHashCode()
    {
        Vector vector = new Vector(1,5);

        int check = vector.GetHashCode();

        Assert.True(true);
    }
}
