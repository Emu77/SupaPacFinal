using System.Media;

namespace supapac
{
    /// <summary>
    /// Einfache Sound-Verwaltung für SupaPac.
    /// Aktuell ohne externe Dateien – du kannst später echte Sounds einbauen.
    /// </summary>
    internal static class SoundManager
    {
        public static void PlayChomp()
        {
            // TODO: Echte Sounds einbauen – vorerst nur ein kurzer Beep
            SystemSounds.Beep.Play();
        }

        public static void PlayPower()
        {
            SystemSounds.Asterisk.Play();
        }

        public static void PlayGhost()
        {
            SystemSounds.Exclamation.Play();
        }

        public static void PlayWin()
        {
            SystemSounds.Asterisk.Play();
        }

        public static void PlayDeath()
        {
            SystemSounds.Hand.Play();
        }
    }
}
