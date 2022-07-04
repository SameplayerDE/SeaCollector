using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Rendering.Cameras;

namespace SeaCollector
{
    public class World
    {
        public string Name = Guid.NewGuid().ToString();

        public GraphicsDevice GraphicsDevice;
        public Camera Camera;
        public Matrix WorldMatrix;
    }
}