using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Framework;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Vector4 = Microsoft.Xna.Framework.Vector4;

namespace SeaCollector.Entities
{
    public class BillboardObject : GameObject3D
    {
        public BillboardMode Mode = BillboardMode.Spherical;
        public VertexBuffer VertexBuffer; //VertexBuffer
        public IndexBuffer IndexBuffer; //IndexBuffer

        public VertexPositionColorTexture[] Particles;

        private int[] _indices;

        public Vector2 Size;
        public bool EnsureOcclusion = true;

        public Texture2D Texture2D;
        public GraphicsDevice GraphicsDevice;
        public Effect Effect;

        public string TextureFile;
        public string EffectFile = "Effects/BasicBillboardShader";
        public float Depth;
        public Color Color;
        public Vector2 Tiling = Vector2.One;
        public TextureAddressMode U = TextureAddressMode.Wrap;
        public TextureAddressMode V = TextureAddressMode.Wrap;

        public float Width => (float)Texture2D.Width;
        public float Height => (float)Texture2D.Height;
        
        
        public BillboardObject(GraphicsDevice graphicsDevice, Vector2 size, string textureFile)
        {
            Size = size;
            GraphicsDevice = graphicsDevice;
            TextureFile = textureFile;
            Color = Color.White;
        }

        public override void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            Texture2D = contentManager.Load<Texture2D>(TextureFile);
            Effect = contentManager.Load<Effect>(EffectFile);
            base.LoadContent(graphicsDevice, contentManager);
        }

        public override void Initialize()
        {
            GenerateGeometry();
            base.Initialize();
        }

        private void GenerateGeometry()
        {
            // Create vertex and index arrays
            Particles = new VertexPositionColorTexture[4];
            _indices = new int[6];
            var x = 0;
            // For each billboard...
            for (var i = 0; i < 4; i += 4)
            {
                var position = Vector3.Zero;
                // Add 4 vertices at the billboard's position
                Particles[i + 0] = new VertexPositionColorTexture(position, Color,
                    new Vector2(0, 0));
                Particles[i + 1] = new VertexPositionColorTexture(position,Color,
                    new Vector2(0, 1));
                Particles[i + 2] = new VertexPositionColorTexture(position,Color,
                    new Vector2(1, 1));
                Particles[i + 3] = new VertexPositionColorTexture(position,Color,
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
                typeof(VertexPositionColorTexture),
                4, BufferUsage.WriteOnly);
            VertexBuffer.SetData(Particles);
            // Create and set the index buffer
            IndexBuffer = new IndexBuffer(GraphicsDevice,
                IndexElementSize.ThirtyTwoBits,
                6, BufferUsage.WriteOnly);
            IndexBuffer.SetData(_indices);
        }

        private void SetEffectParameters(Matrix view, Matrix projection, Vector3 up,
            Vector3 right)
        {
            Effect.Parameters["Texture00"]?.SetValue(Texture2D);
            Effect.Parameters["World"].SetValue(WorldMatrix);
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["Size"].SetValue(Size / 2f);
            Effect.Parameters["Up"].SetValue(Mode == BillboardMode.Spherical ? up : Vector3.Up);
            Effect.Parameters["Side"].SetValue(right);
            Effect.CurrentTechnique.Passes[0].Apply();
        }

        private void DrawOpaquePixels(RenderContext renderContext)
        {
            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Effect.Parameters["AlphaTest"].SetValue(true);
            Effect.Parameters["AlphaTestGreater"].SetValue(true);
            DrawSprite(renderContext);
        }

        private void DrawTransparentPixels(RenderContext renderContext)
        {
            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            Effect.Parameters["AlphaTest"].SetValue(true);
            Effect.Parameters["AlphaTestGreater"].SetValue(false);
            DrawSprite(renderContext);
        }

        private void DrawSprite(RenderContext renderContext)
        {
            Effect.CurrentTechnique.Passes[0].Apply();
            renderContext.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
        }

        public override void Draw(RenderContext renderContext)
        {
            // Set the vertex and index buffer to the graphics card
            renderContext.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            renderContext.GraphicsDevice.Indices = IndexBuffer;
            renderContext.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            SetEffectParameters(renderContext.Camera.View, renderContext.Camera.Projection, renderContext.Camera.Up, Vector3.Cross(renderContext.Camera.Forward, Vector3.Up));
            if (EnsureOcclusion)
            {
                DrawOpaquePixels(renderContext);
                DrawTransparentPixels(renderContext);
            }
            else
            {
                renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                Effect.Parameters["AlphaTest"].SetValue(false);
                DrawSprite(renderContext);
            }

            // Reset render states
            renderContext.GraphicsDevice.BlendState = BlendState.Opaque;
            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            // Un-set the vertex and index buffer
            renderContext.GraphicsDevice.SetVertexBuffer(null);
            renderContext.GraphicsDevice.Indices = null;
            base.Draw(renderContext);
        }

        public void Dispose()
        {
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
        }
    }
}