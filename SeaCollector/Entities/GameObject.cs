using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Rendering;

namespace SeaCollector.Entities
{
    public class GameObject
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
        
        public GameMesh Mesh;
        
        public void Draw(GraphicsDevice graphicsDevice, Effect effect, Matrix world, Matrix view, Matrix proj)
        {
            
            graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            graphicsDevice.BlendState = BlendState.NonPremultiplied;
            
            var l_world = world;
            var l_view = view;
            var l_proj = proj;

            var n_scale = Matrix.CreateScale(Scale);

            var n_rotation =
                Matrix.CreateRotationX(Rotation.X) *
                Matrix.CreateRotationY(Rotation.Y) *
                Matrix.CreateRotationZ(Rotation.Z);

            var n_translation = Matrix.CreateTranslation(Position);

            l_world *= n_scale * n_rotation * n_translation;

            
            graphicsDevice.SetVertexBuffer(Mesh.VertexBuffer);
            graphicsDevice.Indices = Mesh.IndexBuffer;
            
            foreach (var currentTechniquePass in effect.CurrentTechnique.Passes)
            {
                effect.Parameters["WorldViewProjection"]?.SetValue(l_world * l_view * l_proj);
                effect.Parameters["World"]?.SetValue(l_world);
                effect.Parameters["View"]?.SetValue(l_view);
                effect.Parameters["Projection"]?.SetValue(l_proj);
                currentTechniquePass.Apply();
                //graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Mesh.Data, 0, Mesh.Data.Length / 3);
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Mesh.Data.Length / 3);
            }
        }
    }
}