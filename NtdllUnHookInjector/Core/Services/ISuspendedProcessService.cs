using NtdllUnHookInjector.Core.Strategies;

namespace NtdllUnHookInjector.Core.Services
{
    public interface ISuspendedProcessService
    {
        SuspendedProcessInfo CreateSuspendedProcess(string appPath);
    }
}
