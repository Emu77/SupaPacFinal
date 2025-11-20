using System;
using System.IO;
using System.Text.Json;
using supapac;


namespace supapac   // WICHTIG: gleicher Namespace wie GameSettings & Program
{
    public class SettingsData
    {
        public Difficulty Difficulty { get; set; }
        public BoardScale Scale { get; set; }
        public bool SoundEnabled { get; set; }
    }

    public static class SettingsIO
    {
        private static readonly string SettingsFile =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public static void Save()
        {
            var data = new SettingsData()
            {
                Difficulty = GameSettings.Difficulty,
                Scale = GameSettings.Scale,
                SoundEnabled = GameSettings.SoundEnabled
            };

            string json = JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions { WriteIndented = true }
            );

            File.WriteAllText(SettingsFile, json);
        }

        public static void Load()
        {
            if (!File.Exists(SettingsFile))
                return;

            string json = File.ReadAllText(SettingsFile);
            var data = JsonSerializer.Deserialize<SettingsData>(json);

            if (data == null)
                return;

            GameSettings.Difficulty = data.Difficulty;
            GameSettings.Scale = data.Scale;
            GameSettings.SoundEnabled = data.SoundEnabled;
        }

        public static void Export(string path)
        {
            Save(); // erst lokale settings.json aktualisieren
            File.Copy(SettingsFile, path, true);
        }

        public static void Import(string path)
        {
            File.Copy(path, SettingsFile, true);
            Load(); // eingelesene Einstellungen anwenden
        }
    }
}
