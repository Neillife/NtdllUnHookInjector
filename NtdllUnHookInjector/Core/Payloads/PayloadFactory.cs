using System;

namespace NtdllUnHookInjector.Core.Payloads
{
    public static class PayloadFactory
    {
        public static IInjectorPayload CreatePayload(string payloadPath)
        {
            if (payloadPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                return new LoadLibraryPayload(payloadPath);
            }
            else if (payloadPath.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
            {
                return new ShellcodePayload(payloadPath);
            }
            throw new ArgumentException("無效的 Payload 檔案類型，請使用 .dll 或 .bin 檔案。");
        }
    }
}
