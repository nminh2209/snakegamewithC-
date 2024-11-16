using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

public class SnakeGame : Form
{
    private List<Point> Snake = new List<Point>();
    private Point Food = new Point();
    private Random rand = new Random();
    private int score = 0;
    private int dirX, dirY;
    private bool isPaused = true;

    // Controls
    private Timer gameTimer = new Timer();
    private PictureBox gameCanvas = new PictureBox();
    private Label lblScore = new Label();
    private ComboBox cbDifficulty;
    private ComboBox cbMode;
    private Button btnStartPause;

    public SnakeGame()
    {
        // Form Settings
        Text = "Snake Game";
        Size = new Size(800, 600);

        // Score Label
        lblScore.Text = "Score: 0";
        lblScore.Location = new Point(10, 10);
        Controls.Add(lblScore);

        // Difficulty ComboBox
        cbDifficulty = new ComboBox
        {
            Items = { "Easy", "Medium", "Hard" },
            Location = new Point(10, 40),
            DropDownStyle = ComboBoxStyle.DropDownList,
            SelectedIndex = 0
        };
        Controls.Add(cbDifficulty);

        // Mode ComboBox
        cbMode = new ComboBox
        {
            Items = { "Classic", "Modern" },
            Location = new Point(10, 70),
            DropDownStyle = ComboBoxStyle.DropDownList,
            SelectedIndex = 0
        };
        Controls.Add(cbMode);

        // Start/Pause Button
        btnStartPause = new Button
        {
            Text = "Start",
            Location = new Point(10, 100)
        };
        btnStartPause.Click += (sender, e) =>
        {
            if (isPaused)
            {
                isPaused = false;
                btnStartPause.Text = "Pause";
                gameTimer.Start();
            }
            else
            {
                isPaused = true;
                btnStartPause.Text = "Start";
                gameTimer.Stop();
            }

            cbDifficulty.Enabled = isPaused;
            cbMode.Enabled = isPaused;
        };
        Controls.Add(btnStartPause);

        // Game Canvas
        gameCanvas.Size = new Size(600, 400);
        gameCanvas.Location = new Point(100, 100);
        gameCanvas.BackColor = Color.Black;
        gameCanvas.Paint += gameCanvas_Paint;
        Controls.Add(gameCanvas);

        // Game Timer
        gameTimer.Interval = 1000; // It will be updated based on difficulty
        gameTimer.Tick += Update;
        gameTimer.Stop(); // Don't start until the user clicks "Start"

        // Start a new game
        NewGame();
    }

    private void NewGame()
    {
        // Reset settings
        Snake.Clear();
        Snake.Add(new Point(10, 10));
        dirX = 1;
        dirY = 0;
        score = 0;
        lblScore.Text = "Score: " + score.ToString();

        // Place the food
        PlaceFood();

        // Update difficulty
        UpdateDifficulty();
    }

    private void UpdateDifficulty()
    {
        switch (cbDifficulty.SelectedItem.ToString())
        {
            case "Easy": gameTimer.Interval = 200; break;
            case "Medium": gameTimer.Interval = 100; break;
            case "Hard": gameTimer.Interval = 50; break;
        }
    }

    private void PlaceFood()
    {
        Food = new Point(rand.Next(gameCanvas.Width / 10), rand.Next(gameCanvas.Height / 10));
    }

    private void Update(object sender, EventArgs e)
    {
        // Update difficulty
        UpdateDifficulty();

        // Check for collision with self
        for (int i = 1; i < Snake.Count; i++)
            if (Snake[i].Equals(Snake[0])) EndGame();

        // Check for collision with food
        if (Snake[0].Equals(Food))
        {
            score++;
            lblScore.Text = "Score: " + score.ToString();
            // Add to the snake body
            Snake.Add(new Point());
            PlaceFood();
        }

        // Check for out of bounds
        if (cbMode.SelectedItem.ToString() == "Classic")
        {
            if (Snake[0].X < 0 || Snake[0].Y < 0 || Snake[0].X >= gameCanvas.Width / 10 || Snake[0].Y >= gameCanvas.Height / 10)
                EndGame();
        }
        else
        {
            // Wrap around logic
            if (Snake[0].X < 0) Snake[0] = new Point(gameCanvas.Width / 10 - 1, Snake[0].Y);
            else if (Snake[0].X >= gameCanvas.Width / 10) Snake[0] = new Point(0, Snake[0].Y);
            else if (Snake[0].Y < 0) Snake[0] = new Point(Snake[0].X, gameCanvas.Height / 10 - 1);
            else if (Snake[0].Y >= gameCanvas.Height / 10) Snake[0] = new Point(Snake[0].X, 0);
        }

        // Move the snake
        for (int i = Snake.Count - 1; i >= 0; i--)
        {
            if (i == 0)
                Snake[i] = new Point(Snake[i].X + dirX, Snake[i].Y + dirY);
            else
                Snake[i] = Snake[i - 1];
        }

        // Redraw the game
        gameCanvas.Invalidate();
    }

    private void EndGame()
    {
        gameTimer.Stop();
        isPaused = true;
        btnStartPause.Text = "Start";
        cbDifficulty.Enabled = true;
        cbMode.Enabled = true;
        MessageBox.Show("Game Over");
        NewGame();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (!isPaused)
        {
            switch (keyData)
            {
                case Keys.Up:
                    if (dirY != 1)
                    {
                        dirX = 0; dirY = -1;
                    }
                    break;
                case Keys.Down:
                    if (dirY != -1)
                    {
                        dirX = 0; dirY = 1;
                    }
                    break;
                case Keys.Left:
                    if (dirX != 1)
                    {
                        dirX = -1; dirY = 0;
                    }
                    break;
                case Keys.Right:
                    if (dirX != -1)
                    {
                        dirX = 1; dirY = 0;
                    }
                    break;
            }
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void gameCanvas_Paint(object sender, PaintEventArgs e)
    {
        Graphics canvas = e.Graphics;

        for (int i = 0; i < Snake.Count; i++)
            canvas.FillRectangle(Brushes.Green, new Rectangle(Snake[i].X * 10, Snake[i].Y * 10, 10, 10));

        canvas.FillRectangle(Brushes.Red, new Rectangle(Food.X * 10, Food.Y * 10, 10, 10));
    }
}
