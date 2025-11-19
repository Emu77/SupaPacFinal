namespace Supapac
{
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }

    public enum BoardScale
    {
        Small,
        Normal,
        Large
    }

    public static class GameSettings
    {
        public static Difficulty Difficulty { get; set; } = Difficulty.Normal;
        public static BoardScale Scale { get; set; } = BoardScale.Normal;

        public static bool SoundEnabled { get; set; } = true;

        public static int TimerInterval
        {
            get
            {
                switch (Difficulty)
                {
                    case Difficulty.Easy: return 190;
                    case Difficulty.Hard: return 110;
                    default: return 140;
                }
            }
        }

        public static int TileSize
        {
            get
            {
                switch (Scale)
                {
                    case BoardScale.Small: return 20;
                    case BoardScale.Large: return 28;
                    default: return 24;
                }
            }
        }
    }
}
