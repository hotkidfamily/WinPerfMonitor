using CsvHelper;
using CsvHelper.Configuration;
using ScottPlot;
using System.Globalization;
using TraceReloggerLib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Perfmon
{
    public partial class VisualForm : Form
    {
        private readonly string _csvPath;
        private readonly string _descriptor;
        private List<RunStatusItem> _records = default!;

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
            }
        }

        public VisualForm(string path, string descriptor)
        {
            InitializeComponent();
            ConstructTabControl();
            _csvPath = path;
            _descriptor = descriptor;
            _ = UpdateInfo();
            Text = Text + descriptor;
        }

        private void ConstructTabControl()
        {
            formsPlotProcCPU.Name = TAB_HEADER_CPU;
            formsPlotProcCPU.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            formsPlotProcCPU.Plot.Title("CPU usage");
            formsPlotProcCPU.Plot.XLabel("Time");
            formsPlotProcCPU.Plot.YLabel("%");
            formsPlotProcCPU.Plot.YAxis.SetBoundary(0, 100);
            formsPlotProcCPU.Plot.XAxis.SetBoundary(0);
            _procLogger = formsPlotProcCPU.Plot.AddDataLogger();
            _procLogger.LineWidth = 3;
            _procLogger.ViewSlide(width: 200);

            formsPlotProcMem.Name = TAB_HEADER_MEMORY;
            formsPlotProcMem.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            formsPlotProcMem.Plot.Title("Memory usage");
            formsPlotProcMem.Plot.XLabel("Time");
            formsPlotProcMem.Plot.YLabel("MB");
            formsPlotProcMem.Plot.YAxis.SetBoundary(0);
            formsPlotProcMem.Plot.XAxis.SetBoundary(0);
            _memLogger = formsPlotProcMem.Plot.AddDataLogger();
            _memLogger.LineWidth = 3;
            _memLogger.ViewSlide(width: 200);

            formsPlotUpLink.Name = TAB_HEADER_UPLINK;
            formsPlotUpLink.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            formsPlotUpLink.Plot.Title("Uplink Speed");
            formsPlotUpLink.Plot.XLabel("Time");
            formsPlotUpLink.Plot.YLabel("Kb/s");
            formsPlotUpLink.Plot.YAxis.SetBoundary(0);
            formsPlotUpLink.Plot.XAxis.SetBoundary(0);
            _uplinkLogger = formsPlotUpLink.Plot.AddDataLogger();
            _uplinkLogger.LineWidth = 3;
            _uplinkLogger.ViewSlide(width: 200);

            formsPlotSysCpu.Name = TAB_HEADER_SYSTEM;
            formsPlotSysCpu.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            formsPlotSysCpu.Plot.Title("System CPU usage");
            formsPlotSysCpu.Plot.XLabel("Time");
            formsPlotSysCpu.Plot.YLabel("%");
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
                _records = csv.GetRecords<RunStatusItem>().ToList();
                int length = _records.Count;

                for (int i = 0; i < length; i++)
                {
                    _sysLogger.Add(_sysLogger.Count, _records[i].Cpu);
                    _procLogger.Add(_procLogger.Count, _records[i].Cpu);
                    _memLogger.Add(_memLogger.Count, _records[i].TotalMem);
                    _uplinkLogger.Add(_uplinkLogger.Count, _records[i].UpLink);
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
