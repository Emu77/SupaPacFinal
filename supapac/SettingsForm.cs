using System;
using System.Windows.Forms;

namespace Supapac
{
    public class SettingsForm : Form
    {
        private RadioButton? rbEasy;
        private RadioButton? rbNormal;
        private RadioButton? rbHard;

        private RadioButton? rbSmall;
        private RadioButton? rbMedium;
        private RadioButton? rbLarge;

        private CheckBox? cbSound;

        private Button? btnOk;

        public SettingsForm()
        {
            Text = "Einstellungen";
            Width = 340;
            Height = 300;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            // Schwierigkeit
            GroupBox gbDiff = new GroupBox()
            {
                Text = "Schwierigkeit",
                Left = 10,
                Top = 10,
                Width = 300,
                Height = 90
            };

            rbEasy = new RadioButton() { Text = "Einfach", Left = 15, Top = 20, Width = 100 };
            rbNormal = new RadioButton() { Text = "Normal", Left = 15, Top = 40, Width = 100 };
            rbHard = new RadioButton() { Text = "Schwer", Left = 15, Top = 60, Width = 100 };

            gbDiff.Controls.Add(rbEasy);
            gbDiff.Controls.Add(rbNormal);
            gbDiff.Controls.Add(rbHard);

            // Boardgröße
            GroupBox gbScale = new GroupBox()
            {
                Text = "Spielfeldgröße",
                Left = 10,
                Top = 110,
                Width = 300,
                Height = 90
            };

            rbSmall = new RadioButton() { Text = "Klein", Left = 15, Top = 20, Width = 100 };
            rbMedium = new RadioButton() { Text = "Normal", Left = 15, Top = 40, Width = 100 };
            rbLarge = new RadioButton() { Text = "Groß", Left = 15, Top = 60, Width = 100 };

            gbScale.Controls.Add(rbSmall);
            gbScale.Controls.Add(rbMedium);
            gbScale.Controls.Add(rbLarge);

            // Sound
            cbSound = new CheckBox()
            {
                Text = "Sound aktiviert",
                Left = 20,
                Top = 210,
                Width = 200
            };

            // OK-Button
            btnOk = new Button()
            {
                Text = "OK",
                Left = 200,
                Top = 230,
                Width = 100
            };
            btnOk.Click += BtnOk_Click;

            Controls.Add(gbDiff);
            Controls.Add(gbScale);
            Controls.Add(cbSound);
            Controls.Add(btnOk);

            // aktuelle Werte laden
            switch (GameSettings.Difficulty)
            {
                case Difficulty.Easy:
                    if (rbEasy != null) rbEasy.Checked = true;
                    break;
                case Difficulty.Normal:
                    if (rbNormal != null) rbNormal.Checked = true;
                    break;
                case Difficulty.Hard:
                    if (rbHard != null) rbHard.Checked = true;
                    break;
            }

            switch (GameSettings.Scale)
            {
                case BoardScale.Small:
                    if (rbSmall != null) rbSmall.Checked = true;
                    break;
                case BoardScale.Normal:
                    if (rbMedium != null) rbMedium.Checked = true;
                    break;
                case BoardScale.Large:
                    if (rbLarge != null) rbLarge.Checked = true;
                    break;
            }

            if (cbSound != null) cbSound.Checked = GameSettings.SoundEnabled;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (rbEasy != null && rbEasy.Checked) GameSettings.Difficulty = Difficulty.Easy;
            else if (rbNormal != null && rbNormal.Checked) GameSettings.Difficulty = Difficulty.Normal;
            else if (rbHard != null && rbHard.Checked) GameSettings.Difficulty = Difficulty.Hard;

            if (rbSmall != null && rbSmall.Checked) GameSettings.Scale = BoardScale.Small;
            else if (rbMedium != null && rbMedium.Checked) GameSettings.Scale = BoardScale.Normal;
            else if (rbLarge != null && rbLarge.Checked) GameSettings.Scale = BoardScale.Large;

            if (cbSound != null) GameSettings.SoundEnabled = cbSound.Checked;

            Close();
        }
    }
}

