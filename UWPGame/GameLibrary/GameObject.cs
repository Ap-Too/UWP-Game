using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage;

namespace GameLibrary
{
    public abstract class GameObject
    {
        // public current loaded sprite
        public CanvasBitmap CurrentSprite;
        public double X, Y;
        public Direction direction;
        public Rect collisionMesh;

        //TODO: Constructor summary
        protected GameObject(double startX, double startY, Direction startDirection)
        {
            X = startX;
            Y = startY;
            direction = startDirection;
        }
    }
}
