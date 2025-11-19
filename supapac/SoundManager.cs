using System;
using System.IO;
using System.Media;

namespace Supapac
{
    public static class SoundManager
    {
        private static readonly string SoundPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sounds");

        private static SoundPlayer? Load(string fileName)
        {
            try
            {
                string path = Path.Combine(SoundPath, fileName);
                if (File.Exists(path))
                    return new SoundPlayer(path);
            }
            catch { }
            return null;
        }

        private static readonly SoundPlayer? sChomp = Load("chomp.wav");        // Punkt
        private static readonly SoundPlayer? sPower = Load("power.wav");        // Power-Up
        private static readonly SoundPlayer? sGhost = Load("ghost.wav");        // Geist gefressen
        private static readonly SoundPlayer? sDeath = Load("death.wav");        // Tod
        private static readonly SoundPlayer? sWin = Load("win.wav");            // Level/Spiel gewonnen

        private static void Play(SoundPlayer? sp)
        {
            if (!GameSettings.SoundEnabled || sp == null) return;
            try { sp.Play(); } catch { }
        }

        public static void PlayChomp() => Play(sChomp);
        public static void PlayPower() => Play(sPower);
        public static void PlayGhost() => Play(sGhost);
        public static void PlayDeath() => Play(sDeath);
        public static void PlayWin() => Play(sWin);
    }
}
