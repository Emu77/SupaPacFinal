using System;
using System.Drawing;
using System.Windows.Forms;

namespace Supapac
{
    public class StartForm : Form
    {
        private Button? btnStart;
        private Button? btnSettings;
        private Button? btnHighscore;
        private Button? btnExit;

        public StartForm()
        {
            Text = "Supapac - Hauptmenü";
            Width = 400;
            Height = 300;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.Black;

            Label title = new Label()
            {
                Text = "SUPAPAC",
                ForeColor = Color.Yellow,
                Font = new Font("Consolas", 18, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };
            Controls.Add(title);

            Panel panel = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40, 20, 40, 40)
            };
            Controls.Add(panel);

            btnStart = new Button()
            {
                Text = "Spiel starten",
                Dock = DockStyle.Top,
                Height = 40,
                Margin = new Padding(0, 0, 0, 10),
                Font = new Font("Consolas", 12, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.White
            };
            btnStart.Click += (s, e) =>
            {
                using (var game = new GameForm())
                {
                    Hide();
                    game.ShowDialog();
                    Show();
                }
            };
            panel.Controls.Add(btnStart);

            btnSettings = new Button()
            {
                Text = "Einstellungen",
                Dock = DockStyle.Top,
                Height = 40,
                Margin = new Padding(0, 0, 0, 10),
                Font = new Font("Consolas", 12, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.White
            };
            btnSettings.Click += (s, e) =>
            {
                using (var set = new SettingsForm())
                {
                    set.ShowDialog(this);
                }
            };
            panel.Controls.Add(btnSettings);

            btnHighscore = new Button()
            {
                Text = "Highscore",
                Dock = DockStyle.Top,
                Height = 40,
                Margin = new Padding(0, 0, 0, 10),
                Font = new Font("Consolas", 12, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.White
            };
            btnHighscore.Click += (s, e) =>
            {
                using (var h = new HighscoreForm())
                {
                    h.ShowDialog(this);
                }
            };
            panel.Controls.Add(btnHighscore);

            btnExit = new Button()
            {
                Text = "Beenden",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Consolas", 12, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.White
            };
            btnExit.Click += (s, e) => Close();
            panel.Controls.Add(btnExit);
        }
    }
}
