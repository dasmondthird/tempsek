// Models/DiagnosticInfo.cs
using System;

namespace Server.Models
{
    public class DiagnosticInfo
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string IPAddress { get; set; }
        public string UserName { get; set; }
        public string SystemLanguage { get; set; }
        public string Antivirus { get; set; }
        public DateTime CurrentTime { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
