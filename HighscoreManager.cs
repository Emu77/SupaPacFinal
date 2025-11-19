
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

public class HighscoreEntry
{
    public string Name { get; set; }
    public int Score { get; set; }
}

public class HighscoreManager
{
    private static readonly string FILE_PATH =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "highscores.json");

    public List<HighscoreEntry> Highscores { get; private set; }

    public HighscoreManager()
    {
        LoadHighscores();
    }

    /// <summary>
    /// Fügt einen neuen Score hinzu und speichert die besten 3 Highscores.
    /// </summary>
    public void AddScore(string name, int score)
    {
        Highscores.Add(new HighscoreEntry { Name = name, Score = score });

        Highscores = Highscores
            .OrderByDescending(h => h.Score)
            .Take(3)
            .ToList();

        SaveHighscores();
    }

    /// <summary>
    /// Exportiert alle aktuell bekannten Highscores als CSV.
    /// Format: Platz;Name;Score (UTF-8, Semikolontrenner – gut für Excel/LibreOffice).
    /// </summary>
    public void ExportToCsv(string filePath)
    {
        var lines = new List<string>();
        lines.Add("Platz;Name;Score");

        int rank = 1;
        foreach (var entry in Highscores.OrderByDescending(h => h.Score))
        {
            string safeName = entry.Name?.Replace(";", ",") ?? string.Empty;
            lines.Add($"{rank};{safeName};{entry.Score}");
            rank++;
        }

        File.WriteAllLines(filePath, lines, Encoding.UTF8);
    }

    private void SaveHighscores()
    {
        string json = JsonSerializer.Serialize(
            Highscores,
            new JsonSerializerOptions { WriteIndented = true }
        );
        File.WriteAllText(FILE_PATH, json);
    }

    private void LoadHighscores()
    {
        if (File.Exists(FILE_PATH))
        {
            string json = File.ReadAllText(FILE_PATH);
            Highscores = JsonSerializer.Deserialize<List<HighscoreEntry>>(json)
                          ?? new List<HighscoreEntry>();
        }
        else
        {
            Highscores = new List<HighscoreEntry>();
        }
    }

    /// <summary>
    /// Fragt einen Spielernamen ab und speichert den Score in der Highscoreliste.
    /// Wird am Ende des Spiels aus GameForm aufgerufen.
    /// </summary>
    public static void TrySetHighscore(int score, System.Windows.Forms.Form parent)
    {
        string name = "Player";

        // Einfacher Name-Dialog (Microsoft.VisualBasic muss als Referenz vorhanden sein)
        string input = Microsoft.VisualBasic.Interaction.InputBox(
            "Bitte Namen für die Highscore-Liste eingeben:",
            "Highscore",
            "Player"
        );

        if (!string.IsNullOrWhiteSpace(input))
            name = input;

        var hs = new HighscoreManager();
        hs.AddScore(name, score);
    }
}
