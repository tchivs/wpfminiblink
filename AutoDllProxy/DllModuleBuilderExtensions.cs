using System.Runtime.InteropServices;

namespace AutoDllProxy
{
    public static class DllModuleBuilderExtensions
    {
        public static DllModuleBuilder SetDllName(this DllModuleBuilder builder, string name, string x64Name = null)
        {
            builder.DllName = name;
            builder.DllX64Name ??= x64Name;
            return builder;
        }
    }
}