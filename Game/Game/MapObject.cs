using Krypton;
using Krypton.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TileEngine;
using TileEngine.LayerMap;
using TileEngine.Sprite;

namespace TestGame
{
    public class MapObject : IMapObject
    {
        protected string name;
        protected Map currentMap;
        protected ISprite sprite;
        protected Dictionary<string, Light2D> lights;
        protected Dictionary<string, ShadowHull> hulls;
        protected Dictionary<string, ParticleSystem> particleSystems;

        public MapObject(string name, Vector2 startingPos, ISprite sprite)
        {
            this.name = name;
            this.sprite = sprite;
            this.sprite.Position = startingPos;

            lights = new Dictionary<string, Light2D>();
            hulls = new Dictionary<string, ShadowHull>();
            particleSystems = new Dictionary<string, ParticleSystem>(); 
        }

        public Map CurrentMap
        {
            get
            {
                if (currentMap == null)
                {
                    throw new Exception(name + " has no map!");
                }
                return CurrentMap;
            }

            set
            {
                if (currentMap != null)
                {
                    foreach (KeyValuePair<string, Light2D> kvp in lights)
                    {
                        currentMap.RemoveLight(kvp.Value);
                    }
                    foreach (KeyValuePair<string, ShadowHull> kvp in hulls)
                    {
                        currentMap.RemoveHull(kvp.Value);
                    }
                }
                foreach (KeyValuePair<string, Light2D> kvp in lights)
                {
                    value.AddLight(kvp.Value);
                }
                foreach (KeyValuePair<string, ShadowHull> kvp in hulls)
                {
                    value.AddHull(kvp.Value);
                }
                
                SwitchMap(value);
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public ISprite Sprite
        {
            get
            {
                return sprite;
            }
        }

        public Vector2 Position
        {
            get
            {
                return sprite.Position;
            }
            set
            {
                sprite.Position = value;
            }
        }

        public float Angle
        {
            set
            {
                this.sprite.Angle = value;
            }
        }

        public float Scale
        {
            set
            {
                this.sprite.Scale = value;
            }
        }

        public virtual void HandleCollision(IMapObject collidee)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (KeyValuePair<string, ParticleSystem> kvp in particleSystems)
            {
                kvp.Value.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f);
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            foreach (KeyValuePair<string, ParticleSystem> kvp in particleSystems)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, kvp.Value.BlendState, null, null, null, null, camera.TransformMatrix);
                kvp.Value.Draw(spriteBatch, 1, Vector2.Zero);
                spriteBatch.End();
            }
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, camera.TransformMatrix);
            sprite.Draw(spriteBatch, SpriteEffects.None);
            spriteBatch.End();
        }

        protected virtual void SwitchMap(Map map)
        {
            if (currentMap != null)
            {
                if (currentMap.GameObjectsEarly.Contains(this))
                {
                    currentMap.GameObjectsEarly.Remove(this);
                    map.GameObjectsEarly.Add(this);
                }
                else if (currentMap.GameObjectsLate.Contains(this))
                {
                    currentMap.GameObjectsLate.Remove(this);
                    map.GameObjectsLate.Add(this);
                }
            }
            else
            {
                map.GameObjectsEarly.Add(this);
            }
            currentMap = map;
        }
    }
}
