# CPUBench — Multicore CPU Benchmark (x86)

Desktop benchmark for evaluating multicore CPU performance using configurable workloads and thread counts.

The project consists of:
- **C++ native DLL**: CPU detection + benchmark kernels (CPUID/RDTSC, Monte Carlo, Matrix Multiplication, FFT)
- **C# WinForms UI (x86)**: configuration, running benchmarks, displaying results (text + chart), exporting results

> ⚠️ **x86 only:** The native part uses **inline assembly**, so the DLL must be built as **x86 (32-bit)**.  
> The WinForms app is also set to **x86** to match the DLL. This is the only requirement for the project to work.

---

## Features

- CPU info detection via **CPUID** (vendor/brand, logical threads, physical cores (where possible), cache sizes)
- Timing based on **RDTSC** with **CPUID serialization** (cycles-based measurement)
- Benchmarks:
  - **Monte Carlo π estimation** (highly parallel, low synchronization)
  - **Matrix multiplication** (compute + memory heavy)
  - **FFT (radix-2 / Cooley–Tukey)** (complex arithmetic + non-uniform memory access)
- Multi-threaded execution with synchronized start (start event)
- WinForms UI:
  - choose benchmark + number of threads
  - **Full Run** (multiple thread counts) and **Custom Run**
  - results output + chart
  - export to `.txt` / `.csv`

---

## Requirements

- Windows
- Visual Studio (recommended)
- Build target: **x86** (required for both DLL + WinForms app)

---

## Build & Run (Visual Studio)

1. Open the solution (`.sln`) in Visual Studio.
2. In the toolbar:
   - Configuration: **Release** (or Debug)
   - Platform: **x86**
3. Build the solution.

### Running

The WinForms app loads the native DLL at runtime.  
Make sure the DLL is next to the application executable in the output folder:

- `CpuBench.app.exe`
- `CpuBenchDll.dll`

If you see a “DLL not found” / `DllNotFoundException`, copy the DLL into the same directory as the `.exe` (or ensure your build output settings do that automatically).

---

## Notes on measurement

- Uses `RDTSC` for cycle-accurate timing.
- Uses `CPUID` to serialize execution around `RDTSC` reads.
- Results are reported in **CPU cycles** (lower is better).

---

## Development note

A standalone native **C++ application project** was used during initial development/testing.  
The final architecture uses the **native DLL + C# WinForms UI**.

---

## Limitations

- **x64 is not supported** due to inline assembly usage in the native component.
- CPU topology detection can vary across vendors/architectures (different CPUID leaves).

---

## Documentation

A detailed project documentation is available in Romanian: [`CpuBench_Documentation.pdf`](CpuBench_Documenttaion.pdf)
