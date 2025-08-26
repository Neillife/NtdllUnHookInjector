using System;
using System.Text;
using static NtdllUnHookInjector.Native.NativeMethods;

namespace NtdllUnHookInjector.Core.Payloads
{
    public class LoadLibraryPayload : IInjectorPayload
    {
        private readonly string _dllPath;

        public LoadLibraryPayload(string dllPath)
        {
            _dllPath = dllPath;
        }

        public byte[] GetPayloadBytes()
        {
            return Encoding.Unicode.GetBytes(_dllPath + "\0");
        }

        public IntPtr GetRemoteFunctionAddress()
        {
            IntPtr kernel32Handle = GetModuleHandle("kernel32.dll");
            return GetProcAddress(kernel32Handle, "LoadLibraryW");
        }

        public string GetPayloadDescription()
        {
            return $"DLL '{_dllPath}'";
        }
    }

}
