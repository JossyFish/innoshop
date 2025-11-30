
public class CacheDataNotFoundException : Exception
{
    public string DataType { get; }  
    public string Identifier { get; }

    public CacheDataNotFoundException(string dataType, string identifier)
        : base($"{dataType} data not found for: {identifier}")
    {
        DataType = dataType;
        Identifier = identifier;
    }
}