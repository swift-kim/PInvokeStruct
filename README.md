# PInvokeStruct

An example of converting a struct containing a string array in C# into a native struct in C.

The `StringArrayNative` class effectively represents a memory region (`char**`) that can be accessed from the native side.

```csharp
/// <summary>
/// Re-allocates an array of managed strings for unmanaged access.
/// </summary>
internal class StringArrayNative : IDisposable
{
    private readonly List<GCHandle> _handles = new List<GCHandle>();

    public StringArrayNative(string[] managed)
    {
        var pointers = new List<IntPtr>();
        foreach (var str in managed)
        {
            var bytes = Encoding.ASCII.GetBytes(str);
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            _handles.Add(handle);
            pointers.Add(handle.AddrOfPinnedObject());
        }
        _handles.Add(GCHandle.Alloc(pointers.ToArray(), GCHandleType.Pinned));
    }

    public int Length => _handles.Count - 1;

    public IntPtr Address => _handles[Length].AddrOfPinnedObject();

    public void Dispose()
    {
        foreach (var handle in _handles)
        {
            handle.Free();
        }
    }
}
```
