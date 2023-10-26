using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PerfMonitor.Library
{
    internal class HistoryItem
    {
        private uint pid;
        private string marker;
        private string date;
        private string resPath;

        [JsonPropertyName("Pid")]
        public uint Pid { get => pid; set => pid = value; }
        [JsonPropertyName("Marker")]
        public string Marker { get => marker; set => marker = value; }
        [JsonPropertyName("Date")]
        public string Date { get => date; set => date = value; }
        [JsonPropertyName("ResPath")]
        public string ResPath { get => resPath; set => resPath = value; }

        public string[] Info ()
        {
            return new string[] {
                $"{Pid}", $"{Marker}", $"{Date}", $"{ResPath}"
            };
        }
    }

    internal class HistoryController
    {
        private readonly string version = "1.0";
        private string machine = string.Empty;
        private List<HistoryItem> history = new();

        [JsonPropertyName("Version")]
        public string Version { get => version; }
        [JsonPropertyName("Machine")]
        public string Machine { get => machine; set => machine = value; }
        [JsonPropertyName("History")]
        public List<HistoryItem> History { get => history; set => history = value; }

        private string _path = string.Empty;

        public HistoryController () // for Deserialize
        {
        }
        public HistoryController (string path)
        {
            _path = path;
        }

        public void AddItem (uint pid, string respath, string marker)
        {
            var item = new HistoryItem()
            {
                Pid = pid,
                ResPath = respath,
                Marker = marker,
                Date = $"{DateTime.Now:yyyy.MMdd.HHmm.ss}",
            };
            History.Add(item);
        }

        public void Write()
        {
            string json = JsonSerializer.Serialize(this);
            if ( File.Exists(_path) )
            {
                using var sw = new StreamWriter(new FileStream(_path, FileMode.Open, FileAccess.Write, FileShare.Read));
                sw.Write(json);
            }
            else
            {
                using var sw = new StreamWriter(new FileStream(_path, FileMode.CreateNew, FileAccess.Write, FileShare.Read));
                sw.Write(json);
            }
        }

        public void Read ()
        {
            History?.Clear();
            if ( File.Exists(_path) )
            {
                using var sr = new StreamReader(new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                var json = sr.ReadToEnd();

                var v = JsonSerializer.Deserialize<HistoryController>(json);
                if ( v == null || v.version != version ) return;
                History = v.History;
            }
        }
    }
}