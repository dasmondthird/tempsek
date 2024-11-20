// ScreenshotInfo.cs
using System;

namespace Server.Models
{
    public class ScreenshotInfo
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public byte[] ImageData { get; set; }
        public DateTime CaptureTime { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
