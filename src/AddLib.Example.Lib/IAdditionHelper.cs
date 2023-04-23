namespace AddLib.Example.Lib;

/// <summary>
///     Internal service interface.
///     Neither the interface nor the implementation are exposed outside this project.
/// </summary>
internal interface IAdditionHelper
{
    int Add(int x, int y);
}

internal class AdditionHelper : IAdditionHelper
{
    public int Add(int x, int y)
    {
        return x + y;
    }
}
