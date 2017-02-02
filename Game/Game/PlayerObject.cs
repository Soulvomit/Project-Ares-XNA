using Krypton;
using Krypton.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TileEngine;
using TileEngine.LayerMap;
using TileEngine.Sprite;

namespace TestGame
{
    public class PlayerObject : MovableObject
    {
        public PlayerObject(string name, Vector2 startingPos, AnimatedSprite sprite, Texture2D particleTex): base(name, startingPos, sprite)
        {
            base.Angle = MathHelper.PiOver2;
            base.Sprite.CollisionRange = 32 * 1.4f;
            Light2D ambient =   new Light2D
            {
                Texture = LightTextureBuilder.CreatePointLight(GameEngine.CurrentMap.Krypton.GraphicsDevice, 128),
                Intensity = 1.0f,
                Range = 256,
                Angle = Sprite.Angle,
                Fov = MathHelper.TwoPi,
                Color = Color.White,
                IsOn = true
            };
            ambient.Position = Position;
            base.lights.Add("Ambient", ambient);

            ParticleSystem engine = new ParticleSystem(Position);
            engine.AddEmitter(
                new Vector2(0.098f, 0.016f), new Vector2(0, -1), 
                new Vector2(0.001f * MathHelper.Pi, 0.001f * MathHelper.Pi),
                new Vector2(0.85f, 1.05f), new Vector2(120 / 2, 140 / 2), 
                new Vector2(60 / 4, 70 / 4), 
                Color.Yellow, 
                Color.Orange, 
                Color.Red, 
                Color.Crimson,
                new Vector2(400 / 3, 500 / 3), 
                new Vector2(100 / 3, 120 / 3), 
                500, 
                Vector2.Zero, 
                particleTex);
            engine.Emitters[0].StopEmmiting = true;
            base.particleSystems.Add("Engine", engine);
        }
        protected override void SwitchMap(Map map)
        {
            if (currentMap != null)
            {
                currentMap.Player = null;
            }
            map.Player = this;
            currentMap = map;
        }

        public override void HandleCollision(IMapObject collidee)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            MouseState curMouse = Mouse.GetState();
            Vector2 mouseLoc = new Vector2(curMouse.X, curMouse.Y) + GameEngine.Camera.Position;
            Point pointToCell = ConversionHelper.VPointToCell(mouseLoc);
            Vector2 centerOfCell = ConversionHelper.CellToCellCenterPoint(pointToCell.X, pointToCell.Y, Vector2.Zero);
            Vector2 direction = centerOfCell - new Vector2(Sprite.Center.X, Sprite.Center.Y);
            base.Angle = MathHelper.PiOver2 + (float)(Math.Atan2(direction.Y, direction.X));
            if (curMouse.LeftButton == ButtonState.Pressed && !Sprite.IsColliding(mouseLoc))
            {
                Sprite.Position += Vector2.Normalize(direction) * gameTime.ElapsedGameTime.Milliseconds * speed;
                particleSystems["Engine"].Emitters[0].StopEmmiting = false;
            }
            else
            {
                particleSystems["Engine"].Emitters[0].StopEmmiting = true;
            }
            lights["Ambient"].Position = Position + new Vector2(32,32);
            particleSystems["Engine"].Emitters[0].SpawnDirection = -direction;
            particleSystems["Engine"].Position = sprite.Position + new Vector2(32, 32) + (Vector2.Normalize(-direction)*40); //+ kvp.Value.Offset;
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(spriteBatch, camera);
        }
    }
}
