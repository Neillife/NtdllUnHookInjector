namespace NtdllUnHookInjector.Native
{
    public static class NativeConstant
    {
        public const uint GENERIC_READ = 0x80000000;
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint OPEN_EXISTING = 3;
        public const uint PAGE_READONLY = 0x02;
        public const uint SEC_IMAGE = 0x1000000;
        public const uint FILE_MAP_READ = 0x0004;
        public const uint PAGE_EXECUTE_READWRITE = 0x40;
        public const uint PROCESS_ALL_ACCESS = 0x001F0FFF;
        public const uint MEM_COMMIT = 0x00001000;
        public const uint MEM_RESERVE = 0x00002000;
        public const uint PAGE_READWRITE = 0x04;
    }
}
