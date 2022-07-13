using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Vector4 = Microsoft.Xna.Framework.Vector4;

namespace SeaCollector.Rendering
{
    public enum BillboardMode
    {
        Cylindrical,
        Spherical
    };

    public struct InstanceInfo
    {
        public Vector4 World;
    };

    public class BillboardSystem
    {
        public BillboardMode Mode = BillboardMode.Spherical;
        public VertexBuffer VertexBuffer; //VertexBuffer
        public IndexBuffer IndexBuffer; //IndexBuffer

        public VertexPositionTexture[] Particles;

        private int[] _indices;

        public Vector2 BillboardSize;
        public bool EnsureOcclusion = true;


        public Texture2D Texture;
        public GraphicsDevice GraphicsDevice;
        public Effect Effect;


        //Instancing
        public VertexDeclaration InstanceVertexDeclaration;
        public VertexBuffer InstanceBuffer;
        public VertexBufferBinding[] Bindings;
        public InstanceInfo[] Instances;
        public int InstanceCount;

        public BillboardSystem(GraphicsDevice graphicsDevice, Vector2 billboardSize)
        {
            BillboardSize = billboardSize;
            GraphicsDevice = graphicsDevice;
        }

        public void LoadContent(ContentManager contentManager, string texture)
        {
            Texture = contentManager.Load<Texture2D>(texture);
            Effect = contentManager.Load<Effect>("Effects/BillboardShader");
        }

        public void Initialize(GraphicsDevice device)
        {
            GenerateInstanceVertexDeclaration();
            GenerateGeometry();
            GenerateInstanceInformation(device);

            Bindings = new VertexBufferBinding[2];
            Bindings[0] = new VertexBufferBinding(VertexBuffer);
            Bindings[1] = new VertexBufferBinding(InstanceBuffer, 0, 1);
        }

        private void GenerateInstanceVertexDeclaration()
        {
            var instanceStreamElements = new VertexElement[1];

            instanceStreamElements[0] =
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 1);

            InstanceVertexDeclaration = new VertexDeclaration(instanceStreamElements);
        }

        private void GenerateInstanceInformation(GraphicsDevice device)
        {
            // Generate random tree positions
            var random = new Random();

            InstanceCount = 100_000;
            Instances = new InstanceInfo[InstanceCount];
            
            for (var i = 0; i < InstanceCount; i++)
            {
                Instances[i].World = new Vector4(RandomUtil.NextFloat(random, -1000, 1000), 0,
                    RandomUtil.NextFloat(random, -1000, 1000),
                    0);
            }
            
            InstanceBuffer = new VertexBuffer(device, InstanceVertexDeclaration,
                InstanceCount, BufferUsage.WriteOnly);
            InstanceBuffer.SetData(Instances);
        }

        private void GenerateGeometry()
        {
            // Create vertex and index arrays
            Particles = new VertexPositionTexture[4];
            _indices = new int[6];
            var x = 0;
            // For each billboard...
            for (var i = 0; i < 4; i += 4)
            {
                var position = Vector3.Zero;
                // Add 4 vertices at the billboard's position
                Particles[i + 0] = new VertexPositionTexture(position,
                    new Vector2(0, 0));
                Particles[i + 1] = new VertexPositionTexture(position,
                    new Vector2(0, 1));
                Particles[i + 2] = new VertexPositionTexture(position,
                    new Vector2(1, 1));
                Particles[i + 3] = new VertexPositionTexture(position,
                    new Vector2(1, 0));
                // Add 6 indices to form two triangles
                _indices[x++] = i + 0;
                _indices[x++] = i + 3;
                _indices[x++] = i + 2;
                _indices[x++] = i + 2;
                _indices[x++] = i + 1;
                _indices[x++] = i + 0;
            }

            // Create and set the vertex buffer
            VertexBuffer = new VertexBuffer(GraphicsDevice,
                typeof(VertexPositionTexture),
                4, BufferUsage.WriteOnly);
            VertexBuffer.SetData<VertexPositionTexture>(Particles);
            // Create and set the index buffer
            IndexBuffer = new IndexBuffer(GraphicsDevice,
                IndexElementSize.ThirtyTwoBits,
                6, BufferUsage.WriteOnly);
            IndexBuffer.SetData(_indices);
        }

        private void SetEffectParameters(Matrix view, Matrix projection, Vector3 up,
            Vector3 right)
        {
            Effect.Parameters["ParticleTexture"].SetValue(Texture);
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["Size"].SetValue(BillboardSize / 2f);
            Effect.Parameters["Up"].SetValue(Mode == BillboardMode.Spherical ? up : Vector3.Up);
            Effect.Parameters["Side"].SetValue(right);
            Effect.CurrentTechnique.Passes[0].Apply();
        }

        private void DrawOpaquePixels()
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Effect.Parameters["AlphaTest"].SetValue(true);
            Effect.Parameters["AlphaTestGreater"].SetValue(true);
            DrawBillboards();
        }

        private void DrawTransparentPixels()
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            Effect.Parameters["AlphaTest"].SetValue(true);
            Effect.Parameters["AlphaTestGreater"].SetValue(false);
            DrawBillboards();
        }

        private void DrawBillboards()
        {
            Effect.CurrentTechnique.Passes[0].Apply();
#pragma warning disable CS0618
            GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2,
#pragma warning restore CS0618
                InstanceCount);
        }

        public void Draw(Matrix view, Matrix projection, Vector3 up, Vector3
            right)
        {
            // Set the vertex and index buffer to the graphics card
            GraphicsDevice.SetVertexBuffers(Bindings);
            GraphicsDevice.Indices = IndexBuffer;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            SetEffectParameters(view, projection, up, right);
            if (EnsureOcclusion)
            {
                DrawOpaquePixels();
                DrawTransparentPixels();
            }
            else
            {
                GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                Effect.Parameters["AlphaTest"].SetValue(false);
                DrawBillboards();
            }

            // Reset render states
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            // Un-set the vertex and index buffer
            GraphicsDevice.SetVertexBuffer(null);
            GraphicsDevice.Indices = null;
        }

        public void Dispose()
        {
            if (Instances != null) Array.Clear(Instances, 0, Instances.Length);
            if (Particles != null) Array.Clear(Particles, 0, Particles.Length);
            if (Bindings != null) Array.Clear(Bindings, 0, Bindings.Length);
            if (_indices != null) Array.Clear(_indices, 0, _indices.Length);
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
        }
    }
}