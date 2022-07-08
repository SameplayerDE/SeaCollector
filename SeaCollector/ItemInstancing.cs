using System;
using System.Collections.Generic;
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

        private float[] _islandNoiseResultValues;

        struct InstanceInfo
        {
            public Vector4 World;
        };

        public ItemInstancing(float[] values)
        {
            _islandNoiseResultValues = values;
        }

        public void Load(ContentManager Content)
        {
            effect = Content.Load<Effect>("Effects/InstanceShader");
        }

        public void Initialize(GraphicsDevice device)
        {
            GenerateInstanceVertexDeclaration();
            GenerateGeometry(device);
            GenerateInstanceInformation(device);

            bindings = new VertexBufferBinding[2];
            bindings[0] = new VertexBufferBinding(Mesh.VertexBuffer);
            bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);
        }

        private void GenerateInstanceInformation(GraphicsDevice device)
        {
            
            var list = new List<Vector4>();

            for (var y = 0; y < 100; y++)
            {
                for (var x = 0; x < 100; x++)
                {
                    var noiseValue = _islandNoiseResultValues[x + 100 * y];
                    if (noiseValue > 0.5f)
                    {
                        list.Add(
                            new Vector4(-50 + x, 0, -50 + y, 0)
                        );
                    }
                }
            }

            instanceCount = list.Count;
            instances = new InstanceInfo[instanceCount];

            for (var i = 0; i < instanceCount; i++)
            {
                instances[i].World = list[i];
            }

            instanceBuffer = new VertexBuffer(device, instanceVertexDeclaration,
                instanceCount, BufferUsage.WriteOnly);
            instanceBuffer.SetData(instances);
        }

        private void GenerateGeometry(GraphicsDevice device)
        {
            Mesh = GameMesh.LoadFromFile(device, "Content/Models/cube.obj");
        }

        private void GenerateInstanceVertexDeclaration()
        {
            VertexElement[] instanceStreamElements = new VertexElement[1];

            instanceStreamElements[0] =
                new VertexElement(0, VertexElementFormat.Vector4,
                    VertexElementUsage.Position, 1);

            instanceVertexDeclaration = new VertexDeclaration(instanceStreamElements);
        }

        public void Draw(Camera camera, ref Matrix world, ref Matrix view, ref Matrix projection, Vector3 cameraPos,
            Vector3 playerPosition, Vector3 fogCenter, GraphicsDevice device, GameTime gameTime)
        {
            //device.Clear(Color.CornflowerBlue);

            effect.CurrentTechnique = effect.Techniques["Instancing"];
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["CameraPosition"].SetValue(cameraPos);
            effect.Parameters["PlayerPosition"]?.SetValue(playerPosition);
            effect.Parameters["FogCenter"]?.SetValue(fogCenter);
            effect.Parameters["Total"]?.SetValue((float)gameTime.TotalGameTime.TotalSeconds);
            effect.Parameters["Delta"]?.SetValue((float)gameTime.ElapsedGameTime.TotalSeconds);

            device.Indices = Mesh.IndexBuffer;

            effect.CurrentTechnique.Passes[0].Apply();

            device.SetVertexBuffers(bindings);
            device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, Mesh.Data.Length, 0, Mesh.Data.Length / 3,
                instanceCount);
        }
    }
}