using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace GameLibrary
{
    /// <summary>
    /// 
    /// </summary>
    public class Projectile
    {
        // Bullet Sprite sheet
        public CanvasBitmap Sprite;

        public double X {  get; private set; }
        public double Y { get; private set; }
        public float Speed { get; private set; } = 180.0f;
        public Direction Direction { get; private set; }

        public bool IsActive { get; private set; } = true;

        public Rect collisionMesh;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="direction"></param>
        public Projectile(CanvasBitmap sprite, double startX, double startY, Direction direction)
        {
            Sprite = sprite;
            X = startX + 6.5;
            Y = startY + 6.5;
            Direction = direction;
            collisionMesh = new Rect(X,Y,2,2);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Move(double delta, Rect playField, int Border)
        {
            // Delete the bullet if not active
            if (!IsActive) return;

            double tempX = this.X;
            double tempY = this.Y;

            // Send the projectile in the direction the player is facing
            switch (Direction)
            {
                case Direction.Left:
                    tempX -= (double)(Speed * delta);
                    if (tempX > Border + 5)
                    {
                        this.X = tempX;
                        collisionMesh = new Rect(X, Y, 2, 2);
                    }
                    else Deactivate();
                    break;
                case Direction.Right:
                    tempX += (double)(Speed * delta);
                    if (tempX < (playField.Width + (Border * 2) - (Border / 2) - 2))
                    {
                        this.X = tempX;
                        collisionMesh = new Rect(X, Y, 2, 2);
                    }
                    else Deactivate();
                    break;
                case Direction.Up:
                    tempY -= (double)(Speed * delta);
                    if (tempY > (Border + 5))
                    {
                        this.Y = tempY;
                        collisionMesh = new Rect(X, Y, 2, 2);
                    }
                    else Deactivate();
                    break;
                case Direction.Down:
                    tempY += (double)(Speed * delta);
                    if (tempY < (playField.Height + (Border * 2) - (Border/2)) - 3)
                    {
                        this.Y = tempY;
                        collisionMesh = new Rect(X, Y, 2, 2);
                    }
                    else Deactivate();
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
