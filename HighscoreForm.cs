
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Supapac
{
    public class HighscoreForm : Form
    {
        public HighscoreForm()
        {
            Text = "Highscores";
            Width = 360;
            Height = 260;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            var hs = new HighscoreManager();

            // ListView für Tabelle: Platz | Name | Score
            var list = new ListView
            {
                View = View.Details,
                Dock = DockStyle.Fill,
                FullRowSelect = true,
                GridLines = true
            };

            list.Columns.Add("Platz", 60, HorizontalAlignment.Left);
            list.Columns.Add("Name", 180, HorizontalAlignment.Left);
            list.Columns.Add("Score", 80, HorizontalAlignment.Right);

            int rank = 1;
            foreach (var entry in hs.Highscores.OrderByDescending(h => h.Score))
            {
                var item = new ListViewItem(rank.ToString());
                item.SubItems.Add(entry.Name);
                item.SubItems.Add(entry.Score.ToString());
                list.Items.Add(item);
                rank++;
            }

            // Panel für Export-Button
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 45,
                Padding = new Padding(8)
            };

            var btnExport = new Button
            {
                Text = "Als CSV exportieren...",
                Dock = DockStyle.Right,
                Width = 160
            };

            btnExport.Click += (s, e) =>
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Title = "Highscores als CSV speichern";
                    sfd.Filter = "CSV-Datei (*.csv)|*.csv|Alle Dateien (*.*)|*.*";
                    sfd.FileName = "highscores.csv";

                    if (sfd.ShowDialog(this) == DialogResult.OK)
                    {
                        hs.ExportToCsv(sfd.FileName);
                        MessageBox.Show(
                            this,
                            "Highscores wurden erfolgreich als CSV gespeichert.",
                            "Export abgeschlossen",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
            };

            bottomPanel.Controls.Add(btnExport);

            Controls.Add(list);
            Controls.Add(bottomPanel);
        }
    }
}
