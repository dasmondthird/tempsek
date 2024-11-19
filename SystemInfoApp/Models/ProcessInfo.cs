namespace SystemInfoApp.Models
{
    public class ProcessInfo
    {
        public string ProcessName { get; set; }
        public int PID { get; set; }
        public string StartTime { get; set; }
        public string MemoryKB { get; set; }

        public ProcessInfo()
        {
            ProcessName = string.Empty;
            StartTime = string.Empty;
            MemoryKB = string.Empty;
        }
    }
}
