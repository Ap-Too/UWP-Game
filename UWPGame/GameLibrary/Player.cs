using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email.DataProvider;
using Windows.Foundation;
using Windows.Media.PlayTo;
using Windows.System;

namespace GameLibrary
{
    public class Player : GameObject, IAnimate
    {
        #region SpriteSheet
        // Player Animation Spritesheets
        // Left Facing Animation
        CanvasBitmap[] Player_Left = new CanvasBitmap[3];

        // Right Facing Animation
        CanvasBitmap[] Player_Right = new CanvasBitmap[3];

        // Up Facing Animation
        CanvasBitmap[] Player_Up = new CanvasBitmap[3];

        // Down Facing Animation
        CanvasBitmap[] Player_Down = new CanvasBitmap[3];

        // Death animation
        CanvasBitmap[] Player_Death = new CanvasBitmap[13];
        #endregion SpirteSheet

        #region Fields
        // A list of PowerUps attached to the player object 
        public List<PowerUp> powerUps;

        // int to track animation speed
        private int aniSpeed = 10;

        // player current movement speed
        private float Speed = 120.0f;

        // Player Base Movement speed
        private float BASE_SPEED = 120.0f;

        // How many frames between PacMan shooting a pellet
        public int FIRE_RATE = 15;

        // base fire rate
        private int BASE_FIRE_RATE = 15;

        public int PowerPellets;

        // public bool to check if player is alive
        public bool IsAlive = true;
        #endregion Fields

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="point"></param>
        public Player(double x, double y, Direction point) : base(x,y,point)
        {
            powerUps = new List<PowerUp>();
            CurrentSprite = Player_Right[1];
            collisionMesh = new Rect(X, Y, 16, 16);
            powerUps.Add(new PowerUp(0, 0, PowerType.PowerPellet, Player_Left[0]));
        }
        #endregion Constructor

        #region Sprite Sheet Loader
        /// <summary>
        /// This method is used to load the players sprite sheets for animation purposes
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public async Task LoadSpriteSheets(ICanvasResourceCreator sender)
        {
            // Load player facing left sprite sheet
            Player_Left[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Player_0.png"));
            Player_Left[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Left_1.png"));
            Player_Left[2] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Left_2.png"));

            // Load player facing down sprite sheet
            Player_Down[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Player_0.png"));
            Player_Down[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Down_1.png"));
            Player_Down[2] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Down_2.png"));

            // Load player facing Right sprite sheet
            Player_Right[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Player_0.png"));
            Player_Right[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Right_1.png"));
            Player_Right[2] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Right_2.png"));

            // Load player facing up sprite sheet
            Player_Up[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Player_0.png"));
            Player_Up[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Up_1.png"));
            Player_Up[2] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Up_2.png"));

            // Load player death animation
            Player_Death[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Player_0.png"));
            Player_Death[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Death1.png"));
            Player_Death[2] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Death2.png"));
            Player_Death[3] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Death3.png"));
            Player_Death[4] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Death4.png"));
            Player_Death[5] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Death5.png"));
            Player_Death[6] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Death6.png"));
            Player_Death[7] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Death7.png"));
            Player_Death[8] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Death8.png"));
            Player_Death[9] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Death9.png"));
            Player_Death[10] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Death10.png"));
            Player_Death[11] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Death11.png"));
            Player_Death[12] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Player/Death12.png"));
        }
        #endregion Sprite Sheet Loader

        #region Private Methods
        private void Move(List<VirtualKey> moveKeys, double delta, Rect playField, int Border)
        {
            // Move the player according to keyboard inputs
            if (moveKeys.Count == 0)
                return;

            double tempX = this.X;
            double tempY = this.Y;

            // Move the player icon based on key input and delta time
            switch (moveKeys[0])
            {
                case VirtualKey.W:
                    tempY -= (double)(Speed * delta);
                    if (tempY > (Border + 5))
                        this.Y = tempY;
                    break;
                case VirtualKey.D:
                    tempX += (double)(Speed * delta);
                    if (tempX < (playField.Width + Border + 6))
                        this.X = tempX;
                    break;
                case VirtualKey.S:
                    tempY += (double)(Speed * delta);
                    if (tempY < (playField.Height + Border + 6))
                        this.Y = tempY;
                    break;
                case VirtualKey.A:
                    tempX -= (double)(Speed * delta);
                    if (tempX > Border + 5)
                        this.X = tempX;
                    break;
            }
        }

        private void Shoot(List<VirtualKey> shootKeys, int frameCount)
        {
            if (shootKeys.Count == 0)
                return;
            // Change the direction PacMan faces and shoots based on key inputs
            switch (shootKeys[0])
            {
                case VirtualKey.Left:
                    this.direction = Direction.Left;
                    Animate(frameCount, this.direction);
                    break;
                case VirtualKey.Right:
                    this.direction = Direction.Right;
                    Animate(frameCount, this.direction);
                    break;
                case VirtualKey.Up:
                    this.direction = Direction.Up;
                    Animate(frameCount, this.direction);
                    break;
                case VirtualKey.Down:
                    this.direction = Direction.Down;
                    Animate(frameCount, this.direction);
                    break;
            }
        }

        private void ApplyPowerUps()
        {
            int speedup = 0;
            int rateDcrease = 0;
            for (int i = 0; i < powerUps.Count; i++)
            {
                speedup = powerUps.Count(p => p.Type == PowerType.Apple);
                rateDcrease = powerUps.Count(p => p.Type == PowerType.Cherry);
                PowerPellets = powerUps.Count(p => p.Type == PowerType.PowerPellet);
            }

            if (speedup <= 20)
                Speed = BASE_SPEED + (BASE_SPEED * (0.01f * speedup));
            else
                Speed = BASE_SPEED + (BASE_SPEED * 0.2f);

            if (FIRE_RATE > 5)
                FIRE_RATE = BASE_FIRE_RATE - rateDcrease;
            else
                FIRE_RATE = 5;

            if (PowerPellets > 2)
                PowerPellets = 2;
        }
        #endregion Private Methods

        #region Public Methods
        public void Update(int frameCount, List<VirtualKey> moveKeys, double delta, Rect playField,
            int Border, List<VirtualKey> shootKeys, ICanvasAnimatedControl sender)
        {
            if (IsAlive)
            {
                // Apply Power ups to the player
                ApplyPowerUps();

                // Move the player
                Move(moveKeys, delta, playField, Border);
                collisionMesh = new Rect(X, Y, 16, 16);

                // Change player direction
                Shoot(shootKeys, frameCount);

                // Change current sprite
                Animate(frameCount, this.direction);
            }
            else
            {
                AnimateDeath(frameCount);
                if (frameCount == 59)
                {
                    CurrentSprite = Player_Death[12];
                    sender.Paused = true;
                }
            }
                
        }

        public void Animate(int count, Direction Point)
        {
            // Check facing direction
            switch (Point)
            {
                case Direction.Left:
                    if (count <= aniSpeed || (count > (aniSpeed * 3) && count <= (aniSpeed * 4)))
                        CurrentSprite = Player_Left[0];
                    else if (count > aniSpeed && count <= (aniSpeed * 2) || (count > (aniSpeed * 4) && count <= (aniSpeed * 5)))
                        CurrentSprite = Player_Left[1];
                    else
                        CurrentSprite = Player_Left[2];
                    break;
                case Direction.Right:
                    if (count <= aniSpeed || (count > (aniSpeed * 3) && count <= (aniSpeed * 4)))
                        CurrentSprite = Player_Right[0];
                    else if (count > aniSpeed && count <= (aniSpeed * 2) || (count > (aniSpeed * 4) && count <= (aniSpeed * 5)))
                        CurrentSprite = Player_Right[1];
                    else
                        CurrentSprite = Player_Right[2];
                    break;
                case Direction.Up:
                    if (count <= aniSpeed || (count > (aniSpeed * 3) && count <= (aniSpeed * 4)))
                        CurrentSprite = Player_Up[0];
                    else if (count > aniSpeed && count <= (aniSpeed * 2) || (count > (aniSpeed * 4) && count <= (aniSpeed * 5)))
                        CurrentSprite = Player_Up[1];
                    else
                        CurrentSprite = Player_Up[2];
                    break;
                case Direction.Down:
                    if (count <= aniSpeed || (count > (aniSpeed * 3) && count <= (aniSpeed * 4)))
                        CurrentSprite = Player_Down[0];
                    else if (count > aniSpeed && count <= (aniSpeed * 2) || (count > (aniSpeed * 4) && count <= (aniSpeed * 5)))
                        CurrentSprite = Player_Down[1];
                    else
                        CurrentSprite = Player_Down[2];
                    break;
            }
        }

        public void AnimateDeath(int count)
        {
            if (count <= 5)
                CurrentSprite = Player_Death[0];
            else if (count > 5 && count <= 10)
                CurrentSprite = Player_Death[1];
            else if (count > 10 && count <= 15)
                CurrentSprite = Player_Death[2];
            else if (count > 15 && count <= 20)
                CurrentSprite = Player_Death[3];
            else if (count > 20 && count <= 25)
                CurrentSprite = Player_Death[4];
            else if (count > 25 && count <= 30)
                CurrentSprite = Player_Death[5];
            else if (count > 30 && count <= 35)
                CurrentSprite = Player_Death[6];
            else if (count > 35 && count <= 40)
                CurrentSprite = Player_Death[7];
            else if (count > 40 && count <= 45)
                CurrentSprite = Player_Death[8];
            else if (count > 45 && count <= 50)
                CurrentSprite = Player_Death[9];
            else if (count > 50 && count <= 55)
                CurrentSprite = Player_Death[10];
            else if (count > 55)
                CurrentSprite = Player_Death[11];
        }
        #endregion Public Methods
    }
}
