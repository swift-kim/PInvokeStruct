using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;

namespace PInvokeStruct
{
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

    internal class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct Properties
        {
            public IntPtr switches;
            public uint switches_count;
        }

        [DllImport("/mnt/d/Git/pinvoke-struct/PInvokeStruct/libnative.so")]
        internal static extern void NativeFoo([In] ref Properties properties);

        static void Main(string[] args)
        {
            var switches = new string[]
            {
                "--verbose-logging",
                "--trace-startup",
                "--disable-service-auth-codes",
            };

            using var switchesNative = new StringArrayNative(switches);

            var properties = new Properties
            {
                switches = switchesNative.Address,
                switches_count = (uint)switchesNative.Length
            };

            GC.Collect();
            GC.WaitForPendingFinalizers();

            NativeFoo(ref Unsafe.AsRef(properties));
        }
    }
}
