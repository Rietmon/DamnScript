namespace DamnScript.Runtimes.Cores;

public readonly unsafe struct SafeString
{
    public readonly string safeValue;
    public readonly UnsafeString* unsafeValue;
    
    public SafeString(string value) => safeValue = value;
    public SafeString(UnsafeString* value) => unsafeValue = value;

    public static implicit operator SafeString(string value) => new(value);
    public static implicit operator SafeString(UnsafeString* value) => new(value);
}