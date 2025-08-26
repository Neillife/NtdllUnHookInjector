namespace NtdllUnHookInjector.Core.Strategies
{
    public interface IInjectionStrategy
    {
        void Run(string appPath, string injectPath);
    }
}
