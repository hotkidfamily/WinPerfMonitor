using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Perfmon
{
    public partial class VisualForm : Form
    {
        private readonly string _csvPath;

        private static readonly string TAB_HEADER_CPU = "CPU";
        private static readonly string TAB_HEADER_MEMORY = "Memory";
        private static readonly string TAB_HEADER_UPLINK = "UpLink";
        private static readonly string TAB_HEADER_SYSTEM = "System";
        private ScottPlot.Plottable.DataLogger _sysLogger = default!;
        private ScottPlot.Plottable.DataLogger _procLogger = default!;
        private ScottPlot.Plottable.DataLogger _memLogger = default!;
        private ScottPlot.Plottable.DataLogger _uplinkLogger = default!;

        class RunStatusItemMap : ClassMap<RunStatusItem>
        {
            public RunStatusItemMap()
            {
                Map(m => m.Pid).Name("Pid");
                Map(m => m.Cpu).Name("Cpu");
                Map(m => m.ProcName).Name("ProcName");
                Map(m => m.VMem).Name("VMem");
                Map(m => m.PhyMem).Name("PhyMem");
                Map(m => m.TotalMem).Name("TotalMem");
                Map(m => m.DownLink).Name("DownLink");
                Map(m => m.UpLink).Name("UpLink");
                Map(m => m.TotalLinkFlow).Name("TotalLinkFlow");
                Map(m => m.ExcuteSeconds).Name("ExcuteSeconds");
                Map(m => m.ExcuteStatus).Name("ExcuteStatus");
                Map(m => m.SysCpu).Name("SysCpu");
            }
        }

        public VisualForm(string path, string descriptor)
        {
            InitializeComponent();
            ConstructTabControl();
            _csvPath = path;
            _ = UpdateInfo();
            Text += descriptor;
        }

        private void ConstructTabControl()
        {
            formsPlotProcCPU.Name = TAB_HEADER_CPU;
            formsPlotProcCPU.Plot.YLabel("CPU usage (%)");
            formsPlotProcCPU.Plot.YAxis.SetBoundary(0, 100);
            formsPlotProcCPU.Plot.XAxis.SetBoundary(0);
            _procLogger = formsPlotProcCPU.Plot.AddDataLogger();
            _procLogger.LineWidth = 3;
            _procLogger.ViewSlide(width: 200);

            formsPlotProcMem.Name = TAB_HEADER_MEMORY;
            formsPlotProcMem.Plot.YLabel("Memory usage (MB)");
            formsPlotProcMem.Plot.YAxis.SetBoundary(0);
            formsPlotProcMem.Plot.XAxis.SetBoundary(0);
            _memLogger = formsPlotProcMem.Plot.AddDataLogger();
            _memLogger.LineWidth = 3;
            _memLogger.ViewSlide(width: 200);

            formsPlotUpLink.Name = TAB_HEADER_UPLINK;
            formsPlotUpLink.Plot.YLabel("Uplink Speed (Kb/s)");
            formsPlotUpLink.Plot.YAxis.SetBoundary(0);
            formsPlotUpLink.Plot.XAxis.SetBoundary(0);
            _uplinkLogger = formsPlotUpLink.Plot.AddDataLogger();
            _uplinkLogger.LineWidth = 3;
            _uplinkLogger.ViewSlide(width: 200);

            formsPlotSysCpu.Name = TAB_HEADER_SYSTEM;
            formsPlotSysCpu.Plot.YLabel("System CPU usage (%)");
            formsPlotSysCpu.Plot.YAxis.SetBoundary(0, 100);
            formsPlotSysCpu.Plot.XAxis.SetBoundary(0);
            _sysLogger = formsPlotSysCpu.Plot.AddDataLogger();
            _sysLogger.LineWidth = 3;
            _sysLogger.ViewSlide(width: 200);

            formsPlotProcCPU.Refresh();
            formsPlotProcMem.Refresh();
            formsPlotUpLink.Refresh();
            formsPlotSysCpu.Refresh();
        }


        async Task UpdateInfo()
        {
            if (!File.Exists(_csvPath))
            {
                return;
            }
            List<RunStatusItem> records;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                AllowComments = true,
                Comment = '#',
                Delimiter = ",",
                HasHeaderRecord = true,
                HeaderValidated = null,
                MissingFieldFound = null,
            };

            var fs = new FileStream(_csvPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var reader = new StreamReader(fs);
            var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<RunStatusItemMap>();

            while (!IsDisposed)
            {
                records = csv.GetRecords<RunStatusItem>().ToList();
                int length = records.Count;

                for (int i = 0; i < length; i++)
                {
                    _procLogger.Add(_procLogger.Count, records[i].Cpu);
                    _memLogger.Add(_memLogger.Count, records[i].TotalMem);
                    _uplinkLogger.Add(_uplinkLogger.Count, records[i].UpLink);
                    _sysLogger.Add(_sysLogger.Count, records[i].SysCpu);
                }
                formsPlotSysCpu.Refresh();
                formsPlotProcCPU.Refresh();
                formsPlotProcMem.Refresh();
                formsPlotUpLink.Refresh();

                await Task.Delay(TimeSpan.FromMilliseconds(1000));
            }
        }
    }
}
