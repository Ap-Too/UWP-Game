using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace GameLibrary
{
    public class PowerUp
    {
        public CanvasBitmap CurrentSprite;
        public PowerType Type;
        public double X, Y;
        public Rect collisionMesh;
        public int FRAME_COUNT = 0;
        private int MAX_FRAMES = 600;
        public bool Active = true;

        public PowerUp(double startX, double startY, PowerType type, CanvasBitmap sprite)
        {
            X = startX;
            Y = startY;
            Type = type;
            collisionMesh = new Rect(X, Y, 13, 14);
            CurrentSprite = sprite;
        }    

        public void DeSpawn()
        {
            if (FRAME_COUNT == MAX_FRAMES)
                Active = false;
        }
    }
}
