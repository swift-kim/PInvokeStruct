using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System;
using System.Text;

namespace PInvokeStruct
{
    class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct Properties
        {
            public IntPtr switches;
            public uint switches_count;
        }

        [DllImport("/mnt/d/Git/pinvoke-struct/PInvokeStruct/libnative.so")]
        internal static extern void NativeFoo([In] ref Properties properties);

        static unsafe IntPtr ToNativeArray(IList<string> strings)
        {
            var pointers = new List<IntPtr>();

            foreach (var s in strings)
            {
                var bytes = Encoding.ASCII.GetBytes(s);
                var pBytes = IntPtr.Zero;
                fixed (byte* p = bytes)
                {
                    pBytes = (IntPtr)p;
                }
                pointers.Add(pBytes);
            }

            fixed (IntPtr* p = pointers.ToArray())
            {
                return (IntPtr)p;
            }
        }

        static unsafe void Main(string[] args)
        {
            var switches = new List<string>
            {
                "--verbose-logging",
                "--trace-startup",
                "--disable-service-auth-codes",
            };

            //var pointers = new List<IntPtr>();

            //foreach (var s in switches)
            //{
            //    var bytes = Encoding.ASCII.GetBytes(s);
            //    var pBytes = IntPtr.Zero;
            //    fixed (byte* p = bytes)
            //    {
            //        pBytes = (IntPtr)p;
            //    }
            //    pointers.Add(pBytes);
            //}
            //var ppBytes = IntPtr.Zero;
            //fixed (IntPtr* p = pointers.ToArray())
            //{
            //    ppBytes = (IntPtr)p;
            //}

            //var bytes = Encoding.ASCII.GetBytes(switches[0]);

            //// Method 3
            //IntPtr pBytes = IntPtr.Zero;
            //fixed (byte* p = bytes)
            //{
            //    pBytes = (IntPtr)p;
            //}

            //var b = new IntPtr[] { pBytes };
            //IntPtr ppBytes = IntPtr.Zero;
            //fixed (IntPtr* p = b)
            //{
            //    ppBytes = (IntPtr)p;
            //}

            // Method 2
            //GCHandle pinnedArray = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            //IntPtr buffer = pinnedArray.AddrOfPinnedObject();

            // Method 1 (works?)
            //var buffer = Marshal.AllocHGlobal(bytes.Length);
            //Marshal.Copy(bytes, 0, buffer, bytes.Length);

            //ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(buffer.ToPointer(), 4);
            //var reference = MemoryMarshal.GetReference(span);

            var properties = new Properties
            {
                switches = ToNativeArray(switches),
                switches_count = (uint)switches.Count
            };

            //pinnedArray.Free();
            //Marshal.FreeHGlobal(buffer);

            NativeFoo(ref Unsafe.AsRef(properties));
        }
    }
}
