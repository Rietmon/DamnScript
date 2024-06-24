namespace DamnScript.Cores;

public unsafe struct String32
{
    public fixed char value[32];
    
    public String32(string value)
    {
        fixed (char* valuePtr = value)
        {
            for (var i = 0; i < 32; i++)
            {
                this.value[i] = valuePtr[i];
            }
        }
    }
    
    public char this[int index]
    {
        get => value[index];
        set => this.value[index] = value;
    }
}

public unsafe struct String64
{
    public fixed char value[64];
    
    public String64(string value)
    {
        fixed (char* valuePtr = value)
        {
            for (var i = 0; i < 32; i++)
            {
                this.value[i] = valuePtr[i];
            }
        }
    }
    
    public char this[int index]
    {
        get => value[index];
        set => this.value[index] = value;
    }
}