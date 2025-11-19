using System;
using System.Drawing;
using System.Windows.Forms;

namespace supapac;

public class StartForm : Form
{
    private Button btnStart;
    private Button btnSettings;
    private Button btnHighscore;
    private Button btnExit;

    public StartForm()
    {
        Text = "SupaPac - Hauptmenü";
        Width = 400;
        Height = 300;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.Black;

        var title = new Label
        {
            Text = "SUPAPAC",
            ForeColor = Color.Yellow,
            Font = new Font("Consolas", 20, FontStyle.Bold),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Top,
            Height = 60
        };
        Controls.Add(title);

        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(40)
        };
        Controls.Add(panel);

        btnStart = new Button
        {
            Text = "Spiel starten",
            Dock = DockStyle.Top,
            Height = 40
        };
        btnStart.Click += (s, e) =>
        {
            using var game = new GameForm();
            Hide();
            game.ShowDialog();
            Show();
        };

        btnSettings = new Button
        {
            Text = "Einstellungen",
            Dock = DockStyle.Top,
            Height = 40
        };
        btnSettings.Click += (s, e) =>
        {
            using var set = new SettingsForm();
            set.ShowDialog(this);
        };

        btnHighscore = new Button
        {
            Text = "Highscore",
            Dock = DockStyle.Top,
            Height = 40
        };
        btnHighscore.Click += (s, e) =>
        {
            // Statt HighscoreForm: einfacher Dialog
            MessageBox.Show(
                this,
                $"Bester Spieler:\n{HighscoreManager.BestName}\n\nScore: {HighscoreManager.BestScore}",
                "Highscore",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        };

        btnExit = new Button
        {
            Text = "Beenden",
            Dock = DockStyle.Top,
            Height = 40
        };
        btnExit.Click += (s, e) => Close();

        // Reihenfolge: zuletzt hinzugefügt sitzt oben
        panel.Controls.Add(btnExit);
        panel.Controls.Add(btnHighscore);
        panel.Controls.Add(btnSettings);
        panel.Controls.Add(btnStart);
    }
}
