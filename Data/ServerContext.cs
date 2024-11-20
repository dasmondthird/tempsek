// ServerContext.cs
using System.Data.Entity;
using Server.Models;

namespace Server.Data
{
    public class ServerContext : DbContext
    {
        public ServerContext() : base("DefaultConnection")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<DiagnosticInfo> DiagnosticInfos { get; set; }
        public DbSet<ProcessInfo> ProcessInfos { get; set; }
        public DbSet<ScreenshotInfo> ScreenshotInfos { get; set; }
        public DbSet<VideoFrame> VideoFrames { get; set; }
    }
}
