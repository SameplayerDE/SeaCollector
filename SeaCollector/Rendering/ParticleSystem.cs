using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Rendering
{
    public class ParticleSystem
    {
        public VertexBuffer VertexBuffer; //VertexBuffer
        public IndexBuffer IndexBuffer; //IndexBuffer

        public ParticleVertex[] Particles;
        public int[] Indices;

        public Texture2D Texture;
        public GraphicsDevice GraphicsDevice;
        public Effect Effect;


        public int ParticleCount;
        public Vector2 ParticleSize;
        public float Lifespan = 1f;
        public Vector3 Wind;
        public float FadeInTime;

        public int ActiveStart = 0;
        public int ActiveCount = 0;

        public DateTime StartTime;

        public ParticleSystem(GraphicsDevice graphicsDevice,
            ContentManager content, Texture2D texture, int particleCount,
            Vector2 particleSize, float lifespan,
            Vector3 wind, float fadeInTime)
        {
            ParticleCount = particleCount;
            ParticleSize = particleSize;
            Lifespan = lifespan;
            GraphicsDevice = graphicsDevice;
            Wind = wind;
            Texture = texture;
            FadeInTime = fadeInTime;
            // Create vertex and index buffers to accomodate all particles
            VertexBuffer = new VertexBuffer(graphicsDevice, typeof(ParticleVertex),
                particleCount * 4, BufferUsage.WriteOnly);
            IndexBuffer = new IndexBuffer(graphicsDevice,
                IndexElementSize.ThirtyTwoBits, particleCount * 6,
                BufferUsage.WriteOnly);
            generateParticles();
            Effect = content.Load<Effect>("Effects/ParticleShader");
            StartTime = DateTime.Now;
        }

        void generateParticles()
        {
            // Create particle and index arrays
            Particles = new ParticleVertex[ParticleCount * 4];
            Indices = new int[ParticleCount * 6];
            var z = Vector3.Zero;

            int x = 0;
            // Initialize particle settings and fill index and vertex arrays
            for (int i = 0; i < ParticleCount * 4; i += 4)
            {
                Particles[i + 0] = new ParticleVertex(z, new Vector2(0, 0),
                    z, 0, -1);
                Particles[i + 1] = new ParticleVertex(z, new Vector2(0, 1),
                    z, 0, -1);
                Particles[i + 2] = new ParticleVertex(z, new Vector2(1, 1),
                    z, 0, -1);
                Particles[i + 3] = new ParticleVertex(z, new Vector2(1, 0),
                    z, 0, -1);
                Indices[x++] = i + 0;
                Indices[x++] = i + 3;
                Indices[x++] = i + 2;
                Indices[x++] = i + 2;
                Indices[x++] = i + 1;
                Indices[x++] = i + 0;
            }
        }

        public void AddParticle(Vector3 position, Vector3 direction, float
            speed)
        {
            // If there are no available particles, give up
            if (ActiveCount + 4 == ParticleCount * 4)
                return;
            // Determine the index at which this particle should be created
            var index = offsetIndex(ActiveStart, ActiveCount);
            ActiveCount += 4;
            // Determine the start time
            var startTime = (float)(DateTime.Now - StartTime).TotalSeconds;
            // Set the particle settings to each of the particle's vertices
            for (var i = 0; i < 4; i++)
            {
                Particles[index + i].Position = position;
                Particles[index + i].Direction = direction;
                Particles[index + i].Speed = speed;
                Particles[index + i].StartTime = startTime;
            }
        }

        // Increases the 'start' parameter by 'count' positions, wrapping
// around the particle array if necessary
        int offsetIndex(int start, int count)
        {
            for (int i = 0; i < count; i++)
            {
                start++;
                if (start == Particles.Length)
                    start = 0;
            }

            return start;
        }

        public void Update()
        {
            float now = (float)(DateTime.Now - StartTime).TotalSeconds;
            int startIndex = ActiveStart;
            int end = ActiveCount;
            // For each particle marked as active...
            for (int i = 0; i < end; i++)
            {
                // If this particle has gotten older than 'lifespan'...
                if (Particles[ActiveStart].StartTime < now - Lifespan)
                {
                    // Advance the active particle start position past
                    // the particle's index and reduce the number of
                    // active particles by 1
                    ActiveStart++;
                    ActiveCount--;
                    if (ActiveStart == Particles.Length)
                        ActiveStart = 0;
                }
            }

            // Update the vertex and index buffers
            VertexBuffer.SetData(Particles);
            IndexBuffer.SetData(Indices);
        }

        public void Draw(Matrix View, Matrix Projection, Vector3 Up, Vector3
            Right)
        {
            // Set the vertex and index buffer to the graphics card
            GraphicsDevice.SetVertexBuffer(VertexBuffer);
            GraphicsDevice.Indices = IndexBuffer;
            // Set the effect parameters
            Effect.Parameters["ParticleTexture"].SetValue(Texture);
            Effect.Parameters["View"].SetValue(View);
            Effect.Parameters["Projection"].SetValue(Projection);
            Effect.Parameters["Time"].SetValue((float)(DateTime.Now - StartTime).TotalSeconds);
            Effect.Parameters["Lifespan"].SetValue(Lifespan);
            Effect.Parameters["Wind"].SetValue(Wind);
            Effect.Parameters["Size"].SetValue(ParticleSize / 2f);
            Effect.Parameters["Up"].SetValue(Up);
            Effect.Parameters["Side"].SetValue(Right);
            Effect.Parameters["FadeInTime"].SetValue(FadeInTime);
            // Enable blending render states
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            // Apply the effect
            Effect.CurrentTechnique.Passes[0].Apply();
            // Draw the billboards
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 
                0, 0, ParticleCount * 4, 0, ParticleCount * 2);
            // Un-set the buffers
            GraphicsDevice.SetVertexBuffer(null);
            GraphicsDevice.Indices = null;
            // Reset render states
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }
    }
}