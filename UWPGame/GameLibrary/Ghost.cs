using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email.DataProvider;
using Windows.Foundation;

namespace GameLibrary
{
    public class Ghost : GameObject, IAnimate
    {
        #region Sprites
        CanvasBitmap[] Left;
        CanvasBitmap[] Right;
        CanvasBitmap[] Up;
        CanvasBitmap[] Down;
        CanvasBitmap[] Scared;
        #endregion Sprites

        #region Fields
        // float to track the speed of the ghosts
        float Speed;

        // int to declare the frames it takes to update the ghost.
        int aniSpeed = 15;

        // Bool to switch to true when the play uses their PowerPellet
        public bool Frozen = false;

        // Int to track the frames the ghost has been frozen for
        public int FROZEN_COUNT = 0;
        #endregion Fields

        public Ghost(double startX, double startY, Direction startDirection,
            CanvasBitmap[] left, CanvasBitmap[] right, CanvasBitmap[] up, CanvasBitmap[] down, CanvasBitmap[] scared, float speed)
            : base(startX, startY, startDirection)
        {
            Left = left;
            Right = right;
            Up = up;
            Down = down;
            Scared = scared;
            collisionMesh = new Rect(X, Y, 14, 14);
            Speed = speed;
        }

        public void Animate(int count, Direction direction)
        {
            // Check direction
            if (!Frozen)
            {
                switch (direction)
                {
                    case Direction.Left:
                        if (count <= aniSpeed || (count > aniSpeed * 2 && count <= aniSpeed * 3))
                            CurrentSprite = Left[0];
                        else if ((count > aniSpeed && count <= aniSpeed * 2) || count > aniSpeed * 3)
                            CurrentSprite = Left[1];
                        break;
                    case Direction.Right:
                        if (count <= aniSpeed || (count > aniSpeed * 2 && count <= aniSpeed * 3))
                            CurrentSprite = Right[0];
                        else if ((count > aniSpeed && count <= aniSpeed * 2) || count > aniSpeed * 3)
                            CurrentSprite = Right[1];
                        break;
                    case Direction.Up:
                        if (count <= aniSpeed || (count > aniSpeed * 2 && count <= aniSpeed * 3))
                            CurrentSprite = Up[0];
                        else if ((count > aniSpeed && count <= aniSpeed * 2) || count > aniSpeed * 3)
                            CurrentSprite = Up[1];
                        break;
                    case Direction.Down:
                        if (count <= aniSpeed || (count > aniSpeed * 2 && count <= aniSpeed * 3))
                            CurrentSprite = Down[0];
                        else if ((count > aniSpeed && count <= aniSpeed * 2) || count > aniSpeed * 3)
                            CurrentSprite = Down[1];
                        break;
                }
            }
            else
            {
                if (count <= aniSpeed)
                    CurrentSprite = Scared[0];
                else if (count > aniSpeed && count <= aniSpeed * 2)
                    CurrentSprite = Scared[1];
                else if (count > aniSpeed * 2 && count <= aniSpeed * 3)
                    CurrentSprite = Scared[2];
                else
                    CurrentSprite = Scared[3];
            }
            
        }

        private void Move(double delta, Rect playField, int Border, double playerX, double playerY)
        {
            double tempX = this.X;
            double tempY = this.Y;

            double diffX = Math.Abs(X - playerX);
            double diffY = Math.Abs(Y - playerY);

            if (diffX > diffY)
            {
                if (X - playerX > 0)
                {
                    tempX -= (double)(Speed * delta);
                    if (tempX > Border + 5)
                    {
                        if (!Frozen)    this.X = tempX;
                        direction = Direction.Left;
                    }
                }
                else if (X - playerX < 0)
                {
                    tempX += (double)(Speed * delta);
                    if (tempX < (playField.Width + Border + 4))
                    {
                        if (!Frozen)    this.X = tempX;
                        direction = Direction.Right;
                    }
                }
            }
            else
            {
                if (Y - playerY < 0)
                {
                    tempY += (double)(Speed * delta);
                    if (tempY < (playField.Height + Border + 3))
                    {
                        if (!Frozen)    this.Y = tempY;
                        direction = Direction.Down;
                    }
                }
                else if (Y - playerY > 0)
                {
                    tempY -= (double)(Speed * delta);
                    if (tempY > (Border + 5))
                    {
                        if (!Frozen)    this.Y = tempY;
                        direction = Direction.Up;
                    }
                }
            }
        }

        public void Update(int frameCount, double delta, Rect playField, int Border, double playerX, double playerY, bool playerAlive)
        {
            if (playerAlive)
            {
                // Move the ghost
                Move(delta, playField, Border, playerX, playerY);
                collisionMesh = new Rect(X, Y, 14, 14);
            }

            if (Frozen && FROZEN_COUNT != 180)
            {
                FROZEN_COUNT++;
            }
            else
            {
                Frozen = false;
                FROZEN_COUNT = 0;
            }

            // Change current sprite based on frame count and direction
            Animate(frameCount, this.direction);
        }
    }
}
