using System;
using System.IO;
using System.Text.Json;

namespace supapac;

public static class HighscoreManager
{
    private static readonly string HighscoreFile =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "highscore.json");

    // Das, was StartForm braucht:
    public static string BestName { get; private set; } = "Niemand";
    public static int BestScore { get; private set; } = 0;

    // interne Hilfsklasse für JSON
    private class HighscoreData
    {
        public string Name { get; set; } = "Niemand";
        public int Score { get; set; } = 0;
    }

    static HighscoreManager()
    {
        Load();
    }

    public static void Load()
    {
        try
        {
            if (!File.Exists(HighscoreFile))
                return;

            string json = File.ReadAllText(HighscoreFile);
            var data = JsonSerializer.Deserialize<HighscoreData>(json);
            if (data == null)
                return;

            BestName = data.Name;
            BestScore = data.Score;
        }
        catch
        {
            // Wenn was schiefgeht: einfach Standardwerte behalten
        }
    }

    public static void Save()
    {
        try
        {
            var data = new HighscoreData
            {
                Name = BestName,
                Score = BestScore
            };

            string json = JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(HighscoreFile, json);
        }
        catch
        {
        }
    }

    // Highscore setzen, wenn Score besser ist
    public static void SetHighscore(string name, int score)
    {
        if (score <= BestScore)
            return;

        BestName = string.IsNullOrWhiteSpace(name) ? "Unbekannt" : name.Trim();
        BestScore = score;
        Save();
    }
}
