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

        int[] indices;

        // Billboard settings
        int nBillboards;
        public Vector2 BillboardSize;

        public Texture2D Texture;

        // GraphicsDevice and Effect
        public GraphicsDevice GraphicsDevice;

        public bool EnsureOcclusion = true;

        public Effect Effect;


        //Instancing
        public VertexDeclaration InstanceVertexDeclaration;
        public VertexBuffer InstanceBuffer;
        public VertexBufferBinding[] Bindings;
        public InstanceInfo[] Instances;
        public int InstanceCount;

        public BillboardSystem(GraphicsDevice graphicsDevice, ContentManager content, Texture2D texture,
            Vector2 billboardSize)
        {
            BillboardSize = billboardSize;
            GraphicsDevice = graphicsDevice;
            Texture = texture;
            Effect = content.Load<Effect>("Effects/BillboardShader");
            
        }

        public void Initialize(GraphicsDevice device)
        {
            GenerateInstanceVertexDeclaration();
            //GenerateGeometry(device);
            generateParticles(Vector3.Zero);
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
            var list = new List<Vector4>();

            for (var i = 0; i < 100_000; i++)
                list.Add(new Vector4(RandomUtil.NextFloat(random, -1000, 1000), 0, RandomUtil.NextFloat(random, -1000, 1000),
                    0));

            InstanceCount = list.Count;
            Instances = new InstanceInfo[InstanceCount];

            for (var i = 0; i < InstanceCount; i++)
            {
                Instances[i].World = list[i];
            }

            InstanceBuffer = new VertexBuffer(device, InstanceVertexDeclaration,
                InstanceCount, BufferUsage.WriteOnly);
            InstanceBuffer.SetData(Instances);
        }

        void generateParticles(Vector3 particlePositions)
        {
            // Create vertex and index arrays
            Particles = new VertexPositionTexture[4];
            indices = new int[6];
            int x = 0;
            // For each billboard...
            for (int i = 0; i < 4; i += 4)
            {
                var pos = particlePositions;
                // Add 4 vertices at the billboard's position
                Particles[i + 0] = new VertexPositionTexture(pos,
                    new Vector2(0, 0));
                Particles[i + 1] = new VertexPositionTexture(pos,
                    new Vector2(0, 1));
                Particles[i + 2] = new VertexPositionTexture(pos,
                    new Vector2(1, 1));
                Particles[i + 3] = new VertexPositionTexture(pos,
                    new Vector2(1, 0));
                // Add 6 indices to form two triangles
                indices[x++] = i + 0;
                indices[x++] = i + 3;
                indices[x++] = i + 2;
                indices[x++] = i + 2;
                indices[x++] = i + 1;
                indices[x++] = i + 0;
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
            IndexBuffer.SetData<int>(indices);
        }

        void setEffectParameters(Matrix View, Matrix Projection, Vector3 Up,
            Vector3 Right)
        {
            Effect.Parameters["ParticleTexture"].SetValue(Texture);
            Effect.Parameters["View"].SetValue(View);
            Effect.Parameters["Projection"].SetValue(Projection);
            Effect.Parameters["Size"].SetValue(BillboardSize / 2f);
            Effect.Parameters["Up"].SetValue(Mode == BillboardMode.Spherical ? Up : Vector3.Up);
            Effect.Parameters["Side"].SetValue(Right);
            Effect.CurrentTechnique.Passes[0].Apply();
        }

        void drawOpaquePixels()
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Effect.Parameters["AlphaTest"].SetValue(true);
            Effect.Parameters["AlphaTestGreater"].SetValue(true);
            drawBillboards();
        }

        void drawTransparentPixels()
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            Effect.Parameters["AlphaTest"].SetValue(true);
            Effect.Parameters["AlphaTestGreater"].SetValue(false);
            drawBillboards();
        }

        void drawBillboards()
        {
            Effect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2,
                InstanceCount);
        }

        public void Draw(Matrix View, Matrix Projection, Vector3 Up, Vector3
            Right)
        {
            // Set the vertex and index buffer to the graphics card
            GraphicsDevice.SetVertexBuffers(Bindings);
            GraphicsDevice.Indices = IndexBuffer;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            setEffectParameters(View, Projection, Up, Right);
            if (EnsureOcclusion)
            {
                drawOpaquePixels();
                drawTransparentPixels();
            }
            else
            {
                GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                Effect.Parameters["AlphaTest"].SetValue(false);
                drawBillboards();
            }

            // Reset render states
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            // Un-set the vertex and index buffer
            GraphicsDevice.SetVertexBuffer(null);
            GraphicsDevice.Indices = null;
        }
    }
}