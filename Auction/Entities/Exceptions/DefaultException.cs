namespace Entities.Exceptions;

public sealed class DefaultException:Exception
{
    public string PropertyKey { get; }
    public DefaultException(string message)
        : base(message)
    {
        PropertyKey = string.Empty;
    }


    public DefaultException(string propertyKey, string message)
        : base(message)
    {
        PropertyKey = propertyKey;
    }
}