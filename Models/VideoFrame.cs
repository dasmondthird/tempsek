// VideoFrame.cs
using System;

namespace Server.Models
{
    public class VideoFrame
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public byte[] FrameData { get; set; }
        public DateTime CaptureTime { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
