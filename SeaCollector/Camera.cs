﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector
{
    public abstract class Camera
    {
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }
        protected GraphicsDevice GraphicsDevice { get; set; }
 
        public Camera(GraphicsDevice graphicsDevice)
        {
            this.GraphicsDevice = graphicsDevice;
            generatePerspectiveProjectionMatrix(MathHelper.PiOver4);
        }
        private void generatePerspectiveProjectionMatrix(float FieldOfView)
        {
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            float aspectRatio = (float)pp.BackBufferWidth / 
                                (float)pp.BackBufferHeight;
            this.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45), aspectRatio, 0.1f, 1000.0f);
        }
        public virtual void Update()
        {
        }
    }
}