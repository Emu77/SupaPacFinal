using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Supapac
{
    public static class HighscoreManager
    {
        private static readonly string HighscoreFile =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "highscore.txt");

        public static string BestName { get; private set; } = "Niemand";
        public static int BestScore { get; private set; } = 0;

        static HighscoreManager()
        {
            Load();
        }

        private static void Load()
        {
            try
            {
                if (!File.Exists(HighscoreFile)) return;
                var lines = File.ReadAllLines(HighscoreFile);
                if (lines.Length >= 2)
                {
                    BestName = lines[0];
                    int.TryParse(lines[1], out int s);
                    BestScore = s;
                }
            }
            catch { /* egal */ }
        }

        private static void Save()
        {
            try
            {
                File.WriteAllLines(HighscoreFile, new[]
                {
                    BestName,
                    BestScore.ToString()
                });
            }
            catch { /* egal */ }
        }

        public static void TrySetHighscore(int score, IWin32Window owner)
        {
            if (score <= BestScore) return;

            string name = Prompt.ShowDialog(
                $"Neuer Highscore: {score}!\nBitte Namen eingeben:",
                "Neuer Highscore");

            if (string.IsNullOrWhiteSpace(name))
                name = "Unbekannt";

            BestName = name;
            BestScore = score;
            Save();

            MessageBox.Show(owner,
                $"Glückwunsch {BestName}! Neuer Highscore: {BestScore}",
                "Highscore", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    // Kleiner Eingabe-Dialog für Namen
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 160,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label textLabel = new Label() { Left = 20, Top = 20, Width = 340, Text = text };
            TextBox inputBox = new TextBox() { Left = 20, Top = 50, Width = 340 };
            Button confirmation = new Button()
            {
                Text = "OK",
                Left = 280,
                Width = 80,
                Top = 80,
                DialogResult = DialogResult.OK
            };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text : "";
        }
    }
}
