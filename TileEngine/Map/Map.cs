using System.Collections.Generic;
using Krypton;
using Krypton.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine.LayerMap
{
    public class Map
    {
        #region Fields
        //bacground music
        private SoundEffectInstance musicInstance;
        //player
        public IMapObject Player;
        //krypton light engine
        public KryptonEngine Krypton;
        //mapped gameobjects
        public List<IMapObject> GameObjectsEarly = new List<IMapObject>();
        //mapped gameobjects to be drawn late
        public List<IMapObject> GameObjectsLate = new List<IMapObject>();
        //collection of the map layers
        public List<TileLayer> Layers = new List<TileLayer>();
        //collision layer of the map
        public CollisionLayer CollisionLayer = null;
        //tactical layer
        public TileLayer TacticalLayer = null;
        //water layer
        public TileLayer WaterLayer = null;
        //background image
        public Texture2D Background;
        //clear color
        public Color BackBufferColor = Color.DarkGray;
        //ambient color
        public Color AmbientColor = new Color(10, 10, 10, 255);
        //public Color AmbientColor = Color.White;
        #endregion

        #region Properties
        public bool UseTacticalLayer { get; set; }
        public bool UseBackground { get; set; }

        public SoundEffect MusicInstance
        {
            set
            {
                if (musicInstance != null)
                    musicInstance.Stop();
                musicInstance = value.CreateInstance();
                musicInstance.IsLooped = true;
                musicInstance.Play();
            }
        }
        #endregion

        #region Constructors
        public Map()
        {         
        }
   
        public Map(Game game, string lightShaderPath)
        {
            if (lightShaderPath != null)
            {
                //create krypton, link to this game, use shaders
                Krypton = new KryptonEngine(game, lightShaderPath);
                this.InitializeMapLight();
            }
        }
        #endregion

        #region InitializeMapLight
        private void InitializeMapLight()
        {
            //make sure to initialize krypton, unless it has been added to the Game's list of Components
            Krypton.Initialize();
            Krypton.AmbientColor = AmbientColor;
            Krypton.LightMapSize = LightMapSize.Full;
            Krypton.SpriteBatchCompatablityEnabled = true;
            Krypton.Bluriness = 0.1f;
            Krypton.CullMode = CullMode.CullClockwiseFace;
            Krypton.Game.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            //Krypton.CullMode = CullMode.CullClockwiseFace;
            //Krypton.Game.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            //Krypton.CullMode = CullMode.None;
            //Krypton.Game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        }
        #endregion

        #region MusicToggle
        public void MusicToggle()
        {
            if (musicInstance.State == SoundState.Playing)
                musicInstance.Pause();
            else if (musicInstance.State == SoundState.Paused)
                musicInstance.Resume();
        }
        #endregion

        #region Create Light/Hull
        public Light2D CreateLight(Texture2D texture, float range, Color color, float intensity, float angle, Vector2 position, float fov)
        { 
            Light2D light = new Light2D()
            {
                Texture = texture,
                Range = range,
                Color = color,
                Intensity = intensity,
                Angle = angle,
                Position = position,
                Fov = fov
            };
            Krypton.Lights.Add(light);
            return light;
        }

        public ShadowHull CreateHull(float radius, float angle, Vector2 position, Vector2 scale, bool circlular = false)
        {
            ShadowHull hull;

            if (!circlular)
                hull = ShadowHull.CreateRectangle(Vector2.One);
            else
                hull = ShadowHull.CreateCircle(radius, 20);

            hull.Position = position;
            hull.Scale = scale;
            hull.Angle = angle;
            hull.MaxRadius = radius;

            Krypton.Hulls.Add(hull);
            return hull;
        }
        #endregion

        #region Add/Remove Light/Hull
        public void AddLight(Light2D light)
        {
            Krypton.Lights.Add(light);
        }
        public void RemoveLight(Light2D light)
        {
            Krypton.Lights.Remove(light);
        }
        public void AddHull(ShadowHull hull)
        {
            Krypton.Hulls.Add(hull);
        }
        public void RemoveHull(ShadowHull hull)
        {
            Krypton.Hulls.Remove(hull);
        }
        #endregion

        #region UnloadContent
        public void UnloadContent(bool lights, bool gameObjects, bool player)
        {
            if(lights)
            {
                foreach(Light2D light in Krypton.Lights)
                    Krypton.Lights.Remove(light);

                foreach (ShadowHull hull in Krypton.Hulls)
                    Krypton.Hulls.Remove(hull);
            }
            if(gameObjects)
            {
                foreach (IMapObject go in this.GameObjectsEarly)
                    this.GameObjectsEarly.Remove(go);

                foreach (IMapObject gol in this.GameObjectsLate)
                    this.GameObjectsEarly.Remove(gol);
            }
            if(player)
            {
                Player = null;
            }
        }
        #endregion

        #region UpdateLights
        private void UpdateLights(GameTime gameTime)
        {
            foreach (Light2D light in Krypton.Lights)
            {
                if (!light.IsOn && Player.Sprite.InLightingRange(light))
                {
                    light.IsOn = true;
                }
                else if (light.IsOn && !Player.Sprite.InLightingRange(light))
                {
                    light.IsOn = false;
                }
            }

            foreach (ShadowHull hull in Krypton.Hulls)
            {
                if (!hull.Visible && Player.Sprite.InShadowRange(hull))
                {
                    hull.Visible = true;
                }
                else if (hull.Visible && !Player.Sprite.InShadowRange(hull))
                {
                    hull.Visible = false;
                }
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            UpdateLights(gameTime);
            //update early game objects
            foreach (IMapObject go in GameObjectsEarly)
            {
                //if (CurrentMap.Player.Sprite.InUpdateRange(go.Sprite))
                go.Update(gameTime);
            }
            //update player
            GameEngine.CurrentMap.Player.Update(gameTime);
            //EngineGame.CurrentMap.Player.Sprite.ClampToArea(EngineGame.CurrentMap.CollisionLayer.WidthInPixels - EngineGame.CurrentMap.Player.Sprite.Bounds.Width,
            //                                          EngineGame.CurrentMap.CollisionLayer.HeightInPixels - EngineGame.CurrentMap.Player.Sprite.Bounds.Height);
            //update late game objects
            foreach (IMapObject gol in GameObjectsLate)
            {
                //if (CurrentMap.Player.Sprite.InUpdateRange(gol.Sprite))
                gol.Update(gameTime);
            }
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draws the map on a layer by layer basis, starting with the first layer in the Layers collection.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="camera"></param>
        public void Draw(SpriteBatch spriteBatch, Camera camera, GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            Point min = ConversionHelper.VPointToCell(camera.Position);
            Point max = ConversionHelper.VPointToCell(camera.Position +
                new Vector2(spriteBatch.GraphicsDevice.Viewport.Width + ConversionHelper.TileWidth,
                            spriteBatch.GraphicsDevice.Viewport.Height + ConversionHelper.TileHeight));

            //assign the matrix and pre-render the lightmap each draw cycle
            //make sure not to change the position of any lights or shadow hulls after this call, as it won't take effect till the next frame
            this.Krypton.Matrix = camera.TransformMatrix;
            this.Krypton.LightMapPrepare();
            //clear the backbuffer
            graphicsDevice.Clear(BackBufferColor);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, camera.TransformMatrix);
            
            if (UseBackground && Background != null)
                spriteBatch.Draw(Background, new Rectangle(0, 0, Layers[0].WidthInPixels, Layers[0].WidthInPixels), Color.White);

            foreach (TileLayer layer in Layers)
            {
                layer.Draw(spriteBatch, camera, min, max);
                //layer.Draw(spriteBatch, camera);
            }

            if (UseTacticalLayer && TacticalLayer != null)
                TacticalLayer.Draw(spriteBatch, camera, min, max);

            spriteBatch.End();

            //draw early game objects
            foreach (IMapObject go in this.GameObjectsEarly)
            {
                if(Player.Sprite.InDrawRange(go.Sprite))
                    go.Draw(spriteBatch, camera);
            }
            //draw player
            this.Player.Draw(spriteBatch, camera);
            //draw late game objects
            foreach (IMapObject gol in this.GameObjectsLate)
            {
                if (Player.Sprite.InDrawRange(gol.Sprite))
                    gol.Draw(spriteBatch, camera);
            }

            //2D lighting draw
            this.Krypton.Draw(gameTime);
        }
        #endregion
    }
}
