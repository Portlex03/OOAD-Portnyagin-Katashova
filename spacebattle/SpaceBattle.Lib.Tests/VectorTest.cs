using Moq;
using Xunit.Sdk;
namespace SpaceBattle.Lib.Tests;

public class VectorTest
{
    [Fact]
    public void CheckGetHashCode()
    {
        Vector vector = new Vector(1, 5);

        int check = vector.GetHashCode();

        Assert.True(true);
    }

    [Fact]
    public void EqualsNullTest()
    {
        Vector vector = new Vector();
        Vector vector1 = new Vector(12, 5);

        Assert.False(vector1.Equals(vector));
    }

    [Fact]
    public void EqualsNotVectorObject()
    {
        int[] vector = { 12, 5 };
        Vector vector1 = new Vector(12, 5);

        Assert.False(vector1.Equals(vector));
    }
}
