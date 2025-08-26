using NtdllUnHookInjector.Core.Payloads;

namespace NtdllUnHookInjector.Core.Services
{
    public interface IInjectionService
    {
        void Inject(int processId, IInjectorPayload payload);
    }
}
