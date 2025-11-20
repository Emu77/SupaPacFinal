using System;
using System.Windows.Forms;

namespace supapac
{
    public class SettingsForm : Form
    {
        private RadioButton rbEasy;
        private RadioButton rbNormal;
        private RadioButton rbHard;

        private RadioButton rbSmall;
        private RadioButton rbMedium;
        private RadioButton rbLarge;

        private CheckBox cbSound;

        private Button btnOk;
        private Button btnExport;
        private Button btnImport;

        public SettingsForm()
        {
            Text = "Einstellungen";
            Width = 360;
            Height = 330;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            // === Gruppe: Schwierigkeit ===
            GroupBox gbDiff = new GroupBox()
            {
                Text = "Schwierigkeit",
                Left = 10,
                Top = 10,
                Width = 320,
                Height = 90
            };

            rbEasy = new RadioButton() { Text = "Einfach", Left = 15, Top = 20, Width = 100 };
            rbNormal = new RadioButton() { Text = "Normal", Left = 15, Top = 40, Width = 100 };
            rbHard = new RadioButton() { Text = "Schwer", Left = 15, Top = 60, Width = 100 };

            gbDiff.Controls.Add(rbEasy);
            gbDiff.Controls.Add(rbNormal);
            gbDiff.Controls.Add(rbHard);

            // === Gruppe: Spielfeldgröße ===
            GroupBox gbScale = new GroupBox()
            {
                Text = "Spielfeldgröße",
                Left = 10,
                Top = 110,
                Width = 320,
                Height = 90
            };

            rbSmall = new RadioButton() { Text = "Klein", Left = 15, Top = 20, Width = 100 };
            rbMedium = new RadioButton() { Text = "Normal", Left = 15, Top = 40, Width = 100 };
            rbLarge = new RadioButton() { Text = "Groß", Left = 15, Top = 60, Width = 100 };

            gbScale.Controls.Add(rbSmall);
            gbScale.Controls.Add(rbMedium);
            gbScale.Controls.Add(rbLarge);

            // === Sound ===
            cbSound = new CheckBox()
            {
                Text = "Sound aktiviert",
                Left = 20,
                Top = 210,
                Width = 200
            };

            // === Buttons ===
            btnOk = new Button()
            {
                Text = "OK",
                Left = 230,
                Top = 250,
                Width = 100
            };
            btnOk.Click += BtnOk_Click;

            btnExport = new Button()
            {
                Text = "Exportieren...",
                Left = 20,
                Top = 250,
                Width = 100
            };
            btnExport.Click += BtnExport_Click;

            btnImport = new Button()
            {
                Text = "Importieren...",
                Left = 130,
                Top = 250,
                Width = 100
            };
            btnImport.Click += BtnImport_Click;

            Controls.Add(gbDiff);
            Controls.Add(gbScale);
            Controls.Add(cbSound);
            Controls.Add(btnOk);
            Controls.Add(btnExport);
            Controls.Add(btnImport);

            // aktuelle Werte aus GameSettings laden
            switch (GameSettings.Difficulty)
            {
                case Difficulty.Easy: rbEasy.Checked = true; break;
                case Difficulty.Normal: rbNormal.Checked = true; break;
                case Difficulty.Hard: rbHard.Checked = true; break;
            }

            switch (GameSettings.Scale)
            {
                case BoardScale.Small: rbSmall.Checked = true; break;
                case BoardScale.Normal: rbMedium.Checked = true; break;
                case BoardScale.Large: rbLarge.Checked = true; break;
            }

            cbSound.Checked = GameSettings.SoundEnabled;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            // Schwierigkeit übernehmen
            if (rbEasy.Checked) GameSettings.Difficulty = Difficulty.Easy;
            else if (rbNormal.Checked) GameSettings.Difficulty = Difficulty.Normal;
            else if (rbHard.Checked) GameSettings.Difficulty = Difficulty.Hard;

            // Spielfeldgröße übernehmen
            if (rbSmall.Checked) GameSettings.Scale = BoardScale.Small;
            else if (rbMedium.Checked) GameSettings.Scale = BoardScale.Normal;
            else if (rbLarge.Checked) GameSettings.Scale = BoardScale.Large;

            // Sound übernehmen
            GameSettings.SoundEnabled = cbSound.Checked;

            // Einstellungen speichern (falls SettingsIO genutzt wird)
            SettingsIO.Save();

            Close();
        }

        private void BtnExport_Click(object? sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog();
            sfd.Filter = "SupaPac Einstellungen (*.json)|*.json";
            sfd.Title = "Einstellungen exportieren";

            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                SettingsIO.Export(sfd.FileName);
                MessageBox.Show(this,
                    "Einstellungen erfolgreich exportiert!",
                    "Export",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void BtnImport_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "SupaPac Einstellungen (*.json)|*.json";
            ofd.Title = "Einstellungen importieren";

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                SettingsIO.Import(ofd.FileName);
                MessageBox.Show(this,
                    "Einstellungen erfolgreich importiert!\nBitte das Einstellungsfenster schließen und ggf. das Spiel neu starten.",
                    "Import",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // UI aktualisieren
                switch (GameSettings.Difficulty)
                {
                    case Difficulty.Easy: rbEasy.Checked = true; break;
                    case Difficulty.Normal: rbNormal.Checked = true; break;
                    case Difficulty.Hard: rbHard.Checked = true; break;
                }

                switch (GameSettings.Scale)
                {
                    case BoardScale.Small: rbSmall.Checked = true; break;
                    case BoardScale.Normal: rbMedium.Checked = true; break;
                    case BoardScale.Large: rbLarge.Checked = true; break;
                }

                cbSound.Checked = GameSettings.SoundEnabled;
            }
        }
    }
}
