using System;
using System.IO;

namespace NtdllUnHookInjector.Core.Payloads
{
    public class ShellcodePayload : IInjectorPayload
    {
        private readonly string _filePath;

        public ShellcodePayload(string filePath)
        {
            _filePath = filePath;
        }

        public byte[] GetPayloadBytes()
        {
            return File.ReadAllBytes(_filePath);
        }

        public IntPtr GetRemoteFunctionAddress()
        {
            return IntPtr.Zero; // 對於 Shellcode 執行的地址就是它自己的內存地址
        }

        public string GetPayloadDescription()
        {
            return $"Shellcode '{_filePath}'";
        }
    }
}
