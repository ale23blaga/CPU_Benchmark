#define CPUBENCH_EXPORTS
#include "cpubench.h"
#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include <string.h>
#include <Windows.h>
#include <math.h>
#include <iostream>

#define _CRT_SECURE_NO_WARNINGS


#define FREQUENCY 3000000000u // Hz (3.0 GHz )
#define RUNS 10
#define N 10000
#define MAX 1000
#define DEFAULT_THREADS 1
#define ITERATIONS_PER_THREAD 1000000u 
#define TOTAL_ITERATIONS 747483642u
#define MAX_THREADS 64
#define MM_NR 950
#define FFT_N 1024 // trebe sa fie putere de 2
#define FFT_BLOCKS 65536 

#define rdtsc  __asm __emit 0fh __asm __emit 031h 
#define cpuid  __asm __emit 0fh __asm __emit 0a2h


// CPU INFO SECTION

void GetCpuInfo(CpuInfo* info)
{
    if (!info)
        return;
    memset(info, 0, sizeof(CpuInfo));

    info->physicalCores = -1;
    info->logicalThreads = -1;
    info->l1DataKB = -1;
    info->l1InstrKB = -1;
    info->l2KB = -1;
    info->l3KB = -1;

    // 0. Producatorul CPUID(0)
    unsigned int eax0 = 0, ebx0 = 0, ecx0 = 0, edx0 = 0;
    __asm
    {
        mov eax, 0x0
        cpuid
        mov eax0, eax
        mov ebx0, ebx
        mov ecx0, ecx
        mov edx0, edx
    }

    // vendor = EBX + EDX + ECX as ASCII
    // "GenuineIntel" sau "AuthenticAMD"
    char vendor[16] = { 0 };
    memcpy(vendor + 0, &ebx0, 4);
    memcpy(vendor + 4, &edx0, 4);
    memcpy(vendor + 8, &ecx0, 4);
    vendor[12] = '\0';

    strncpy_s(info->vendorString, vendor, sizeof(info->vendorString) - 1);


    // 1. BRAND STRING (0x80000002 / 03 / 04)
    unsigned int regs[4];
    char brand[64] = { 0 };
    char* out = brand;

    __asm
    {
        mov eax, 0x80000002
        cpuid
        mov regs[0 * 4], eax
        mov regs[1 * 4], ebx
        mov regs[2 * 4], ecx
        mov regs[3 * 4], edx
    }
    memcpy(out, regs, 16);
    out += 16;

    __asm
    {
        mov eax, 0x80000003
        cpuid
        mov regs[0 * 4], eax
        mov regs[1 * 4], ebx
        mov regs[2 * 4], ecx
        mov regs[3 * 4], edx
    }
    memcpy(out, regs, 16);
    out += 16;

    __asm
    {
        mov eax, 0x80000004
        cpuid
        mov regs[0 * 4], eax
        mov regs[1 * 4], ebx
        mov regs[2 * 4], ecx
        mov regs[3 * 4], edx
    }
    memcpy(out, regs, 16);
    out += 16;


    char* p = brand;
    while (*p == ' ')
        p++;
    strncpy_s(info->brandString, p, sizeof(info->brandString) - 1);
    info->brandString[sizeof(info->brandString) - 1] = '\0';


    // 2. nr thread-uri logice: CPUID(1).EBX[23:16]
    unsigned int logicalCount = 0;
    unsigned int eax1 = 0, ebx1 = 0, ecx1 = 0, edx1 = 0;
    __asm
    {
        mov eax, 0x01
        cpuid
        mov eax1, eax
        mov ebx1, ebx
        mov ecx1, ecx
        mov edx1, edx
    }
    logicalCount = (ebx1 >> 16) & 0xFF;
    if (logicalCount == 0)
    {
        logicalCount = 1;
    }
    info->logicalThreads = (int)logicalCount;

    // Verficare hyperThreading
    int hasHT = (edx1 & (1 << 28)) != 0;

    // 3. Physical cores:
    // Daca vendorul este "AuthenticAMD": atunci 0x80000008 (AMD)
    // Daca vendorul == "GenuineIntel": atunci 0x04 (Intel)
    if (strncmp(info->vendorString, "AuthenticAMD", 12) == 0)
    {
        //Verificam daca putem folosi 0x80000008
        unsigned int maxExtended = 0;
        __asm
        {
            mov eax, 0x80000000
            cpuid
            mov maxExtended, eax
        }


        int threadsPerCore = 1;
        if (maxExtended >= 0x8000001E) // Amd zen3+
        {
            unsigned int eax1e = 0, ebx1e = 0, ecx1e = 0, edx1e = 0;
            __asm
            {
                mov eax, 0x8000001E
                xor ecx, ecx
                cpuid
                mov eax1e, eax
                mov ebx1e, ebx
                mov ecx1e, ecx
                mov edx1e, edx
            }

            unsigned int tpc_minus1 = (ebx1e >> 8) & 0xFF;
            threadsPerCore = (int)(tpc_minus1 + 1);
            if (threadsPerCore <= 0) threadsPerCore = 1;

            if (info->logicalThreads > 0)
            {
                int cores = info->logicalThreads / threadsPerCore;
                if (cores <= 0) cores = 1;
                info->physicalCores = cores;
            }
        }
        else
            if (maxExtended >= 0x80000008) // amd zen2
            {
                unsigned int eax8 = 0, ebx8 = 0, ecx8 = 0, edx8 = 0;
                __asm
                {
                    mov eax, 0x80000008
                    cpuid
                    mov eax8, eax
                    mov ebx8, ebx
                    mov ecx8, ecx
                    mov edx8, edx
                }

                // AMD coreuri:
                // Incercam ECX[15:12] prima data (pentru core-uri). Daca e 0, incercam si ECX[7:0].
                unsigned int coreField = (ecx8 >> 12) & 0xF; // layout now pt AMD
                if (coreField == 0)
                {
                    coreField = ecx8 & 0xFF; // layout vechi pt AMD
                }

                if (coreField == 0)
                {
                    info->physicalCores = 1;
                }
                else
                {
                    info->physicalCores = (int)(coreField + 1);
                }
            }
    }
    else if (strncmp(info->vendorString, "GenuineIntel", 12) == 0)
    {
        unsigned int maxLeaf = eax0;
        int smtWidth = 0;
        int coreCount = 0;
        int totalLogical = 0;

        // Metoda1 1: CPUID leaf 0x1F
        if (maxLeaf >= 0x1F)
        {
            int smtMaskWidth = 0;
            int coreMaskWidth = 0;

            for (unsigned int i = 0; i < 10; i++)
            {
                unsigned int eax1f = 0, ebx1f = 0, ecx1f = 0, edx1f = 0;
                __asm
                {
                    mov eax, 0x1F
                    mov ecx, i
                    cpuid
                    mov eax1f, eax
                    mov ebx1f, ebx
                    mov ecx1f, ecx
                    mov edx1f, edx
                }

                unsigned int levelType = (ecx1f >> 8) & 0xFF;
                if (levelType == 0) break;

                unsigned int shiftWidth = eax1f & 0x1F;
                unsigned int numLogicalThisLevel = ebx1f & 0xFFFF;

                if (levelType == 1)
                {
                    smtMaskWidth = shiftWidth;
                    smtWidth = numLogicalThisLevel;
                }
                else if (levelType == 2)
                {
                    coreMaskWidth = shiftWidth;
                    totalLogical = numLogicalThisLevel;

                    if (smtWidth > 0)
                    {
                        coreCount = totalLogical / smtWidth;
                    }
                    else
                    {
                        coreCount = 1 << (coreMaskWidth - smtMaskWidth);
                    }
                }
            }

            if (totalLogical > 0)
            {
                info->logicalThreads = totalLogical;
            }
            if (coreCount > 0)
            {
                info->physicalCores = coreCount;
            }
        }

        // Metoda 2: CPUID leaf 0xB (Extended Topology)
        if ((info->physicalCores <= 0 || info->logicalThreads <= 0) && maxLeaf >= 0xB)
        {
            int smtMaskWidth = 0;
            int coreMaskWidth = 0;

            for (unsigned int i = 0; i < 10; i++)
            {
                unsigned int eaxB = 0, ebxB = 0, ecxB = 0, edxB = 0;
                __asm
                {
                    mov eax, 0x0B
                    mov ecx, i
                    cpuid
                    mov eaxB, eax
                    mov ebxB, ebx
                    mov ecxB, ecx
                    mov edxB, edx
                }

                unsigned int levelType = (ecxB >> 8) & 0xFF;
                if (levelType == 0) break;

                unsigned int shiftWidth = eaxB & 0x1F;
                unsigned int numLogicalThisLevel = ebxB & 0xFFFF;

                if (levelType == 1)
                {
                    smtMaskWidth = shiftWidth;
                    smtWidth = numLogicalThisLevel;
                }
                else if (levelType == 2)
                {
                    coreMaskWidth = shiftWidth;
                    totalLogical = numLogicalThisLevel;

                    if (smtWidth > 0)
                    {
                        coreCount = totalLogical / smtWidth;
                    }
                    else
                    {
                        coreCount = 1 << (coreMaskWidth - smtMaskWidth);
                    }
                }
            }

            if (totalLogical > 0)
            {
                info->logicalThreads = totalLogical;
            }
            if (coreCount > 0)
            {
                info->physicalCores = coreCount;
            }
        }

        // Metoda 3: Fallback CPUID(4)
        if (info->physicalCores <= 0 && maxLeaf >= 0x04)
        {
            unsigned int maxCoresForCache = 0;
            unsigned int maxCacheLevel = 0;

            for (unsigned int i = 0; i < 10; i++)
            {
                unsigned int eax4 = 0, ebx4 = 0, ecx4 = 0, edx4 = 0;
                __asm
                {
                    mov eax, 0x04
                    mov ecx, i
                    cpuid
                    mov eax4, eax
                    mov ebx4, ebx
                    mov ecx4, ecx
                    mov edx4, edx
                }

                unsigned int cacheType = eax4 & 0x1F;
                if (cacheType == 0) break;

                unsigned int cacheLevel = (eax4 >> 5) & 0x7;
                unsigned int maxThreadsSharingCache = ((eax4 >> 14) & 0xFFF) + 1;
                unsigned int maxCoresSharingCache = ((eax4 >> 26) & 0x3F) + 1;

                if (cacheLevel >= maxCacheLevel)
                {
                    maxCacheLevel = cacheLevel;
                    maxCoresForCache = maxCoresSharingCache;

                    if (smtWidth == 0 && maxThreadsSharingCache > maxCoresSharingCache)
                    {
                        smtWidth = maxThreadsSharingCache / maxCoresSharingCache;
                    }
                }
            }

            if (maxCoresForCache > 0)
            {
                info->physicalCores = maxCoresForCache;
            }
        }

        // Metoa 4
        if (info->physicalCores <= 0)
        {
            if (smtWidth > 0)
            {
                info->physicalCores = info->logicalThreads / smtWidth;
            }
            else if (hasHT && info->logicalThreads > 1)
            {
                info->physicalCores = info->logicalThreads / 2;
            }
            else
            {
                info->physicalCores = info->logicalThreads;
            }
        }
    }
    // daca e altceva numarul de core-uri ramane -1

    // 4. Dimensiuni cache
    // Incercam 0x04 prima data
    // Daca nu merge incercam0x80000005 / 0x80000006.
    int l1dKB = -1;
    int l1iKB = -1;
    int l2KB = -1;
    int l3KB = -1;

    // CPUID leaf 0x04 detectam diferitele niveluri de cache
    for (unsigned int subleaf = 0; subleaf < 32; subleaf++)
    {
        unsigned int eax4 = 0, ebx4 = 0, ecx4 = 0, edx4 = 0;

        __asm
        {
            mov eax, 0x04
            mov ecx, subleaf
            cpuid
            mov eax4, eax
            mov ebx4, ebx
            mov ecx4, ecx
            mov edx4, edx
        }

        unsigned int cacheType = (eax4 & 0x1F);
        if (cacheType == 0)
            break;

        unsigned int cacheLevel = (eax4 >> 5) & 0x7;     // 1=L1,2=L2,3=L3...

        unsigned int lineSize = (ebx4 & 0xFFF) + 1;
        unsigned int partitions = ((ebx4 >> 12) & 0x3FF) + 1;
        unsigned int ways = ((ebx4 >> 22) & 0x3FF) + 1;
        unsigned int sets = ecx4 + 1;

        unsigned int sizeBytes = lineSize * partitions * ways * sets;
        int sizeKB = (int)(sizeBytes / 1024);

        // cacheType: 1=data, 2=instruction, 3=unified
        if (cacheLevel == 1)
        {
            if (cacheType == 1) // data 
            {
                l1dKB = sizeKB;
            }
            else if (cacheType == 2) //instruction
            {
                l1iKB = sizeKB;
            }
            else if (cacheType == 3) // nu e def
            {
                if (l1dKB < 0) l1dKB = sizeKB;
                if (l1iKB < 0) l1iKB = sizeKB;
            }
        }
        else if (cacheLevel == 2)
        {
            l2KB = sizeKB;
        }
        else if (cacheLevel == 3)
        {
            l3KB = sizeKB;
        }
    }

    // AMD: daca nu a mers pana acum incercam valorile extinse
    unsigned int maxExt = 0;
    __asm
    {
        mov eax, 0x80000000
        cpuid
        mov maxExt, eax
    }

    if (maxExt >= 0x80000005)
    {
        unsigned int eax5 = 0, ebx5 = 0, ecx5 = 0, edx5 = 0;
        __asm
        {
            mov eax, 0x80000005
            cpuid
            mov eax5, eax
            mov ebx5, ebx
            mov ecx5, ecx
            mov edx5, edx
        }
        // AMD codificari:
        // ECX[31:24] = L1 data in KB
        // EDX[31:24] = L1 instruction in KB
        int amdL1D = (ecx5 >> 24) & 0xFF;
        int amdL1I = (edx5 >> 24) & 0xFF;

        if (l1dKB < 0 && amdL1D != 0) l1dKB = amdL1D;
        if (l1iKB < 0 && amdL1I != 0) l1iKB = amdL1I;
    }

    if (maxExt >= 0x80000006)
    {
        unsigned int eax6 = 0, ebx6 = 0, ecx6 = 0, edx6 = 0;
        __asm
        {
            mov eax, 0x80000006
            cpuid
            mov eax6, eax
            mov ebx6, ebx
            mov ecx6, ecx
            mov edx6, edx
        }
        // AMD encodes:
        // ECX[31:16] = L2 in KB
        // EDX[31:18] = L3 in 512KB 
        int amdL2 = (ecx6 >> 16) & 0xFFFF;
        int amdL3 = ((edx6 >> 18) & 0x3FFF) * 512;

        if (l2KB < 0 && amdL2 != 0) l2KB = amdL2;
        if (l3KB < 0 && amdL3 != 0) l3KB = amdL3;
    }

    info->l1DataKB = l1dKB;
    info->l1InstrKB = l1iKB;
    info->l2KB = l2KB;
    info->l3KB = l3KB;

}


// MONTE CARLO


// Input data in thread
typedef struct ThreadArgs
{
    unsigned int iterations;   // cate iteratii face tredul 
    unsigned int seed;         // seed pentru pseudo rando number generator (PRNG)
    unsigned int hits;         // output: cate puncte sunt in cerc
    HANDLE startEvent;         // toate thread-urile asteapta dupa acelasi startEvent
    HANDLE doneEvent;          // fiecare thread semnaleaza doneEvent-ul propriu cand e gata
} ThreadArgs;

static void MonteCarloDoWork(ThreadArgs* arg)
{
    unsigned int localHits = 0;
    unsigned int seed = arg->seed;

    for (unsigned int i = 0; i < arg->iterations; i++)
    {
        // LCG PRNG
        seed = 1664525u * seed + 1013904223u;
        unsigned int rx = seed;
        seed = 1664525u * seed + 1013904223u;
        unsigned int ry = seed;

        float fx = (rx / 4294967296.0f);
        float fy = (ry / 4294967296.0f);

        float d2 = fx * fx + fy * fy;
        if (d2 <= 1.0f) {
            localHits++;
        }
    }

    arg->hits = localHits;
    arg->seed = seed;
}

// Pornirea thread-urilor: toata lumea asteapta dupa start, face calcule, semnalizeaza ca a terminat
DWORD WINAPI WorkerProc(LPVOID lpParam)
{
    ThreadArgs* args = (ThreadArgs*)lpParam;

    // wait dupa main
    WaitForSingleObject(args->startEvent, INFINITE);

    // doar matematica
    MonteCarloDoWork(args);

    // notifica main ca a terminat
    SetEvent(args->doneEvent);

    return 0;
}

__int64 BenchmarkMonteCarloBatch(unsigned int numThreads) //Cod preluat si adaptat din laboratorul 1 si 2 - Structura sistemelor de calcul, Universitatea Tehnica din Cluj-Napoca
{
    if (numThreads == 0)
        numThreads = 1;
    if (numThreads > MAX_THREADS)
        numThreads = MAX_THREADS;

    //setup (nu masuram timpul inca)

    //cream un singur semnal de start pentru toate thread-urile
    HANDLE startEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

    // fiecare thread primest propriul doneEvent dupa care asteptam
    HANDLE doneEvents[MAX_THREADS] = { 0 };
    HANDLE threadHandles[MAX_THREADS] = { 0 };
    ThreadArgs args[MAX_THREADS] = { 0 };

    unsigned int baseIter = TOTAL_ITERATIONS / numThreads;
    unsigned int leftover = TOTAL_ITERATIONS % numThreads;

    for (unsigned int i = 0; i < numThreads; i++)
    {
        unsigned int thisIter = baseIter;
        if (i == numThreads - 1)
        {
            thisIter += leftover; // ultimul thread primeste restul
        }

        args[i].iterations = thisIter;
        args[i].seed = (unsigned int)(clock() ^ (i * 2654435761u)); // generare seed pentru fiecare thread dupa care face numerele random
        args[i].hits = 0;
        args[i].startEvent = startEvent;
        args[i].doneEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
        doneEvents[i] = args[i].doneEvent;

        threadHandles[i] = CreateThread(NULL, 0, WorkerProc, &args[i], 0, NULL);
    }

    // variabile pentru temporizare
    unsigned cycles_high1 = 0, cycles_low1 = 0;
    unsigned cycles_high2 = 0, cycles_low2 = 0;
    unsigned cpuid_time = 0;
    unsigned __int64 temp_cycles1 = 0, temp_cycles2 = 0;
    __int64 total_cycles = 0;

    // pregaitm cpuid, primele 3 dati dureaza mai mult
    __asm
    {
        pushad
        cpuid
        rdtsc
        mov cycles_high1, edx
        mov cycles_low1, eax
        popad
        pushad
        cpuid
        rdtsc
        popad

        pushad
        cpuid
        rdtsc
        mov cycles_high1, edx
        mov cycles_low1, eax
        popad
        pushad
        cpuid
        rdtsc
        popad

        pushad
        cpuid
        rdtsc
        mov cycles_high1, edx
        mov cycles_low1, eax
        popad
        pushad
        cpuid
        rdtsc
        sub eax, cycles_low1
        mov cpuid_time, eax
        popad
    }

    // resetam valorile
    cycles_high1 = 0;
    cycles_low1 = 0;

    // ZONA TEMPORIZATA
    __asm //cicluri inainte
    {
        pushad
        cpuid
        rdtsc
        mov cycles_high1, edx
        mov cycles_low1, eax
        popad
    }

    // Da drumul la toate thread-urile
    SetEvent(startEvent);

    // Asteapta pana ce toate thread-urile termina
    WaitForMultipleObjects(numThreads, doneEvents, TRUE, INFINITE);

    __asm //cicluri dupa
    {
        pushad
        cpuid
        rdtsc
        mov cycles_high2, edx
        mov cycles_low2, eax
        popad
    }

    temp_cycles1 = ((unsigned __int64)cycles_high1 << 32) | cycles_low1;
    temp_cycles2 = ((unsigned __int64)cycles_high2 << 32) | cycles_low2;
    total_cycles = temp_cycles2 - temp_cycles1 - cpuid_time;

    // cleanup
    for (unsigned int i = 0; i < numThreads; i++)
    {
        CloseHandle(threadHandles[i]);
        CloseHandle(doneEvents[i]);
    }
    CloseHandle(startEvent);
    return total_cycles;
}

// MATRIX MULTIPLICTAION

// Input data in thread
typedef struct ThreadArgsMatrix
{
    float* A;
    float* B;
    float* C;
    unsigned int num;

    unsigned int startRow; // inclusiv randul acesta
    unsigned int rowCount; // cate randuri din C calculeaza acest thread

    HANDLE startEvent;
    HANDLE doneEvent;
}ThreadArgsMatrix;

static void MatrixMulDoWork(ThreadArgsMatrix* arg)
{
    const unsigned int num = arg->num;
    const unsigned int iStart = arg->startRow;
    const unsigned int iEnd = iStart + arg->rowCount;

    if (iStart > num || iEnd > num || iStart > iEnd) 
        return;

    float* A = arg->A;
    float* B = arg->B;
    float* C = arg->C;

    for (unsigned int i = iStart; i < iEnd; i++) {
        float* Ci = &C[i * num];
        const float* Ai = &A[i * num];
        for (unsigned int j = 0; j < num; j++) {
            float sum = 0.0f;
            for (unsigned int k = 0; k < num; k++) {
                sum += Ai[k] * B[k * num + j];
            }
            Ci[j] = sum;
        }
    }
}


DWORD WINAPI MatrixWorkerProc(LPVOID lpParam)
{
    ThreadArgsMatrix* args = (ThreadArgsMatrix*)lpParam;
    WaitForSingleObject(args->startEvent, INFINITE);
    MatrixMulDoWork(args);
    SetEvent(args->doneEvent);
    return 0;
}

__int64 BenchmarkMatrixBatch(unsigned int numThreads)
{
    if (numThreads == 0)
        numThreads = 1;
    if (numThreads > MAX_THREADS) numThreads = MAX_THREADS;

    //Alocare si initializare matrici
    size_t sizeMat = (size_t)MM_NR * MM_NR * sizeof(float); // toate matricile sunt patrate

    float* A = (float*)_aligned_malloc(sizeMat + 4, 64);
    float* B = (float*)_aligned_malloc(sizeMat + 4, 64);
    float* C = (float*)_aligned_malloc(sizeMat + 4, 64);
    if (!A || !B || !C)
    {
        printf("Matrix alloc failed \n");
        if (A)
            _aligned_free(A);
        if (B) _aligned_free(B);
        if (C) _aligned_free(C);
        return -1;
    }

    //Initializare cu valori
    for (unsigned int i = 0; i < MM_NR; i++)
        for (unsigned int k = 0; k < MM_NR; k++)
            A[i * MM_NR + k] = (float)((i + k) % 13) * 0.1f;
    for (unsigned int i = 0; i < MM_NR; i++)
        for (unsigned int j = 0; j < MM_NR; j++)
            B[i * MM_NR + j] = (float)((i + 2 * j) % 17) * 0.05f;
    memset(C, 0, sizeMat);

    // Partea de sincronizare
    HANDLE startEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
    HANDLE doneEvent[MAX_THREADS] = { 0 };
    HANDLE threadHandles[MAX_THREADS] = { 0 };
    ThreadArgsMatrix args[MAX_THREADS] = { 0 };

    // Impartire workload
    unsigned int baseRows = MM_NR / numThreads;
    unsigned int leftover = MM_NR % numThreads;
    for (unsigned int i = 0; i < numThreads; i++)
    {
        unsigned int rows = baseRows + (i < leftover ? 1u : 0u);
        unsigned int start = i * baseRows + (i < leftover ? i : leftover);

        args[i].A = A; args[i].B = B; args[i].C = C;
        args[i].num = MM_NR;
        args[i].startRow = start;
        args[i].rowCount = rows;
        args[i].startEvent = startEvent;
        args[i].doneEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
        doneEvent[i] = args[i].doneEvent;

        threadHandles[i] = CreateThread(NULL, 0, MatrixWorkerProc, &args[i], 0, NULL);


    }
    //Temporizare, aceeasi
    unsigned cycles_high1 = 0, cycles_low1 = 0, cycles_high2 = 0, cycles_low2 = 0, cpuid_time = 0;
    unsigned __int64 t1 = 0, t2 = 0; __int64 total_cycles = 0;

    __asm
    {
        pushad
        cpuid
        rdtsc
        mov cycles_high1, edx
        mov cycles_low1, eax
        popad
        pushad
        cpuid
        rdtsc
        popad

        pushad
        cpuid
        rdtsc
        mov cycles_high1, edx
        mov cycles_low1, eax
        popad
        pushad
        cpuid
        rdtsc
        popad

        pushad
        cpuid
        rdtsc
        mov cycles_high1, edx
        mov cycles_low1, eax
        popad
        pushad
        cpuid
        rdtsc
        sub eax, cycles_low1
        mov cpuid_time, eax
        popad
    }

    // resetam valorile
    cycles_high1 = 0;
    cycles_low1 = 0;

    // ZONA TEMPORIZATA
    __asm //cicluri inainte
    {
        pushad
        cpuid
        rdtsc
        mov cycles_high1, edx
        mov cycles_low1, eax
        popad
    }

    // Da drumul la toate thread-urile
    SetEvent(startEvent);

    // Asteapta pana ce toate thread-urile termina
    WaitForMultipleObjects(numThreads, doneEvent, TRUE, INFINITE);

    __asm //cicluri dupa
    {
        pushad
        cpuid
        rdtsc
        mov cycles_high2, edx
        mov cycles_low2, eax
        popad
    }

    t1 = ((unsigned __int64)cycles_high1 << 32) | cycles_low1;
    t2 = ((unsigned __int64)cycles_high2 << 32) | cycles_low2;
    total_cycles = (__int64)(t2 - t1) - (int)cpuid_time;

    // Cleanup
    for (unsigned int i = 0; i < numThreads; ++i) {
        CloseHandle(threadHandles[i]);
        CloseHandle(doneEvent[i]);
    }
    CloseHandle(startEvent);

    _aligned_free(A);
    _aligned_free(B);
    _aligned_free(C);

    return total_cycles;
}

// Bench-mark - probabil folosit dupa pentru a transmite la c#
typedef struct BenchmarkResult {
    double avg_cycles;
    double avg_seconds;
    long long last_cycles;
    double last_seconds;
} BenchmarkResult;


// FFT SECTION

typedef struct ThreadArgsFft
{
    float* real;
    float* imag;
    unsigned int nr;
    unsigned int startBlock;
    unsigned int blockCount;

    const float* twRe;
    const float* twIm;

    HANDLE startEvent;
    HANDLE doneEvent;
}ThreadArgsFFT;

//helpers
static unsigned int ReverseBits(unsigned int x, unsigned int bits)
{
    unsigned int r = 0;
    for (unsigned int i = 0; i < bits; i++)
    {
        r = (r << 1) | (x & 1u);
        x >>= 1;
    }
    return r;
}

static void BitReverseReorder(float* re, float* im, unsigned int nr)
{
    unsigned int bits = 0;
    while ((1u << bits) < nr)
        bits++;
    for (unsigned int i = 0; i < nr; i++)
    {
        unsigned int j = ReverseBits(i, bits);
        if (j > i)
        {
            float aux = re[i];
            re[i] = re[j];
            re[j] = aux;
            float auxi = im[i];
            im[i] = im[j];
            im[j] = auxi;
        }
    }

}

static void PrecomputeTwiddles(unsigned int nr, float* twRe, float* twIm)
{
    for (unsigned int k = 0; k < nr / 2; k++)
    {
        float angle = -2.0f * 3.14159265358979323846f * (float)k / (float)nr;
        twRe[k] = cosf(angle);
        twIm[k] = sinf(angle);
    }
}

static void FFT_Block(float* re, float* im, unsigned int nr, const float* twRe, const float* twIm)
{
    BitReverseReorder(re, im, nr);
    for (unsigned int m = 2; m <= nr; m <<= 1)
    {
        unsigned int half = m >> 1;
        unsigned int step = nr / m; 

        for (unsigned int k = 0; k < half; k++)
        {
            unsigned int tIndex = k * step;
            float wr = twRe[tIndex];
            float wi = twIm[tIndex];

            for (unsigned int j = k; j < nr; j += m)
            {
                unsigned int i0 = j;
                unsigned int i1 = j + half;

                float xr0 = re[i0], xi0 = im[i0];
                float xr1 = re[i1], xi1 = im[i1];

                float tr = wr * xr1 - wi * xi1;
                float ti = wr * xi1 + wi * xr1;

                re[i0] = xr0 + tr;
                im[i0] = xi0 + ti;
                re[i1] = xr0 - tr;
                im[i1] = xi0 - ti;
            }
        }
    }
}

static void FFTDoWork(ThreadArgsFFT* arg)
{
    const unsigned int nr = arg->nr;
    const unsigned int start = arg->startBlock;
    const unsigned int count = arg->blockCount;

    for (unsigned int b = 0; b < count; b++)
    {
        unsigned int base = (start + b) * nr;
        FFT_Block(arg->real + base, arg->imag + base, nr, arg->twRe, arg->twIm);
    }
}

DWORD WINAPI FFTWorkerProc(LPVOID lpParam)
{
    ThreadArgsFFT* args = (ThreadArgsFFT*)lpParam;
    WaitForSingleObject(args->startEvent, INFINITE);
    FFTDoWork(args);
    SetEvent(args->doneEvent);
    return 0;
}

__int64 BenchmarkFFTBatch(unsigned int numThreads)
{
    if (numThreads == 0)
        numThreads = 1;
    if (numThreads > MAX_THREADS)
        numThreads = MAX_THREADS;

    const unsigned int nr = FFT_N;
    const unsigned int BLOCKS = FFT_BLOCKS;

    size_t bufCount = (size_t)nr * BLOCKS;
    size_t sizeBuf = bufCount * sizeof(float);
    size_t sizeTw = (size_t)(nr / 2) * sizeof(float);
    float* real = (float*)_aligned_malloc(sizeBuf, 64);
    float* imag = (float*)_aligned_malloc(sizeBuf, 64);
    float* twRe = (float*)_aligned_malloc(sizeTw, 64);
    float* twIm = (float*)_aligned_malloc(sizeTw, 64);
    if (!real || !imag || !twRe || !twIm)
    {
        if (real) _aligned_free(real);
        if (imag) _aligned_free(imag);
        if (twRe) _aligned_free(twRe);
        if (twIm) _aligned_free(twIm);
        return -1;
    }
    for (size_t i = 0; i < bufCount; ++i)
    {
        unsigned int r = (unsigned int)i * 1664525u + 1013904223u;
        real[i] = (float)((int)(r & 0xFFFF) - 32768) / 32768.0f;
        imag[i] = 0.0f;
    }
    PrecomputeTwiddles(nr, twRe, twIm);

    HANDLE startEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
    HANDLE doneEvents[MAX_THREADS] = { 0 };
    HANDLE threadHandles[MAX_THREADS] = { 0 };
    ThreadArgsFFT args[MAX_THREADS] = { 0 };

    // split blocks la threads
    unsigned int blocksPer = BLOCKS / numThreads;
    unsigned int extra = BLOCKS % numThreads;
    unsigned int cursor = 0;

    for (unsigned int i = 0; i < numThreads; ++i) {
        unsigned int cnt = blocksPer + (i < extra ? 1u : 0u);

        args[i].real = real; args[i].imag = imag;
        args[i].nr = nr;
        args[i].startBlock = cursor;
        args[i].blockCount = cnt;
        args[i].twRe = twRe; args[i].twIm = twIm;
        args[i].startEvent = startEvent;
        args[i].doneEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

        doneEvents[i] = args[i].doneEvent;

        threadHandles[i] = CreateThread(NULL, 0, FFTWorkerProc, &args[i], 0, NULL);
        if (!threadHandles[i]) {
            SetEvent(doneEvents[i]); 
        }
        cursor += cnt;
    }

    unsigned cycles_high1 = 0, cycles_low1 = 0;
    unsigned cycles_high2 = 0, cycles_low2 = 0;
    unsigned cpuid_time = 0;
    unsigned __int64 t1 = 0, t2 = 0;
    __int64 total_cycles = 0;

    __asm
    {
        pushad
        cpuid
        rdtsc
        mov cycles_high1, edx
        mov cycles_low1, eax
        popad
        pushad
        cpuid
        rdtsc
        popad

        pushad
        cpuid
        rdtsc
        mov cycles_high1, edx
        mov cycles_low1, eax
        popad
        pushad
        cpuid
        rdtsc
        popad

        pushad
        cpuid
        rdtsc
        mov cycles_high1, edx
        mov cycles_low1, eax
        popad
        pushad
        cpuid
        rdtsc
        sub eax, cycles_low1
        mov cpuid_time, eax
        popad
    }

    // resetam valorile
    cycles_high1 = 0;
    cycles_low1 = 0;

    // ZONA TEMPORIZATA
    __asm //cicluri inainte
    {
        pushad
        cpuid
        rdtsc
        mov cycles_high1, edx
        mov cycles_low1, eax
        popad
    }

    // Da drumul la toate thread-urile
    SetEvent(startEvent);

    // Asteapta pana ce toate thread-urile termina
    WaitForMultipleObjects(numThreads, doneEvents, TRUE, INFINITE);

    __asm //cicluri dupa
    {
        pushad
        cpuid
        rdtsc
        mov cycles_high2, edx
        mov cycles_low2, eax
        popad
    }

    t1 = ((unsigned __int64)cycles_high1 << 32) | cycles_low1;
    t2 = ((unsigned __int64)cycles_high2 << 32) | cycles_low2;
    total_cycles = (__int64)(t2 - t1) - (int)cpuid_time;

    double seconds = (double)total_cycles / (double)FREQUENCY;

    // cleanup
    for (unsigned int i = 0; i < numThreads; ++i) {
        if (threadHandles[i]) CloseHandle(threadHandles[i]);
        CloseHandle(doneEvents[i]);
    }
    CloseHandle(startEvent);

    _aligned_free(real); _aligned_free(imag);
    _aligned_free(twRe); _aligned_free(twIm);

    return total_cycles;
}

