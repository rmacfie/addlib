using AddLib.Example.Lib;

namespace AddLib.Example.App;

internal interface IMessageFactory
{
    string GetMessage();
}

internal class MessageFactory : IMessageFactory
{
    private readonly ICalculator _calculator;

    public MessageFactory(ICalculator calculator)
    {
        _calculator = calculator;
    }

    public string GetMessage()
    {
        var sum = _calculator.Add(3, 6);
        return $"Hello, today's number is {sum}.";
    }
}
