using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PacmanGame
{
    public class GameForm : Form
    {
        private readonly System.Windows.Forms.Timer gameTimer;
        private char[,]? map;
        private int rows;
        private int cols;
        private int tileSize = 24;

        private Point pacman = Point.Empty;
        private int pacmanDirX = 0;
        private int pacmanDirY = 0;
        private int desiredDirX = 0;
        private int desiredDirY = 0;

        private class Ghost
        {
            public Point Position;
            public int DirX;
            public int DirY;
        }

        private readonly List<Ghost> ghosts = new List<Ghost>();
        private readonly Random rng = new Random();

        private int score = 0;
        private bool gameOver = false;

        public GameForm()
        {
            // Grundlegende Form-Einstellungen
            this.Text = "Pacman (C# WinForms)";
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;   // gegen Flackern
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.KeyPreview = true;

            // Karte initialisieren
            InitLevel();

            // Clientgröße basierend auf Karte
            this.ClientSize = new Size(cols * tileSize, rows * tileSize + 40); // +40 für Score-Anzeige

            // Timer einrichten
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 150; // Millisekunden pro "Tick"
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            this.KeyDown += (object? sender, KeyEventArgs e) => GameForm_KeyDown(sender ?? new object(), e);
        }

        private void InitLevel()
        {
            // Einfaches Level-Layout
            // # = Wand, . = Punkt, ' ' = leer, P = Pacman, G = Geist
            string[] level =
            {
                "#####################",
                "#.........#.........#",
                "#.###.###.#.###.###.#",
                "#...................#",
                "#.###.#.#####.#.###.#",
                "#.....#...#...#.....#",
                "#####.###.#.###.#####",
                "#   #.#   G   #.#   #",
                "#.# #.# ##### #.# #.#",
                "#.#.............#.#.#",
                "#.#.###.###.###.#.#.#",
                "#.....#...P...#.....#",
                "#####################"
            };

            rows = level.Length;
            cols = level[0].Length;

            map = new char[rows, cols];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    char c = level[y][x];
                    if (c == 'P')
                    {
                        pacman = new Point(x, y);
                        map[y, x] = '.'; // Pacman startet auf einem Punkt
                    }
                    else if (c == 'G')
                    {
                        ghosts.Add(new Ghost
                        {
                            Position = new Point(x, y),
                            DirX = 0,
                            DirY = 0
                        });
                        map[y, x] = '.'; // Geist startet auch auf einem Punkt
                    }
                    else
                    {
                        map[y, x] = c;
                    }
                }
            }
        }

        private void GameForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (gameOver)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    // Neustart
                    RestartGame();
                }
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Up:
                    desiredDirX = 0;
                    desiredDirY = -1;
                    break;
                case Keys.Down:
                    desiredDirX = 0;
                    desiredDirY = 1;
                    break;
                case Keys.Left:
                    desiredDirX = -1;
                    desiredDirY = 0;
                    break;
                case Keys.Right:
                    desiredDirX = 1;
                    desiredDirY = 0;
                    break;
            }
        }

        private void RestartGame()
        {
            score = 0;
            pacmanDirX = pacmanDirY = desiredDirX = desiredDirY = 0;
            ghosts.Clear();
            gameOver = false;
            InitLevel();
            gameTimer.Start();
            Invalidate();
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            if (gameOver) return;

            // Erst Wunschrichtung testen (falls frei)
            if (CanMove(pacman, desiredDirX, desiredDirY))
            {
                pacmanDirX = desiredDirX;
                pacmanDirY = desiredDirY;
            }

            // Pacman bewegen
            if (CanMove(pacman, pacmanDirX, pacmanDirY))
            {
                pacman.X += pacmanDirX;
                pacman.Y += pacmanDirY;
            }

            // Punkt einsammeln
            if (map != null && map[pacman.Y, pacman.X] == '.')
            {
                map[pacman.Y, pacman.X] = ' ';
                score += 10;
            }

            // Geister bewegen
            MoveGhosts();

            // Kollision prüfen
            foreach (var ghost in ghosts)
            {
                if (ghost.Position == pacman)
                {
                    GameOver();
                    break;
                }
            }

            Invalidate(); // neu zeichnen
        }

        private bool CanMove(Point pos, int dx, int dy)
        {
            if (map == null) return false;

            int newX = pos.X + dx;
            int newY = pos.Y + dy;

            if (newX < 0 || newX >= cols || newY < 0 || newY >= rows)
                return false;

            return map[newY, newX] != '#';
        }

        private void MoveGhosts()
        {
            foreach (var ghost in ghosts)
            {
                // Liste der möglichen Richtungen
                List<(int dx, int dy)> possible = new List<(int dx, int dy)>();

                (int dx, int dy)[] directions =
                {
                    (1,0), (-1,0), (0,1), (0,-1)
                };

                foreach (var dir in directions)
                {
                    if (CanMove(ghost.Position, dir.dx, dir.dy))
                        possible.Add(dir);
                }

                // Wenn aktuelle Richtung blockiert oder zufällig
                if (!CanMove(ghost.Position, ghost.DirX, ghost.DirY) || rng.NextDouble() < 0.3)
                {
                    if (possible.Count > 0)
                    {
                        var choice = possible[rng.Next(possible.Count)];
                        ghost.DirX = choice.dx;
                        ghost.DirY = choice.dy;
                    }
                    else
                    {
                        ghost.DirX = 0;
                        ghost.DirY = 0;
                    }
                }

                // Bewegen
                if (CanMove(ghost.Position, ghost.DirX, ghost.DirY))
                {
                    ghost.Position = new Point(
                        ghost.Position.X + ghost.DirX,
                        ghost.Position.Y + ghost.DirY
                    );
                }
            }
        }

        private void GameOver()
        {
            gameOver = true;
            gameTimer.Stop();
            Invalidate();
            MessageBox.Show($"Game Over!\nScore: {score}\n\nDrücke ENTER zum Neustart.",
                "Pacman", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            if (map == null) return;

            // Spielfeld zeichnen
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    Rectangle tileRect = new Rectangle(x * tileSize, y * tileSize + 40, tileSize, tileSize);

                    // Hintergrund
                    g.FillRectangle(Brushes.Black, tileRect);

                    if (map != null && map[y, x] == '#')
                    {
                        g.FillRectangle(Brushes.Blue, tileRect);
                    }
                    else if (map != null && map[y, x] == '.')
                    {
                        int cx = tileRect.X + tileSize / 2;
                        int cy = tileRect.Y + tileSize / 2;
                        int r = 4;
                        g.FillEllipse(Brushes.White, cx - r, cy - r, r * 2, r * 2);
                    }
                }
            }

            // Pacman
            Rectangle pacRect = new Rectangle(
                pacman.X * tileSize,
                pacman.Y * tileSize + 40,
                tileSize,
                tileSize);

            g.FillEllipse(Brushes.Yellow, pacRect);

            // Geister
            foreach (var ghost in ghosts)
            {
                Rectangle ghostRect = new Rectangle(
                    ghost.Position.X * tileSize,
                    ghost.Position.Y * tileSize + 40,
                    tileSize,
                    tileSize);

                g.FillEllipse(Brushes.Red, ghostRect);
            }

            // Score und Status
            using (Font f = new Font("Consolas", 14, FontStyle.Bold))
            {
                g.DrawString($"Score: {score}", f, Brushes.Yellow, 5, 5);
                if (gameOver)
                {
                    g.DrawString("GAME OVER - ENTER für Neustart", f, Brushes.Red, 150, 5);
                }
            }
        }
    }
}
