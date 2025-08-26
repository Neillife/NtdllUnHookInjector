using System;

namespace NtdllUnHookInjector.Native
{
    public class Enums
    {
        [Flags]
        public enum ProcessCreationFlags : uint
        {
            CREATE_SUSPENDED = 0x00000004
        }
    }
}
