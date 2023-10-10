namespace PerfMonitor
{
    public class Loader
    {
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

            MainForm mainForm = new();
            mainForm.ShowDialog();
        }
    }

}