using System;

namespace NtdllUnHookInjector.Core.Strategies
{
    public class SuspendedProcessInfo
    {
        public int ProcessId { get; }
        public IntPtr ProcessHandle { get; }
        public IntPtr ThreadHandle { get; }

        public SuspendedProcessInfo(int processId, IntPtr processHandle, IntPtr threadHandle)
        {
            ProcessId = processId;
            ProcessHandle = processHandle;
            ThreadHandle = threadHandle;
        }
    }
}
