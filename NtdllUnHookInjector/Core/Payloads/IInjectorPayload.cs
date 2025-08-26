using System;

namespace NtdllUnHookInjector.Core.Payloads
{
    public interface IInjectorPayload
    {
        byte[] GetPayloadBytes();
        IntPtr GetRemoteFunctionAddress();
        string GetPayloadDescription();
    }
}
