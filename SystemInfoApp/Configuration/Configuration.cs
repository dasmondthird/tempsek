using System;
using System.Configuration;

namespace SystemInfoApp.Configuration
{
    public static class Configuration
    {
        public static string GetUsername()
        {
            return Properties.Settings.Default["Username"] as string ?? string.Empty;
        }

        public static string GetPassword()
        {
            return Properties.Settings.Default["Password"] as string ?? string.Empty;
        }

        public static string GetBaseAddress()
        {
            return Properties.Settings.Default["BaseAddress"] as string ?? string.Empty;
        }

        public static string GetWebSocketUri()
        {
            // Ensure that the WebSocketUri property exists in the Settings class
            return Properties.Settings.Default["WebSocketUri"] as string ?? string.Empty;
        }
    }
}
