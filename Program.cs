using System;
using System.Windows.Forms;

namespace supapac   // <--- WICHTIG: so lassen
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Einstellungen beim Start laden (falls SettingsIO vorhanden)
            SettingsIO.Load();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new StartForm());
        }
    }
}
