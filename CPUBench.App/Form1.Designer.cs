namespace CPUBench.app
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelSpecs = new System.Windows.Forms.Panel();
            this.labelL3 = new System.Windows.Forms.Label();
            this.labelL2 = new System.Windows.Forms.Label();
            this.labelL1I = new System.Windows.Forms.Label();
            this.labelL1D = new System.Windows.Forms.Label();
            this.labelThreads = new System.Windows.Forms.Label();
            this.labelCores = new System.Windows.Forms.Label();
            this.labelCPU = new System.Windows.Forms.Label();
            this.lblBrand = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonExportResults = new System.Windows.Forms.Button();
            this.buttonCustomRun = new System.Windows.Forms.Button();
            this.labelCustomBenchmark = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.buttonFullRun = new System.Windows.Forms.Button();
            this.labelBenchmarkType = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.labelBenchmark = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBoxResults = new System.Windows.Forms.TextBox();
            this.panelPlaceHolder = new System.Windows.Forms.Panel();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelSpecs.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelPlaceHolder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panelSpecs, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 2);
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1640, 886);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // panelSpecs
            // 
            this.panelSpecs.Controls.Add(this.labelL3);
            this.panelSpecs.Controls.Add(this.labelL2);
            this.panelSpecs.Controls.Add(this.labelL1I);
            this.panelSpecs.Controls.Add(this.labelL1D);
            this.panelSpecs.Controls.Add(this.labelThreads);
            this.panelSpecs.Controls.Add(this.labelCores);
            this.panelSpecs.Controls.Add(this.labelCPU);
            this.panelSpecs.Controls.Add(this.lblBrand);
            this.panelSpecs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSpecs.Location = new System.Drawing.Point(3, 3);
            this.panelSpecs.Name = "panelSpecs";
            this.panelSpecs.Size = new System.Drawing.Size(1634, 74);
            this.panelSpecs.TabIndex = 0;
            // 
            // labelL3
            // 
            this.labelL3.AutoSize = true;
            this.labelL3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelL3.Location = new System.Drawing.Point(802, 38);
            this.labelL3.Name = "labelL3";
            this.labelL3.Size = new System.Drawing.Size(35, 17);
            this.labelL3.TabIndex = 7;
            this.labelL3.Text = "L3: -";
            // 
            // labelL2
            // 
            this.labelL2.AutoSize = true;
            this.labelL2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelL2.Location = new System.Drawing.Point(802, 10);
            this.labelL2.Name = "labelL2";
            this.labelL2.Size = new System.Drawing.Size(35, 17);
            this.labelL2.TabIndex = 6;
            this.labelL2.Text = "L2: -";
            // 
            // labelL1I
            // 
            this.labelL1I.AutoSize = true;
            this.labelL1I.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelL1I.Location = new System.Drawing.Point(626, 38);
            this.labelL1I.Name = "labelL1I";
            this.labelL1I.Size = new System.Drawing.Size(39, 17);
            this.labelL1I.TabIndex = 5;
            this.labelL1I.Text = "L1I: -";
            // 
            // labelL1D
            // 
            this.labelL1D.AutoSize = true;
            this.labelL1D.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelL1D.Location = new System.Drawing.Point(626, 10);
            this.labelL1D.Name = "labelL1D";
            this.labelL1D.Size = new System.Drawing.Size(49, 17);
            this.labelL1D.TabIndex = 4;
            this.labelL1D.Text = "L1D: - ";
            // 
            // labelThreads
            // 
            this.labelThreads.AutoSize = true;
            this.labelThreads.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelThreads.Location = new System.Drawing.Point(385, 38);
            this.labelThreads.Name = "labelThreads";
            this.labelThreads.Size = new System.Drawing.Size(74, 17);
            this.labelThreads.TabIndex = 3;
            this.labelThreads.Text = "Threads: - ";
            // 
            // labelCores
            // 
            this.labelCores.AutoSize = true;
            this.labelCores.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCores.Location = new System.Drawing.Point(385, 10);
            this.labelCores.Name = "labelCores";
            this.labelCores.Size = new System.Drawing.Size(55, 17);
            this.labelCores.TabIndex = 2;
            this.labelCores.Text = "Cores: -";
            this.labelCores.Click += new System.EventHandler(this.label2_Click);
            // 
            // labelCPU
            // 
            this.labelCPU.AutoSize = true;
            this.labelCPU.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCPU.Location = new System.Drawing.Point(23, 38);
            this.labelCPU.Name = "labelCPU";
            this.labelCPU.Size = new System.Drawing.Size(46, 17);
            this.labelCPU.TabIndex = 1;
            this.labelCPU.Text = "CPU: -";
            this.labelCPU.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // lblBrand
            // 
            this.lblBrand.AutoSize = true;
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBrand.Location = new System.Drawing.Point(23, 10);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(57, 17);
            this.lblBrand.TabIndex = 0;
            this.lblBrand.Text = "Brand: -";
            this.lblBrand.Click += new System.EventHandler(this.label1_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonExportResults);
            this.panel1.Controls.Add(this.buttonCustomRun);
            this.panel1.Controls.Add(this.labelCustomBenchmark);
            this.panel1.Controls.Add(this.comboBox2);
            this.panel1.Controls.Add(this.buttonFullRun);
            this.panel1.Controls.Add(this.labelBenchmarkType);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.labelBenchmark);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 83);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1634, 94);
            this.panel1.TabIndex = 1;
            // 
            // buttonExportResults
            // 
            this.buttonExportResults.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExportResults.Location = new System.Drawing.Point(916, 51);
            this.buttonExportResults.Name = "buttonExportResults";
            this.buttonExportResults.Size = new System.Drawing.Size(125, 25);
            this.buttonExportResults.TabIndex = 9;
            this.buttonExportResults.Text = "Export Results";
            this.buttonExportResults.UseVisualStyleBackColor = true;
            this.buttonExportResults.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // buttonCustomRun
            // 
            this.buttonCustomRun.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCustomRun.Location = new System.Drawing.Point(173, 51);
            this.buttonCustomRun.Name = "buttonCustomRun";
            this.buttonCustomRun.Size = new System.Drawing.Size(125, 25);
            this.buttonCustomRun.TabIndex = 8;
            this.buttonCustomRun.Text = "Custom Run";
            this.buttonCustomRun.UseVisualStyleBackColor = true;
            // 
            // labelCustomBenchmark
            // 
            this.labelCustomBenchmark.AutoSize = true;
            this.labelCustomBenchmark.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCustomBenchmark.Location = new System.Drawing.Point(665, 16);
            this.labelCustomBenchmark.Name = "labelCustomBenchmark";
            this.labelCustomBenchmark.Size = new System.Drawing.Size(167, 17);
            this.labelCustomBenchmark.TabIndex = 7;
            this.labelCustomBenchmark.Text = "Select number of threads:";
            this.labelCustomBenchmark.Click += new System.EventHandler(this.labelCustomBenchmark_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "1",
            "2",
            "4",
            "8",
            "16",
            "32",
            "64"});
            this.comboBox2.Location = new System.Drawing.Point(688, 49);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 25);
            this.comboBox2.TabIndex = 6;
            // 
            // buttonFullRun
            // 
            this.buttonFullRun.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonFullRun.Location = new System.Drawing.Point(38, 51);
            this.buttonFullRun.Name = "buttonFullRun";
            this.buttonFullRun.Size = new System.Drawing.Size(110, 25);
            this.buttonFullRun.TabIndex = 5;
            this.buttonFullRun.Text = "Full Run";
            this.buttonFullRun.UseVisualStyleBackColor = true;
            // 
            // labelBenchmarkType
            // 
            this.labelBenchmarkType.AutoSize = true;
            this.labelBenchmarkType.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBenchmarkType.Location = new System.Drawing.Point(35, 16);
            this.labelBenchmarkType.Name = "labelBenchmarkType";
            this.labelBenchmarkType.Size = new System.Drawing.Size(113, 17);
            this.labelBenchmarkType.TabIndex = 4;
            this.labelBenchmarkType.Text = "Benchmark Type:";
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Monte Carlo",
            "Matrix Multiplication",
            "Fast Fourier Tranform"});
            this.comboBox1.Location = new System.Drawing.Point(388, 49);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(150, 25);
            this.comboBox1.TabIndex = 3;
            // 
            // labelBenchmark
            // 
            this.labelBenchmark.AutoSize = true;
            this.labelBenchmark.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBenchmark.Location = new System.Drawing.Point(403, 16);
            this.labelBenchmark.Name = "labelBenchmark";
            this.labelBenchmark.Size = new System.Drawing.Size(121, 17);
            this.labelBenchmark.TabIndex = 2;
            this.labelBenchmark.Text = "Select Benchmark:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 183);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBoxResults);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelPlaceHolder);
            this.splitContainer1.Size = new System.Drawing.Size(1634, 700);
            this.splitContainer1.SplitterDistance = 365;
            this.splitContainer1.TabIndex = 2;
            // 
            // textBoxResults
            // 
            this.textBoxResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxResults.Location = new System.Drawing.Point(0, 0);
            this.textBoxResults.Multiline = true;
            this.textBoxResults.Name = "textBoxResults";
            this.textBoxResults.Size = new System.Drawing.Size(365, 700);
            this.textBoxResults.TabIndex = 0;
            this.textBoxResults.Text = "Benchmark Results will appear here...";
            this.textBoxResults.TextChanged += new System.EventHandler(this.textBoxResults_TextChanged);
            // 
            // panelPlaceHolder
            // 
            this.panelPlaceHolder.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.panelPlaceHolder.Controls.Add(this.chart1);
            this.panelPlaceHolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPlaceHolder.Location = new System.Drawing.Point(0, 0);
            this.panelPlaceHolder.Name = "panelPlaceHolder";
            this.panelPlaceHolder.Size = new System.Drawing.Size(1265, 700);
            this.panelPlaceHolder.TabIndex = 0;
            // 
            // chart1
            // 
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(58, 46);
            this.chart1.Name = "chart1";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart1.Series.Add(series2);
            this.chart1.Size = new System.Drawing.Size(1146, 632);
            this.chart1.TabIndex = 1;
            this.chart1.Text = "chart1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1652, 898);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "CPU Benchmark";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panelSpecs.ResumeLayout(false);
            this.panelSpecs.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelPlaceHolder.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panelSpecs;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.Label labelThreads;
        private System.Windows.Forms.Label labelCores;
        private System.Windows.Forms.Label labelCPU;
        private System.Windows.Forms.Label labelL3;
        private System.Windows.Forms.Label labelL2;
        private System.Windows.Forms.Label labelL1I;
        private System.Windows.Forms.Label labelL1D;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label labelBenchmark;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button buttonFullRun;
        private System.Windows.Forms.Label labelBenchmarkType;
        private System.Windows.Forms.Button buttonCustomRun;
        private System.Windows.Forms.Label labelCustomBenchmark;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox textBoxResults;
        private System.Windows.Forms.Panel panelPlaceHolder;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button buttonExportResults;
    }
}

