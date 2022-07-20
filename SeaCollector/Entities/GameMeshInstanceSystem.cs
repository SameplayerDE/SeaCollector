
    using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Framework;
    using SeaCollector.Rendering;
    using SharpDX;
using Matrix = Microsoft.Xna.Framework.Matrix;
    using RenderContext = SeaCollector.Framework.RenderContext;
    using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Vector4 = Microsoft.Xna.Framework.Vector4;

namespace SeaCollector.Entities
{
    
    public class GameMeshInstanceSystem : GameObject3D
    {
        public bool EnsureOcclusion = true;
        
        public GraphicsDevice GraphicsDevice;

        //Instancing
        public VertexDeclaration InstanceVertexDeclaration;
        public VertexBuffer InstanceBuffer;
        public VertexBufferBinding[] Bindings;
        public InstanceInfo[] Instances;
        public int InstanceCount;
        
        public string TextureFile;
        public string EffectFile = "Effects/InstanceTextureShader";
        public string GameMeshFile;
        
        public Texture2D Texture2D;
        public Effect Effect;
        public GameMesh GameMesh;

        public GameMeshInstanceSystem(GraphicsDevice graphicsDevice, string textureFile, string gameMeshFile)
        {
            GameMeshFile = gameMeshFile;
            TextureFile = textureFile;
            GraphicsDevice = graphicsDevice;
        }

        public override void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            
            Effect = contentManager.Load<Effect>(EffectFile);
            if (!string.IsNullOrEmpty(TextureFile))
            {
                Texture2D = contentManager.Load<Texture2D>(TextureFile);
            }
            base.LoadContent(graphicsDevice, contentManager);
        }

        public override void Initialize()
        {
            GenerateInstanceVertexDeclaration();
            GameMesh = GameMesh.LoadFromFile(GraphicsDevice, GameMeshFile);
            GenerateInstanceInformation(GraphicsDevice);

            Bindings = new VertexBufferBinding[2];
            Bindings[0] = new VertexBufferBinding(GameMesh.VertexBuffer);
            Bindings[1] = new VertexBufferBinding(InstanceBuffer, 0, 1);
            base.Initialize();
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

            InstanceCount = 1_000;
            Instances = new InstanceInfo[InstanceCount];
            
            for (var i = 0; i < InstanceCount; i++)
            {
                Instances[i].World = new Vector4(RandomUtil.NextFloat(random, -100, 100), 0,
                    RandomUtil.NextFloat(random, -100, 100),
                    0);
            }
            
            InstanceBuffer = new VertexBuffer(device, InstanceVertexDeclaration,
                InstanceCount, BufferUsage.WriteOnly);
            InstanceBuffer.SetData(Instances);
        }
        
        private void DrawOpaquePixels(RenderContext renderContext)
        {
            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Effect.Parameters["AlphaTest"].SetValue(true);
            Effect.Parameters["AlphaTestGreater"].SetValue(true);
            DrawBillboards(renderContext);
        }

        private void DrawTransparentPixels(RenderContext renderContext)
        {
            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            Effect.Parameters["AlphaTest"].SetValue(true);
            Effect.Parameters["AlphaTestGreater"].SetValue(false);
            DrawBillboards(renderContext);
        }

        private void DrawBillboards(RenderContext renderContext)
        {
            Effect.CurrentTechnique.Passes[0].Apply();
            renderContext.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, GameMesh.VertexBuffer.VertexCount, 0, GameMesh.VertexBuffer.VertexCount / 3, InstanceCount);
        }

        public override void Draw(RenderContext renderContext)
        {
            // Set the vertex and index buffer to the graphics card
            renderContext.GraphicsDevice.SetVertexBuffers(Bindings);
            renderContext.GraphicsDevice.Indices = GameMesh.IndexBuffer;
            renderContext.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            //SetEffectParameters(renderContext.Camera.View, renderContext.Camera.Projection, renderContext.Camera.Up, Vector3.Cross(renderContext.Camera.Forward, Vector3.Up));
            Effect.Parameters["Texture00"]?.SetValue(Texture2D);
            Effect.Parameters["World"]?.SetValue(WorldMatrix);
            Effect.Parameters["View"]?.SetValue(renderContext.Camera.View);
            Effect.Parameters["Projection"]?.SetValue(renderContext.Camera.Projection);
            
            if (EnsureOcclusion)
            {
                DrawOpaquePixels(renderContext);
                DrawTransparentPixels(renderContext);
            }
            else
            {
                renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                Effect.Parameters["AlphaTest"].SetValue(false);
                DrawBillboards(renderContext);
            }

            // Reset render states
            renderContext.GraphicsDevice.BlendState = BlendState.Opaque;
            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            // Un-set the vertex and index buffer
            renderContext.GraphicsDevice.SetVertexBuffer(null);
            renderContext.GraphicsDevice.Indices = null;
            base.Draw(renderContext);
        }
    }
}