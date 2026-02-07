#pragma once
#ifdef __cplusplus
extern "C" {
#endif

#if defined(CPUBENCH_EXPORTS)
#  define CPUBENCH_API __declspec(dllexport)
#else
#  define CPUBENCH_API __declspec(dllimport)
#endif

    typedef struct CpuInfo {
        char brandString[64];
        char vendorString[16];
        int  physicalCores;
        int  logicalThreads;
        int  l1DataKB;
        int  l1InstrKB;
        int  l2KB;
        int  l3KB;
    } CpuInfo;

    CPUBENCH_API void     GetCpuInfo(CpuInfo* info);
    CPUBENCH_API __int64  BenchmarkMonteCarloBatch(unsigned int numThreads);
    CPUBENCH_API __int64  BenchmarkMatrixBatch(unsigned int numThreads);
    CPUBENCH_API __int64  BenchmarkFFTBatch(unsigned int numThreads);

#ifdef __cplusplus
}
#endif