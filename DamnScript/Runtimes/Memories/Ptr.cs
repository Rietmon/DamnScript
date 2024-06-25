namespace DamnScript.Runtimes.Memories;

public unsafe struct Ptr<T> where T : unmanaged
{
    public int id;
    public T* ptr;
}