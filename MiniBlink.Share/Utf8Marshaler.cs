using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MiniBlink.Share
{
    public class Utf8Marshaler : ICustomMarshaler
    {
        public static Utf8Marshaler Instance = new Utf8Marshaler();

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return Instance;
        }

        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            //对于const utf8应交由MiniBlink.Share内部回收
            //Marshal.FreeHGlobal(pNativeData);
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            if (ManagedObj is string str)
            {
                var data = Encoding.UTF8.GetBytes(str);
                IntPtr handle = Marshal.AllocHGlobal(data.Length + 1);
                Marshal.Copy(data, 0, handle, data.Length);
                Marshal.WriteByte(handle, data.Length, 0);
                return handle;
            }

            throw new InvalidOperationException();
        }

        private static bool IsWin32Atom(IntPtr ptr)
        {
            long num = (long) ptr;
            return (num & -65536L) == 0L;
        }

        public object MarshalNativeToManaged(IntPtr ptr)
        {
            if (ptr == default || IsWin32Atom(ptr))
                return null;


            var data = new List<byte>();
            var off = 0;
            while (true)
            {
                var ch = Marshal.ReadByte(ptr, off++);
                if (ch == 0)
                {
                    break;
                }

                data.Add(ch);
            }

            var str = Encoding.UTF8.GetString(data.ToArray());
            return str;
            // unsafe
            // {
            //     byte* p = (byte*)pNativeData;
            //     int len = 0;
            //     while (*p++ != 0)
            //         len++;
            //     var str= new string((sbyte*)pNativeData, 0, len, Encoding.UTF8);
            //     return str;
            // }
        }
    }
}