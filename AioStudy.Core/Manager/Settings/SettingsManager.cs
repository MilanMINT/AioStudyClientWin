using AioStudy.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AioStudy.Core.Manager.Settings
{
    public sealed class SettingsManager
    {
        private static SettingsManager instance = null;
        private static readonly object padlock = new object();
        private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AioStudy", "settings.json");
        private IThemeService _themeService;

        public AppSettings Settings { get; private set; }

        public static SettingsManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SettingsManager();
                    }
                    return instance;
                }
            }
        }

        private SettingsManager()
        {
        }

        public void SetThemeService(IThemeService themeService)
        {
            _themeService = themeService;
        }

        public void Init()
        {
            try
            {
                var directory = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                LoadSettings();
                SaveSettings();
            }
            catch (Exception)
            {
                Settings = new AppSettings();
                SaveSettings();
            }
        }

        public bool SaveSettings()
        {
            try
            {
                var directory = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string _jsonString = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, _jsonString);
                _themeService?.ApplyTheme(Settings.Theme);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void LoadSettings()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    string _jsonString = File.ReadAllText(FilePath);
                    Settings = JsonSerializer.Deserialize<AppSettings>(_jsonString) ?? new AppSettings();
                }
                catch (Exception)
                {
                    Settings = new AppSettings();
                }
            }
            else
            {
                Settings = new AppSettings();
            }
        }
    }
}
