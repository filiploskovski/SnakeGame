using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();

        public Form1()
        {
            InitializeComponent();

            new Settings();
            gameTimer.Interval = 1000 / Settings.Speed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();
            StartGame();
        }

        private void UpdateScreen(object sender, EventArgs e)
        {
            if (Settings.GameOver)
            {
                if (Input.KeyPressed(Keys.Enter))
                    StartGame();
            }
            else
            {
                if (Input.KeyPressed(Keys.Right) && Settings.Direction != Direction.Left)
                    Settings.Direction = Direction.Right;
                else if(Input.KeyPressed(Keys.Left) && Settings.Direction != Direction.Right)
                    Settings.Direction = Direction.Left;
                else if (Input.KeyPressed(Keys.Up) && Settings.Direction != Direction.Down)
                    Settings.Direction = Direction.Up;
                else if (Input.KeyPressed(Keys.Down) && Settings.Direction != Direction.Up)
                    Settings.Direction = Direction.Down;

                MovePlayer();

            }
            pbCanvas.Invalidate();
        }

        private void StartGame()
        {
            lbGameOver.Visible = false;
            pbCanvas.BackColor = Color.Aqua;


            new Settings();
            Snake.Clear();

            var head = new Circle { X = 10, Y = 5 };
            Snake.Add(head);

            lbScore.Text = Settings.Score.ToString();
            GenerateFood();
        }

        private void GenerateFood()
        {
            var maxXPos = pbCanvas.Size.Width / Settings.Width;
            var maxYPos = pbCanvas.Size.Height / Settings.Height;

            var random = new Random();
            food = new Circle() { X = random.Next(0, maxXPos), Y = random.Next(0, maxYPos) };
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            var canvas = e.Graphics;

            if (!Settings.GameOver)
            {
                Brush snakeColor;

                for (var i = 0; i < Snake.Count; i++)
                {
                    snakeColor = i == 0 ? Brushes.Black : Brushes.Green;
                    
                    canvas.FillEllipse(snakeColor,new Rectangle(
                        Snake[i].X * Settings.Width, 
                        Snake[i].Y * Settings.Height,
                        Settings.Width,
                        Settings.Height));

                    canvas.FillEllipse(Brushes.Red, new Rectangle(
                        food.X * Settings.Width,
                        food.Y * Settings.Height,
                        Settings.Width,
                        Settings.Height));
                }
            }
            else
            {
                var gameOver = $"Game Over \nYourScore is: {Settings.Score} \n Press Enter to try again!";
                lbGameOver.Text = gameOver;
                lbGameOver.Visible = true;
            }
        }

        private void MovePlayer()
        {
            for (var i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (Settings.Direction)
                    {
                        case Direction.Right:
                            Snake[i].X++;
                            break;

                        case Direction.Left:
                            Snake[i].X--;
                            break;

                        case Direction.Up:
                            Snake[i].Y--;
                            break;

                        case Direction.Down:
                            Snake[i].Y++;
                            break;
                    }

                    var maxXPos = pbCanvas.Size.Width / Settings.Width;
                    var maxYPos = pbCanvas.Size.Height / Settings.Height;

                    if (Snake[i].X < 0 || Snake[i].Y < 0 || Snake[i].X >= maxXPos || Snake[i].Y >= maxYPos)
                    {
                        Die();
                    }

                    for (var j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X && Snake[i].Y == Snake[j].Y)
                        {
                            Die();
                        }
                    }

                    if (Snake[0].X == food.X && Snake[0].Y == food.Y)
                    {
                        Eat();
                    }


                }
                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
        }

        private void Eat()
        {
            var food = new Circle {X = Snake[Snake.Count - 1].X, Y = Snake[Snake.Count - 1].Y};
            Snake.Add(food);

            Settings.Score += Settings.Points;
            lbScore.Text = Settings.Score.ToString();

            GenerateFood();
        }

        private void Die()
        {
            Settings.GameOver = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode,true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }
    }
}
