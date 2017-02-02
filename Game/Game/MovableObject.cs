using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TileEngine;
using TileEngine.LayerMap;
using TileEngine.Sprite;

namespace TestGame
{
    public class MovableObject : MapObject
    {
        protected float speed;

        public MovableObject(string name, Vector2 startingPos, AnimatedSprite sprite): base(name, startingPos, sprite)
        {
            this.speed = 0.18f;
        }

        public AnimatedSprite AnimatedSprite
        {
            get
            {
                return (AnimatedSprite)sprite;
            }
        }
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public override void HandleCollision(IMapObject collidee)
        {
            throw new NotImplementedException();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(spriteBatch, camera);
        }
    }
}
