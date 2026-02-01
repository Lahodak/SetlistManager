namespace SetlistManager.Common.Exceptions;

public class ProviderNotFoundException : Exception
{
    public string ProviderName { get; }
    public ProviderNotFoundException(string providerName)
        : base($"Provider '{providerName}' was not found.")
    {
        ProviderName = providerName;
    }
    public ProviderNotFoundException(string providerName, string message) 
        : base(message)
    {
        ProviderName = providerName;
    }
    public ProviderNotFoundException(string providerName, string message, Exception innerException) 
        : base(message, innerException)
    {
        ProviderName = providerName;
    }
}