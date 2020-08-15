using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PInvokeStruct
{
    class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct Properties
        {
            //[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
            public string[] switches;
            public uint switches_count;
        }

        [DllImport("/mnt/d/Git/pinvoke-struct/PInvokeStruct/libnative.so")]
        internal static extern IntPtr NativeFoo(IntPtr /*Properties*/ properties);

        static void Main(string[] args)
        {
            var switches = new List<string>
            {
                "--verbose-logging",
                "--trace-startup",
                "--disable-service-auth-codes",
            };

            var properties = new Properties
            {
                switches = switches.ToArray(),
                switches_count = (uint)switches.Count
            };

            var pProperties = Marshal.AllocHGlobal(Marshal.SizeOf(properties));
            Marshal.StructureToPtr(properties, pProperties, false);
            NativeFoo(pProperties);
            Marshal.FreeHGlobal(pProperties);
        }
    }
}
