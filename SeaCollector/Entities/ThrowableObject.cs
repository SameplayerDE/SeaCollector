using HxTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Framework;

namespace SeaCollector.Entities
{
    public class ThrowableObject : BillboardObject
    {

        public bool Held;
        public bool Thrown;
        public bool OnGround = true;
        public bool InAir;
        public Vector3 Direction;
        public float Gravity = 5f;
        
        public ThrowableObject(GraphicsDevice graphicsDevice, Vector2 size, string textureFile) : base(graphicsDevice, size, textureFile)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (!OnGround)
            {
                if (!Held)
                {
                    if (!OnGround && WorldPosition.Y <= 0)
                    {
                        InAir = false;
                        OnGround = true;
                    }
                    if (InAir)
                    {
                        Direction.Y -= Gravity * Time.Instance.DeltaSecondsF;
                        LocalPosition += Direction * Time.Instance.DeltaSecondsF;
                    }
                }
            }

            

            

           
            
            base.Update(gameTime);
        }
    }
}