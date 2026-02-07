using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace CPUBench.app
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            buttonFullRun.Click += buttonFullRun_Click;
            buttonCustomRun.Click += buttonCustomRun_Click;

            buttonExportResults.Click += buttonExportResults_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // CPU info
                var info = new CpuInfo();
                Native.GetCpuInfo(ref info);

                lblBrand.Text = $"Brand: {info.vendorString}";
                labelCPU.Text = $"CPU: {info.brandString}";
                labelCores.Text = $"Cores: {info.physicalCores}";
                labelThreads.Text = $"Threads: {info.logicalThreads}";
                labelL1D.Text = $"L1D: {info.l1DataKB} KB";
                labelL1I.Text = $"L1I: {info.l1InstrKB} KB";
                labelL2.Text = $"L2: {info.l2KB} KB";
                labelL3.Text = $"L3: {info.l3KB} KB";

                // ComboBox1 - alg
                comboBox1.Items.Clear();
                comboBox1.Items.Add("Mote Carlo");
                comboBox1.Items.Add("Matrix Multiplication");
                comboBox1.Items.Add("FFT");
                comboBox1.SelectedIndex = 0;

                // ComboBox2 - number of threads
                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(new object[] { 1, 2, 4, 8, 16, 32, 64 });
                comboBox2.SelectedIndex = 0;

                textBoxResults.ReadOnly = true;
                textBoxResults.Multiline = true;
                textBoxResults.ScrollBars = ScrollBars.Vertical;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Getting CPU Info");
            }

        }

        private enum BenchmarkAlg
        {
            MonteCarlo,
            Matrix,
            FFT
        }

        private sealed class GradingProfile
        {
            public double B1, B8, B16, Brest, Bcycle;
            public long[] GradeMaxCP = new long[11];
        }

        private static readonly Dictionary<BenchmarkAlg, GradingProfile> GradingProfiles = new Dictionary<BenchmarkAlg, GradingProfile>
        {
            [BenchmarkAlg.MonteCarlo] = new GradingProfile
            {
                B1 = 4209558769d,
                B8 = 689617692d,
                B16 = 481031573d,
                Brest = 1145684140.25d,
                Bcycle = 1422243361.35d,
                GradeMaxCP = new long[11]
            {
                0,
                2844486723, // 1
                2585897021, // 2
                2370405603, // 3
                2188066710, // 4
                2031776231, // 5
                1896324482, // 6
                1777804202, // 7
                1673227484, // 8
                1580270402, // 9
                1497098276  // 10
            }
            },

            [BenchmarkAlg.Matrix] = new GradingProfile
            {
                B1 = 1905285141d,
                B8 = 313536492d,
                B16 = 223060890d,
                Brest = 529422991.25d,
                Bcycle = 647920841.05d,
                GradeMaxCP = new long[11]
            {
                0,
                1295841683, // 1
                1178037893, // 2
                1079868069, // 3
                996801294,  // 4
                925601202,  // 5
                863894455,  // 6
                809901052,  // 7
                762259813,  // 8
                719912046,  // 9
                682021938   // 10
            }
            },

            [BenchmarkAlg.FFT] = new GradingProfile
            {
                B1 = 2834101591d,
                B8 = 512386356d,
                B16 = 402521984d,
                Brest = 862659248.25d,
                Bcycle = 1013824669.85d,
                GradeMaxCP = new long[11]
            {
                0,
                2027649340, // 1
                1843317582, // 2
                1689707784, // 3
                1559730262, // 4
                1448320957, // 5
                1351766227, // 6
                1267280838, // 7
                1192734906, // 8
                1126471856, // 9
                1067183864  // 10
            }
            }
        };

        private static bool TryGetCycles(List<(int Threads, long Cycles)> list, int threads, out long cycles)
        {
            foreach (var p in list)
            {
                if (p.Threads == threads)
                {
                    cycles = p.Cycles;
                    return true;
                }
            }
            cycles = 0;
            return false;
        }

        private bool TryComputeWeightedCyclesAndGrade(
            BenchmarkAlg alg,
            List<(int Threads, long Cycles)> results,
            out long weightedCyclesCP,
            out int grade)
        {
            weightedCyclesCP = 0;
            grade = 1;

            if (!GradingProfiles.TryGetValue(alg, out var gp))
                return false;

            if (!TryGetCycles(results, 1, out var C1) ||
                !TryGetCycles(results, 2, out var C2) ||
                !TryGetCycles(results, 4, out var C4) ||
                !TryGetCycles(results, 8, out var C8) ||
                !TryGetCycles(results, 16, out var C16) ||
                !TryGetCycles(results, 32, out var C32) ||
                !TryGetCycles(results, 64, out var C64))
            {
                return false;
            }

            double Crest = (C2 + C4 + C32 + C64) / 4.0;

            double score =
                0.20 * (gp.B1 / C1) +
                0.30 * (gp.B8 / C8) +
                0.30 * (gp.B16 / C16) +
                0.20 * (gp.Brest / Crest);

            if (score <= 0.0 || double.IsNaN(score) || double.IsInfinity(score))
                return false;

            double cp = gp.Bcycle / score;
            weightedCyclesCP = (long)Math.Round(cp);

            for (int g = 10; g >= 1; g--)
            {
                if (weightedCyclesCP <= gp.GradeMaxCP[g])
                {
                    grade = g;
                    return true;
                }
            }

            grade = 1;
            return true;
        }

        private class FullRunResult
        {
            public string Text;
            public Dictionary<BenchmarkAlg, List<(int Threads, long Cycles)>> Data;
        }

        private BenchmarkAlg GetSelectedAlgorithm()
        {
            switch (comboBox1.SelectedIndex )
            {
                case 0: return BenchmarkAlg.MonteCarlo;
                case 1: return BenchmarkAlg.Matrix;
                case 2: return BenchmarkAlg.FFT;
                default: return BenchmarkAlg.MonteCarlo;
            }
        }

        private string GetAlgDisplayName(BenchmarkAlg alg)
        {
            switch (alg)
            {
                case BenchmarkAlg.MonteCarlo:
                    return "Monte Carlo";
                case BenchmarkAlg.Matrix:
                    return "Matrix multiplication";
                case BenchmarkAlg.FFT:
                    return "FFT";
                default: return alg.ToString();
            }

        }

        private uint GetSelectedThreads()
        {
            if (comboBox2.SelectedItem is int t)
                return (uint)t;

            if (uint.TryParse(comboBox2.Text, out var parsed))
                return parsed;
            return 1;
        }

        private long RunSingleBenchmark(BenchmarkAlg alg, uint threads)
        {
            switch (alg)
            {
                case BenchmarkAlg.MonteCarlo:
                    return Native.BenchmarkMonteCarloBatch(threads);
                case BenchmarkAlg.Matrix:
                    return Native.BenchmarkMatrixBatch(threads);
                case BenchmarkAlg.FFT:
                    return Native.BenchmarkFFTBatch(threads);
                default:
                    return 0;
            }
        }

        // Animatie
        private readonly Timer runTimer = new Timer();
        private int runDotCount = 0;
        private bool isRunningbench = false;

        private void InitRunTimer()
        {
            runTimer.Interval = 300;
            runTimer.Tick += (s, e) =>
            {
                if (!isRunningbench)
                {
                    runTimer.Stop();
                    buttonFullRun.Text = "Full Run";
                    return;
                }
                runDotCount = (runDotCount + 1) % 4;
                string dots = new string('.', runDotCount);
                buttonFullRun.Text = "Running" + dots;
            };
        }

        // Full Run
        private async void buttonFullRun_Click(object sender, EventArgs e)
        {
            if (isRunningbench)
                return;

            isRunningbench = true;
            buttonFullRun.Enabled = false;
            buttonCustomRun.Enabled = false;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;

            runDotCount = 0;
            InitRunTimer();
            runTimer.Start();

            textBoxResults.Clear();
            textBoxResults.AppendText("Starting full run... \r\n\r\n");

            int[] threadCounts = { 1, 2, 4, 8, 16, 32, 64 };
            const int iterations = 5;
            try
            {
                var fullResult = await Task.Run(() =>
                {
                    var sb = new StringBuilder();
                    var data = new Dictionary<BenchmarkAlg, List<(int Threads, long Cycles)>>();

                    var algs = new[]
                    {
                        BenchmarkAlg.MonteCarlo,
                        BenchmarkAlg.Matrix,
                        BenchmarkAlg.FFT
                    };


                    foreach (var alg in algs)
                    {
                        sb.AppendLine($"---{alg}---");

                        var list = new List<(int Threads, long Cycles)>();

                        foreach (int t in threadCounts)
                        {
                            double accumulator = 0.0;

                            for (int i = 0; i < iterations; i++)
                            {
                                long cycles = RunSingleBenchmark(alg, (uint)t);
                                accumulator += cycles / (double)iterations;                            
                            }

                            long avgCycles = (long)Math.Round(accumulator);

                            list.Add((t, avgCycles));
                            sb.AppendLine($"Threads {t,2}: {avgCycles} cycles");
                        }
                        data[alg] = list;

                        //  Grade output 
                        if (TryComputeWeightedCyclesAndGrade(alg, list, out long cp, out int grade))
                        {
                            sb.AppendLine($"Weighted cycles: {cp}  =>  Grade: {grade}/10");
                        }
                        else
                        {
                            sb.AppendLine("Grade: N/A (missing required thread results)");
                        }

                        sb.AppendLine();
                    }

                    return new FullRunResult
                    {
                        Text = sb.ToString(),
                        Data = data
                    };
                });

                textBoxResults.AppendText(fullResult.Text);
                UpdateChartWithFullRun(fullResult.Data);
            }
            catch (Exception ex)
            {
                textBoxResults.AppendText($"Error while running benchmarks: {ex.Message}\r\n");
            }
            finally
            {
                isRunningbench = false;
                buttonFullRun.Enabled = true;
                buttonCustomRun.Enabled = true;
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                buttonFullRun.Text = "Full Run";
            }

        }


        private async void buttonCustomRun_Click(object sender, EventArgs e)
        {
            if (isRunningbench) 
                return;

            var alg = GetSelectedAlgorithm();
            uint threads = GetSelectedThreads();

            isRunningbench = true;
            buttonFullRun.Enabled = false;
            buttonCustomRun.Enabled = false;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;

            textBoxResults.Clear();
            textBoxResults.AppendText($"Running {alg} with {threads} threads...\r\n");

            string dots = "";
            buttonCustomRun.Text = "Running...";

            try
            {
                var result = await Task.Run(() =>
                {
                    double accumulator = 0.0;
                    for (int i = 0; i < 10; i++)
                    {
                        long cycles = RunSingleBenchmark(alg, threads);
                        accumulator += cycles / (double)10;
                    }

                    return (long)Math.Round(accumulator);
                });
                
                textBoxResults.AppendText($"Result: {result} cycles");
            }
            catch (Exception ex)
            {
                textBoxResults.AppendText($"Error in custom run: {ex.Message}\r\n");
            }
            finally
            {
                isRunningbench = false;
                buttonFullRun.Enabled = true;
                buttonCustomRun.Enabled = true;
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                buttonCustomRun.Text = "Run";
            }
        }

        // Realizare grafice
        private void UpdateChartWithFullRun(Dictionary<BenchmarkAlg, List<(int Threads, long Cycles)>> data)
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Legends.Clear();

            // Zona pentru grafic
            var area = new ChartArea("Main");
            area.AxisX.Title = "Threads";
            area.AxisY.Title = "Cycles (lower is better)";
            area.AxisX.Interval = 4;
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisY.MajorGrid.Enabled = true;
            chart1.ChartAreas.Add(area);

            // Legenda
            var legend = new Legend
            {
                Docking = Docking.Top,
                Alignment = StringAlignment.Center
            };
            chart1.Legends.Add(legend);
            
            foreach(var kvp in data)
            {
                var alg = kvp.Key;
                var points = kvp.Value;

                string seriesName = GetAlgDisplayName(alg);

                var series = new Series(seriesName);
                series.ChartType = SeriesChartType.Line;
                series.ChartArea = "Main";
                series.BorderWidth = 2;
                series.MarkerStyle = MarkerStyle.Circle;
                series.MarkerSize = 6;

                foreach(var (threads, cycles) in points)
                {
                    series.Points.AddXY(threads, cycles);
                }

                chart1.Series.Add(series);
            }

            chart1.Invalidate();
        }

        //Export text
        private void buttonExportResults_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxResults.Text))
            {
                MessageBox.Show("No results to export yet.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Export Benchmark Results";
                sfd.Filter = "Text file (*.txt)|*.txt|CSV file (*.csv)|*.csv|All files (*.*)|*.*";
                sfd.DefaultExt = "txt";
                sfd.AddExtension = true;
                sfd.FileName = $"CPUBench_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";

                if (sfd.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    File.WriteAllText(sfd.FileName, textBoxResults.Text, Encoding.UTF8);
                    MessageBox.Show($"Exported successfully:\r\n{sfd.FileName}", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to export:\r\n{ex.Message}", "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBoxResults_TextChanged(object sender, EventArgs e)
        {

        }

        private void labelCustomBenchmark_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
