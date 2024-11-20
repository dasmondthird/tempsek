// ProcessInfo.cs
using System;

namespace Server.Models
{
    public class ProcessInfo
    {
        public int Id { get; set; }
        public string ProcessName { get; set; }
        public int PID { get; set; }
        public string StartTime { get; set; }
        public string MemoryKB { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
