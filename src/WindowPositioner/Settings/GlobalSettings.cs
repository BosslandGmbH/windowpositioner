using System.Collections.Generic;
using System.ComponentModel;

namespace WindowPositioner.Settings
{
    public class GlobalSettings : JsonSettings
    {
        private GlobalSettings() : base(GetSettingsFilePath("GlobalSettings.json"))
        {
        }

        public static GlobalSettings Instance { get; } = new GlobalSettings();

        public List<string> ProcessNames { get; set; }

        [DefaultValue(0)]
        public int ScreenIndex { get; set; }
    }
}