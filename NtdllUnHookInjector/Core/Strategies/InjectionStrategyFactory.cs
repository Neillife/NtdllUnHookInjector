using System;
using NtdllUnHookInjector.Core.Services;

namespace NtdllUnHookInjector.Core.Strategies
{
    public static class InjectionStrategyFactory
    {
        public static IInjectionStrategy CreateStrategy(string mode)
        {
            IInjectionService injectionService = new InjectionService();
            ISuspendedProcessService suspendedProcessService = new SuspendedProcessService();

            switch (mode.ToLower())
            {
                case "attach":
                    return new AttachInjectionStrategy(injectionService);
                case "wait":
                    return new WaitAndInjectStrategy(injectionService);
                case "suspend":
                    return new SuspendedInjectionStrategy(suspendedProcessService, injectionService);
                // ... 其他模式也依此類推
                default:
                    throw new ArgumentException("Invalid injection mode", nameof(mode));
            }
        }
    }
}
