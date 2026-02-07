using System.CodeDom;
using System.Runtime.InteropServices;

namespace CPUBench.app
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct CpuInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string brandString;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string vendorString;

        public int physicalCores;
        public int logicalThreads;
        public int l1DataKB;
        public int l1InstrKB;
        public int l2KB;
        public int l3KB;
    }

    internal static class Native
    {
        private const string DllName = "CpuBenchDLL.dll";

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetCpuInfo(ref CpuInfo info);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long BenchmarkMonteCarloBatch(uint numThreads);

        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern long BenchmarkMatrixBatch(uint numThreads);

        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern long BenchmarkFFTBatch(uint numThreads);
    }
}
