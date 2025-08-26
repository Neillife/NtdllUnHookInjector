using System;
using System.Runtime.InteropServices;
using static NtdllUnHookInjector.Native.Structs;

namespace NtdllUnHookInjector.Native
{
    public static class Delegates
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint NtAllocateVirtualMemoryDelegate(
            IntPtr ProcessHandle,
            ref IntPtr BaseAddress,
            IntPtr ZeroBits,
            ref UIntPtr RegionSize,
            uint AllocationType,
            uint Protect);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint NtWriteVirtualMemoryDelegate(
            IntPtr ProcessHandle,
            IntPtr BaseAddress,
            byte[] Buffer,
            uint NumberOfBytesToWrite,
            out IntPtr NumberOfBytesWritten);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint RtlCreateUserThreadDelegate(
            IntPtr Process,
            IntPtr SecurityDescriptor,
            bool CreateSuspended,
            IntPtr ZeroBits,
            IntPtr MaximumStackSize,
            IntPtr CommittedStackSize,
            IntPtr StartAddress,
            IntPtr Parameter,
            out IntPtr ThreadHandle,
            out CLIENT_ID ClientId);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint NtWaitForSingleObjectDelegate(IntPtr Handle, bool Alertable, IntPtr Timeout);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint NtCloseDelegate(IntPtr Handle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint NtResumeThreadDelegate(IntPtr ThreadHandle, out uint PreviousSuspendCount);
    }
}
