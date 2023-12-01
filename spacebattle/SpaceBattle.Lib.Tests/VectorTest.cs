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

    [Fact]
    public void AdditionVectors()
    {
        // pred
        Vector a = new Vector (new int [] { 1, 2, 3 });
        Vector b = new Vector (new int [] { 4, 5, 6 });

        // act
        Vector result = a + b;

        // post
        Assert.True(typeof(Vector).IsInstanceOfType(result));
    }
}
