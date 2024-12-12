using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GameLibrary;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI;
using Windows.System;
using Microsoft.Graphics.Canvas.UI;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using System.Diagnostics;
using Windows.System.Diagnostics.Telemetry;
using System.Security.Cryptography;
using Windows.ApplicationModel.VoiceCommands;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

/* UWP Game Template
 * Created By: Melissa VanderLely
 * Modified By: Ben Allen
 */


namespace GameInterface
{
    public sealed partial class LoadLevel : Page
    {
        #region Event
        public EventHandler PlayerDead;
        #endregion Event

        #region Fields
        // A counter for the frames per second to help process different aspects of the game
        public int FRAME_COUNT = 0;

        // int to count the frames between a Ghost spawning
        private int GHOST_SPAWNRATE = 0;

        // private int to count how many ghosts the player has killed in the round
        private int GHOST_KILL_COUNT = 0;

        // private ints to store the min and max height of the play area
        private int minHeight, maxHeight;

        // Default Ghost speed
        private int GhostDefaultSpeed = 60;

        // Declaring a new map for the game to be played on
        private Map map = new Map();

        // Declaring a new player at the start of the game
        private Player player = new Player(312, 172, Direction.Right);

        private CanvasDevice device = CanvasDevice.GetSharedDevice();

        // An empty list of keys to hold the directions of movement the player wishes to take
        private List<VirtualKey> MoveKeys = new List<VirtualKey>();

        // An empty list of Keys to hold the direction of fire for pac-mans pellets
        private List<VirtualKey> ShootKeys = new List<VirtualKey>();

        // The canvas bitmap to hold the sprite for PacMan's projectiles
        private CanvasBitmap PelletSprite;

        // An empty list of projectiles, "Pellets", that will fill as the player fires
        // and empty as the Pellets interact with different aspects of the game
        private List<Projectile> Pellets = new List<Projectile>();

        // An empty list of Ghost, to store the spawned in enemies
        // the list will fill and empty as the Ghosts interact with the level
        private List<Ghost> Ghosts = new List<Ghost>();

        // An empty list of PowerUps, to store the powerups showing on the screen
        // for collection
        private List<PowerUp> LivePowerUps = new List<PowerUp>();

        // New instance of random for use
        private Random random = new Random();

        // instance of Collision to use for collision checks
        private Collision coll = new Collision();

        // Default score for start of round
        private int Score = 0;
        #endregion Fields

        #region Ghost Sprites
        // Blinky Sprite Sheets
        public CanvasBitmap[] Blinky_Left = new CanvasBitmap[2];
        public CanvasBitmap[] Blinky_Up = new CanvasBitmap[2];
        public CanvasBitmap[] Blinky_Right = new CanvasBitmap[2];
        public CanvasBitmap[] Blinky_Down = new CanvasBitmap[2];

        // Pinky Sprite Sheets
        public CanvasBitmap[] Pinky_Left = new CanvasBitmap[2];
        public CanvasBitmap[] Pinky_Up = new CanvasBitmap[2];
        public CanvasBitmap[] Pinky_Right = new CanvasBitmap[2];
        public CanvasBitmap[] Pinky_Down = new CanvasBitmap[2];

        // Inky Sprite Sheets
        public CanvasBitmap[] Inky_Left = new CanvasBitmap[2];
        public CanvasBitmap[] Inky_Up = new CanvasBitmap[2];
        public CanvasBitmap[] Inky_Right = new CanvasBitmap[2];
        public CanvasBitmap[] Inky_Down = new CanvasBitmap[2];

        // Clyde Sprite Sheets
        public CanvasBitmap[] Clyde_Left = new CanvasBitmap[2];
        public CanvasBitmap[] Clyde_Up = new CanvasBitmap[2];
        public CanvasBitmap[] Clyde_Right = new CanvasBitmap[2];
        public CanvasBitmap[] Clyde_Down = new CanvasBitmap[2];

        // Stunned Ghost Sprite
        public CanvasBitmap[] Stunned_Ghost = new CanvasBitmap[4];
        #endregion Ghost Sprites

        #region PowerUp Sprites
        public CanvasBitmap Apple;
        public CanvasBitmap Cherry;
        public CanvasBitmap PowerPellet;
        #endregion

        #region Constructor
        public LoadLevel()
        {
            ApplicationView.PreferredLaunchViewSize = new Size(640, 360);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            this.InitializeComponent();

            player.IsAlive = true;

            // Add key press event
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            // Add key release Event
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;

            minHeight = map.BORDER_SIZE + map.TILE_SIZE - 3;
            maxHeight = (int)map.PlayField.Height + map.BORDER_SIZE + map.TILE_SIZE - 4;  
        }
        #endregion Constructor

        #region Key Press Handlers
        private async void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            // Check for key held down, don't add more than one reference.
            if (args.KeyStatus.WasKeyDown)
                return;

            // Check for a shooting direction being pressed
            // Add to list of shooting direction if key is down
            if (args.VirtualKey == VirtualKey.Up)
                ShootKeys.Add(args.VirtualKey);
            if (args.VirtualKey == VirtualKey.Right)
                ShootKeys.Add(args.VirtualKey);
            if (args.VirtualKey == VirtualKey.Down)
                ShootKeys.Add(args.VirtualKey);
            if (args.VirtualKey == VirtualKey.Left)
                ShootKeys.Add(args.VirtualKey);

            // Check for a move key being pressed
            // Add to list of move keys if being pressed
            if (args.VirtualKey == VirtualKey.W)
                MoveKeys.Add(args.VirtualKey);
            if (args.VirtualKey == VirtualKey.D)
                MoveKeys.Add(args.VirtualKey);
            if (args.VirtualKey == VirtualKey.S)
                MoveKeys.Add(args.VirtualKey);
            if (args.VirtualKey == VirtualKey.A)
                MoveKeys.Add(args.VirtualKey);

            // Check for space bar being pressed
            // if the player is alive and has a Power Pellet power up freeze the ghosts and remove the Power Pellet from the player
            // If the player is dead restart the game
            if (args.VirtualKey == VirtualKey.Space)
                if (player.IsAlive && player.PowerPellets > 0)
                {
                    for (int i = 0; i < Ghosts.Count; i++)
                    {
                        Ghosts[i].Frozen = true;
                        Ghosts[i].FROZEN_COUNT = 0;
                    }
                    player.PowerPellets--;
                    player.powerUps.Remove(player.powerUps.First(n => n.Type == PowerType.PowerPellet));
                }
                else if (!player.IsAlive)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        PlayerDead?.Invoke(this, new EventArgs());
                    });
                    Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
                }
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            // Check for release of direction key
            // remove from the list of shooting direction if released
            if (args.VirtualKey == VirtualKey.Up)
                ShootKeys.Remove(args.VirtualKey);
            if (args.VirtualKey == VirtualKey.Right)
                ShootKeys.Remove(args.VirtualKey);
            if (args.VirtualKey == VirtualKey.Down)
                ShootKeys.Remove(args.VirtualKey);
            if (args.VirtualKey == VirtualKey.Left)
                ShootKeys.Remove(args.VirtualKey);

            // Check for release of a movement key
            // remove key from list of movement keys if released
            if (args.VirtualKey == VirtualKey.W)
                MoveKeys.Remove(args.VirtualKey);
            if (args.VirtualKey == VirtualKey.D)
                MoveKeys.Remove(args.VirtualKey);
            if (args.VirtualKey == VirtualKey.S)
                MoveKeys.Remove(args.VirtualKey);
            if (args.VirtualKey == VirtualKey.A)
                MoveKeys.Remove(args.VirtualKey);
        }
        #endregion Key Press Handlers

        #region Page Methods
        private void CanvasCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            // Call the async function to load sprite sheets
            args.TrackAsyncAction(LoadResources(sender).AsAsyncAction());
        }

        private void canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            // Maintain aspect ratio with screen resizing
            WindowSizer.UpdateRenderSize();

            // Set a Canvas render target to use later
            CanvasRenderTarget target = new CanvasRenderTarget(device, 640, 360, 96);

            // Draw all shapes to put on screen using the canvas render target
            using (CanvasDrawingSession ds = target.CreateDrawingSession())
            {
                // Clear the screen before drawing
                ds.Clear(Colors.Black);

                // Draw the room
                map.DrawRoom(ds);

                // Draw all the Pellets
                for (int i = 0; i < Pellets.Count(); i++)
                    ds.DrawImage(Pellets[i].Sprite, new Rect(Pellets[i].X, Pellets[i].Y, 2, 2));

                // Draw all the Ghosts
                if (Ghosts.Count() > 0)
                    for (int i = 0; i < Ghosts.Count(); i++)
                        ds.DrawImage(Ghosts[i].CurrentSprite, new Rect(Ghosts[i].X, Ghosts[i].Y, 14, 14));

                // Draw all the PowerUps active on screen
                if (LivePowerUps.Count() > 0)
                    for (int i = 0; i < LivePowerUps.Count(); i++)
                        ds.DrawImage(LivePowerUps[i].CurrentSprite, new Rect(LivePowerUps[i].X, LivePowerUps[i].Y, 13, 14));

                //Draw the player
                ds.DrawImage(player.CurrentSprite, new Rect(player.X, player.Y, 13, 13));

                // Draw the HighScore
                if (HighScore.Highscore > Score)
                    ds.DrawText("HighScore " + HighScore.Highscore.ToString(), 40, 10, Colors.White);
                else
                    ds.DrawText("HighScore " + Score.ToString(), 40, 10, Colors.White);

                // Draw the Current Score
                ds.DrawText("Score " + Score.ToString(), 460, 10, Colors.White);
                
                if (player.PowerPellets >= 1)
                    ds.DrawImage(PowerPellet, new Rect(40, 335, 14, 14));
                if (player.PowerPellets == 2)
                    ds.DrawImage(PowerPellet, new Rect(60, 335, 14, 14));

                if (!player.IsAlive)
                {
                    ds.DrawText("GAME OVER", 260, 70, Colors.White);
                    if (Score > HighScore.Highscore)
                        ds.DrawText("New High Score", 240, 90, Colors.White);
                    ds.DrawText("Space to Try Again...", 230, 325, Colors.White);
                }
            }

            // Draw the target image to the screen at the desired size
            args.DrawingSession.DrawImage(
                target,                                     // Canvas Render Target
                new Rect(                                   // How big to draw the image and where
                    WindowSizer.Xcenter,
                    WindowSizer.Ycenter,
                    WindowSizer.RenderWidth,
                    WindowSizer.RenderHeight
                ),
                new Rect(0, 0, 640, 360),                   // What to draw from the target source
                1,                                          // The Alpha dog
                CanvasImageInterpolation.NearestNeighbor    // Prevents Anti-Alising
            );
        }

        private void canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            // Update player stats as game continues
            player.Update(FRAME_COUNT, MoveKeys, args.Timing.ElapsedTime.TotalSeconds,
                map.PlayField, map.BORDER_SIZE, ShootKeys, sender);

            if (GHOST_SPAWNRATE == 60 && Ghosts.Count != 25 && player.IsAlive)
            {
                SpawnEnemy(player, 100f);
                GHOST_SPAWNRATE = 0;
            }
            else
                GHOST_SPAWNRATE++;

            // Update Pellet list
            for (int i = 0; i < Pellets.Count; i++)
                if (!Pellets[i].IsActive)
                    Pellets.Remove(Pellets[i]);

            // Update the ghosts
            for (int i = 0; i < Ghosts.Count(); i++)
            {
                Ghosts[i].Update(FRAME_COUNT, args.Timing.ElapsedTime.TotalSeconds, map.PlayField, map.BORDER_SIZE, player.X, player.Y, player.IsAlive);

                // Check if collided with player
                if (coll.CheckCollision(player.collisionMesh, Ghosts[i].collisionMesh))
                {
                    // Play death animation
                    player.IsAlive = false;

                }
            }

            // Add a pellet to the Pellet list if a ShootKey is present
            if (ShootKeys.Count > 0 && FRAME_COUNT % player.FIRE_RATE == 0 && FRAME_COUNT != 60)
                Pellets.Add(new Projectile(PelletSprite, player.X, player.Y, player.direction));

            // Move the pellets
            for (int i = 0; i < Pellets.Count; i++)
            {
                Pellets[i].Move(args.Timing.ElapsedTime.TotalSeconds, map.PlayField, map.BORDER_SIZE);

                for (int j = 0; j < Ghosts.Count(); j++)
                {
                    // Check if pellet hits a ghost
                    if (coll.CheckCollision(Pellets[i].collisionMesh, Ghosts[j].collisionMesh))
                    {
                        Pellets[i].Deactivate();
                        GHOST_KILL_COUNT++;
                        // Spawn power up when a count is hit
                        if (GHOST_KILL_COUNT % 30 == 0 && GHOST_KILL_COUNT != 0)
                        {
                            // Select the power up to spawn
                            PowerType temp = SelectPowerType();

                            // Add power up where the ghost was killed
                            if (temp != PowerType.none)
                                LivePowerUps.Add(new PowerUp(Ghosts[j].X, Ghosts[j].Y, temp, SelectPowerSprite(temp)));
                        }
                        Ghosts.Remove(Ghosts[j]);

                        // Add to score
                        Score += 100;
                    }
                }
            }

            // Check if the player collects a powerup
            for (int i = 0; i < LivePowerUps.Count; i++)
            {
                if (coll.CheckCollision(player.collisionMesh, LivePowerUps[i].collisionMesh))
                {
                    if (player.powerUps.Count(p => p.Type == PowerType.PowerPellet) < 2 && LivePowerUps[i].Type == PowerType.PowerPellet)
                        player.powerUps.Add(LivePowerUps[i]);
                    else if (LivePowerUps[i].Type != PowerType.PowerPellet)
                        player.powerUps.Add(LivePowerUps[i]);
                    LivePowerUps.Remove(LivePowerUps[i]);
                    Score += 100;
                }
            }

            // Remove PowerUp from screen
            for (int i = 0; i < LivePowerUps.Count; i++)
            {
                LivePowerUps[i].DeSpawn();
                if (!LivePowerUps[i].Active)
                    LivePowerUps.Remove(LivePowerUps[i]);
            }

            // Update Frame Count for animation
            if (FRAME_COUNT >= 59)
            {
                FRAME_COUNT = 0;
                Score += 10;
            }
            else
            {
                FRAME_COUNT++;
                for (int i = 0; i < LivePowerUps.Count; i++)
                    LivePowerUps[i].FRAME_COUNT++;
            }
        }

        private PowerType SelectPowerType()
        {
            int rng = random.Next(0, 29);

            switch (rng)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    // Speed up player
                    return PowerType.Apple;
                case 8:
                case 9:
                case 10:
                case 11:
                    // Add Powerpellet charge
                    return PowerType.PowerPellet;
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                    // Speed up fire rate
                    return PowerType.Cherry;
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                    return PowerType.none;
            }
            return PowerType.none;
        }

        private CanvasBitmap SelectPowerSprite(PowerType temp)
        {
            switch (temp)
            {
                case PowerType.Apple:
                    return Apple;
                case PowerType.Cherry:
                    return Cherry;
                case PowerType.PowerPellet:
                    return PowerPellet;
            }
            return null;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Set the render height to the height of the game window
            WindowSizer.RenderHeight = e.NewSize.Height;

            // Set the window height and width for rendering calculations
            WindowSizer.Width = e.NewSize.Width;
            WindowSizer.Height = e.NewSize.Height;
        }
        #endregion Page Methods

        #region Private Methods
        private async Task LoadResources(ICanvasResourceCreator sender)
        {
            // Load in sprite sheets for use in game
            await map.LoadSprites(sender);
            await player.LoadSpriteSheets(sender);
            PelletSprite = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Pellets/Pellet.png"));
            await LoadGhostSprites(sender);
            await LoadPowerUpSprites(sender);
        }

        private async Task LoadGhostSprites(ICanvasResourceCreator sender)
        {
            #region Blinky Sprites
            // Load in Blinky's Sprites
            Blinky_Left[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Red/Red_Left_1.png"));
            Blinky_Left[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Red/Red_Left_2.png"));

            Blinky_Right[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Red/Red_Right_1.png"));
            Blinky_Right[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Red/Red_Right_2.png"));

            Blinky_Up[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Red/Red_Up_1.png"));
            Blinky_Up[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Red/Red_Up_2.png"));

            Blinky_Down[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Red/Red_Down_1.png"));
            Blinky_Down[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Red/Red_Down_2.png"));
            #endregion Blinky Sprites

            #region Pinky Sprites
            // Load in Blinky's Sprites
            Pinky_Left[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Pink/Pink_Left_1.png"));
            Pinky_Left[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Pink/Pink_Left_2.png"));

            Pinky_Right[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Pink/Pink_Right_1.png"));
            Pinky_Right[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Pink/Pink_Right_2.png"));

            Pinky_Up[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Pink/Pink_Up_1.png"));
            Pinky_Up[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Pink/Pink_Up_2.png"));

            Pinky_Down[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Pink/Pink_Down_1.png"));
            Pinky_Down[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Pink/Pink_Down_2.png"));
            #endregion Pinky Sprites

            #region Inky Sprites
            // Load in Blinky's Sprites
            Inky_Left[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Blue/Blue_Left_1.png"));
            Inky_Left[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Blue/Blue_Left_2.png"));

            Inky_Right[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Blue/Blue_Right_1.png"));
            Inky_Right[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Blue/Blue_Right_2.png"));

            Inky_Up[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Blue/Blue_Up_1.png"));
            Inky_Up[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Blue/Blue_Up_2.png"));

            Inky_Down[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Blue/Blue_Down_1.png"));
            Inky_Down[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Blue/Blue_Down_2.png"));
            #endregion Inky Sprites

            #region Clyde Sprites
            // Load in Blinky's Sprites
            Clyde_Left[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Orange/Orange_Left_1.png"));
            Clyde_Left[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Orange/Orange_Left_2.png"));

            Clyde_Right[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Orange/Orange_Right_1.png"));
            Clyde_Right[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Orange/Orange_Right_2.png"));

            Clyde_Up[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Orange/Orange_Up_1.png"));
            Clyde_Up[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Orange/Orange_Up_2.png"));

            Clyde_Down[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Orange/Orange_Down_1.png"));
            Clyde_Down[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Orange/Orange_Down_2.png"));
            #endregion Clyde Sprites

            #region Stunned Ghost
            Stunned_Ghost[0] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Scared/Fright_1.png"));
            Stunned_Ghost[1] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Scared/Fright2_2.png"));
            Stunned_Ghost[2] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Scared/Fright_2.png"));
            Stunned_Ghost[3] = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/Ghosts/Scared/Fright2_1.png"));
            #endregion Stunned Ghost
        }

        private async Task LoadPowerUpSprites(ICanvasResourceCreator sender)
        {
            Apple = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/PowerUps/Apple.png"));
            Cherry = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/PowerUps/Cherry.png"));
            PowerPellet = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Sprites/PowerUps/PowerPellet.png"));
        }

        private void SpawnEnemy(Player player, float radius)
        {
            Ghost temp;
            double ghostX;
            double ghostY;

            CanvasBitmap[] Left = new CanvasBitmap[3];
            CanvasBitmap[] Right = new CanvasBitmap[3];
            CanvasBitmap[] Up = new CanvasBitmap[3];
            CanvasBitmap[] Down = new CanvasBitmap[3];
            do
            {
                // Generate a random angle in radians
                float angle = (float)(random.NextDouble() * 2 * Math.PI);

                // Generate a random distance outside the radius
                double distance = (float)(radius + random.NextDouble() * radius);

                // Calculate ghost position using polar coordinates
                ghostX = player.X + distance * (float)Math.Cos(angle);
                ghostY = player.Y + distance * (float)Math.Sin(angle);

                // Constrain the enemy position to the play area
                ghostX = Math.Clamp(ghostX, map.BORDER_SIZE + (map.TILE_SIZE / 2), map.WIDTH - map.BORDER_SIZE - (map.TILE_SIZE + (map.TILE_SIZE / 2)));
                ghostY = Math.Clamp(ghostY, map.BORDER_SIZE + (map.TILE_SIZE / 2), map.HEIGHT - map.BORDER_SIZE - (map.TILE_SIZE + (map.TILE_SIZE / 2)));

                int rng = random.Next(7);
                switch (rng)
                {
                    case 0:
                    case 1:
                    case 2:
                        Left = Blinky_Left;
                        Right = Blinky_Right;
                        Up = Blinky_Up;
                        Down = Blinky_Down;
                        break;
                    case 3:
                    case 4:
                        Left = Pinky_Left;
                        Right = Pinky_Right;
                        Up = Pinky_Up;
                        Down = Pinky_Down;
                        break;
                    case 5:
                    case 6:
                        Left = Inky_Left;
                        Right = Inky_Right;
                        Up = Inky_Up;
                        Down = Inky_Down;
                        break;
                    case 7:
                        Left = Clyde_Left;
                        Right = Clyde_Right;
                        Up = Clyde_Up;
                        Down = Clyde_Down;
                        break;
                }

                temp = new Ghost(ghostX,ghostY, Direction.Down,Left,Right,Up,Down, Stunned_Ghost, Math.Clamp((GhostDefaultSpeed + GHOST_KILL_COUNT / 20), 60, 90));
                
            } while (IsInsideRadius(player, temp, radius));
            
            Ghosts.Add(temp);
        }

        private bool IsInsideRadius(Player player, Ghost ghost, float radius)
        {
            double dx = ghost.X - player.X;
            double dy = ghost.Y - player.Y;
            double distanceSquared = dx * dx + dy * dy;
            return distanceSquared < (radius * radius);
        }

        private async void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            canvas = null;
            await SaveScore();
        }

        private async Task SaveScore()
        {
            if (Score > HighScore.Highscore)
                await HighScore.SaveHighScore(Score);   
        }
        #endregion Private Methods
    }
}
