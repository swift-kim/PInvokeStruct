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

        static unsafe IntPtr ToNativeArray(string[] strings)
        {
            var pointers = new List<IntPtr>();

            foreach (var s in strings)
            {
                fixed (byte* pb = Encoding.ASCII.GetBytes(s))
                {
                    pointers.Add((IntPtr)pb);
                }
            }

            fixed (IntPtr* pp = pointers.ToArray())
            {
                return (IntPtr)pp;
            }
        }

        static void Main(string[] args)
        {
            var switches = new string[]
            {
                "--verbose-logging",
                "--trace-startup",
                "--disable-service-auth-codes",
            };

            var properties = new Properties
            {
                switches = ToNativeArray(switches),
                switches_count = (uint)switches.Length
            };

            NativeFoo(ref Unsafe.AsRef(properties));
        }
    }
}
