using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary
{
    public static class WindowSizer
    {
        public const double ASPECT = 640.0 / 360.0;

        public static double Xcenter => (Width - RenderWidth) / 2;

        public static double Ycenter => (Height - RenderHeight) / 2;

        public static double RenderWidth { get; set; } = 640;

        public static double RenderHeight { get; set; } = 360;

        public static double Width { get; set; } = 640;

        public static double Height { get; set; } = 360;

        public static void UpdateRenderSize()
        {
            if (Width / Height > ASPECT)
            {
                RenderHeight = Height;
                RenderWidth = RenderHeight * ASPECT;
            }
            else
            {
                RenderWidth = Width;
                RenderHeight = RenderWidth / ASPECT;
            }
        }
    }
}
