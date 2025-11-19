using System.Drawing;
using System.Windows.Forms;

namespace Supapac
{
    public class HighscoreForm : Form
    {
        public HighscoreForm()
        {
            Text = "Highscore";
            Width = 300;
            Height = 180;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            Label lbl = new Label()
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Consolas", 12, FontStyle.Bold),
                Text = $"Bester Spieler:\n{HighscoreManager.BestName}\n\nScore: {HighscoreManager.BestScore}"
            };

            Controls.Add(lbl);
        }
    }
}
