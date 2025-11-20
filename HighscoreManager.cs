using System;
using System.IO;
using System.Windows.Forms;

namespace supapac
{
    internal static class HighscoreManager
    {
        private static readonly string FilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "highscores.txt");

        public static string BestName { get; private set; } = "Niemand";
        public static int BestScore { get; private set; } = 0;

        // Statischer Konstruktor: wird einmal beim ersten Zugriff aufgerufen
        static HighscoreManager()
        {
            Load();
        }

        private static void Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    BestName = "Niemand";
                    BestScore = 0;
                    return;
                }

                var line = File.ReadAllText(FilePath).Trim();
                if (string.IsNullOrWhiteSpace(line))
                    return;

                var parts = line.Split(';');
                if (parts.Length >= 2 && int.TryParse(parts[1], out int score))
                {
                    BestName = parts[0];
                    BestScore = score;
                }
            }
            catch
            {
                // Ignorieren – dann einfach Default-Werte benutzen
                BestName = "Niemand";
                BestScore = 0;
            }
        }

        private static void Save()
        {
            try
            {
                File.WriteAllText(FilePath, $"{BestName};{BestScore}");
            }
            catch
            {
                // Wenn Speichern nicht klappt, Spiel trotzdem weiterlaufen lassen
            }
        }

        public static void SetHighscore(string name, int score)
        {
            if (score < 0) score = 0;
            if (string.IsNullOrWhiteSpace(name))
                name = "Niemand";

            BestName = name.Trim();
            BestScore = score;
            Save();
        }

        /// <summary>
        /// Prüft, ob der Score ein neuer Highscore ist und fragt bei Erfolg einen Namen ab.
        /// </summary>
        public static void TrySetHighscore(int score, Form owner)
        {
            if (score <= BestScore)
                return; // kein neuer Highscore

            string name = PromptForName(owner, score);
            SetHighscore(name, score);

            MessageBox.Show(
                owner,
                $"Neuer Highscore!\n{name}: {score}",
                "Highscore",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private static string PromptForName(Form owner, int score)
        {
            using (var dialog = new Form())
            using (var txtName = new TextBox())
            using (var lbl = new Label())
            using (var btnOk = new Button())
            using (var btnCancel = new Button())
            {
                dialog.Text = "Neuer Highscore!";
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ClientSize = new System.Drawing.Size(300, 140);
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.ShowInTaskbar = false;

                lbl.Text = $"Du hast {score} Punkte erreicht!\nBitte Namen eingeben:";
                lbl.AutoSize = false;
                lbl.SetBounds(10, 10, 280, 40);

                txtName.SetBounds(10, 55, 280, 20);
                txtName.Text = BestName == "Niemand" ? "" : BestName;

                btnOk.Text = "OK";
                btnOk.DialogResult = DialogResult.OK;
                btnOk.SetBounds(115, 90, 75, 25);

                btnCancel.Text = "Abbrechen";
                btnCancel.DialogResult = DialogResult.Cancel;
                btnCancel.SetBounds(200, 90, 90, 25);

                dialog.Controls.AddRange(new Control[] { lbl, txtName, btnOk, btnCancel });
                dialog.AcceptButton = btnOk;
                dialog.CancelButton = btnCancel;

                var result = dialog.ShowDialog(owner);
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(txtName.Text))
                    return txtName.Text.Trim();

                // Fallback
                return string.IsNullOrWhiteSpace(BestName) ? "Spieler" : BestName;
            }
        }
    }
}
