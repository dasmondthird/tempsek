// ScreenshotInfo.cs
using System;
using System.Reflection;

namespace SystemInfoApp.Models
{
    public class ScreenshotInfo
    {
        public string DeviceId { get; set; }
        public byte[] ImageData { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime CaptureTime { get; set; }

        public ScreenshotInfo()
        {
            DeviceId = string.Empty;
            ImageData = Array.Empty<byte>();
        }
    }
}
