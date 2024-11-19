using System;
using System.Reflection;

namespace SystemInfoApp.Models
{
    public class DiagnosticInfo
    {
        public string DeviceId { get; set; }
        public string IPAddress { get; set; }
        public string UserName { get; set; }
        public string SystemLanguage { get; set; }
        public string Antivirus { get; set; }
        public DateTime CurrentTime { get; set; }
        public DateTime Timestamp { get; set; }

        public DiagnosticInfo()
        {
            DeviceId = string.Empty;
            IPAddress = string.Empty;
            UserName = string.Empty;
            SystemLanguage = string.Empty;
            Antivirus = string.Empty;
        }
    }
}
