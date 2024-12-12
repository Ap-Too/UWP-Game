using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace GameLibrary
{
    public class Map
    {
        #region Sprites
        // Bitmaps for the outside borders of the map
        private CanvasBitmap TopLeft_Corner, TopRight_Corner, BottomLeft_Corner, BottomRight_Corner;
        private CanvasBitmap LeftWall, RightWall, BottomWall, TopWall;
        #endregion Sprites

        #region Fields
        public int TILE_SIZE = 8;
        public int BORDER_SIZE = 40;
        public int WIDTH = 640;
        public int HEIGHT = 360;
        public Rect PlayField;
        #endregion Fields

        #region Constuctor
        public Map()
        {
            PlayField = new Rect((BORDER_SIZE+TILE_SIZE), (BORDER_SIZE+TILE_SIZE), (WIDTH-((BORDER_SIZE+TILE_SIZE)*2)), (HEIGHT - ((BORDER_SIZE + TILE_SIZE) * 2)));
        }
        #endregion Constructor

        #region SpriteLoader
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public async Task LoadSprites(ICanvasResourceCreator sender)
        {
            // Load Outside borders
            TopLeft_Corner = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Wall/Corner_TopLeft.png"));
            TopRight_Corner = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Wall/Corner_TopRight.png"));
            BottomLeft_Corner = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Wall/Corner_BottomLeft.png"));
            BottomRight_Corner = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Wall/Corner_BottomRight.png"));
            LeftWall = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Wall/VerticalWallLeft.png"));
            RightWall = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Wall/VerticalWallRight.png"));
            TopWall = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Wall/HorizontalWall_Top.png"));
            BottomWall = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Wall/HorizontalWall_Bottom.png"));
        }
        #endregion SpriteLoader

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        public void DrawRoom(CanvasDrawingSession ds)
        {
            // Draw the background colour
            ds.DrawRectangle(0, 0, WIDTH, HEIGHT, Colors.Black);

            // Draw the outside walls
            // Corners
            ds.DrawImage(TopLeft_Corner, BORDER_SIZE, BORDER_SIZE); // Draws the top left Corner
            ds.DrawImage(TopRight_Corner, (WIDTH - BORDER_SIZE), BORDER_SIZE); // Draws the top right corner
            ds.DrawImage(BottomLeft_Corner, BORDER_SIZE, HEIGHT - BORDER_SIZE); // Draws the bottom left corner
            ds.DrawImage(BottomRight_Corner, (WIDTH - BORDER_SIZE), HEIGHT - BORDER_SIZE); // Draws the bottom right corner

            // walls
            // Draw the Top Wall
            for (int i = 1; i < ((WIDTH - (BORDER_SIZE * 2) - (TILE_SIZE*2)) / TILE_SIZE) + 2; i++)
                ds.DrawImage(TopWall, (BORDER_SIZE + (i * TILE_SIZE)), BORDER_SIZE);

            // Draw the Bottom Wall
            for (int i =  1; i < ((WIDTH - (BORDER_SIZE * 2) - (TILE_SIZE * 2)) / TILE_SIZE) + 2; i++)
                ds.DrawImage(BottomWall, (BORDER_SIZE + (i * TILE_SIZE)), HEIGHT - BORDER_SIZE);

            // Draw the right wall
            for (int i = 1; i < ((HEIGHT - (BORDER_SIZE * 2) - (TILE_SIZE * 2)) / TILE_SIZE) + 2; i++)
                ds.DrawImage(RightWall, WIDTH - BORDER_SIZE, (BORDER_SIZE + (i * TILE_SIZE)));

            // Draw the Left Wall
            for (int i = 1; i < ((HEIGHT - (BORDER_SIZE * 2) - (TILE_SIZE * 2)) / TILE_SIZE) + 2; i++)
                ds.DrawImage( LeftWall, BORDER_SIZE, (BORDER_SIZE + (i * TILE_SIZE)));
        }
        #endregion Public Methods
    }
}
