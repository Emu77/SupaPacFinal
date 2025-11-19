using System;
using System.Drawing;
using System.Windows.Forms;

namespace supapac;

public class GameForm : Form
{
    public GameForm()
    {
        Text = "SupaPac – Spiel";
        Width = 800;
        Height = 600;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.Black;

        var lbl = new Label
        {
            Text = "Hier kommt dein SupaPac-Spiel hin 🙂",
            ForeColor = Color.Yellow,
            BackColor = Color.Black,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Consolas", 16, FontStyle.Bold)
        };

        Controls.Add(lbl);
    }
}
