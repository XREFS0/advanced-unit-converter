using System.Text.Json;
using AdvancedUnitConverter.Core.Interfaces;
using AdvancedUnitConverter.Core.Models;

namespace AdvancedUnitConverter.Infrastructure.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsFilePath;
        private AppSettings _settings = new();

        public AppSettings Settings => _settings;
        public event Action? SettingsChanged;

        public SettingsService(string? customPath = null)
        {
            if (string.IsNullOrWhiteSpace(customPath))
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appFolder = Path.Combine(appData, "AdvancedUnitConverter");
                Directory.CreateDirectory(appFolder);
                _settingsFilePath = Path.Combine(appFolder, "settings.json");
            }
            else
            {
                _settingsFilePath = customPath;
            }
        }

        public async Task LoadAsync()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    string json = await File.ReadAllTextAsync(_settingsFilePath);
                    var loaded = JsonSerializer.Deserialize<AppSettings>(json);
                    if (loaded != null)
                    {
                        _settings = loaded;
                    }
                }
            }
            catch
            {
                // Fallback to default on corrupt settings file
                _settings = new AppSettings();
            }
            SettingsChanged?.Invoke();
        }

        public async Task SaveAsync()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_settings, options);
                await File.WriteAllTextAsync(_settingsFilePath, json);
                SettingsChanged?.Invoke();
            }
            catch
            {
                // Suppress or log file write exception
            }
        }
    }
}
