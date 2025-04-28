using SmartBook.ConsoleUI.Helpers;

namespace SmartBook.Tests.ConsoleUITests;
public class ConsoleHelperTests
{
    [Fact]
    public void WriteTitle_ShouldOutputCorrectFormat()
    {
        var sw = new StringWriter();
        Console.SetOut(sw);

        ConsoleHelper.PrintActionTitle(">>>Test Title<<<", "Additional test clarification:");

        var expected = ">>>Test Title<<<" + Environment.NewLine + Environment.NewLine + "Additional test clarification:" + Environment.NewLine;
        Assert.Equal(expected, sw.ToString());
    }
}
