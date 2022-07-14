using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Rendering;

namespace SeaCollector.Entities
{
    public class GameObject
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale = Vector3.One;

        public Matrix Matrix = Matrix.Identity;
        
        
        public GameMesh Mesh;

        public virtual void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            
        }

        public virtual void Update(GameTime gameTime)
        {
            var l_world = Matrix;

            var n_scale = Matrix.CreateScale(Scale);

            var n_rotation =
                Matrix.CreateRotationX(Rotation.X) *
                Matrix.CreateRotationY(Rotation.Y) *
                Matrix.CreateRotationZ(Rotation.Z);

            var n_translation = Matrix.CreateTranslation(Position);

            Matrix = n_scale * n_rotation * n_translation;
        }
        
        public void Draw(GraphicsDevice graphicsDevice, Effect effect, Matrix world, Matrix view, Matrix projection)
        {
            var l_world = world;
            var l_view = view;
            var l_projection = projection;

            var n_scale = Matrix.CreateScale(Scale);

            var n_rotation =
                Matrix.CreateRotationX(Rotation.X) *
                Matrix.CreateRotationY(Rotation.Y) *
                Matrix.CreateRotationZ(Rotation.Z);

            var n_translation = Matrix.CreateTranslation(Position);

            l_world *= n_scale * n_rotation * n_translation;
            
            graphicsDevice.SetVertexBuffer(Mesh.VertexBuffer);
            graphicsDevice.Indices = Mesh.IndexBuffer;
            
            effect.Parameters["WorldViewProjection"]?.SetValue(l_world * l_view * l_projection);
            effect.Parameters["World"]?.SetValue(l_world);
            effect.Parameters["View"]?.SetValue(l_view);
            effect.Parameters["Projection"]?.SetValue(l_projection);
            
            foreach (var currentTechniquePass in effect.CurrentTechnique.Passes)
            {
                
                currentTechniquePass.Apply();
                //graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Mesh.Data, 0, Mesh.Data.Length / 3);
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Mesh.Data.Length / 3);
            }
        }
    }
}