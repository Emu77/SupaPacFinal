using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using supapac;


namespace supapac
{
    public class GameForm : Form
    {
        private readonly System.Windows.Forms.Timer gameTimer;
        private char[,] map = new char[1,1];
        private int rows;
        private int cols;
        private int tileSize;

        private Point pacman;
        private int pacmanDirX = 0;
        private int pacmanDirY = 0;
        private int desiredDirX = 0;
        private int desiredDirY = 0;

        // Für Blickrichtung (auch im Stand)
        private int lastDirX = 1;  // Start: nach rechts
        private int lastDirY = 0;

        private class Ghost
        {
            public Point Position;
            public Point StartPosition;
            public int DirX;
            public int DirY;
        }

        private readonly List<Ghost> ghosts = new List<Ghost>();
        private readonly Random rng = new Random();

        private int score = 0;
        private bool gameOver = false;

        // Power-Up / Geister verwundbar
        private bool ghostsFrightened = false;
        private int frightenedTicks = 0;
        private const int FrightenedDurationTicks = 60;

        // Mehrere Level
        private readonly List<string[]> levels = new List<string[]>();
        private int currentLevel = 0;

        // Animation
        private int animationTick = 0;

        public GameForm()
        {
            Text = "SupaPac";
            Text = "Pacman Deluxe";
            BackColor = Color.Black;
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            KeyPreview = true;
            StartPosition = FormStartPosition.CenterScreen;

            tileSize = GameSettings.TileSize;

            InitLevels();

            // Prüfe Level-Konnektivität automatisch (zeigt Ergebnis beim Start)
            ValidateLevelsConnectivity();

            LoadLevel(0);

            ClientSize = new Size(cols * tileSize, rows * tileSize + 40);

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = GameSettings.TimerInterval;
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            KeyDown += GameForm_KeyDown;
        }

        private void InitLevels()
        {
            // Level 1 – ziemlich einfach
            levels.Add(new[]
            {
                "#####################",
                "#o........#........o#",
                "#.###.###.#.###.###.#",
                "#...................#",
                "#.###.#.#####.#.###.#",
                "#.....#...#...#.....#",
                "#####.###.#.###.#####",
                "#   #.#   G   #.#   #",
                "#.# #.# ##### #.# #.#",
                "#.#.............#.#.#",
                "#.#.###.###.###.#.#.#",
                "#o....#...P...#....o#",
                "#####################"
            });

            // Level 2 – mehr Wände, enger
            levels.Add(new[]
            {
                "#####################",
                "#o..#........#..#..o#",
                "#.###.#####.###.###.#",
                "#.....#.....#.......#",
                "###.#.#.###.#.###.###",
                "#...#.#...#.#...#...#",
                "#.###.###.#.###.###.#",
                "#G  #.....P.....#  G#",
                "#.###.###.#.###.###.#",
                "#...#.#...#...#.#...#",
                "###.#.#.#####.#.#.###",
                "#o..#........#..#..o#",
                "#####################"
            });

            // Level 3 – schwer
            levels.Add(new[]
            {
                "#####################",
                "#o....#.......#....o#",
                "#.##.#.#####.#.##.###",
                "#....#...#...#......#",
                "####.###.#.###.######",
                "#....#...G...#......#",
                "#.####.##P##.####.#.#",
                "#......#...#......#.#",
                "######.#.#.#.######.#",
                "#o.....#.#.#.....o..#",
                "###.#####.#.#####.###",
                "#.........#.........#",
                "#####################"
            });
        }

        // Validiert, dass jedes Level vollständig verbunden ist (keine unerreichbaren Bereiche)
        private List<string> GetConnectivityProblems()
        {
            var problems = new List<string>();

            for (int li = 0; li < levels.Count; li++)
            {
                string[] level = levels[li];
                int h = level.Length;
                int w = level[0].Length;

                bool[,] visited = new bool[h, w];
                int totalWalkable = 0;
                Point start = new Point(-1, -1);

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        char c = level[y][x];
                        if (c != '#')
                        {
                            totalWalkable++;
                            if (start.X == -1)
                                start = new Point(x, y);
                        }
                    }
                }

                if (start.X == -1)
                {
                    problems.Add($"Level {li + 1}: keine begehbaren Felder gefunden");
                    continue;
                }

                // BFS
                var q = new Queue<Point>();
                q.Enqueue(start);
                visited[start.Y, start.X] = true;
                int reachable = 0;

                while (q.Count > 0)
                {
                    var p = q.Dequeue();
                    reachable++;

                    (int dx, int dy)[] dirs = { (1, 0), (-1, 0), (0, 1), (0, -1) };
                    foreach (var d in dirs)
                    {
                        int nx = p.X + d.dx;
                        int ny = p.Y + d.dy;
                        if (nx >= 0 && nx < w && ny >= 0 && ny < h && !visited[ny, nx])
                        {
                            if (level[ny][nx] != '#')
                            {
                                visited[ny, nx] = true;
                                q.Enqueue(new Point(nx, ny));
                            }
                        }
                    }
                }

                if (reachable != totalWalkable)
                {
                    problems.Add($"Level {li + 1}: erreichbar {reachable}/{totalWalkable} begehbare Felder");
                }
            }

            return problems;
        }

        private void ValidateLevelsConnectivity()
        {
            var problems = GetConnectivityProblems();

            if (problems.Count == 0)
            {
                MessageBox.Show(this, "Connectivity-Check: Alle Levels sind vollständig verbunden.", "Connectivity Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string msg = "Connectivity-Probleme gefunden:\n" + string.Join("\n", problems) + "\n\nMöchtest du automatisch reparieren lassen?";
            var res = MessageBox.Show(this, msg, "Connectivity Check - Probleme", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes)
            {
                RepairLevelsConnectivity();
                var remaining = GetConnectivityProblems();
                if (remaining.Count == 0)
                {
                    MessageBox.Show(this, "Automatische Reparatur erfolgreich: Alle Levels jetzt verbunden.", "Connectivity Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string rmsg = "Reparatur durchgeführt, aber noch Probleme:\n" + string.Join("\n", remaining);
                    MessageBox.Show(this, rmsg, "Connectivity Check - Probleme verbleibend", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show(this, "Keine Änderungen vorgenommen.", "Connectivity Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RepairLevelsConnectivity()
        {
            for (int li = 0; li < levels.Count; li++)
            {
                string[] level = levels[li];
                int h = level.Length;
                int w = level[0].Length;

                // convert to mutable grid
                char[,] grid = new char[h, w];
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        grid[y, x] = level[y][x];

                bool[,] visited = new bool[h, w];
                Point start = new Point(-1, -1);
                var walkableList = new List<Point>();
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        if (grid[y, x] != '#')
                        {
                            walkableList.Add(new Point(x, y));
                            if (start.X == -1) start = new Point(x, y);
                        }
                    }

                if (start.X == -1) continue; // nothing to do

                // mark reachable
                var q = new Queue<Point>();
                q.Enqueue(start);
                visited[start.Y, start.X] = true;
                while (q.Count > 0)
                {
                    var p = q.Dequeue();
                    (int dx, int dy)[] dirs = { (1, 0), (-1, 0), (0, 1), (0, -1) };
                    foreach (var d in dirs)
                    {
                        int nx = p.X + d.dx;
                        int ny = p.Y + d.dy;
                        if (nx >= 0 && nx < w && ny >= 0 && ny < h && !visited[ny, nx])
                        {
                            if (grid[ny, nx] != '#')
                            {
                                visited[ny, nx] = true;
                                q.Enqueue(new Point(nx, ny));
                            }
                        }
                    }
                }

                // collect unreachable walkable
                var unreachable = new List<Point>();
                var reachable = new List<Point>();
                foreach (var p in walkableList)
                {
                    if (visited[p.Y, p.X]) reachable.Add(p);
                    else unreachable.Add(p);
                }

                // connect each unreachable by carving straight path to nearest reachable
                foreach (var u in unreachable)
                {
                    // find nearest reachable by Manhattan distance
                    Point best = reachable.Count > 0 ? reachable[0] : start;
                    int bestDist = Math.Abs(u.X - best.X) + Math.Abs(u.Y - best.Y);
                    foreach (var r in reachable)
                    {
                        int d = Math.Abs(u.X - r.X) + Math.Abs(u.Y - r.Y);
                        if (d < bestDist)
                        {
                            bestDist = d;
                            best = r;
                        }
                    }

                    // carve path from u to best (step in x then y)
                    int cx = u.X;
                    int cy = u.Y;
                    while (cx != best.X)
                    {
                        if (grid[cy, cx] == '#') grid[cy, cx] = '.';
                        cx += Math.Sign(best.X - cx);
                    }
                    while (cy != best.Y)
                    {
                        if (grid[cy, cx] == '#') grid[cy, cx] = '.';
                        cy += Math.Sign(best.Y - cy);
                    }
                    if (grid[cy, cx] == '#') grid[cy, cx] = '.';

                    // mark newly carved path as reachable (so subsequent unreachable can connect to it)
                    var qq = new Queue<Point>();
                    if (!visited[u.Y, u.X]) { visited[u.Y, u.X] = true; reachable.Add(u); }
                    qq.Enqueue(u);
                    while (qq.Count > 0)
                    {
                        var p = qq.Dequeue();
                        (int dx, int dy)[] dirs = { (1, 0), (-1, 0), (0, 1), (0, -1) };
                        foreach (var d in dirs)
                        {
                            int nx = p.X + d.dx;
                            int ny = p.Y + d.dy;
                            if (nx >= 0 && nx < w && ny >= 0 && ny < h && !visited[ny, nx])
                            {
                                if (grid[ny, nx] != '#')
                                {
                                    visited[ny, nx] = true;
                                    reachable.Add(new Point(nx, ny));
                                    qq.Enqueue(new Point(nx, ny));
                                }
                            }
                        }
                    }
                }

                // write back to levels
                string[] newLevel = new string[h];
                for (int y = 0; y < h; y++)
                {
                    char[] row = new char[w];
                    for (int x = 0; x < w; x++) row[x] = grid[y, x];
                    newLevel[y] = new string(row);
                }
                levels[li] = newLevel;
            }
        }

        private void LoadLevel(int index)
        {
            if (index < 0 || index >= levels.Count)
                index = 0;

            currentLevel = index;

            string[] level = levels[index];
            rows = level.Length;
            cols = level[0].Length;

            ghosts.Clear();
            map = new char[rows, cols];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    char c = level[y][x];
                    if (c == 'P')
                    {
                        pacman = new Point(x, y);
                        map[y, x] = '.'; // darunter Punkt
                    }
                    else if (c == 'G')
                    {
                        var g = new Ghost
                        {
                            Position = new Point(x, y),
                            StartPosition = new Point(x, y),
                            DirX = 0,
                            DirY = 0
                        };
                        ghosts.Add(g);
                        map[y, x] = '.'; // darunter Punkt
                    }
                    else
                    {
                        map[y, x] = c;
                    }
                }
            }

            pacmanDirX = pacmanDirY = desiredDirX = desiredDirY = 0;
            lastDirX = 1; lastDirY = 0;
            ghostsFrightened = false;
            frightenedTicks = 0;
            gameOver = false;

            tileSize = GameSettings.TileSize;
            ClientSize = new Size(cols * tileSize, rows * tileSize + 40);
        }

        private void GameForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (gameOver)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    RestartGame();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    Close();
                }
                return;
            }

            // Pfeiltasten
            if (e.KeyCode == Keys.Up) { desiredDirX = 0; desiredDirY = -1; }
            else if (e.KeyCode == Keys.Down) { desiredDirX = 0; desiredDirY = 1; }
            else if (e.KeyCode == Keys.Left) { desiredDirX = -1; desiredDirY = 0; }
            else if (e.KeyCode == Keys.Right) { desiredDirX = 1; desiredDirY = 0; }

            // WASD
            else if (e.KeyCode == Keys.W) { desiredDirX = 0; desiredDirY = -1; }
            else if (e.KeyCode == Keys.S) { desiredDirX = 0; desiredDirY = 1; }
            else if (e.KeyCode == Keys.A) { desiredDirX = -1; desiredDirY = 0; }
            else if (e.KeyCode == Keys.D) { desiredDirX = 1; desiredDirY = 0; }

            else if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void RestartGame()
        {
            // ganzes Spiel neu: Score auf 0, Level 0
            score = 0;
            LoadLevel(0);
            gameTimer.Interval = GameSettings.TimerInterval;
            gameTimer.Start();
            animationTick = 0;
            Invalidate();
        }

        private void NextLevel()
        {
            if (currentLevel < levels.Count - 1)
            {
                currentLevel++;
                LoadLevel(currentLevel);
                gameTimer.Interval = GameSettings.TimerInterval;
                Invalidate();

                MessageBox.Show(this,
                    $"Level {currentLevel + 1} startet!\nScore: {score}",
                    "Nächstes Level",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                GameOver(true);
            }
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            if (gameOver) return;

            // Animations-Tick hochzählen
            animationTick++;
            if (animationTick > 1000000) animationTick = 0;

            // Wunschrichtung prüfen
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

                if (pacmanDirX != 0 || pacmanDirY != 0)
                {
                    lastDirX = pacmanDirX;
                    lastDirY = pacmanDirY;
                }
            }

            // Punkt oder Power-Up einsammeln
            char field = map[pacman.Y, pacman.X];
            if (field == '.')
            {
                map[pacman.Y, pacman.X] = ' ';
                score += 10;
                SoundManager.PlayChomp();
            }
            else if (field == 'o')
            {
                map[pacman.Y, pacman.X] = ' ';
                score += 50;
                ghostsFrightened = true;
                frightenedTicks = FrightenedDurationTicks;
                SoundManager.PlayPower();
            }

            // Power-Up ablaufen lassen
            if (ghostsFrightened)
            {
                frightenedTicks--;
                if (frightenedTicks <= 0)
                {
                    ghostsFrightened = false;
                }
            }

            // Geister bewegen
            MoveGhosts();

            // Kollision prüfen
            foreach (var ghost in ghosts)
            {
                if (ghost.Position == pacman)
                {
                    if (ghostsFrightened)
                    {
                        // Geist "fressen" -> zurück zum Start
                        ghost.Position = ghost.StartPosition;
                        ghost.DirX = ghost.DirY = 0;
                        score += 200;
                        SoundManager.PlayGhost();
                    }
                    else
                    {
                        GameOver(false);
                        return;
                    }
                }
            }

            // Haben wir alle Punkte gefressen?
            if (AllDotsCollected())
            {
                SoundManager.PlayWin();
                NextLevel();
                return;
            }

            Invalidate();
        }

        private bool AllDotsCollected()
        {
            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                    if (map[y, x] == '.' || map[y, x] == 'o')
                        return false;
            return true;
        }

        private bool CanMove(Point pos, int dx, int dy)
        {
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
                if (ghostsFrightened)
                {
                    MoveGhostRandom(ghost);
                }
                else
                {
                    MoveGhostChasing(ghost);
                }
            }
        }

        private void MoveGhostRandom(Ghost ghost)
        {
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

            if (possible.Count > 0)
            {
                var choice = possible[rng.Next(possible.Count)];
                ghost.DirX = choice.dx;
                ghost.DirY = choice.dy;
            }

            if (CanMove(ghost.Position, ghost.DirX, ghost.DirY))
            {
                ghost.Position = new Point(
                    ghost.Position.X + ghost.DirX,
                    ghost.Position.Y + ghost.DirY);
            }
        }

        // einfache Jagd-KI: Richtung wählen, die Pacman näher kommt
        private void MoveGhostChasing(Ghost ghost)
        {
            (int dx, int dy)[] directions =
            {
                (1,0), (-1,0), (0,1), (0,-1)
            };

            List<(int dx, int dy)> possible = new List<(int dx, int dy)>();
            foreach (var dir in directions)
            {
                if (CanMove(ghost.Position, dir.dx, dir.dy))
                    possible.Add(dir);
            }

            if (possible.Count == 0) return;

            // manchmal zufällig, damit es nicht zu berechenbar ist
            if (rng.NextDouble() < 0.2)
            {
                var choice = possible[rng.Next(possible.Count)];
                ghost.DirX = choice.dx;
                ghost.DirY = choice.dy;
            }
            else
            {
                int bestDist = int.MaxValue;
                int bestDx = 0, bestDy = 0;
                foreach (var dir in possible)
                {
                    int nx = ghost.Position.X + dir.dx;
                    int ny = ghost.Position.Y + dir.dy;
                    int dist = Math.Abs(nx - pacman.X) + Math.Abs(ny - pacman.Y);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestDx = dir.dx;
                        bestDy = dir.dy;
                    }
                }

                ghost.DirX = bestDx;
                ghost.DirY = bestDy;
            }

            if (CanMove(ghost.Position, ghost.DirX, ghost.DirY))
            {
                ghost.Position = new Point(
                    ghost.Position.X + ghost.DirX,
                    ghost.Position.Y + ghost.DirY);
            }
        }

        private void GameOver(bool win)
        {
            gameOver = true;
            gameTimer.Stop();
            Invalidate();

            if (!win)
                SoundManager.PlayDeath();
            else
                SoundManager.PlayWin();

            string msg = win
                ? $"Du hast alle Level geschafft!\nScore: {score}"
                : $"Game Over!\nScore: {score}";

            MessageBox.Show(this,
                msg + "\n\nENTER = Neustart, ESC = zurück zum Menü",
                "Pacman Deluxe",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // Highscore prüfen
            HighscoreManager.SetHighscore(HighscoreManager.BestName, score);
            HighscoreManager.TrySetHighscore(score, this);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Spielfeld
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    Rectangle tileRect = new Rectangle(x * tileSize, y * tileSize + 40, tileSize, tileSize);

                    g.FillRectangle(Brushes.Black, tileRect);

                    char c = map[y, x];
                    if (c == '#')
                    {
                        g.FillRectangle(Brushes.Blue, tileRect);
                    }
                    else if (c == '.')
                    {
                        int cx = tileRect.X + tileSize / 2;
                        int cy = tileRect.Y + tileSize / 2;
                        int r = Math.Max(3, tileSize / 8);
                        g.FillEllipse(Brushes.White, cx - r, cy - r, r * 2, r * 2);
                    }
                    else if (c == 'o')
                    {
                        int cx = tileRect.X + tileSize / 2;
                        int cy = tileRect.Y + tileSize / 2;
                        int r = Math.Max(6, tileSize / 4);
                        g.FillEllipse(Brushes.Gold, cx - r, cy - r, r * 2, r * 2);
                    }
                }
            }

            // Pacman zeichnen (animiert)
            Rectangle pacRect = new Rectangle(
                pacman.X * tileSize,
                pacman.Y * tileSize + 40,
                tileSize,
                tileSize);
            DrawPacman(g, pacRect);

            // Geister zeichnen (animiert)
            bool legsPhase = (animationTick % 8) < 4; // 2-Phasen-Benutzung
            foreach (var ghost in ghosts)
            {
                Rectangle ghostRect = new Rectangle(
                    ghost.Position.X * tileSize,
                    ghost.Position.Y * tileSize + 40,
                    tileSize,
                    tileSize);

                DrawGhost(g, ghostRect, ghostsFrightened, legsPhase);
            }

            // HUD
            using (Font f = new Font("Consolas", 12, FontStyle.Bold))
            {
                g.DrawString($"Score: {score}", f, Brushes.Yellow, 5, 5);
                g.DrawString($"Mode: {GameSettings.Difficulty}", f, Brushes.Yellow, 150, 5);
                g.DrawString($"Level: {currentLevel + 1}/{levels.Count}", f, Brushes.Yellow, 320, 5);

                if (gameOver)
                {
                    g.DrawString("ENTER = Neustart | ESC = Menü", f, Brushes.Red, 520, 5);
                }
            }
        }

        // ===================== ZEICHEN-HILFSMETHODEN =======================

        private void DrawPacman(Graphics g, Rectangle rect)
        {
            // Mund öffnet/schließt
            bool mouthOpen = (animationTick % 8) < 4;

            // Blickrichtung
            int dirX = lastDirX;
            int dirY = lastDirY;

            if (dirX == 0 && dirY == 0)
            {
                dirX = 1; dirY = 0; // Default nach rechts
            }

            float startAngle;
            if (dirX > 0)      // rechts
                startAngle = 30f;
            else if (dirX < 0) // links
                startAngle = 210f;
            else if (dirY > 0) // unten
                startAngle = 120f;
            else               // oben
                startAngle = 300f;

            float sweep = mouthOpen ? 300f : 360f;

            // wenn Mund zu -> ganzer Kreis
            if (!mouthOpen)
            {
                g.FillEllipse(Brushes.Yellow, rect);
            }
            else
            {
                g.FillPie(Brushes.Yellow, rect, startAngle, sweep);
            }
        }

        private void DrawGhost(Graphics g, Rectangle rect, bool frightened, bool legsPhase)
        {
            // Farben
            Brush bodyBrush = frightened ? Brushes.LightBlue : Brushes.Red;
            Brush eyeWhite = Brushes.White;
            Brush pupilBrush = frightened ? Brushes.DarkBlue : Brushes.Blue;

            int w = rect.Width;
            int h = rect.Height;

            // Kopf (halbkreis) + Körper
            Rectangle headRect = new Rectangle(rect.X, rect.Y, w, h * 2 / 3);
            Rectangle bodyRect = new Rectangle(rect.X, rect.Y + h / 3, w, h * 2 / 3);

            g.FillPie(bodyBrush, headRect, 0, 180);
            g.FillRectangle(bodyBrush, bodyRect);

            // Füßchen (wackelnd)
            int feetCount = 4;
            int footWidth = w / feetCount;
            int footHeight = h / 4;
            int baseY = rect.Bottom - footHeight;

            for (int i = 0; i < feetCount; i++)
            {
                int fx = rect.X + i * footWidth;
                int offset = ((i + (legsPhase ? 1 : 0)) % 2 == 0) ? 0 : footHeight / 2;
                Rectangle footRect = new Rectangle(fx, baseY + offset, footWidth, footHeight);
                g.FillEllipse(bodyBrush, footRect);
            }

            // Augen
            int eyeWidth = w / 5;
            int eyeHeight = h / 4;
            int eyeOffsetX = w / 5;
            int eyeOffsetY = h / 5;

            Rectangle leftEye = new Rectangle(
                rect.X + eyeOffsetX,
                rect.Y + eyeOffsetY,
                eyeWidth,
                eyeHeight);

            Rectangle rightEye = new Rectangle(
                rect.X + w - eyeOffsetX - eyeWidth,
                rect.Y + eyeOffsetY,
                eyeWidth,
                eyeHeight);

            g.FillEllipse(eyeWhite, leftEye);
            g.FillEllipse(eyeWhite, rightEye);

            // Pupillen leicht zur Mitte / nach unten
            int pupilWidth = eyeWidth / 2;
            int pupilHeight = eyeHeight / 2;

            Rectangle leftPupil = new Rectangle(
                leftEye.X + eyeWidth / 3,
                leftEye.Y + eyeHeight / 3,
                pupilWidth,
                pupilHeight);

            Rectangle rightPupil = new Rectangle(
                rightEye.X + eyeWidth / 3,
                rightEye.Y + eyeHeight / 3,
                pupilWidth,
                pupilHeight);

            g.FillEllipse(pupilBrush, leftPupil);
            g.FillEllipse(pupilBrush, rightPupil);
        }
    }
}