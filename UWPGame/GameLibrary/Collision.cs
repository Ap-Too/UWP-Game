using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;

namespace GameLibrary
{
    public class Collision
    {
        public Collision() { }

        public bool CheckCollision(Rect rect1, Rect rect2)
        {
            rect1.Intersect(rect2);

            if (rect1.IsEmpty)
                return false;
            else return true;
        }
    }
}
