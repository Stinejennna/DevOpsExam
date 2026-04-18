namespace DevOpsTests;

using Xunit;
using DevOpsExam;

public class CalculatorTests
{
    [Fact]
    public void Add_ShouldReturnCorrectSum()
    {
        var calc = new Calculator();
        var result = calc.Add(2, 3);

        Assert.Equal(5, result);
    }
}