using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Rendering;

namespace SeaCollector.Framework
{
    public class GameSprite3D : GameObject3D
    {
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
        public string EffectFile = "Effects/BasicSpriteShader";
        public float Depth;
        public Color Color;
        public Vector2 Tiling = Vector2.One;
        public TextureAddressMode U = TextureAddressMode.Wrap;
        public TextureAddressMode V = TextureAddressMode.Wrap;

        public float Width => (float)Texture2D.Width;
        public float Height => (float)Texture2D.Height;
        
        
        public GameSprite3D(GraphicsDevice graphicsDevice, Vector2 size, string textureFile)
        {
            Size = size;
            GraphicsDevice = graphicsDevice;
            TextureFile = textureFile;
            Color = Color.White;
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
                // Add 4 vertices at the billboard's position
                Particles[i + 0] = new VertexPositionColorTexture(new Vector3(-0.5f * Size.X, +0.5f * Size.Y, 0.0f), Color,
                    new Vector2(0, 0) * Tiling);
                Particles[i + 1] = new VertexPositionColorTexture(new Vector3(-0.5f * Size.X, -0.5f * Size.Y, 0.0f), Color,
                    new Vector2(0, 1) * Tiling);
                Particles[i + 2] = new VertexPositionColorTexture(new Vector3(+0.5f * Size.X, -0.5f * Size.Y, 0.0f), Color,
                    new Vector2(1, 1) * Tiling);
                Particles[i + 3] = new VertexPositionColorTexture(new Vector3(+0.5f * Size.X, +0.5f * Size.Y, 0.0f), Color,
                    new Vector2(1, 0) * Tiling);
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
        
        private void DrawSprite(RenderContext renderContext)
        {
            Effect.CurrentTechnique.Passes[0].Apply();
            renderContext.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
        }
        
        private void DrawOpaquePixels(RenderContext renderContext)
        {
            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Effect.Parameters["AlphaTest"]?.SetValue(true);
            Effect.Parameters["AlphaTestGreater"]?.SetValue(true);
            DrawSprite(renderContext);
        }

        private void DrawTransparentPixels(RenderContext renderContext)
        {
            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            Effect.Parameters["AlphaTest"]?.SetValue(true);
            Effect.Parameters["AlphaTestGreater"]?.SetValue(false);
            DrawSprite(renderContext);
        }

        public override void Draw(RenderContext renderContext)
        {
            /*var samplerState = new SamplerState();
            samplerState.AddressU = U;
            samplerState.AddressV = V;
            renderContext.GraphicsDevice.SamplerStates[0] = samplerState;*/
            // Set the vertex and index buffer to the graphics card
            renderContext.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            renderContext.GraphicsDevice.Indices = IndexBuffer;
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
                Effect.Parameters["AlphaTest"]?.SetValue(false);
                DrawSprite(renderContext);
            }

            // Reset render states
            renderContext.GraphicsDevice.BlendState = BlendState.Opaque;
            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            // Un-set the vertex and index buffer
            renderContext.GraphicsDevice.SetVertexBuffer(null);
            renderContext.GraphicsDevice.Indices = null;
            base.Draw(renderContext);
            //samplerState.Dispose();
        }
    }
}