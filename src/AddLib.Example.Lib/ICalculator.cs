namespace AddLib.Example.Lib;

/// <summary>
///     Public service. This interface is exposed to consumers of this library.
/// </summary>
public interface ICalculator
{
    int Add(int x, int y);
}

/// <summary>
///     The implementation class can be internal, because consumers don't need to handle the registration.
/// </summary>
internal class Calculator : ICalculator
{
    private readonly IAdditionHelper _helper;

    public Calculator(IAdditionHelper helper)
    {
        _helper = helper;
    }

    public int Add(int x, int y)
    {
        return _helper.Add(x, y);
    }
}
