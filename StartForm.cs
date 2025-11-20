using System;
using System.Drawing;
using System.Windows.Forms;

namespace supapac
{
    public class StartForm : Form
    {
        private Button btnStart;
        private Button btnSettings;
        private Button btnHighscore;
        private Button btnExit;

        public StartForm()
        {
            // --- Fenster ---
            Text = "SupaPac - Hauptmenü";
            Width = 450;
            Height = 550;
            BackColor = Color.Black;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // --- Titel ---
            var title = new Label
            {
                Text = "SUPAPAC",
                Font = new Font("Consolas", 34, FontStyle.Bold),
                AutoSize = false,
                Height = 140,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Yellow
            };
            Controls.Add(title);

            // --- Buttons erstellen ---
            btnStart     = CreateButton("Spiel starten");
            btnSettings  = CreateButton("Einstellungen");
            btnHighscore = CreateButton("Highscore");
            btnExit      = CreateButton("Beenden");

            Controls.Add(btnStart);
            Controls.Add(btnSettings);
            Controls.Add(btnHighscore);
            Controls.Add(btnExit);

            // --- Button Events (jetzt korrekt) ---
            btnStart.Click += (s, e) =>
            {
                try
                {
                    using var game = new GameForm();   // <--- Existiert in deinem Projekt
                    Hide();
                    game.ShowDialog();
                    Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("GameForm konnte nicht geöffnet werden:\n" + ex.Message);
                }
            };

            btnSettings.Click += (s, e) =>
            {
                try
                {
                    using var settings = new SettingsForm();  // <--- Existiert in deinem Projekt
                    settings.ShowDialog(this);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SettingsForm konnte nicht geöffnet werden:\n" + ex.Message);
                }
            };

            btnHighscore.Click += (s, e) =>
            {
                try
                {
                    using var high = new HighscoreForm();  // <--- existiert in deinem Projekt
                    high.ShowDialog(this);
                }
                catch
                {
                    // Fallback: Nutze Manager, falls HighscoreForm leer ist
                    MessageBox.Show(
                        this,
                        $"Bester Spieler:\n{HighscoreManager.BestName}\n\nScore: {HighscoreManager.BestScore}",
                        "Highscore",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            };

            btnExit.Click += (s, e) => Close();

            // Buttons zentrieren
            Shown += (s, e) => CenterButtons();
            Resize += (s, e) => CenterButtons();
        }

        private Button CreateButton(string text)
        {
            var b = new Button
            {
                Text = text,
                Width = 260,
                Height = 50,
                Font = new Font("Consolas", 16, FontStyle.Bold),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            b.FlatAppearance.BorderColor = Color.White;
            b.FlatAppearance.BorderSize = 2;

            return b;
        }

        private void CenterButtons()
        {
            int spacing = 15;
            int totalHeight =
                btnStart.Height +
                btnSettings.Height +
                btnHighscore.Height +
                btnExit.Height +
                spacing * 3;

            int startY = (ClientSize.Height - totalHeight) / 2 + 40;
            int centerX = (ClientSize.Width - btnStart.Width) / 2;

            btnStart.Location     = new Point(centerX, startY);
            btnSettings.Location  = new Point(centerX, startY + btnStart.Height + spacing);
            btnHighscore.Location = new Point(centerX, startY + btnStart.Height + btnSettings.Height + spacing * 2);
            btnExit.Location      = new Point(centerX, startY + btnStart.Height + btnSettings.Height + btnHighscore.Height + spacing * 3);
        }
    }
}
