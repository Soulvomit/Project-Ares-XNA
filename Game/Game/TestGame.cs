using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine;
using TileEngine.LayerMap;
using TileEngine.Sprite;

namespace TestGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TestGame : GameEngine
    {
        SpriteBatch spriteBatch;

        public TestGame() : base(1280, 720)
        {
            base.GraphicsDeviceManager.IsFullScreen = false;
            //EngineGame.CurrentGameState = GameState.Debugging;
            base.CurrentInputType = InputType.MouseKeyboard;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //create a new SpriteBatch, which can be used to draw textures.
            base.SpriteBatch = new SpriteBatch(GraphicsDevice);
            //create krypton, link to this game, use shaders
            base.Maps.Add(new Map(this, "shaders/KryptonEffect"));
            //set currentMap
            GameEngine.CurrentMap = base.Maps[0];

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //use base.Content to load your game content here
            //load map0 layers and backbuffer
            base.Maps[0].CollisionLayer = CollisionLayer.FromFile("Content/Layers/Map1/collision.lyr");
            base.Maps[0].TacticalLayer = TileLayer.FromFile(Content, "Content/Layers/Map1/tactical_map.lyr");
            base.Maps[0].TacticalLayer.Alpha = 1.0f;
            base.Maps[0].Layers.Add(TileLayer.FromFile(Content, "Content/Layers/Map1/map1.lyr"));
            base.Maps[0].Layers[0].Alpha = 1f;

            //player setup
            AnimatedSprite pas = new AnimatedSprite(base.Content.Load<Texture2D>("sprites/ship"), null);
            pas.AddAnimation(1, 64, 64, 0, 0, 1, "Idle");
            pas.Animations["Idle"].UpdateType = UpdateType.Forward;
            pas.CurrentAnimationName = "Idle";
            PlayerObject player = new PlayerObject("Player", new Vector2(0, 0), pas, base.Content.Load<Texture2D>("particles/particle_base"));
            player.CurrentMap = CurrentMap;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //input and paused user interface logic goes here
            TileEngine.GameEngine.NewKeyState = Keyboard.GetState();

            if (TileEngine.GameEngine.CurrentGameState != GameState.Paused)
            {
                //game logic goes here
                TileEngine.GameEngine.CurrentMap.Update(gameTime);
                //camera update
                Camera.LockToTarget(((AnimatedSprite)TileEngine.GameEngine.CurrentMap.Player.Sprite), base.Resolution.X, base.Resolution.Y);
                Camera.ClampToArea(TileEngine.GameEngine.CurrentMap.CollisionLayer.WidthInPixels - base.Resolution.X, TileEngine.GameEngine.CurrentMap.CollisionLayer.HeightInPixels - base.Resolution.Y);

                //handle gameobject collisions
                if (CurrentMap.GameObjectsEarly.Count != 0)
                {
                    for (int i = 0; i < CurrentMap.GameObjectsEarly.Count; i++)
                    {
                        //GameObject.HandleCollision((GameObject)CurrentMap.GameObjectsEarly[i], (GameObject)CurrentMap.Player);

                        //for (int j = 0; j < CurrentMap.GameObjectsEarly.Count; j++)
                        //{
                        //    GameObject.HandleCollision((GameObject)CurrentMap.GameObjectsEarly[i], (GameObject)CurrentMap.GameObjectsEarly[j]);
                        //}
                    }
                }
            }
            base.UpdateGameStateControls();
            //debugging code goes here
            if (TileEngine.GameEngine.CurrentGameState == GameState.Debugging)
                base.Maps[0].UseTacticalLayer = true;
            else
                base.Maps[0].UseTacticalLayer = false;
            base.TickFrameCounter(gameTime, " - NO MESSAGE");
            //update the base code
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            //draw layered tiles
            GameEngine.CurrentMap.Draw(base.SpriteBatch, Camera, gameTime, base.GraphicsDevice);

            base.Draw(gameTime);
        }
    }
}
