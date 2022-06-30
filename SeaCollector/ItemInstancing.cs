using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Rendering;

namespace SeaCollector
{
    public class ItemInstancing
    {
        Effect effect;
        VertexDeclaration instanceVertexDeclaration;

        VertexBuffer instanceBuffer;

        GameMesh Mesh;

        VertexBufferBinding[] bindings;
        InstanceInfo[] instances;
        
        Int32 instanceCount = 1000;
        
        struct InstanceInfo
        {
            public Vector4 World;
        };
        
        public void Load(ContentManager Content)
        {
            effect = Content.Load<Effect>("InstanceShader");
        }
        
        public void Initialize(GraphicsDevice device)
        {
            GenerateInstanceVertexDeclaration();
            GenerateGeometry(device);
            GenerateInstanceInformation(device, instanceCount);

            bindings = new VertexBufferBinding[2];
            bindings[0] = new VertexBufferBinding(Mesh.VertexBuffer);
            bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);
        }
        
        private void GenerateInstanceInformation(GraphicsDevice device, int count)
        {
            instances = new InstanceInfo[count];
            var rnd = new Random();

            for (int i = 0; i < count; i++)
            {
                //random position example
                instances[i].World = new Vector4(i,
                    0,
                    i, 1);
            }

            instanceBuffer = new VertexBuffer(device, instanceVertexDeclaration,
                count, BufferUsage.WriteOnly);
            instanceBuffer.SetData(instances);
        }

        private void GenerateGeometry(GraphicsDevice device)
        {
            Mesh = GameMesh.LoadFromFile(device, "Content/ship.ply");
        }

        private void GenerateInstanceVertexDeclaration()
        {
            VertexElement[] instanceStreamElements = new VertexElement[2];

            instanceStreamElements[0] =
                new VertexElement(0, VertexElementFormat.Vector4,
                    VertexElementUsage.Position, 1);
            
            instanceStreamElements[1] =
                new VertexElement(0, VertexElementFormat.Vector3,
                    VertexElementUsage.TextureCoordinate, 1);

            instanceVertexDeclaration = new VertexDeclaration(instanceStreamElements);
        }
        
        public void Draw(ref Matrix world, ref Matrix view, ref Matrix projection, Vector3 cameraPos, GraphicsDevice device)
        {
            //device.Clear(Color.CornflowerBlue);

            effect.CurrentTechnique = effect.Techniques["Instancing"];
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["CameraPosition"].SetValue(cameraPos);

            device.Indices = Mesh.IndexBuffer;

            effect.CurrentTechnique.Passes[0].Apply();

            device.SetVertexBuffers(bindings);
            device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, Mesh.Data.Length, 0, Mesh.Data.Length / 3, instanceCount);
        }
    }
}